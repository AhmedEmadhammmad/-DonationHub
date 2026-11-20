using DonationHub.Application.Features.Auth.Dtos;
using DonationHub.Domain.Common.Results;
using MediatR;

namespace DonationHub.Application.Features.Auth.CreateStaff;

/// <summary>
/// Command creating privileged staff user accounts under Admin authorization (IDN-08).
/// </summary>
public sealed record CreateStaffCommand(
    string Email,
    string FullName,
    string Password,
    string Role,
    string? PreferredLanguage = "en") : IRequest<Result<AuthUserDto>>;
