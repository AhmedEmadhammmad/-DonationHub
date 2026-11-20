using DonationHub.Application.Features.Auth.Dtos;
using DonationHub.Domain.Common.Results;
using MediatR;

namespace DonationHub.Application.Features.Auth.RegisterDonor;

/// <summary>
/// Command requesting public registration of a new donor account (IDN-01).
/// </summary>
public sealed record RegisterDonorCommand(
    string Email,
    string FullName,
    string Password,
    string? PreferredLanguage = "en") : IRequest<Result<AuthUserDto>>;
