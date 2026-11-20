using DonationHub.Application.Features.Auth.Dtos;
using DonationHub.Domain.Common.Results;
using MediatR;

namespace DonationHub.Application.Features.Auth.RefreshToken;

/// <summary>
/// Command requesting atomic token rotation using an active refresh token (IDN-05, IDN-06).
/// </summary>
public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<Result<AuthResponseDto>>;
