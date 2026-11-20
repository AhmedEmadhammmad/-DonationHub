using DonationHub.Application.Common.Interfaces;
using DonationHub.Application.Common.Options;
using DonationHub.Application.Features.Auth.Dtos;
using DonationHub.Domain.Common.Errors;
using DonationHub.Domain.Common.Results;
using DonationHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DonationHub.Application.Features.Auth.Login;

/// <summary>
/// Authenticates credentials, checks active status, and issues access and refresh tokens (IDN-03, IDN-10, IDN-12).
/// </summary>
public sealed class LoginCommandHandler(
    IAppDbContext context,
    IGenericRepository<Domain.Entities.RefreshToken> refreshTokenRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IOptions<JwtOptions> jwtOptions,
    TimeProvider timeProvider) : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);

        // IDN-12: Generic error for unknown user or wrong password to prevent user enumeration
        if (user is null || !passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Result.Failure<AuthResponseDto>(AuthErrors.InvalidCredentials);
        }

        // IDN-10: Deactivated account check returns 403 Forbidden
        if (!user.IsActive)
        {
            return Result.Failure<AuthResponseDto>(AuthErrors.AccountDeactivated);
        }

        var accessToken = tokenService.GenerateAccessToken(user);
        var (rawRefreshToken, tokenHash) = tokenService.GenerateRefreshToken();
        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;
        var expiresAtUtc = nowUtc.AddDays(_jwtOptions.RefreshTokenExpiryDays);

        var refreshToken = Domain.Entities.RefreshToken.Create(
            id: Guid.NewGuid(),
            userId: user.Id,
            tokenHash: tokenHash,
            nowUtc: nowUtc,
            expiresAtUtc: expiresAtUtc);

        await refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = rawRefreshToken,
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
