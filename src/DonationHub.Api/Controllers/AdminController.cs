using DonationHub.Application.Common.Exceptions;
using DonationHub.Application.Features.Auth.Dtos;
using DonationHub.Application.Features.Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DonationHub.Api.Controllers;

/// <summary>
/// Exposes administrative endpoints restricted to users with the Admin role.
/// </summary>
[ApiController]
[Route("api/v1/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAuthService _authService;

    /// <summary>
    /// Initializes AdminController with the application authentication service contract.
    /// </summary>
    public AdminController(IAuthService authService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
    }

    /// <summary>
    /// Creates a privileged staff user account (Admin or CampaignManager) under administrator authorization (IDN-08).
    /// </summary>
    [HttpPost("staff")]
    [ProducesResponseType(typeof(AuthUserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateStaff([FromBody] CreateStaffRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authService.CreateStaffAsync(request, cancellationToken);
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
        catch (DbUpdateException ex) when (IsUniqueViolation(ex))
        {
            return Conflict(new
            {
                error = DuplicateEmailException.ErrorCode,
                message = "A user with the specified email address already exists."
            });
        }
    }

    private static bool IsUniqueViolation(DbUpdateException ex)
        => ex.InnerException is SqlException { Number: 2601 or 2627 };
}

