using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using KCCMaterialFlow.Module.Securite.Entities;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité ScanEvenement
/// </summary>
public class ScanEvenementConfiguration : IEntityTypeConfiguration<ScanEvenement>
{
    public void Configure(EntityTypeBuilder<ScanEvenement> builder)
    {
        builder.ToTable("T_ScanEvenements", "dbo");

        builder.HasKey(s => s.IdScan);

        builder.Property(s => s.IdScan)
            .HasColumnName("IdScan")
            .ValueGeneratedOnAdd();

        builder.Property(s => s.DateHeureScan)
            .HasColumnName("DateHeureScan")
            .HasColumnType("datetime2")
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        builder.Property(s => s.StatutScan)
            .HasColumnName("StatutScan")
            .HasMaxLength(30)
            .IsRequired()
            .HasDefaultValue("Valid");

        builder.Property(s => s.TypeMouvement)
            .HasColumnName("TypeMouvement")
            .HasMaxLength(20)
            .IsRequired()
            .HasDefaultValue("Sortie");

        builder.Property(s => s.BonId)
            .HasColumnName("BonId");

        builder.Property(s => s.TypeBon)
            .HasColumnName("TypeBon")
            .HasMaxLength(10);

        builder.Property(s => s.NumeroReferenceBon)
            .HasColumnName("NumeroReferenceBon")
            .HasMaxLength(30);

        builder.Property(s => s.BarriereId)
            .HasColumnName("BarriereId")
            .IsRequired();

        builder.Property(s => s.AgentLogin)
            .HasColumnName("AgentLogin")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(s => s.AgentNom)
            .HasColumnName("AgentNom")
            .HasMaxLength(200);

        builder.Property(s => s.QRCodeData)
            .HasColumnName("QRCodeData")
            .HasMaxLength(1000);

        builder.Property(s => s.QRCodeHash)
            .HasColumnName("QRCodeHash")
            .HasMaxLength(128);

        builder.Property(s => s.Message)
            .HasColumnName("Message")
            .HasMaxLength(500);

        builder.Property(s => s.Observations)
            .HasColumnName("Observations")
            .HasMaxLength(1000);

        builder.Property(s => s.AnomalieSignalee)
            .HasColumnName("AnomalieSignalee")
            .HasDefaultValue(false);

        // FK vers Barriere
        builder.HasOne(s => s.Barriere)
            .WithMany()
            .HasForeignKey(s => s.BarriereId)
            .OnDelete(DeleteBehavior.Restrict);

        // Collection d'anomalies
        builder.HasMany(s => s.Anomalies)
            .WithOne(a => a.ScanEvenement)
            .HasForeignKey(a => a.ScanId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index pour optimiser les requêtes fréquentes
        builder.HasIndex(s => s.DateHeureScan)
            .HasDatabaseName("IX_ScanEvenements_DateHeureScan");

        builder.HasIndex(s => s.BarriereId)
            .HasDatabaseName("IX_ScanEvenements_BarriereId");

        builder.HasIndex(s => s.BonId)
            .HasDatabaseName("IX_ScanEvenements_BonId");

        builder.HasIndex(s => s.StatutScan)
            .HasDatabaseName("IX_ScanEvenements_StatutScan");

        builder.HasIndex(s => s.AgentLogin)
            .HasDatabaseName("IX_ScanEvenements_AgentLogin");

        builder.HasIndex(s => new { s.TypeBon, s.BonId })
            .HasDatabaseName("IX_ScanEvenements_TypeBon_BonId");
    }
}
