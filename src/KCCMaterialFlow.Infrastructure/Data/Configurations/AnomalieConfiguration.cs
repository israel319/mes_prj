using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using KCCMaterialFlow.Module.Securite.Entities;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité Anomalie
/// </summary>
public class AnomalieConfiguration : IEntityTypeConfiguration<Anomalie>
{
    public void Configure(EntityTypeBuilder<Anomalie> builder)
    {
        builder.ToTable("T_Anomalies", "dbo");

        builder.HasKey(a => a.IdAnomalie);

        builder.Property(a => a.IdAnomalie)
            .HasColumnName("IdAnomalie")
            .ValueGeneratedOnAdd();

        builder.Property(a => a.TypeAnomalie)
            .HasColumnName("TypeAnomalie")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.NiveauGravite)
            .HasColumnName("NiveauGravite")
            .HasMaxLength(20)
            .IsRequired()
            .HasDefaultValue("Moyen");

        builder.Property(a => a.DateSignalement)
            .HasColumnName("DateSignalement")
            .HasColumnType("datetime2")
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        builder.Property(a => a.Description)
            .HasColumnName("Description")
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(a => a.BonId)
            .HasColumnName("BonId");

        builder.Property(a => a.TypeBon)
            .HasColumnName("TypeBon")
            .HasMaxLength(10);

        builder.Property(a => a.NumeroReferenceBon)
            .HasColumnName("NumeroReferenceBon")
            .HasMaxLength(30);

        builder.Property(a => a.ScanId)
            .HasColumnName("ScanId");

        builder.Property(a => a.BarriereId)
            .HasColumnName("BarriereId");

        builder.Property(a => a.EstTraitee)
            .HasColumnName("EstTraitee")
            .HasDefaultValue(false);

        builder.Property(a => a.DateTraitement)
            .HasColumnName("DateTraitement")
            .HasColumnType("datetime2");

        builder.Property(a => a.SignalePar)
            .HasColumnName("SignalePar")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.SignaleParNom)
            .HasColumnName("SignaleParNom")
            .HasMaxLength(200);

        builder.Property(a => a.TraitePar)
            .HasColumnName("TraitePar")
            .HasMaxLength(100);

        builder.Property(a => a.Resolution)
            .HasColumnName("Resolution")
            .HasMaxLength(2000);

        builder.Property(a => a.ActionsCorrectives)
            .HasColumnName("ActionsCorrectives")
            .HasMaxLength(2000);

        // FK vers ScanEvenement
        builder.HasOne(a => a.ScanEvenement)
            .WithMany(s => s.Anomalies)
            .HasForeignKey(a => a.ScanId)
            .OnDelete(DeleteBehavior.Cascade);

        // FK vers Barriere
        builder.HasOne(a => a.Barriere)
            .WithMany()
            .HasForeignKey(a => a.BarriereId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index pour optimiser les requêtes fréquentes
        builder.HasIndex(a => a.DateSignalement)
            .HasDatabaseName("IX_Anomalies_DateSignalement");

        builder.HasIndex(a => a.EstTraitee)
            .HasDatabaseName("IX_Anomalies_EstTraitee");

        builder.HasIndex(a => a.TypeAnomalie)
            .HasDatabaseName("IX_Anomalies_TypeAnomalie");

        builder.HasIndex(a => a.NiveauGravite)
            .HasDatabaseName("IX_Anomalies_NiveauGravite");

        builder.HasIndex(a => a.ScanId)
            .HasDatabaseName("IX_Anomalies_ScanId");

        builder.HasIndex(a => a.BonId)
            .HasDatabaseName("IX_Anomalies_BonId");

        builder.HasIndex(a => a.SignalePar)
            .HasDatabaseName("IX_Anomalies_SignalePar");

        builder.HasIndex(a => new { a.EstTraitee, a.NiveauGravite })
            .HasDatabaseName("IX_Anomalies_EstTraitee_NiveauGravite");
    }
}
