using KCCMaterialFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

public sealed class AllEmployeeConfiguration : IEntityTypeConfiguration<AllEmployee>
{
    public void Configure(EntityTypeBuilder<AllEmployee> b)
    {
        b.ToTable("T_AllEmployees", "dbo");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("IdAllEmployee");

        b.Property(x => x.EmployeeCode).IsRequired();
        b.HasIndex(x => x.EmployeeCode).IsUnique();
        b.HasIndex(x => x.UserName);
        b.HasIndex(x => x.DepartementCode);
    }
}
