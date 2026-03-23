using KCCMaterialFlow.Module.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using KCCMaterialFlow.Module.BonEntree.Entities;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration Entity Framework pour l'entité TypeMateriel
/// </summary>
public class TypeMaterielConfiguration : IEntityTypeConfiguration<TypeMateriel>
{
    public void Configure(EntityTypeBuilder<TypeMateriel> builder)
    {
        builder.ToTable("T_TypesMateriels", "dbo");

        builder.HasKey(t => t.IdTypeMateriel);

        builder.Property(t => t.CodeType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.NomType)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(t => t.Description)
            .HasMaxLength(500);

        builder.Property(t => t.Categorie)
            .HasMaxLength(100);

        builder.Property(t => t.ChampsPersonnalises)
            .HasMaxLength(2000);

        builder.Property(t => t.WorkflowConfig)
            .HasMaxLength(4000);

        builder.HasIndex(t => t.CodeType)
            .IsUnique()
            .HasDatabaseName("IX_TypesMateriels_CodeType");

        builder.HasIndex(t => new { t.EstActif, t.Ordre })
            .HasDatabaseName("IX_TypesMateriels_EstActif_Ordre");

        // Données de base pour les types de matériel
        builder.HasData(
            new TypeMateriel
            {
                IdTypeMateriel = 1,
                CodeType = "VEHICULE",
                NomType = "Véhicule",
                Description = "Véhicules de société (voitures, camions, engins)",
                Categorie = "Matériel roulant",
                Icone = "bi-truck",
                Couleur = "#0d6efd",
                RequiertApprobationDepartement = true,
                RequiertApprobationDirection = false,
                NiveauxApprobation = 1,
                DureeValiditeDefautJours = 1,
                DureeMaximumJours = 30,
                NumeroSerieObligatoire = true,
                PhotoObligatoire = false,
                Ordre = 1,
                EstActif = true,
                DateCreation = new DateTime(2024, 1, 1)
            },
            new TypeMateriel
            {
                IdTypeMateriel = 2,
                CodeType = "EQUIPEMENT_IT",
                NomType = "Équipement informatique",
                Description = "Ordinateurs, laptops, serveurs, équipements réseau",
                Categorie = "Informatique",
                Icone = "bi-laptop",
                Couleur = "#6610f2",
                RequiertApprobationDepartement = true,
                RequiertApprobationDirection = true,
                NiveauxApprobation = 2,
                DureeValiditeDefautJours = 30,
                DureeMaximumJours = 365,
                NumeroSerieObligatoire = true,
                PhotoObligatoire = true,
                Ordre = 2,
                EstActif = true,
                DateCreation = new DateTime(2024, 1, 1)
            },
            new TypeMateriel
            {
                IdTypeMateriel = 3,
                CodeType = "OUTILLAGE",
                NomType = "Outillage",
                Description = "Outils et équipements de travail",
                Categorie = "Équipement",
                Icone = "bi-tools",
                Couleur = "#fd7e14",
                RequiertApprobationDepartement = true,
                RequiertApprobationDirection = false,
                NiveauxApprobation = 1,
                DureeValiditeDefautJours = 7,
                DureeMaximumJours = 90,
                NumeroSerieObligatoire = false,
                PhotoObligatoire = false,
                Ordre = 3,
                EstActif = true,
                DateCreation = new DateTime(2024, 1, 1)
            },
            new TypeMateriel
            {
                IdTypeMateriel = 4,
                CodeType = "DOCUMENT",
                NomType = "Document",
                Description = "Documents confidentiels, plans, archives",
                Categorie = "Documentation",
                Icone = "bi-file-earmark-text",
                Couleur = "#20c997",
                RequiertApprobationDepartement = true,
                RequiertApprobationDirection = false,
                NiveauxApprobation = 1,
                DureeValiditeDefautJours = 1,
                DureeMaximumJours = 7,
                NumeroSerieObligatoire = false,
                PhotoObligatoire = false,
                Ordre = 4,
                EstActif = true,
                DateCreation = new DateTime(2024, 1, 1)
            },
            new TypeMateriel
            {
                IdTypeMateriel = 5,
                CodeType = "MATERIEL_DIVERS",
                NomType = "Matériel divers",
                Description = "Autre matériel non catégorisé",
                Categorie = "Divers",
                Icone = "bi-box-seam",
                Couleur = "#6c757d",
                RequiertApprobationDepartement = true,
                RequiertApprobationDirection = false,
                NiveauxApprobation = 1,
                DureeValiditeDefautJours = 7,
                DureeMaximumJours = 180,
                NumeroSerieObligatoire = false,
                PhotoObligatoire = false,
                Ordre = 99,
                EstActif = true,
                DateCreation = new DateTime(2024, 1, 1)
            }
        );
    }
}
