using DonationHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DonationHub.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.NameAr)
            .HasMaxLength(Category.MaxNameLength)
            .IsRequired();

        builder.HasIndex(c => c.NameAr)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.Property(c => c.NameEn)
            .HasMaxLength(Category.MaxNameLength)
            .IsRequired();

        builder.HasIndex(c => c.NameEn)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.Property(c => c.DescriptionAr)
            .HasMaxLength(1000);

        builder.Property(c => c.DescriptionEn)
            .HasMaxLength(1000);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt);

        builder.Property(c => c.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.DeletedAtUtc);

        // Global query filter (Task 5)
        builder.HasQueryFilter(c => !c.IsDeleted);

        // علاقة 1 إلى متعدد مع منع الحذف التعاقبي (DeleteBehavior.Restrict)
        builder.HasMany<Campaign>()
            .WithOne()
            .HasForeignKey(c => c.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

