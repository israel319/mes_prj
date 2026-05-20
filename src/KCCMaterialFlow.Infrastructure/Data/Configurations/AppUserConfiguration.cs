using KCCMaterialFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

public sealed class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> b)
    {
        b.ToTable("T_Users", "dbo");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("IdUser");

        b.Property(x => x.Login)
            .IsRequired()
            .HasMaxLength(150);
        b.HasIndex(x => x.Login).IsUnique();

        b.Property(x => x.NiveauAdmin)
            .HasConversion<int>();

        b.HasOne(x => x.Employee)
            .WithMany()
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
