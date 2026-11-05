using DonationHub.Application.Common.Exceptions;
using DonationHub.Application.Common.Interfaces;
using DonationHub.Application.Common.Options;
using DonationHub.Application.Features.Auth.Dtos;
using DonationHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DonationHub.Application.Features.Auth.Services;

/// <summary>
/// Implements authentication workflows including registration, login, token rotation, and logout.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IAppDbContext _context;
    private readonly IGenericRepository<ApplicationUser> _userRepository;
    private readonly IGenericRepository<RefreshToken> _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly JwtOptions _jwtOptions;

    /// <summary>
    /// Initializes AuthService with persistence, repositories, unit of work, token service, and options.
    /// </summary>
    public AuthService(
        IAppDbContext context,
        IGenericRepository<ApplicationUser> userRepository,
        IGenericRepository<RefreshToken> refreshTokenRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IOptions<JwtOptions> jwtOptions)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _refreshTokenRepository = refreshTokenRepository ?? throw new ArgumentNullException(nameof(refreshTokenRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _jwtOptions = jwtOptions?.Value ?? throw new ArgumentNullException(nameof(jwtOptions));
    }

    /// <summary>
    /// Registers a new donor user, enforces email uniqueness, hashes password, and persists domain entity (IDN-01).
    /// </summary>
    public async Task<AuthUserDto> RegisterDonorAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        // 1. Case-insensitive email uniqueness check (IDN-02)
        var emailExists = await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Email == normalizedEmail, cancellationToken);

        if (emailExists)
        {
            throw new DuplicateEmailException(normalizedEmail);
        }

        // 2. Cryptographic password hashing (PBKDF2)
        var passwordHash = _passwordHasher.HashPassword(request.Password);

        // 3. Domain entity creation via factory method ensuring invariants (IDN-01: Donor role)
        var user = ApplicationUser.Create(
            id: Guid.NewGuid(),
            email: normalizedEmail,
            userName: normalizedEmail,
            fullName: request.FullName.Trim(),
            passwordHash: passwordHash,
            nowUtc: DateTime.UtcNow,
            role: ApplicationUser.RoleDonor,
            preferredLanguage: request.PreferredLanguage);

        // 4. Persistence via Generic Repository and Unit of Work
        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Safe DTO projection response
        return MapToUserDto(user);
    }

    /// <summary>
    /// Authenticates user credentials, validates account status, and issues access and refresh tokens (IDN-03).
    /// </summary>
    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        // 1. Find user by normalized email
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);

        // 2. Generic error for missing user or invalid password (IDN-12: prevent user enumeration)
        if (user is null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new InvalidCredentialsException();
        }

        // 3. Check if account is deactivated (IDN-10)
        if (!user.IsActive)
        {
            throw new AccountDeactivatedException();
        }

        // 4. Issue short-lived JWT access token
        var accessToken = _tokenService.GenerateAccessToken(user);

        // 5. Issue secure hashed refresh token (IDN-05)
        var (rawRefreshToken, tokenHash) = _tokenService.GenerateRefreshToken();
        var nowUtc = DateTime.UtcNow;
        var expiresAtUtc = nowUtc.AddDays(_jwtOptions.RefreshTokenExpiryDays);

        var refreshToken = RefreshToken.Create(
            id: Guid.NewGuid(),
            userId: user.Id,
            tokenHash: tokenHash,
            nowUtc: nowUtc,
            expiresAtUtc: expiresAtUtc);

        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = rawRefreshToken,
            ExpiresIn = _jwtOptions.ExpiryMinutes * 60,
            TokenType = "Bearer",
            User = MapToUserDto(user)
        };
    }

    /// <summary>
    /// Rotates refresh tokens, invalidates previous token, and returns new access/refresh pair (IDN-05/06).
    /// </summary>
    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // 1. Hash incoming token for lookup
        var incomingHash = _tokenService.HashToken(request.RefreshToken);
        var nowUtc = DateTime.UtcNow;

        // 2. Load stored token entity for mutation (tracked)
        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == incomingHash, cancellationToken);

        // 3. Validate existence, revocation, and expiration (IDN-06)
        if (storedToken is null || !storedToken.IsActive(nowUtc))
        {
            throw new InvalidRefreshTokenException();
        }

        // 4. Load associated user and ensure active state (IDN-10)
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == storedToken.UserId, cancellationToken);

        if (user is null || !user.IsActive)
        {
            throw new AccountDeactivatedException();
        }

        // 5. Generate new access token and new refresh token
        var newAccessToken = _tokenService.GenerateAccessToken(user);
        var (newRawRefreshToken, newTokenHash) = _tokenService.GenerateRefreshToken();
        var newExpiresAtUtc = nowUtc.AddDays(_jwtOptions.RefreshTokenExpiryDays);

        // 6. Rotate atomically: only ONE concurrent request may claim the token (single-use)
        var newRefreshToken = RefreshToken.Create(
            id: Guid.NewGuid(),
            userId: user.Id,
            tokenHash: newTokenHash,
            nowUtc: nowUtc,
            expiresAtUtc: newExpiresAtUtc);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var claimed = await _context.RefreshTokens
                .Where(t => t.Id == storedToken.Id && t.RevokedAt == null)
                .ExecuteUpdateAsync(setters => setters
                        .SetProperty(t => t.RevokedAt, nowUtc)
                        .SetProperty(t => t.ReplacedByTokenId, newRefreshToken.Id),
                    cancellationToken);

            if (claimed == 0)
            {
                // Another concurrent request already claimed this token — replay attempt
                throw new InvalidRefreshTokenException();
            }

            await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }

        return new AuthResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRawRefreshToken,
            ExpiresIn = _jwtOptions.ExpiryMinutes * 60,
            TokenType = "Bearer",
            User = MapToUserDto(user)
        };
    }

    /// <summary>
    /// Revokes an existing refresh token on logout (IDN-07).
    /// </summary>
    public async Task RevokeRefreshTokenAsync(string rawRefreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(rawRefreshToken))
            return;

        var incomingHash = _tokenService.HashToken(rawRefreshToken);
        var nowUtc = DateTime.UtcNow;

        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == incomingHash, cancellationToken);

        if (storedToken is not null && storedToken.RevokedAt is null)
        {
            storedToken.Revoke(nowUtc);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Retrieves authenticated user profile details by identifier (IDN-09).
    /// </summary>
    public async Task<AuthUserDto> GetCurrentUserProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null || !user.IsActive)
        {
            throw new AccountDeactivatedException();
        }

        return MapToUserDto(user);
    }

    /// <summary>
    /// Creates a privileged staff user account with assigned role (Admin or CampaignManager) (IDN-08).
    /// </summary>
    public async Task<AuthUserDto> CreateStaffAsync(CreateStaffRequestDto request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        // 1. Case-insensitive email uniqueness check
        var emailExists = await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Email == normalizedEmail, cancellationToken);

        if (emailExists)
        {
            throw new DuplicateEmailException(normalizedEmail);
        }

        // 2. Cryptographic password hashing
        var passwordHash = _passwordHasher.HashPassword(request.Password);

        // 3. Domain entity creation ensuring invariants
        var user = ApplicationUser.Create(
            id: Guid.NewGuid(),
            email: normalizedEmail,
            userName: normalizedEmail,
            fullName: request.FullName.Trim(),
            passwordHash: passwordHash,
            nowUtc: DateTime.UtcNow,
            role: request.Role,
            preferredLanguage: request.PreferredLanguage);

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToUserDto(user);
    }

    private static AuthUserDto MapToUserDto(ApplicationUser user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        FullName = user.FullName,
        Role = user.Role,
        PreferredLanguage = user.PreferredLanguage
    };
}
