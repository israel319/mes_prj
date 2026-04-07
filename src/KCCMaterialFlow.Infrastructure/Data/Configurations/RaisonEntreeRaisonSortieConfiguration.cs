using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF Core pour la table de jonction RaisonEntree ↔ RaisonSortie.
/// </summary>
public class RaisonEntreeRaisonSortieConfiguration : IEntityTypeConfiguration<RaisonEntreeRaisonSortie>
{
    public void Configure(EntityTypeBuilder<RaisonEntreeRaisonSortie> builder)
    {
        builder.ToTable("T_RaisonEntreeRaisonsSortie", "dbo");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        builder.Property(r => r.RaisonEntreeId)
            .HasColumnName("RaisonEntreeId")
            .IsRequired();

        builder.Property(r => r.RaisonSortieId)
            .HasColumnName("RaisonSortieId")
            .IsRequired();

        builder.Property(r => r.AutoSelection)
            .HasColumnName("AutoSelection")
            .HasDefaultValue(false);

        builder.Property(r => r.OrdreAffichage)
            .HasColumnName("OrdreAffichage")
            .HasDefaultValue(0);

        // FK vers RaisonEntree
        builder.HasOne(r => r.RaisonEntree)
            .WithMany(re => re.RaisonsSortieAutorisees)
            .HasForeignKey(r => r.RaisonEntreeId)
            .OnDelete(DeleteBehavior.Cascade);

        // FK vers RaisonSortie
        builder.HasOne(r => r.RaisonSortie)
            .WithMany(rs => rs.RaisonsEntreeAutorisees)
            .HasForeignKey(r => r.RaisonSortieId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index unique pour éviter les doublons
        builder.HasIndex(r => new { r.RaisonEntreeId, r.RaisonSortieId })
            .IsUnique()
            .HasDatabaseName("IX_RaisonEntreeRaisonsSortie_EntreeId_SortieId");

        // Index pour lookup rapide par raison d'entrée
        builder.HasIndex(r => r.RaisonEntreeId)
            .HasDatabaseName("IX_RaisonEntreeRaisonsSortie_EntreeId");
    }
}
