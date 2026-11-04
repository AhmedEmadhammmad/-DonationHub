using System.Security.Claims;
using DonationHub.Application.Common.Exceptions;
using DonationHub.Application.Features.Auth.Dtos;
using DonationHub.Application.Features.Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DonationHub.Api.Controllers;

/// <summary>
/// Exposes authentication HTTP endpoints including registration, login, token refresh, revocation, and profile retrieval.
/// </summary>
[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    /// <summary>
    /// Initializes AuthController with the application authentication service contract.
    /// </summary>
    public AuthController(IAuthService authService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
    }

    /// <summary>
    /// Registers a new donor user account with default Donor role (IDN-01).
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthUserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authService.RegisterDonorAsync(request, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, result);
        }
        catch (DuplicateEmailException ex)
        {
            return Conflict(new
            {
                error = DuplicateEmailException.ErrorCode,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Authenticates credentials and issues signed access and refresh tokens (IDN-03).
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authService.LoginAsync(request, cancellationToken);
            return Ok(result);
        }
        catch (InvalidCredentialsException ex)
        {
            return Unauthorized(new
            {
                error = InvalidCredentialsException.ErrorCode,
                message = ex.Message
            });
        }
        catch (AccountDeactivatedException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new
            {
                error = AccountDeactivatedException.ErrorCode,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Rotates a valid refresh token and issues a refreshed access/refresh pair (IDN-05/06).
    /// </summary>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authService.RefreshTokenAsync(request, cancellationToken);
            return Ok(result);
        }
        catch (InvalidRefreshTokenException ex)
        {
            return Unauthorized(new
            {
                error = InvalidRefreshTokenException.ErrorCode,
                message = ex.Message
            });
        }
        catch (AccountDeactivatedException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new
            {
                error = AccountDeactivatedException.ErrorCode,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Revokes an active refresh token upon user logout (IDN-07).
    /// </summary>
    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto? request, CancellationToken cancellationToken)
    {
        if (request is not null && !string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            await _authService.RevokeRefreshTokenAsync(request.RefreshToken, cancellationToken);
        }

        return NoContent();
    }

    /// <summary>
    /// Retrieves user profile details for the currently authenticated caller (IDN-09).
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(AuthUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (string.IsNullOrWhiteSpace(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized(new
            {
                error = "Auth.InvalidToken",
                message = "User identifier claim is missing or invalid."
            });
        }

        try
        {
            var profile = await _authService.GetCurrentUserProfileAsync(userId, cancellationToken);
            return Ok(profile);
        }
        catch (AccountDeactivatedException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new
            {
                error = AccountDeactivatedException.ErrorCode,
                message = ex.Message
            });
        }
    }
}
