using Dima.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dima.Api.Data.Mappings;

public class TransactionMapping : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title)
            .HasColumnName("Title")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(c => c.Type)
            .HasColumnName("Type")
            .HasColumnType("SMALLINT")
            .IsRequired();

        builder.Property(c => c.Amount)
            .HasColumnName("Amount")
            .HasColumnType("MONEY")
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        builder.Property(c => c.PaidOrReceivedAt)
            .HasColumnName("PaidOrReceivedAt")
            .IsRequired(false);

        builder.Property(c => c.UserId)
            .HasColumnName("UserId")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(160)
            .IsRequired();

    }
}
