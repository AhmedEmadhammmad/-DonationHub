using DonationHub.Application.Features.Auth.Dtos;
using DonationHub.Domain.Common.Results;
using MediatR;

namespace DonationHub.Application.Features.Auth.Login;

/// <summary>
/// Command submitting user credentials for authentication and token issuance (IDN-03).
/// </summary>
public sealed record LoginCommand(string Email, string Password) : IRequest<Result<AuthResponseDto>>;
