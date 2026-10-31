using DonationHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DonationHub.Infrastructure.Persistence.Configurations;

public class DonationConfiguration : IEntityTypeConfiguration<Donation>
{
    public void Configure(EntityTypeBuilder<Donation> builder)
    {
        builder.ToTable("Donations");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(d => d.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(d => d.CreatedAt)
            .IsRequired();

        builder.Property(d => d.CompletedAt);

        builder.Property(d => d.FailureReason)
            .HasMaxLength(1000);

        builder.Property(d => d.ReceiptNumber)
            .HasMaxLength(100);

        builder.Property(d => d.ReceiptFileName)
            .HasMaxLength(500);

        // علاقة 1 إلى متعدد مع Campaign مع منع الحذف التعاقبي
        builder.HasOne<Campaign>()
            .WithMany()
            .HasForeignKey(d => d.CampaignId)
            .OnDelete(DeleteBehavior.Restrict);

        // علاقة مع المتبرع ApplicationUser
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(d => d.DonorUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

