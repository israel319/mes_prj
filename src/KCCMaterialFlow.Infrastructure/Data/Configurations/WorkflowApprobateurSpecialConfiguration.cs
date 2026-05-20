using KCCMaterialFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF pour T_WorkflowApprobateurSpecial : Superintendent / OPJ / Identification.
/// </summary>
public class WorkflowApprobateurSpecialConfiguration : IEntityTypeConfiguration<WorkflowApprobateurSpecial>
{
    public void Configure(EntityTypeBuilder<WorkflowApprobateurSpecial> builder)
    {
        builder.ToTable("T_WorkflowApprobateurSpecial", "dbo");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("Id");

        builder.Property(x => x.Type).HasConversion<int>().IsRequired();
        builder.Property(x => x.Ordre).HasDefaultValue(1);
        builder.Property(x => x.EstActif).HasDefaultValue(true);
        builder.Property(x => x.DateCreation).HasDefaultValueSql("GETDATE()");

        builder.HasOne(x => x.Employee)
            .WithMany()
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Site optionnel : NULL = approbateur global (tous sites)
        builder.HasOne(x => x.Site)
            .WithMany()
            .HasForeignKey(x => x.SiteId)
            .OnDelete(DeleteBehavior.Restrict);

        // Unicité par (Type, Employee, Site) pour permettre même employé sur plusieurs sites
        builder.HasIndex(x => new { x.Type, x.EmployeeId, x.SiteId }).IsUnique();
        builder.HasIndex(x => x.Type);
        builder.HasIndex(x => x.SiteId);
    }
}
