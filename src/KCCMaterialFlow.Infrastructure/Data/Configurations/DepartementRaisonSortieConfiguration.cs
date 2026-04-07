using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF Core pour la table de jonction Département ↔ RaisonSortie.
/// DepartementId nullable = mapping par défaut (tous les départements non-mappés).
/// </summary>
public class DepartementRaisonSortieConfiguration : IEntityTypeConfiguration<DepartementRaisonSortie>
{
    public void Configure(EntityTypeBuilder<DepartementRaisonSortie> builder)
    {
        builder.ToTable("T_DepartementRaisonsSortie", "dbo");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        builder.Property(d => d.DepartementId)
            .HasColumnName("DepartementId");

        builder.Property(d => d.RaisonSortieId)
            .HasColumnName("RaisonSortieId")
            .IsRequired();

        builder.Property(d => d.AutoSelection)
            .HasColumnName("AutoSelection")
            .HasDefaultValue(false);

        builder.Property(d => d.OrdreAffichage)
            .HasColumnName("OrdreAffichage")
            .HasDefaultValue(0);

        // FK vers Departement (nullable — NULL = défaut)
        builder.HasOne(d => d.Departement)
            .WithMany(dept => dept.RaisonsAutorisees)
            .HasForeignKey(d => d.DepartementId)
            .OnDelete(DeleteBehavior.Cascade);

        // FK vers RaisonSortie
        builder.HasOne(d => d.RaisonSortie)
            .WithMany(r => r.DepartementsAutorises)
            .HasForeignKey(d => d.RaisonSortieId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index unique pour éviter les doublons
        builder.HasIndex(d => new { d.DepartementId, d.RaisonSortieId })
            .IsUnique()
            .HasDatabaseName("IX_DeptRaisonSortie_DeptId_RaisonId");

        // Index pour lookup rapide par département
        builder.HasIndex(d => d.DepartementId)
            .HasDatabaseName("IX_DeptRaisonSortie_DeptId");
    }
}
