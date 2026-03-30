using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité HistoriqueScan
/// </summary>
public class HistoriqueScanConfiguration : IEntityTypeConfiguration<HistoriqueScan>
{
    public void Configure(EntityTypeBuilder<HistoriqueScan> builder)
    {
        builder.ToTable("T_HistoriqueScans", "dbo");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.Id)
            .HasColumnName("IdHistorique")
            .ValueGeneratedOnAdd();

        builder.Property(h => h.ScanId)
            .HasColumnName("ScanId")
            .IsRequired();

        builder.Property(h => h.DateHeureMouvement)
            .HasColumnName("DateHeureMouvement")
            .HasColumnType("datetime2")
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        builder.Property(h => h.TypeMouvement)
            .HasColumnName("TypeMouvement")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(h => h.TypeBon)
            .HasColumnName("TypeBon")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(h => h.NumeroReferenceBon)
            .HasColumnName("NumeroReferenceBon")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(h => h.CodeBarriere)
            .HasColumnName("CodeBarriere")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(h => h.NomBarriere)
            .HasColumnName("NomBarriere")
            .HasMaxLength(100);

        builder.Property(h => h.Direction)
            .HasColumnName("Direction")
            .HasMaxLength(30);

        builder.Property(h => h.Departement)
            .HasColumnName("Departement")
            .HasMaxLength(100);

        builder.Property(h => h.Provenance)
            .HasColumnName("Provenance")
            .HasMaxLength(200);

        builder.Property(h => h.Destination)
            .HasColumnName("Destination")
            .HasMaxLength(200);

        builder.Property(h => h.NombreMateriels)
            .HasColumnName("NombreMateriels")
            .HasDefaultValue(0);

        builder.Property(h => h.ResumeMateriels)
            .HasColumnName("ResumeMateriels")
            .HasMaxLength(2000);

        builder.Property(h => h.MaterielsJson)
            .HasColumnName("MaterielsJson")
            .HasColumnType("nvarchar(max)");

        builder.Property(h => h.StatutScan)
            .HasColumnName("StatutScan")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(h => h.PassageAutorise)
            .HasColumnName("PassageAutorise")
            .HasDefaultValue(true);

        builder.Property(h => h.AgentLogin)
            .HasColumnName("AgentLogin")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(h => h.AgentNom)
            .HasColumnName("AgentNom")
            .HasMaxLength(200);

        builder.Property(h => h.NomDemandeur)
            .HasColumnName("NomDemandeur")
            .HasMaxLength(200);

        builder.Property(h => h.MatriculeVehicule)
            .HasColumnName("MatriculeVehicule")
            .HasMaxLength(50);

        builder.Property(h => h.Observations)
            .HasColumnName("Observations")
            .HasMaxLength(1000);

        builder.Property(h => h.AnomalieSignalee)
            .HasColumnName("AnomalieSignalee")
            .HasDefaultValue(false);

        builder.Property(h => h.NombreAnomalies)
            .HasColumnName("NombreAnomalies")
            .HasDefaultValue(0);

        // FK vers ScanEvenement
        builder.HasOne<ScanEvenement>()
            .WithMany()
            .HasForeignKey(h => h.ScanId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index pour optimiser les requêtes de la vue Identification
        builder.HasIndex(h => h.DateHeureMouvement)
            .HasDatabaseName("IX_HistoriqueScans_DateHeureMouvement");

        builder.HasIndex(h => h.TypeMouvement)
            .HasDatabaseName("IX_HistoriqueScans_TypeMouvement");

        builder.HasIndex(h => h.TypeBon)
            .HasDatabaseName("IX_HistoriqueScans_TypeBon");

        builder.HasIndex(h => h.NumeroReferenceBon)
            .HasDatabaseName("IX_HistoriqueScans_NumeroReferenceBon");

        builder.HasIndex(h => h.CodeBarriere)
            .HasDatabaseName("IX_HistoriqueScans_CodeBarriere");

        builder.HasIndex(h => h.AgentLogin)
            .HasDatabaseName("IX_HistoriqueScans_AgentLogin");

        builder.HasIndex(h => h.PassageAutorise)
            .HasDatabaseName("IX_HistoriqueScans_PassageAutorise");

        builder.HasIndex(h => h.AnomalieSignalee)
            .HasDatabaseName("IX_HistoriqueScans_AnomalieSignalee");

        // Index composite pour filtres fréquents
        builder.HasIndex(h => new { h.DateHeureMouvement, h.TypeMouvement })
            .HasDatabaseName("IX_HistoriqueScans_Date_TypeMouvement");

        builder.HasIndex(h => new { h.CodeBarriere, h.DateHeureMouvement })
            .HasDatabaseName("IX_HistoriqueScans_Barriere_Date");

        builder.HasIndex(h => new { h.StatutScan, h.PassageAutorise })
            .HasDatabaseName("IX_HistoriqueScans_Statut_Passage");
    }
}
