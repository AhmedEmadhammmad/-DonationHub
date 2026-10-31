using DonationHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DonationHub.Infrastructure.Persistence.Configurations;

public class PaymentAttemptConfiguration : IEntityTypeConfiguration<PaymentAttempt>
{
    public void Configure(EntityTypeBuilder<PaymentAttempt> builder)
    {
        builder.ToTable("PaymentAttempts");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.StripeCheckoutSessionId)
            .HasMaxLength(255)
            .IsRequired();

        builder.HasIndex(p => p.StripeCheckoutSessionId);

        builder.Property(p => p.StripePaymentIntentId)
            .HasMaxLength(255);

        builder.HasIndex(p => p.StripePaymentIntentId);

        builder.Property(p => p.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt);

        builder.Property(p => p.FailureCode)
            .HasMaxLength(100);

        builder.HasOne<Donation>()
            .WithMany()
            .HasForeignKey(p => p.DonationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

