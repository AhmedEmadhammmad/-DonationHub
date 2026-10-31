using DonationHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DonationHub.Infrastructure.Persistence.Configurations;

public class IdempotencyRecordConfiguration : IEntityTypeConfiguration<IdempotencyRecord>
{
    public void Configure(EntityTypeBuilder<IdempotencyRecord> builder)
    {
        builder.ToTable("IdempotencyRecords");

        builder.HasKey(i => i.Key);

        builder.Property(i => i.Key)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(i => i.UserId)
            .IsRequired();

        builder.Property(i => i.RequestHash)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(i => i.ResponseStatusCode)
            .IsRequired();

        builder.Property(i => i.ResponseBody)
            .IsRequired();

        builder.Property(i => i.CreatedAt)
            .IsRequired();

        builder.Property(i => i.ExpiresAt)
            .IsRequired();

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(i => i.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

