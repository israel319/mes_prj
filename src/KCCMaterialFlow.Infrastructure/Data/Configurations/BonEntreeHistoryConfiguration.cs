using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité BonEntreeHistory.
/// </summary>
public class BonEntreeHistoryConfiguration : IEntityTypeConfiguration<BonEntreeHistory>
{
    public void Configure(EntityTypeBuilder<BonEntreeHistory> builder)
    {
        builder.ToTable("T_BonEntreeHistory", "dbo");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.Id)
            .HasColumnName("IdHistory")
            .ValueGeneratedOnAdd();

        builder.Property(h => h.BonId)
            .HasColumnName("BonId")
            .IsRequired();

        builder.Property(h => h.Action)
            .HasColumnName("Action")
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(h => h.ActionDescription)
            .HasColumnName("ActionDescription")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(h => h.ActionBy)
            .HasColumnName("ActionBy")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(h => h.ActionByNom)
            .HasColumnName("ActionByNom")
            .HasMaxLength(200);

        builder.Property(h => h.ActionDate)
            .HasColumnName("ActionDate")
            .HasDefaultValueSql("GETDATE()");

        builder.Property(h => h.Comment)
            .HasColumnName("Comment")
            .HasMaxLength(1000);

        builder.Property(h => h.StatutAvant)
            .HasColumnName("StatutAvant")
            .HasMaxLength(30);

        builder.Property(h => h.StatutApres)
            .HasColumnName("StatutApres")
            .HasMaxLength(30);

        builder.Property(h => h.ChangementsJson)
            .HasColumnName("ChangementsJson")
            .HasColumnType("nvarchar(max)");

        builder.Property(h => h.AdresseIP)
            .HasColumnName("AdresseIP")
            .HasMaxLength(50);

        // Relation avec le Bon (définie dans BonEntreeConfiguration — ne pas redéfinir ici)

        // Index sur le bon pour récupérer l'historique complet
        builder.HasIndex(h => h.BonId)
            .HasDatabaseName("IX_BonEntreeHistory_Bon");

        // Index sur l'utilisateur pour voir ses actions
        builder.HasIndex(h => h.ActionBy)
            .HasDatabaseName("IX_BonEntreeHistory_ActionBy");

        // Index sur la date pour les requêtes chronologiques
        builder.HasIndex(h => h.ActionDate)
            .HasDatabaseName("IX_BonEntreeHistory_ActionDate");

        // Index sur le type d'action pour les statistiques
        builder.HasIndex(h => h.Action)
            .HasDatabaseName("IX_BonEntreeHistory_Action");

        // Index composite pour l'historique d'un bon trié par date
        builder.HasIndex(h => new { h.BonId, h.ActionDate })
            .HasDatabaseName("IX_BonEntreeHistory_Bon_Date");
    }
}
