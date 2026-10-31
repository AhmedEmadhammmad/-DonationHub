using DonationHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DonationHub.Infrastructure.Persistence.Configurations;

public class ProcessedStripeEventConfiguration : IEntityTypeConfiguration<ProcessedStripeEvent>
{
    public void Configure(EntityTypeBuilder<ProcessedStripeEvent> builder)
    {
        builder.ToTable("ProcessedStripeEvents");

        builder.HasKey(e => e.EventId);

        builder.Property(e => e.EventId)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.Type)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.ProcessedAt)
            .IsRequired();
    }
}

