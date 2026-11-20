using DonationHub.Application.Common.Interfaces;
using DonationHub.Application.Features.Auth.Dtos;
using DonationHub.Domain.Common.Errors;
using DonationHub.Domain.Common.Results;
using DonationHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DonationHub.Application.Features.Auth.CreateStaff;

/// <summary>
/// Handles administrative staff creation with role assignment (IDN-08).
/// </summary>
public sealed class CreateStaffCommandHandler(
    IAppDbContext context,
    IGenericRepository<ApplicationUser> userRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    TimeProvider timeProvider) : IRequestHandler<CreateStaffCommand, Result<AuthUserDto>>
{
    public async Task<Result<AuthUserDto>> Handle(CreateStaffCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var emailExists = await context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Email == normalizedEmail, cancellationToken);

        if (emailExists)
        {
            return Result.Failure<AuthUserDto>(AuthErrors.EmailAlreadyExists);
        }

        var passwordHash = passwordHasher.HashPassword(request.Password);
        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;

        var user = ApplicationUser.Create(
            id: Guid.NewGuid(),
            email: normalizedEmail,
            userName: normalizedEmail,
            fullName: request.FullName.Trim(),
            passwordHash: passwordHash,
            nowUtc: nowUtc,
            role: request.Role,
            preferredLanguage: request.PreferredLanguage ?? "en");

        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new AuthUserDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role,
            PreferredLanguage = user.PreferredLanguage
        };

        return Result.Success(dto);
    }
}
