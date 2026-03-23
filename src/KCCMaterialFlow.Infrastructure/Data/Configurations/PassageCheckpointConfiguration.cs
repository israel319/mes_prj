using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KCCMaterialFlow.Module.Shared.Entities;
using KCCMaterialFlow.Domain.Enums;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité PassageCheckpoint (table dbo.PassagesCheckpoint)
/// </summary>
public class PassageCheckpointConfiguration : IEntityTypeConfiguration<PassageCheckpoint>
{
    public void Configure(EntityTypeBuilder<PassageCheckpoint> builder)
    {
        builder.ToTable("T_PassagesCheckpoint", "dbo");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.TypeBon)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(p => p.BonId)
            .IsRequired();

        builder.Property(p => p.NumeroReference)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.CheckpointId)
            .IsRequired();

        builder.Property(p => p.Statut)
            .IsRequired();

        builder.Property(p => p.ScannePar)
            .HasMaxLength(100);

        builder.Property(p => p.DescriptionAnomalie)
            .HasMaxLength(500);

        builder.Property(p => p.Observations)
            .HasMaxLength(500);

        builder.Property(p => p.CoordonneeGPS)
            .HasMaxLength(100);

        builder.HasOne(p => p.Checkpoint)
            .WithMany()
            .HasForeignKey(p => p.CheckpointId);
    }
}
