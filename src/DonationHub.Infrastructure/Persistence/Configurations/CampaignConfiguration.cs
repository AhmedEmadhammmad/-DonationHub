using DonationHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DonationHub.Infrastructure.Persistence.Configurations;

public class CampaignConfiguration : IEntityTypeConfiguration<Campaign>
{
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

