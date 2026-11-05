using DonationHub.Application.Common.Interfaces;
using DonationHub.Application.Common.Options;
using DonationHub.Domain.Entities;
using DonationHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DonationHub.Api.Extensions;

/// <summary>
/// Extension methods for application initialization, database migrations, and identity seed data.
/// </summary>
public static class MigrationExtensions
{
    /// <summary>
    /// Seeds initial platform administrator user account at startup using configured options (Task 7: IDN-11).
    /// </summary>
    public static async Task SeedAdminUserAsync(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var adminOptions = scope.ServiceProvider.GetRequiredService<IOptions<SeedAdminOptions>>().Value;
        var logger = scope.ServiceProvider.GetService<ILoggerFactory>()?.CreateLogger(nameof(MigrationExtensions));

        if (string.IsNullOrWhiteSpace(adminOptions.Email) || string.IsNullOrWhiteSpace(adminOptions.Password))
        {
            return;
        }

        var normalizedEmail = adminOptions.Email.Trim().ToLowerInvariant();
        var adminExists = await context.Users
            .IgnoreQueryFilters()
            .AnyAsync(u => u.Email == normalizedEmail);

        if (!adminExists)
        {
            var passwordHash = passwordHasher.HashPassword(adminOptions.Password);
            var adminUser = ApplicationUser.Create(
                id: Guid.NewGuid(),
                email: normalizedEmail,
                userName: normalizedEmail,
                fullName: string.IsNullOrWhiteSpace(adminOptions.FullName) ? "Platform Administrator" : adminOptions.FullName.Trim(),
                passwordHash: passwordHash,
                nowUtc: DateTime.UtcNow,
                role: ApplicationUser.RoleAdmin,
                preferredLanguage: "ar");

            await context.Users.AddAsync(adminUser);
            await context.SaveChangesAsync();

            logger?.LogInformation("Successfully seeded default Admin account: {Email}", normalizedEmail);
        }
    }

    /// <summary>
    /// Seeds the five default bilingual categories if missing in Development environment (SRS §9.1).
    /// </summary>
    public static async Task SeedDefaultCategoriesAsync(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetService<ILoggerFactory>()?.CreateLogger(nameof(MigrationExtensions));

        var defaultCategories = new (string NameAr, string NameEn, string DescAr, string DescEn)[]
        {
            ("صحة", "Health", "دعم الرعاية الصحية والعمليات الجراحية", "Healthcare and medical assistance"),
            ("تعليم", "Education", "دعم التعليم وبناء المدارس والمنح الدراسية", "Educational support and scholarships"),
            ("إغاثة", "Relief", "الإغاثة العاجلة للمتضررين من الأزمات والكوارث", "Emergency relief for crisis-affected communities"),
            ("أيتام", "Orphans", "كفالة ورعاية الأيتام وتوفير احتياجاتهم", "Orphan sponsorship and comprehensive care"),
            ("بيئة", "Environment", "مشاريع حماية البيئة والتنمية المستدامة", "Environmental protection and sustainability")
        };

        var nowUtc = DateTime.UtcNow;
        var addedAny = false;

        foreach (var (nameAr, nameEn, descAr, descEn) in defaultCategories)
        {
            var exists = await context.Categories
                .AnyAsync(c => c.NameAr == nameAr || c.NameEn == nameEn);

            if (!exists)
            {
                var category = Category.Create(
                    id: Guid.NewGuid(),
                    nameAr: nameAr,
                    nameEn: nameEn,
                    nowUtc: nowUtc,
                    descriptionAr: descAr,
                    descriptionEn: descEn);

                await context.Categories.AddAsync(category);
                addedAny = true;
            }
        }

        if (addedAny)
        {
            await context.SaveChangesAsync();
            logger?.LogInformation("Successfully seeded default bilingual categories.");
        }
    }

    /// <summary>
    /// Seeds development-only accounts (Campaign Manager + Donor) — Sprint 1 deliverable.
    /// Uses the configured development password; never hard-coded.
    /// </summary>
    public static async Task SeedDevelopmentDataAsync(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        if (!app.Environment.IsDevelopment())
        {
            return;
        }

        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var adminOptions = scope.ServiceProvider.GetRequiredService<IOptions<SeedAdminOptions>>().Value;
        var logger = scope.ServiceProvider.GetService<ILoggerFactory>()?.CreateLogger(nameof(MigrationExtensions));

        if (string.IsNullOrWhiteSpace(adminOptions.Password))
        {
            logger?.LogWarning("Dev seed skipped: SeedAdmin:Password is not configured.");
            return;
        }

        var devUsers = new (string Email, string FullName, string Role, string Lang)[]
        {
            ("campaign.manager@donationhub.org", "Campaign Manager", ApplicationUser.RoleCampaignManager, "en"),
            ("donor@donationhub.org",            "Demo Donor",       ApplicationUser.RoleDonor,           "ar")
        };

        var addedAny = false;
        foreach (var (email, fullName, role, lang) in devUsers)
        {
            if (await context.Users.AnyAsync(u => u.Email == email))
            {
                continue;
            }

            var user = ApplicationUser.Create(
                id: Guid.NewGuid(),
                email: email,
                userName: email,
                fullName: fullName,
                passwordHash: passwordHasher.HashPassword(adminOptions.Password),
                nowUtc: DateTime.UtcNow,
                role: role,
                preferredLanguage: lang);

            await context.Users.AddAsync(user);
            addedAny = true;
        }

        if (addedAny)
        {
            await context.SaveChangesAsync();
            logger?.LogInformation("Successfully seeded development users (CampaignManager + Donor).");
        }
    }
}
