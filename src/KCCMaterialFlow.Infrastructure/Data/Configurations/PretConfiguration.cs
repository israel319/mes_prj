using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité Pret.
/// Configure les propriétés spécifiques aux prêts de matériel.
/// </summary>
public class PretConfiguration : IEntityTypeConfiguration<Pret>
{
    public void Configure(EntityTypeBuilder<Pret> builder)
    {
        // Propriétés spécifiques au Pret
        builder.Property(p => p.DateRetourPrevue)
            .HasColumnName("DateRetourPrevue")
            .IsRequired();

        builder.Property(p => p.DateRetourEffective)
            .HasColumnName("DateRetourEffective");

        builder.Property(p => p.EstRetourne)
            .HasColumnName("EstRetourne")
            .HasDefaultValue(false);

        builder.Property(p => p.EtatRetour)
            .HasColumnName("EtatRetour")
            .HasMaxLength(1000);

        builder.Property(p => p.ReceptionnePar)
            .HasColumnName("ReceptionnePar")
            .HasMaxLength(200);

        // JoursRetard est une propriété calculée, pas mappée en base
        builder.Ignore(p => p.JoursRetard);
        builder.Ignore(p => p.EstEnRetard);

        // Index sur la date de retour prévue (pour les alertes)
        builder.HasIndex(p => p.DateRetourPrevue)
            .HasDatabaseName("IX_BonsSortie_DateRetourPrevue");

        // Index sur le statut de retour
        builder.HasIndex(p => p.EstRetourne)
            .HasDatabaseName("IX_BonsSortie_EstRetourne");

        // Index composé pour les prêts en retard (non retournés avec date dépassée)
        builder.HasIndex(p => new { p.EstRetourne, p.DateRetourPrevue })
            .HasDatabaseName("IX_BonsSortie_PretsEnRetard");
    }
}
