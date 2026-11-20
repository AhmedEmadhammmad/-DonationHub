using DonationHub.Application.Common.Interfaces;
using DonationHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DonationHub.Application.Features.Auth.Logout;

/// <summary>
/// Revokes a supplied refresh token in storage to terminate the session (IDN-07).
/// </summary>
public sealed class LogoutCommandHandler(
    IAppDbContext context,
    IUnitOfWork unitOfWork,
    ITokenService tokenService,
    TimeProvider timeProvider) : IRequestHandler<LogoutCommand, Result>
{
    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return Result.Success();
        }

        var incomingHash = tokenService.HashToken(request.RefreshToken);
        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;

        var storedToken = await context.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == incomingHash, cancellationToken);

        if (storedToken is not null && storedToken.RevokedAt is null)
        {
            storedToken.Revoke(nowUtc);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }
}
