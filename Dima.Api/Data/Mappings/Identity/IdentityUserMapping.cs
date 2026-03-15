using Dima.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dima.Api.Data.Mappings.Identity;

public class IdentityUserMapping : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("IdentityUser");
        builder.HasKey(a => a.Id);

        builder.HasIndex(a => a.NormalizedEmail).IsUnique();
        builder.HasIndex(a => a.UserName).IsUnique();

        builder.Property(a => a.Email).HasMaxLength(180);

        //NormalizedEmail -> email em maiusculo
        builder.Property(a => a.NormalizedEmail).HasMaxLength(180);
        builder.Property(a => a.UserName).HasMaxLength(180);
        //NormalizedUserName -> userName em maiusculo
        builder.Property(a => a.NormalizedUserName).HasMaxLength(180);
        builder.Property(a => a.PhoneNumber).HasMaxLength(20);
        //ConcurrencyStamp -> registro de hora ao criar o registro na tabela
        builder.Property(a => a.ConcurrencyStamp).IsConcurrencyToken();

        builder.HasMany<IdentityUserClaim<long>>().WithOne().HasForeignKey(uc => uc.UserId).IsRequired();
        builder.HasMany<IdentityUserLogin<long>>().WithOne().HasForeignKey(ul => ul.UserId).IsRequired();
        builder.HasMany<IdentityUserToken<long>>().WithOne().HasForeignKey(ut => ut.UserId).IsRequired();
        builder.HasMany<IdentityUserRole<long>>().WithOne().HasForeignKey(ur => ur.UserId).IsRequired();



    }
}
