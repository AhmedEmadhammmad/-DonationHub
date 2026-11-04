using DonationHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DonationHub.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework Core Fluent API mapping configuration for the Campaign entity.
/// </summary>
public class CampaignConfiguration : IEntityTypeConfiguration<Campaign>
{
    /// <summary>
    /// Configures table schema, constraints, soft delete filter, and foreign key relations for Campaign.
    /// </summary>
    public void Configure(EntityTypeBuilder<Campaign> builder)
    {
        builder.ToTable("Campaigns");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.TitleAr)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.TitleEn)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.DescriptionAr)
            .IsRequired();

        builder.Property(c => c.DescriptionEn)
            .IsRequired();

        // دقة الحقول المالية 18, 2
        builder.Property(c => c.GoalAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(c => c.RaisedAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(c => c.EndDate)
            .IsRequired();

        builder.Property(c => c.BannerFileName)
            .HasMaxLength(500);

        builder.Property(c => c.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt);

        builder.Property(c => c.IsDeleted)
            .IsRequired();

        builder.Property(c => c.DeletedAtUtc);


        builder.HasQueryFilter(c => !c.IsDeleted);

        // Optimistic Concurrency Token
        builder.Property(c => c.RowVersion)
            .IsRowVersion();

        // علاقة الحملة بالمستخدم المنشئ (منع الحذف التعاقبي)
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(c => c.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // علاقة الحملة بالتصنيف (منع الحذف التعاقبي)
        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(c => c.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

