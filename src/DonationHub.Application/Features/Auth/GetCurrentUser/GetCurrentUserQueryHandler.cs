using DonationHub.Application.Common.Interfaces;
using DonationHub.Application.Features.Auth.Dtos;
using DonationHub.Domain.Common.Errors;
using DonationHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DonationHub.Application.Features.Auth.GetCurrentUser;

/// <summary>
/// Handles profile query through direct read path, verifying user existence and active status (IDN-09, IDN-10).
/// </summary>
public sealed class GetCurrentUserQueryHandler(IAppDbContext context)
    : IRequestHandler<GetCurrentUserQuery, Result<AuthUserDto>>
{
    public async Task<Result<AuthUserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .AsNoTracking()
            .Where(u => u.Id == request.UserId && u.IsActive)
            .Select(u => new AuthUserDto
            {
                Id = u.Id,
                Email = u.Email,
                FullName = u.FullName,
                Role = u.Role,
                PreferredLanguage = u.PreferredLanguage
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return Result.Failure<AuthUserDto>(AuthErrors.InvalidCredentials);
        }

        return Result.Success(user);
    }
}
