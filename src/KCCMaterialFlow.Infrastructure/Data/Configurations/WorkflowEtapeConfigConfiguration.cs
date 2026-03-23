using KCCMaterialFlow.Module.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

public class WorkflowEtapeConfigConfiguration : IEntityTypeConfiguration<WorkflowEtapeConfig>
{
    public void Configure(EntityTypeBuilder<WorkflowEtapeConfig> builder)
    {
        builder.ToTable("T_WorkflowEtapesConfig", "dbo");

        builder.HasKey(x => x.IdWorkflowEtapeConfig);

        builder.Property(x => x.IdWorkflowEtapeConfig)
            .HasColumnName("IdWorkflowEtapeConfig")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.BonType)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.RaisonSortieCode)
            .HasMaxLength(50);

        builder.Property(x => x.OrdreEtape)
            .IsRequired();

        builder.Property(x => x.RoleCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.NomEtape)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.EstActif)
            .HasDefaultValue(true);

        builder.Property(x => x.DateCreation)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(x => x.ModifieParLogin)
            .HasMaxLength(100);

        builder.HasIndex(x => new { x.BonType, x.RaisonSortieCode, x.OrdreEtape })
            .HasDatabaseName("IX_WorkflowEtapesConfig_BonType_Raison_Ordre");

        builder.HasIndex(x => new { x.BonType, x.RaisonSortieCode, x.EstActif })
            .HasDatabaseName("IX_WorkflowEtapesConfig_BonType_Raison_Actif");
    }
}
