using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration Entity Framework pour l'entité TypeMateriel
/// </summary>
public class TypeMaterielConfiguration : IEntityTypeConfiguration<TypeMaterielEntity>
{
    public void Configure(EntityTypeBuilder<TypeMaterielEntity> builder)
    {
        builder.ToTable("T_TypesMateriels", "dbo");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasColumnName("IdTypeMateriel").ValueGeneratedOnAdd();

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

        builder.Property(t => t.WorkflowRoutage)
            .HasColumnName("WorkflowRoutage")
            .HasConversion<int>()
            .HasDefaultValue(WorkflowRoutage.Standard);

        // Données de base pour les types de matériel
        builder.HasData(
            new TypeMaterielEntity
            {
                Id =1,
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
            new TypeMaterielEntity
            {
                Id =2,
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
            new TypeMaterielEntity
            {
                Id =3,
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
            new TypeMaterielEntity
            {
                Id =4,
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
            new TypeMaterielEntity
            {
                Id =5,
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
            },
            // ─── Types de sortie (Workflow Sortie) ───────────────────────────────
            new TypeMaterielEntity
            {
                Id = 10,
                CodeType = "BSM_CIRCULANT",
                NomType = "Circulant",
                Description = "Matériel circulant entre sites — workflow standard.",
                Categorie = "Workflow Sortie",
                Icone = "bi-arrow-repeat",
                Couleur = "#0d6efd",
                RequiertApprobationDepartement = true,
                RequiertApprobationDirection = false,
                NiveauxApprobation = 1,
                DureeValiditeDefautJours = 30,
                DureeMaximumJours = 365,
                NumeroSerieObligatoire = false,
                PhotoObligatoire = false,
                WorkflowRoutage = WorkflowRoutage.Standard,
                Ordre = 10,
                EstActif = true,
                DateCreation = new DateTime(2025, 1, 1)
            },
            new TypeMaterielEntity
            {
                Id = 11,
                CodeType = "BSM_EQUIPEMENT_IT",
                NomType = "Équipement IT",
                Description = "Équipement informatique — passe par le département IT.",
                Categorie = "Workflow Sortie",
                Icone = "bi-laptop",
                Couleur = "#6610f2",
                RequiertApprobationDepartement = true,
                RequiertApprobationDirection = true,
                NiveauxApprobation = 2,
                DureeValiditeDefautJours = 30,
                DureeMaximumJours = 365,
                NumeroSerieObligatoire = true,
                PhotoObligatoire = false,
                WorkflowRoutage = WorkflowRoutage.IT,
                Ordre = 11,
                EstActif = true,
                DateCreation = new DateTime(2025, 1, 1)
            },
            new TypeMaterielEntity
            {
                Id = 12,
                CodeType = "BSM_FIN_PROJET",
                NomType = "Fin de projet",
                Description = "Sortie en fin de chantier / projet — workflow standard.",
                Categorie = "Workflow Sortie",
                Icone = "bi-flag",
                Couleur = "#20c997",
                RequiertApprobationDepartement = true,
                RequiertApprobationDirection = false,
                NiveauxApprobation = 1,
                DureeValiditeDefautJours = 30,
                DureeMaximumJours = 365,
                NumeroSerieObligatoire = false,
                PhotoObligatoire = false,
                WorkflowRoutage = WorkflowRoutage.Standard,
                Ordre = 12,
                EstActif = true,
                DateCreation = new DateTime(2025, 1, 1)
            },
            new TypeMaterielEntity
            {
                Id = 13,
                CodeType = "BSM_RESIDU_DECHET",
                NomType = "Résidu / Déchet",
                Description = "Résidu ou déchet industriel — passe par le département Environnement.",
                Categorie = "Workflow Sortie",
                Icone = "bi-trash",
                Couleur = "#fd7e14",
                RequiertApprobationDepartement = true,
                RequiertApprobationDirection = false,
                NiveauxApprobation = 1,
                DureeValiditeDefautJours = 30,
                DureeMaximumJours = 365,
                NumeroSerieObligatoire = false,
                PhotoObligatoire = false,
                WorkflowRoutage = WorkflowRoutage.Environment,
                Ordre = 13,
                EstActif = true,
                DateCreation = new DateTime(2025, 1, 1)
            },
            new TypeMaterielEntity
            {
                Id = 14,
                CodeType = "BSM_RADIOPROTECTION",
                NomType = "Radioprotection",
                Description = "Matériel de radioprotection — passe par le département Environnement.",
                Categorie = "Workflow Sortie",
                Icone = "bi-radioactive",
                Couleur = "#dc3545",
                RequiertApprobationDepartement = true,
                RequiertApprobationDirection = false,
                NiveauxApprobation = 1,
                DureeValiditeDefautJours = 30,
                DureeMaximumJours = 365,
                NumeroSerieObligatoire = true,
                PhotoObligatoire = false,
                WorkflowRoutage = WorkflowRoutage.Environment,
                Ordre = 14,
                EstActif = true,
                DateCreation = new DateTime(2025, 1, 1)
            },
            new TypeMaterielEntity
            {
                Id = 15,
                CodeType = "BSM_MODIFICATION",
                NomType = "Modification",
                Description = "Matériel sorti pour modification — passe par le département Environnement.",
                Categorie = "Workflow Sortie",
                Icone = "bi-tools",
                Couleur = "#ffc107",
                RequiertApprobationDepartement = true,
                RequiertApprobationDirection = false,
                NiveauxApprobation = 1,
                DureeValiditeDefautJours = 30,
                DureeMaximumJours = 365,
                NumeroSerieObligatoire = false,
                PhotoObligatoire = false,
                WorkflowRoutage = WorkflowRoutage.Environment,
                Ordre = 15,
                EstActif = true,
                DateCreation = new DateTime(2025, 1, 1)
            },
            new TypeMaterielEntity
            {
                Id = 16,
                CodeType = "BSM_PRET_MATERIEL",
                NomType = "Matériel prêté",
                Description = "Matériel prêté temporairement — workflow standard.",
                Categorie = "Workflow Sortie",
                Icone = "bi-box-arrow-up-right",
                Couleur = "#6c757d",
                RequiertApprobationDepartement = true,
                RequiertApprobationDirection = false,
                NiveauxApprobation = 1,
                DureeValiditeDefautJours = 30,
                DureeMaximumJours = 180,
                NumeroSerieObligatoire = false,
                PhotoObligatoire = false,
                WorkflowRoutage = WorkflowRoutage.Standard,
                Ordre = 16,
                EstActif = true,
                DateCreation = new DateTime(2025, 1, 1)
            }
        );
    }
}
