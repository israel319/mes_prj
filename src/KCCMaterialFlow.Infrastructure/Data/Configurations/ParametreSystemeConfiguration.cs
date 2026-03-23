using KCCMaterialFlow.Module.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration Entity Framework pour l'entité ParametreSysteme
/// </summary>
public class ParametreSystemeConfiguration : IEntityTypeConfiguration<ParametreSysteme>
{
    public void Configure(EntityTypeBuilder<ParametreSysteme> builder)
    {
        builder.ToTable("T_ParametresSysteme", "dbo");

        builder.HasKey(p => p.IdParametre);

        builder.Property(p => p.Cle)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Valeur)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(p => p.TypeDonnee)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Categorie)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Libelle)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(p => p.Cle)
            .IsUnique()
            .HasDatabaseName("IX_ParametresSysteme_Cle");

        builder.HasIndex(p => p.Categorie)
            .HasDatabaseName("IX_ParametresSysteme_Categorie");

        // Données de base pour les paramètres système
        builder.HasData(
            // Catégorie: General
            new ParametreSysteme
            {
                IdParametre = 1,
                Cle = "APP_NOM",
                Valeur = "KCC Material Flow",
                TypeDonnee = "String",
                Categorie = "General",
                Libelle = "Nom de l'application",
                Description = "Nom affiché dans l'en-tête et les emails",
                EstModifiable = true,
                EstVisible = true,
                Ordre = 1
            },
            new ParametreSysteme
            {
                IdParametre = 2,
                Cle = "APP_VERSION",
                Valeur = "1.0.0",
                TypeDonnee = "String",
                Categorie = "General",
                Libelle = "Version de l'application",
                Description = "Version actuelle du système",
                EstModifiable = false,
                EstVisible = true,
                EstSysteme = true,
                Ordre = 2
            },
            
            // Catégorie: Workflow
            new ParametreSysteme
            {
                IdParametre = 10,
                Cle = "WORKFLOW_DUREE_VALIDITE_DEFAUT",
                Valeur = "30",
                TypeDonnee = "Integer",
                Categorie = "Workflow",
                Libelle = "Durée de validité par défaut (jours)",
                Description = "Durée de validité par défaut pour les bons de sortie",
                ValeurDefaut = "30",
                ValeurMin = 1,
                ValeurMax = 365,
                Unite = "jours",
                EstModifiable = true,
                EstVisible = true,
                Ordre = 1
            },
            new ParametreSysteme
            {
                IdParametre = 11,
                Cle = "WORKFLOW_DELAI_RAPPEL_EXPIRATION",
                Valeur = "3",
                TypeDonnee = "Integer",
                Categorie = "Workflow",
                Libelle = "Délai de rappel avant expiration (jours)",
                Description = "Nombre de jours avant expiration pour envoyer un rappel",
                ValeurDefaut = "3",
                ValeurMin = 1,
                ValeurMax = 30,
                Unite = "jours",
                EstModifiable = true,
                EstVisible = true,
                Ordre = 2
            },
            new ParametreSysteme
            {
                IdParametre = 12,
                Cle = "WORKFLOW_DELAI_APPROBATION_MAX",
                Valeur = "7",
                TypeDonnee = "Integer",
                Categorie = "Workflow",
                Libelle = "Délai maximum d'approbation (jours)",
                Description = "Délai maximum pour qu'un approbateur valide un bon",
                ValeurDefaut = "7",
                ValeurMin = 1,
                ValeurMax = 30,
                Unite = "jours",
                EstModifiable = true,
                EstVisible = true,
                Ordre = 3
            },

            // Catégorie: Email
            new ParametreSysteme
            {
                IdParametre = 20,
                Cle = "EMAIL_ACTIVER_NOTIFICATIONS",
                Valeur = "true",
                TypeDonnee = "Boolean",
                Categorie = "Email",
                Libelle = "Activer les notifications email",
                Description = "Active ou désactive l'envoi des notifications par email",
                ValeurDefaut = "true",
                ValeursPossibles = "true|false",
                EstModifiable = true,
                EstVisible = true,
                Ordre = 1
            },
            new ParametreSysteme
            {
                IdParametre = 21,
                Cle = "EMAIL_EXPEDITEUR",
                Valeur = "noreply@kccmaterialflow.local",
                TypeDonnee = "String",
                Categorie = "Email",
                Libelle = "Adresse email expéditeur",
                Description = "Adresse email utilisée comme expéditeur pour les notifications",
                EstModifiable = true,
                EstVisible = true,
                Ordre = 2
            },
            new ParametreSysteme
            {
                IdParametre = 22,
                Cle = "EMAIL_ADMIN",
                Valeur = "admin@kccmaterialflow.local",
                TypeDonnee = "String",
                Categorie = "Email",
                Libelle = "Email administrateur",
                Description = "Adresse email pour les notifications administratives",
                EstModifiable = true,
                EstVisible = true,
                Ordre = 3
            },

            // Catégorie: Securite
            new ParametreSysteme
            {
                IdParametre = 30,
                Cle = "SECURITE_QRCODE_DUREE_VALIDITE",
                Valeur = "60",
                TypeDonnee = "Integer",
                Categorie = "Securite",
                Libelle = "Durée de validité QR Code (minutes)",
                Description = "Durée pendant laquelle un QR Code scanné est valide",
                ValeurDefaut = "60",
                ValeurMin = 5,
                ValeurMax = 1440,
                Unite = "minutes",
                EstModifiable = true,
                EstVisible = true,
                Ordre = 1
            },
            new ParametreSysteme
            {
                IdParametre = 31,
                Cle = "SECURITE_MAX_SCANS_JOUR",
                Valeur = "10",
                TypeDonnee = "Integer",
                Categorie = "Securite",
                Libelle = "Nombre maximum de scans par jour",
                Description = "Nombre maximum de scans autorisés par bon par jour",
                ValeurDefaut = "10",
                ValeurMin = 1,
                ValeurMax = 100,
                EstModifiable = true,
                EstVisible = true,
                Ordre = 2
            },
            new ParametreSysteme
            {
                IdParametre = 32,
                Cle = "SECURITE_DETECTER_ANOMALIES_AUTO",
                Valeur = "true",
                TypeDonnee = "Boolean",
                Categorie = "Securite",
                Libelle = "Détection automatique des anomalies",
                Description = "Active la détection automatique des anomalies lors des scans",
                ValeurDefaut = "true",
                ValeursPossibles = "true|false",
                EstModifiable = true,
                EstVisible = true,
                Ordre = 3
            },

            // Catégorie: Interface
            new ParametreSysteme
            {
                IdParametre = 40,
                Cle = "UI_ITEMS_PAR_PAGE",
                Valeur = "20",
                TypeDonnee = "Integer",
                Categorie = "Interface",
                Libelle = "Éléments par page",
                Description = "Nombre d'éléments affichés par page dans les listes",
                ValeurDefaut = "20",
                ValeursPossibles = "10|20|50|100",
                ValeurMin = 10,
                ValeurMax = 100,
                EstModifiable = true,
                EstVisible = true,
                Ordre = 1
            },
            new ParametreSysteme
            {
                IdParametre = 41,
                Cle = "UI_THEME_DEFAUT",
                Valeur = "light",
                TypeDonnee = "String",
                Categorie = "Interface",
                Libelle = "Thème par défaut",
                Description = "Thème visuel par défaut de l'application",
                ValeurDefaut = "light",
                ValeursPossibles = "light|dark|auto",
                EstModifiable = true,
                EstVisible = true,
                Ordre = 2
            },
            new ParametreSysteme
            {
                IdParametre = 42,
                Cle = "UI_LANGUE_DEFAUT",
                Valeur = "fr",
                TypeDonnee = "String",
                Categorie = "Interface",
                Libelle = "Langue par défaut",
                Description = "Langue par défaut de l'interface",
                ValeurDefaut = "fr",
                ValeursPossibles = "fr|en",
                EstModifiable = true,
                EstVisible = true,
                Ordre = 3
            }
        );
    }
}
