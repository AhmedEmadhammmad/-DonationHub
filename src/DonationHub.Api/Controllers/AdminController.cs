using DonationHub.Api.Extensions;
using DonationHub.Application.Features.Auth.CreateStaff;
using DonationHub.Application.Features.Auth.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DonationHub.Api.Controllers;

/// <summary>
/// Exposes administrative endpoints restricted to users with the Admin role via CQRS commands.
/// </summary>
[ApiController]
[Route("api/v1/admin")]
[Authorize(Roles = "Admin")]
public class AdminController(ISender mediator) : ControllerBase
{
    /// <summary>
    /// Creates a privileged staff user account (Admin or CampaignManager) under administrator authorization (IDN-08).
    /// </summary>
    [HttpPost("staff")]
    [ProducesResponseType(typeof(AuthUserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateStaff(
        [FromBody] CreateStaffCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : result.ToActionResult(this);
    }
}
