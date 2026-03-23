using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using KCCMaterialFlow.Module.BonSortie.Entities;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité BonSortieHistory.
/// Trace l'historique des actions sur les bons de sortie.
/// </summary>
public class BonSortieHistoryConfiguration : IEntityTypeConfiguration<BonSortieHistory>
{
    public void Configure(EntityTypeBuilder<BonSortieHistory> builder)
    {
        builder.ToTable("T_BonSortieHistories", "dbo");

        builder.HasKey(h => h.IdHistory);

        builder.Property(h => h.IdHistory)
            .HasColumnName("IdHistory")
            .ValueGeneratedOnAdd();

        builder.Property(h => h.BonSortieId)
            .HasColumnName("BonSortieId")
            .IsRequired();

        builder.Property(h => h.TypeAction)
            .HasColumnName("TypeAction")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(h => h.StatutAvant)
            .HasColumnName("StatutAvant")
            .HasMaxLength(50);

        builder.Property(h => h.StatutApres)
            .HasColumnName("StatutApres")
            .HasMaxLength(50);

        builder.Property(h => h.Description)
            .HasColumnName("Description")
            .HasMaxLength(1000);

        builder.Property(h => h.UtilisateurLogin)
            .HasColumnName("UtilisateurLogin")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(h => h.UtilisateurNom)
            .HasColumnName("UtilisateurNom")
            .HasMaxLength(200);

        builder.Property(h => h.DateAction)
            .HasColumnName("DateAction")
            .HasDefaultValueSql("GETDATE()");

        builder.Property(h => h.AdresseIP)
            .HasColumnName("AdresseIP")
            .HasMaxLength(50);

        // Index sur le bon de sortie pour récupérer l'historique
        builder.HasIndex(h => h.BonSortieId)
            .HasDatabaseName("IX_BonSortieHistories_BonSortieId");

        // Index sur la date d'action pour le tri chronologique
        builder.HasIndex(h => h.DateAction)
            .HasDatabaseName("IX_BonSortieHistories_DateAction");

        // Index sur l'utilisateur pour audit
        builder.HasIndex(h => h.UtilisateurLogin)
            .HasDatabaseName("IX_BonSortieHistories_Utilisateur");
    }
}
