using Dima.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dima.Api.Data.Mappings;

public class VoucherMapping : IEntityTypeConfiguration<Voucher>
{
    public void Configure(EntityTypeBuilder<Voucher> builder)
    {
        builder.ToTable("Vouchers");
        
        builder.HasKey(voucher => voucher.Id);
        
        builder.Property(voucher => voucher.Number)
            .IsRequired()
            .HasColumnType("CHAR")
            .HasMaxLength(8);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasColumnType("NVARCHAR")
            .HasMaxLength(80);
        
        builder.Property(x => x.Description)
            .IsRequired(false)
            .HasColumnType("NVARCHAR")
            .HasMaxLength(255);
        
        builder.Property(x => x.Amount)
            .IsRequired()
            .HasColumnType("MONEY");
        
        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasColumnType("BIT");
        
        builder.Property(x => x.StartDate)
            .IsRequired(false)
            .HasColumnType("DATETIME2");
        
        builder.Property(x => x.EndDate)
            .IsRequired(false)
            .HasColumnType("DATETIME2");
    }
}