using System.Security.Claims;
using DonationHub.Api.Extensions;
using DonationHub.Application.Features.Campaigns.CreateCampaign;
using DonationHub.Application.Features.Campaigns.DeleteCampaign;
using DonationHub.Application.Features.Campaigns.Dtos;
using DonationHub.Application.Features.Campaigns.GetActiveCampaigns;
using DonationHub.Application.Features.Campaigns.GetCampaignById;
using DonationHub.Application.Features.Campaigns.GetSuccessStories;
using DonationHub.Application.Features.Campaigns.UpdateCampaign;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DonationHub.Api.Controllers;

/// <summary>
/// Exposes crowdfunding campaign management and public browsing endpoints via CQRS slices (CMP-01...CMP-06).
/// </summary>
[ApiController]
[Route("api/v1/campaigns")]
public class CampaignsController(ISender mediator) : ControllerBase
{
    /// <summary>
    /// Creates a new campaign under Administrator authorization (CMP-01, CMP-06).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CampaignAdminDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateCampaign(
        [FromBody] CreateCampaignRequestDto request,
        CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdClaim, out var createdByUserId))
        {
            return Unauthorized();
        }

        var command = new CreateCampaignCommand(
            request.TitleAr,
            request.TitleEn,
            request.DescriptionAr,
            request.DescriptionEn,
            request.GoalAmount,
            request.EndDate,
            request.CategoryId,
            createdByUserId);

        var result = await mediator.Send(command, cancellationToken);
        return result.ToCreatedAtActionResult(
            this,
            nameof(GetCampaignById),
            new { id = result.IsSuccess ? result.Value.Id : Guid.Empty });
    }

    /// <summary>
    /// Updates campaign details and enforces optimistic concurrency using RowVersion token (CMP-02, CMP-10).
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CampaignAdminDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateCampaign(
        Guid id,
        [FromBody] UpdateCampaignRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCampaignCommand(
            id,
            request.TitleAr,
            request.TitleEn,
            request.DescriptionAr,
            request.DescriptionEn,
            request.GoalAmount,
            request.EndDate,
            request.CategoryId,
            request.RowVersion);

        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Soft deletes an existing campaign (CMP-03).
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCampaign(Guid id, CancellationToken cancellationToken) =>
        (await mediator.Send(new DeleteCampaignCommand(id), cancellationToken)).ToActionResult(this);

    /// <summary>
    /// Retrieves active public campaigns localized by Accept-Language header (CMP-04, CMP-11).
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<CampaignPublicDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActiveCampaigns(CancellationToken cancellationToken)
    {
        var acceptLanguage = Request.Headers.AcceptLanguage.ToString();
        var result = await mediator.Send(new GetActiveCampaignsQuery(acceptLanguage), cancellationToken);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Retrieves completed campaigns (success stories) for public browsing (CMP-04).
    /// </summary>
    [HttpGet("success-stories")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<CampaignPublicDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSuccessStories(CancellationToken cancellationToken)
    {
        var acceptLanguage = Request.Headers.AcceptLanguage.ToString();
        var result = await mediator.Send(new GetSuccessStoriesQuery(acceptLanguage), cancellationToken);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Retrieves details of an individual campaign with calculated metrics (CMP-05, CMP-11).
    /// </summary>
    [HttpGet("{id:guid}", Name = nameof(GetCampaignById))]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CampaignPublicDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCampaignById(Guid id, CancellationToken cancellationToken)
    {
        var acceptLanguage = Request.Headers.AcceptLanguage.ToString();
        var result = await mediator.Send(new GetCampaignByIdQuery(id, acceptLanguage), cancellationToken);
        return result.ToActionResult(this);
    }
}
