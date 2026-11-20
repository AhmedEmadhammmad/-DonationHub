using DonationHub.Application.Common.Interfaces;
using DonationHub.Application.Common.Options;
using DonationHub.Application.Features.Auth.Dtos;
using DonationHub.Domain.Common.Errors;
using DonationHub.Domain.Common.Results;
using DonationHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DonationHub.Application.Features.Auth.RefreshToken;

/// <summary>
/// Executes atomic refresh token rotation and issues fresh token pairs (IDN-05, IDN-06, IDN-10).
/// </summary>
public sealed class RefreshTokenCommandHandler(
    IAppDbContext context,
    IGenericRepository<Domain.Entities.RefreshToken> refreshTokenRepository,
    IUnitOfWork unitOfWork,
    ITokenService tokenService,
    IOptions<JwtOptions> jwtOptions,
    TimeProvider timeProvider) : IRequestHandler<RefreshTokenCommand, Result<AuthResponseDto>>
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<Result<AuthResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var incomingHash = tokenService.HashToken(request.RefreshToken);
        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;

        var storedToken = await context.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == incomingHash, cancellationToken);

        if (storedToken is null || !storedToken.IsActive(nowUtc))
        {
            return Result.Failure<AuthResponseDto>(AuthErrors.RefreshTokenInvalid);
        }

        var user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == storedToken.UserId, cancellationToken);

        if (user is null || !user.IsActive)
        {
            return Result.Failure<AuthResponseDto>(AuthErrors.AccountDeactivated);
        }

        var newAccessToken = tokenService.GenerateAccessToken(user);
        var (newRawRefreshToken, newTokenHash) = tokenService.GenerateRefreshToken();
        var newExpiresAtUtc = nowUtc.AddDays(_jwtOptions.RefreshTokenExpiryDays);

        var newRefreshToken = Domain.Entities.RefreshToken.Create(
            id: Guid.NewGuid(),
            userId: user.Id,
            tokenHash: newTokenHash,
            nowUtc: nowUtc,
            expiresAtUtc: newExpiresAtUtc);

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var claimed = await context.RefreshTokens
                .Where(t => t.Id == storedToken.Id && t.RevokedAt == null)
                .ExecuteUpdateAsync(setters => setters
                        .SetProperty(t => t.RevokedAt, nowUtc)
                        .SetProperty(t => t.ReplacedByTokenId, newRefreshToken.Id),
                    cancellationToken);

            if (claimed == 0)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure<AuthResponseDto>(AuthErrors.RefreshTokenInvalid);
            }

            await refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }

        var response = new AuthResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRawRefreshToken,
            ExpiresIn = _jwtOptions.ExpiryMinutes * 60,
            TokenType = "Bearer",
            User = new AuthUserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                PreferredLanguage = user.PreferredLanguage
            }
        };

        return Result.Success(response);
    }
}
