using Dima.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dima.Api.Data.Mappings;

public class CategoryMapping : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title)
            .HasColumnName("Title")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasColumnName("Description")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(c => c.UserId)
            .HasColumnName("UserId")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(160)
            .IsRequired(true);

    }
}
