using DonationHub.Application.Features.Auth.Dtos;

namespace DonationHub.Application.Features.Auth.Services;

/// <summary>
/// Service interface defining authentication, account registration, and token lifecycle workflows.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new donor user, validates uniqueness, hashes password, and persists user record (IDN-01).
    /// </summary>
    Task<AuthUserDto> RegisterDonorAsync(RegisterRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticates user credentials, validates active state, and issues access and refresh tokens (IDN-03).
    /// </summary>
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Rotates a valid refresh token, revokes the prior token, and issues a new token pair (IDN-05/06).
    /// </summary>
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes an active refresh token upon user logout (IDN-07).
    /// </summary>
    Task RevokeRefreshTokenAsync(string rawRefreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves user profile details by user identifier for the authenticated caller (IDN-09).
    /// </summary>
    Task<AuthUserDto> GetCurrentUserProfileAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a privileged staff user account (Admin or CampaignManager) under administrator authorization (IDN-08).
    /// </summary>
    Task<AuthUserDto> CreateStaffAsync(CreateStaffRequestDto request, CancellationToken cancellationToken = default);
}
