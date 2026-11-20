using DonationHub.Domain.Common.Results;
using MediatR;

namespace DonationHub.Application.Features.Auth.Logout;

/// <summary>
/// Command requesting revocation of an authenticated refresh token (IDN-07).
/// </summary>
public sealed record LogoutCommand(string RefreshToken) : IRequest<Result>;
