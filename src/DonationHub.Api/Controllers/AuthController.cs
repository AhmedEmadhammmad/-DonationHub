using System.Security.Claims;
using DonationHub.Api.Extensions;
using DonationHub.Application.Features.Auth.Dtos;
using DonationHub.Application.Features.Auth.GetCurrentUser;
using DonationHub.Application.Features.Auth.Login;
using DonationHub.Application.Features.Auth.Logout;
using DonationHub.Application.Features.Auth.RefreshToken;
using DonationHub.Application.Features.Auth.RegisterDonor;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DonationHub.Api.Controllers;

/// <summary>
/// Exposes authentication HTTP endpoints including registration, login, token refresh, revocation, and profile retrieval via CQRS.
/// </summary>
[ApiController]
[Route("api/v1/auth")]
public class AuthController(ISender mediator) : ControllerBase
{
    /// <summary>
    /// Registers a new donor user account with default Donor role (IDN-01).
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthUserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterDonorCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : result.ToActionResult(this);
    }

    /// <summary>
    /// Authenticates credentials and issues signed access and refresh tokens (IDN-03).
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Login(
        [FromBody] LoginCommand command,
        CancellationToken cancellationToken) =>
        (await mediator.Send(command, cancellationToken)).ToActionResult(this);

    /// <summary>
    /// Rotates a valid refresh token and issues a refreshed access/refresh pair (IDN-05/06).
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshTokenCommand command,
        CancellationToken cancellationToken) =>
        (await mediator.Send(command, cancellationToken)).ToActionResult(this);

    /// <summary>
    /// Revokes an authenticated refresh token to terminate session (IDN-07).
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout(
        [FromBody] LogoutCommand command,
        CancellationToken cancellationToken) =>
        (await mediator.Send(command, cancellationToken)).ToActionResult(this);

    /// <summary>
    /// Retrieves authenticated user profile details by identifier (IDN-09).
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(AuthUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var result = await mediator.Send(new GetCurrentUserQuery(userId), cancellationToken);
        return result.ToActionResult(this);
    }
}
