using KCCMaterialFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration Entity Framework pour l'entité Statut
/// </summary>
public class StatutConfiguration : IEntityTypeConfiguration<Statut>
{
    public void Configure(EntityTypeBuilder<Statut> builder)
    {
        builder.ToTable("T_Statuts", "dbo");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnName("IdStatut").ValueGeneratedOnAdd();

        builder.Property(s => s.CodeStatut)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.LibelleStatut)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.TypeBon)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.CouleurFond)
            .HasMaxLength(20);

        builder.Property(s => s.CouleurTexte)
            .HasMaxLength(20);

        builder.HasIndex(s => s.CodeStatut)
            .IsUnique()
            .HasDatabaseName("IX_Statuts_CodeStatut");

        builder.HasIndex(s => new { s.TypeBon, s.EstActif })
            .HasDatabaseName("IX_Statuts_TypeBon_EstActif");

        // Données de base pour les statuts système
        builder.HasData(
            // Statuts communs
            new Statut
            {
                Id =1,
                CodeStatut = "BROUILLON",
                LibelleStatut = "Brouillon",
                Description = "Bon en cours de création",
                TypeBon = "Tous",
                CouleurFond = "#6c757d",
                CouleurTexte = "#ffffff",
                Icone = "bi-pencil",
                Ordre = 1,
                EstFinal = false,
                RequiertAction = true,
                StatutsSuivants = "2",
                EstActif = true,
                EstSysteme = true,
                DateCreation = new DateTime(2024, 1, 1)
            },
            new Statut
            {
                Id =2,
                CodeStatut = "EN_ATTENTE_APPROBATION",
                LibelleStatut = "En attente d'approbation",
                Description = "Bon soumis, en attente de validation",
                TypeBon = "Tous",
                CouleurFond = "#ffc107",
                CouleurTexte = "#212529",
                Icone = "bi-hourglass-split",
                Ordre = 2,
                EstFinal = false,
                RequiertAction = true,
                StatutsSuivants = "3,4",
                EstActif = true,
                EstSysteme = true,
                DateCreation = new DateTime(2024, 1, 1)
            },
            new Statut
            {
                Id =3,
                CodeStatut = "APPROUVE",
                LibelleStatut = "Approuvé",
                Description = "Bon approuvé par le responsable",
                TypeBon = "Tous",
                CouleurFond = "#28a745",
                CouleurTexte = "#ffffff",
                Icone = "bi-check-circle",
                Ordre = 3,
                EstFinal = false,
                RequiertAction = false,
                StatutsSuivants = "5,6",
                EstActif = true,
                EstSysteme = true,
                DateCreation = new DateTime(2024, 1, 1)
            },
            new Statut
            {
                Id =4,
                CodeStatut = "REJETE",
                LibelleStatut = "Rejeté",
                Description = "Bon rejeté par le responsable",
                TypeBon = "Tous",
                CouleurFond = "#dc3545",
                CouleurTexte = "#ffffff",
                Icone = "bi-x-circle",
                Ordre = 4,
                EstFinal = true,
                RequiertAction = false,
                EstActif = true,
                EstSysteme = true,
                DateCreation = new DateTime(2024, 1, 1)
            },
            new Statut
            {
                Id =5,
                CodeStatut = "EN_COURS",
                LibelleStatut = "En cours",
                Description = "Matériel en cours d'utilisation/sortie",
                TypeBon = "BonSortie",
                CouleurFond = "#17a2b8",
                CouleurTexte = "#ffffff",
                Icone = "bi-arrow-repeat",
                Ordre = 5,
                EstFinal = false,
                RequiertAction = false,
                StatutsSuivants = "6,7",
                EstActif = true,
                EstSysteme = true,
                DateCreation = new DateTime(2024, 1, 1)
            },
            new Statut
            {
                Id =6,
                CodeStatut = "TERMINE",
                LibelleStatut = "Terminé",
                Description = "Processus terminé avec succès",
                TypeBon = "Tous",
                CouleurFond = "#198754",
                CouleurTexte = "#ffffff",
                Icone = "bi-check2-all",
                Ordre = 6,
                EstFinal = true,
                RequiertAction = false,
                EstActif = true,
                EstSysteme = true,
                DateCreation = new DateTime(2024, 1, 1)
            },
            new Statut
            {
                Id =7,
                CodeStatut = "EXPIRE",
                LibelleStatut = "Expiré",
                Description = "Bon expiré - matériel non retourné à temps",
                TypeBon = "BonSortie",
                CouleurFond = "#fd7e14",
                CouleurTexte = "#ffffff",
                Icone = "bi-exclamation-triangle",
                Ordre = 7,
                EstFinal = false,
                RequiertAction = true,
                StatutsSuivants = "6",
                EstActif = true,
                EstSysteme = true,
                DateCreation = new DateTime(2024, 1, 1)
            },
            new Statut
            {
                Id =8,
                CodeStatut = "ANNULE",
                LibelleStatut = "Annulé",
                Description = "Bon annulé par l'utilisateur ou le système",
                TypeBon = "Tous",
                CouleurFond = "#6c757d",
                CouleurTexte = "#ffffff",
                Icone = "bi-slash-circle",
                Ordre = 8,
                EstFinal = true,
                RequiertAction = false,
                EstActif = true,
                EstSysteme = true,
                DateCreation = new DateTime(2024, 1, 1)
            }
        );
    }
}
