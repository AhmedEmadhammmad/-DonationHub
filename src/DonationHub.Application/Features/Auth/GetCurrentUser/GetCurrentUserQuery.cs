using DonationHub.Application.Features.Auth.Dtos;
using DonationHub.Domain.Common.Results;
using MediatR;

namespace DonationHub.Application.Features.Auth.GetCurrentUser;

/// <summary>
/// Query retrieving profile details for the currently authenticated user (IDN-09).
/// </summary>
public sealed record GetCurrentUserQuery(Guid UserId) : IRequest<Result<AuthUserDto>>;
