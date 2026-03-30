using KCCMaterialFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KCCMaterialFlow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration Entity Framework pour l'entité Activite
/// </summary>
public class ActiviteConfiguration : IEntityTypeConfiguration<Activite>
{
    public void Configure(EntityTypeBuilder<Activite> builder)
    {
        builder.ToTable("T_Activites", "dbo");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasColumnName("IdActivite")
            .ValueGeneratedOnAdd();

        builder.Property(a => a.CodeActivite)
            .HasColumnName("CodeActivite")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.NomActivite)
            .HasColumnName("NomActivite")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Description)
            .HasColumnName("Description")
            .HasMaxLength(500);

        builder.Property(a => a.Module)
            .HasColumnName("Module")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.Categorie)
            .HasColumnName("Categorie")
            .HasMaxLength(50);

        builder.Property(a => a.OrdreAffichage)
            .HasColumnName("OrdreAffichage")
            .HasDefaultValue(0);

        builder.Property(a => a.EstActif)
            .HasColumnName("EstActif")
            .HasDefaultValue(true);

        builder.Property(a => a.EstSysteme)
            .HasColumnName("EstSysteme")
            .HasDefaultValue(false);

        builder.Property(a => a.DateCreation)
            .HasColumnName("DateCreation")
            .HasDefaultValueSql("GETDATE()");

        // Index unique sur le code
        builder.HasIndex(a => a.CodeActivite)
            .IsUnique()
            .HasDatabaseName("IX_Activites_CodeActivite");

        // Index sur le module pour les requêtes de filtrage
        builder.HasIndex(a => a.Module)
            .HasDatabaseName("IX_Activites_Module");

        // Index sur la catégorie
        builder.HasIndex(a => a.Categorie)
            .HasDatabaseName("IX_Activites_Categorie");

        // Index sur EstActif
        builder.HasIndex(a => a.EstActif)
            .HasDatabaseName("IX_Activites_EstActif");

        var seedDate = new DateTime(2024, 1, 1);

        // Données de base : toutes les activités métier du système
        builder.HasData(
            // =============================================
            // MODULE: BonEntree (BEM)
            // =============================================
            new Activite
            {
                Id =1,
                CodeActivite = "BEM_CREER",
                NomActivite = "Créer un Bon d'Entrée",
                Description = "Saisir et enregistrer un nouveau bon d'entrée matériel",
                Module = "BonEntree",
                Categorie = "Création",
                OrdreAffichage = 10,
                EstActif = true,
                EstSysteme = true,
                DateCreation = seedDate
            },
            new Activite
            {
                Id =2,
                CodeActivite = "BEM_MODIFIER",
                NomActivite = "Modifier un Bon d'Entrée",
                Description = "Éditer un bon d'entrée en brouillon",
                Module = "BonEntree",
                Categorie = "Modification",
                OrdreAffichage = 20,
                EstActif = true,
                EstSysteme = true,
                DateCreation = seedDate
            },
            new Activite
            {
                Id =3,
                CodeActivite = "BEM_SOUMETTRE",
                NomActivite = "Soumettre un Bon d'Entrée",
                Description = "Envoyer un bon d'entrée en approbation",
                Module = "BonEntree",
                Categorie = "Workflow",
                OrdreAffichage = 30,
                EstActif = true,
                EstSysteme = true,
                DateCreation = seedDate
            },
            new Activite
            {
                Id =4,
                CodeActivite = "BEM_APPROUVER",
                NomActivite = "Approuver un Bon d'Entrée",
                Description = "Valider et approuver un bon d'entrée soumis",
                Module = "BonEntree",
                Categorie = "Approbation",
                OrdreAffichage = 40,
                EstActif = true,
                EstSysteme = true,
                DateCreation = seedDate
            },
            new Activite
            {
                Id =5,
                CodeActivite = "BEM_REJETER",
                NomActivite = "Rejeter un Bon d'Entrée",
                Description = "Rejeter un bon d'entrée avec motif obligatoire",
                Module = "BonEntree",
                Categorie = "Approbation",
                OrdreAffichage = 50,
                EstActif = true,
                EstSysteme = true,
                DateCreation = seedDate
            },
            new Activite
            {
                Id =6,
                CodeActivite = "BEM_RETOURNER",
                NomActivite = "Retourner un Bon d'Entrée",
                Description = "Renvoyer un bon d'entrée au demandeur pour corrections",
                Module = "BonEntree",
                Categorie = "Approbation",
                OrdreAffichage = 60,
                EstActif = true,
                EstSysteme = true,
                DateCreation = seedDate
            },
            new Activite
            {
                Id =7,
                CodeActivite = "BEM_SUPPRIMER",
                NomActivite = "Supprimer un brouillon BEM",
                Description = "Supprimer un bon d'entrée en statut brouillon",
                Module = "BonEntree",
                Categorie = "Suppression",
                OrdreAffichage = 70,
                EstActif = true,
                EstSysteme = true,
                DateCreation = seedDate
            },
            new Activite
            {
                Id =8,
                CodeActivite = "BEM_IMPRIMER",
                NomActivite = "Imprimer / Exporter PDF un BEM",
                Description = "Imprimer ou télécharger le PDF d'un bon d'entrée",
                Module = "BonEntree",
                Categorie = "Export",
                OrdreAffichage = 80,
                EstActif = true,
                EstSysteme = true,
                DateCreation = seedDate
            },

            // =============================================
            // MODULE: BonSortie (BSM)
            // =============================================
            new Activite
            {
                Id =10,
                CodeActivite = "BSM_CREER",
                NomActivite = "Créer un Bon de Sortie",
                Description = "Saisir et enregistrer un nouveau bon de sortie matériel",
                Module = "BonSortie",
                Categorie = "Création",
                OrdreAffichage = 100,
                EstActif = true,
                EstSysteme = true,
                DateCreation = seedDate
            },
            new Activite
            {
                Id =11,
                CodeActivite = "BSM_MODIFIER",
                NomActivite = "Modifier un Bon de Sortie",
                Description = "Éditer un bon de sortie en brouillon",
                Module = "BonSortie",
                Categorie = "Modification",
                OrdreAffichage = 110,
                EstActif = true,
                EstSysteme = true,
                DateCreation = seedDate
            },
            new Activite
            {
                Id =12,
                CodeActivite = "BSM_SOUMETTRE",
                NomActivite = "Soumettre un Bon de Sortie",
                Description = "Envoyer un bon de sortie dans la chaîne d'approbation",
                Module = "BonSortie",
                Categorie = "Workflow",
                OrdreAffichage = 120,
                EstActif = true,
                EstSysteme = true,
                DateCreation = seedDate
            },
            new Activite
            {
                Id =13,
                CodeActivite = "BSM_APPROUVER",
                NomActivite = "Approuver un Bon de Sortie",
                Description = "Valider et approuver un bon de sortie à l'étape courante",
                Module = "BonSortie",
                Categorie = "Approbation",
                OrdreAffichage = 130,
                EstActif = true,
                EstSysteme = true,
                DateCreation = seedDate
            },
            new Activite
            {
                Id =14,
                CodeActivite = "BSM_REJETER",
                NomActivite = "Rejeter un Bon de Sortie",
                Description = "Rejeter un bon de sortie avec motif obligatoire",
                Module = "BonSortie",
                Categorie = "Approbation",
                OrdreAffichage = 140,
                EstActif = true,
                EstSysteme = true,
                DateCreation = seedDate
            },
            new Activite
            {
                Id =15,
                CodeActivite = "BSM_RETOURNER",
                NomActivite = "Retourner un Bon de Sortie",
                Description = "Renvoyer un bon de sortie au demandeur pour corrections",
                Module = "BonSortie",
                Categorie = "Approbation",
                OrdreAffichage = 150,
                EstActif = true,
                EstSysteme = true,
                DateCreation = seedDate
            },
            new Activite
            {
                Id =16,
                CodeActivite = "BSM_SUPPRIMER",
                NomActivite = "Supprimer un brouillon BSM",
                Description = "Supprimer un bon de sortie en statut brouillon",
                Module = "BonSortie",
                Categorie = "Suppression",
                OrdreAffichage = 160,
                EstActif = true,
                EstSysteme = true,
                DateCreation = seedDate
            },
            new Activite
            {
                Id =17,
                CodeActivite = "BSM_IMPRIMER",
                NomActivite = "Imprimer / Exporter PDF un BSM",
                Description = "Imprimer ou télécharger le PDF d'un bon de sortie",
                Module = "BonSortie",
                Categorie = "Export",
                OrdreAffichage = 170,
                EstActif = true,
                EstSysteme = true,
                DateCreation = seedDate
            },

            // =============================================
            // MODULE: Prêts (BSM sous-module)
            // =============================================
            new Activite
            {
                Id =18,
                CodeActivite = "PRET_RETOUR",
                NomActivite = "Enregistrer un retour de prêt",
                Description = "Confirmer le retour d'un matériel prêté",
                Module = "BonSortie",
                Categorie = "Prêts",
                OrdreAffichage = 180,
                EstActif = true,
                EstSysteme = true,
                DateCreation = seedDate
            },
            new Activite
            {
                Id =19,
                CodeActivite = "PRET_EXTENSION",
                NomActivite = "Demander une extension de prêt",
                Description = "Prolonger la date de retour d'un prêt en cours",
                Module = "BonSortie",
                Categorie = "Prêts",
                OrdreAffichage = 190,
                EstActif = true,
                EstSysteme = true,
                DateCreation = seedDate
            },

            // =============================================
            // MODULE: Sécurité (SEC)
            // =============================================
            new Activite
            {
                Id =20,
                CodeActivite = "SEC_SCANNER",
                NomActivite = "Scanner un QR Code",
                Description = "Scanner un QR code à la barrière pour contrôler un passage",
                Module = "Securite",
                Categorie = "Scan",
                OrdreAffichage = 200,
                EstActif = true,
                EstSysteme = true,
                DateCreation = seedDate
            },
            new Activite
            {
                Id =21,
                CodeActivite = "SEC_CONFIRMER_PASSAGE",
                NomActivite = "Confirmer un passage",
                Description = "Valider le passage d'un matériel à la barrière après scan",
                Module = "Securite",
                Categorie = "Scan",
                OrdreAffichage = 210,
                EstActif = true,
                EstSysteme = true,
                DateCreation = seedDate
            },
            new Activite
            {
                Id =22,
                CodeActivite = "SEC_SIGNALER_ANOMALIE",
                NomActivite = "Signaler une anomalie",
                Description = "Signaler manuellement une anomalie lors d'un contrôle",
                Module = "Securite",
                Categorie = "Anomalies",
                OrdreAffichage = 220,
                EstActif = true,
                EstSysteme = true,
                DateCreation = seedDate
            },
            new Activite
            {
                Id =23,
                CodeActivite = "SEC_TRAITER_ANOMALIE",
                NomActivite = "Traiter une anomalie",
                Description = "Résoudre une anomalie avec commentaire et action corrective",
                Module = "Securite",
                Categorie = "Anomalies",
                OrdreAffichage = 230,
                EstActif = true,
                EstSysteme = true,
                DateCreation = seedDate
            },
            new Activite
            {
                Id =24,
                CodeActivite = "SEC_REOUVRIR_ANOMALIE",
                NomActivite = "Réouvrir une anomalie",
                Description = "Réouvrir une anomalie traitée pour investigation complémentaire",
                Module = "Securite",
                Categorie = "Anomalies",
                OrdreAffichage = 240,
                EstActif = true,
                EstSysteme = true,
                DateCreation = seedDate
            },
            new Activite
            {
                Id =25,
                CodeActivite = "SEC_VOIR_HISTORIQUE",
                NomActivite = "Consulter l'historique des scans",
                Description = "Voir l'historique complet des scans QR et passages",
                Module = "Securite",
                Categorie = "Consultation",
                OrdreAffichage = 250,
                EstActif = true,
                EstSysteme = true,
                DateCreation = seedDate
            },

            // =============================================
            // MODULE: Administration Sécurité
            // =============================================
            new Activite
            {
                Id =26,
                CodeActivite = "SEC_GERER_BARRIERES",
                NomActivite = "Gérer les barrières",
                Description = "Créer, modifier, activer/désactiver les barrières de contrôle",
                Module = "Securite",
                Categorie = "Administration",
                OrdreAffichage = 260,
                EstActif = true,
                EstSysteme = true,
                DateCreation = seedDate
            },
            new Activite
            {
                Id =27,
                CodeActivite = "SEC_GERER_ITINERAIRES",
                NomActivite = "Gérer les itinéraires",
                Description = "Configurer les itinéraires et séquences de checkpoints",
                Module = "Securite",
                Categorie = "Administration",
                OrdreAffichage = 270,
                EstActif = true,
                EstSysteme = true,
                DateCreation = seedDate
            },
            new Activite
            {
                Id =28,
                CodeActivite = "SEC_GERER_AGENTS",
                NomActivite = "Gérer les agents de barrière",
                Description = "Affecter et gérer les agents aux barrières",
                Module = "Securite",
                Categorie = "Administration",
                OrdreAffichage = 280,
                EstActif = true,
                EstSysteme = true,
                DateCreation = seedDate
            },

            // =============================================
            // MODULE: Administration Système (ADMIN)
            // =============================================
            new Activite
            {
                Id =30,
                CodeActivite = "ADMIN_UTILISATEURS",
                NomActivite = "Gérer les utilisateurs",
                Description = "Créer, modifier, activer/désactiver les utilisateurs",
                Module = "Admin",
                Categorie = "Administration",
                OrdreAffichage = 300,
                EstActif = true,
                EstSysteme = true,
                DateCreation = seedDate
            },
            new Activite
            {
                Id =31,
                CodeActivite = "ADMIN_ROLES",
                NomActivite = "Gérer les rôles",
                Description = "Créer, modifier les rôles et leurs permissions",
                Module = "Admin",
                Categorie = "Administration",
                OrdreAffichage = 310,
                EstActif = true,
                EstSysteme = true,
                DateCreation = seedDate
            },
            new Activite
            {
                Id =32,
                CodeActivite = "ADMIN_DEPARTEMENTS",
                NomActivite = "Gérer les départements",
                Description = "Créer, modifier, activer/désactiver les départements",
                Module = "Admin",
                Categorie = "Administration",
                OrdreAffichage = 320,
                EstActif = true,
                EstSysteme = true,
                DateCreation = seedDate
            },
            new Activite
            {
                Id =33,
                CodeActivite = "ADMIN_TYPES_MATERIELS",
                NomActivite = "Gérer les types de matériel",
                Description = "Créer, modifier les types de matériel",
                Module = "Admin",
                Categorie = "Administration",
                OrdreAffichage = 330,
                EstActif = true,
                EstSysteme = true,
                DateCreation = seedDate
            },
            new Activite
            {
                Id =34,
                CodeActivite = "ADMIN_STATUTS",
                NomActivite = "Gérer les statuts",
                Description = "Créer, modifier les statuts de workflow",
                Module = "Admin",
                Categorie = "Administration",
                OrdreAffichage = 340,
                EstActif = true,
                EstSysteme = true,
                DateCreation = seedDate
            },
            new Activite
            {
                Id =35,
                CodeActivite = "ADMIN_PARAMETRES",
                NomActivite = "Gérer les paramètres système",
                Description = "Configurer les paramètres globaux de l'application",
                Module = "Admin",
                Categorie = "Administration",
                OrdreAffichage = 350,
                EstActif = true,
                EstSysteme = true,
                DateCreation = seedDate
            },
            new Activite
            {
                Id =36,
                CodeActivite = "ADMIN_AUDIT",
                NomActivite = "Consulter le journal d'audit",
                Description = "Voir les logs d'audit des actions système",
                Module = "Admin",
                Categorie = "Administration",
                OrdreAffichage = 360,
                EstActif = true,
                EstSysteme = true,
                DateCreation = seedDate
            },
            new Activite
            {
                Id =37,
                CodeActivite = "ADMIN_ACTIVITES",
                NomActivite = "Gérer les activités utilisateurs",
                Description = "Assigner et retirer des activités aux utilisateurs",
                Module = "Admin",
                Categorie = "Administration",
                OrdreAffichage = 370,
                EstActif = true,
                EstSysteme = true,
                DateCreation = seedDate
            },

            // =============================================
            // MODULE: Transversal (consultation / export)
            // =============================================
            new Activite
            {
                Id =40,
                CodeActivite = "VOIR_TOUS_BONS",
                NomActivite = "Voir tous les bons",
                Description = "Consulter la liste complète de tous les bons du système",
                Module = "Transversal",
                Categorie = "Consultation",
                OrdreAffichage = 400,
                EstActif = true,
                EstSysteme = true,
                DateCreation = seedDate
            },
            new Activite
            {
                Id =41,
                CodeActivite = "VOIR_APPROBATIONS",
                NomActivite = "Voir les approbations en attente",
                Description = "Consulter la liste des bons en attente d'approbation",
                Module = "Transversal",
                Categorie = "Approbation",
                OrdreAffichage = 410,
                EstActif = true,
                EstSysteme = true,
                DateCreation = seedDate
            },
            new Activite
            {
                Id =42,
                CodeActivite = "EXPORT_EXCEL",
                NomActivite = "Exporter les données en Excel",
                Description = "Exporter les listes de bons et l'historique au format Excel",
                Module = "Transversal",
                Categorie = "Export",
                OrdreAffichage = 420,
                EstActif = true,
                EstSysteme = true,
                DateCreation = seedDate
            },
            new Activite
            {
                Id =43,
                CodeActivite = "VOIR_HISTORIQUE",
                NomActivite = "Consulter l'historique",
                Description = "Consulter l'historique complet des bons et mouvements",
                Module = "Transversal",
                Categorie = "Consultation",
                OrdreAffichage = 430,
                EstActif = true,
                EstSysteme = true,
                DateCreation = seedDate
            },
            new Activite
            {
                Id =44,
                CodeActivite = "VOIR_TABLEAU_BORD",
                NomActivite = "Voir le tableau de bord",
                Description = "Accéder au tableau de bord avec statistiques et raccourcis",
                Module = "Transversal",
                Categorie = "Consultation",
                OrdreAffichage = 440,
                EstActif = true,
                EstSysteme = true,
                DateCreation = seedDate
            }
        );
    }
}

/// <summary>
/// Configuration Entity Framework pour l'entité UtilisateurActivite
/// </summary>
public class UtilisateurActiviteConfiguration : IEntityTypeConfiguration<UtilisateurActivite>
{
    public void Configure(EntityTypeBuilder<UtilisateurActivite> builder)
    {
        builder.ToTable("T_UtilisateurActivites", "dbo");

        builder.HasKey(ua => ua.Id);

        builder.Property(ua => ua.Id)
            .HasColumnName("IdUtilisateurActivite")
            .ValueGeneratedOnAdd();

        builder.Property(ua => ua.IdUtilisateur)
            .HasColumnName("IdUtilisateur")
            .IsRequired();

        builder.Property(ua => ua.IdActivite)
            .HasColumnName("IdActivite")
            .IsRequired();

        builder.Property(ua => ua.DateAttribution)
            .HasColumnName("DateAttribution")
            .HasDefaultValueSql("GETDATE()");

        builder.Property(ua => ua.AttribueParLogin)
            .HasColumnName("AttribueParLogin")
            .HasMaxLength(100);

        builder.Property(ua => ua.EstActif)
            .HasColumnName("EstActif")
            .HasDefaultValue(true);

        // Index unique : un utilisateur ne peut avoir la même activité qu'une seule fois
        builder.HasIndex(ua => new { ua.IdUtilisateur, ua.IdActivite })
            .IsUnique()
            .HasDatabaseName("IX_UtilisateurActivites_Utilisateur_Activite");

        // Index sur IdUtilisateur pour les requêtes de filtrage
        builder.HasIndex(ua => ua.IdUtilisateur)
            .HasDatabaseName("IX_UtilisateurActivites_IdUtilisateur");

        // Index sur IdActivite
        builder.HasIndex(ua => ua.IdActivite)
            .HasDatabaseName("IX_UtilisateurActivites_IdActivite");

        // Relation UtilisateurActivite -> Utilisateur
        builder.HasOne(ua => ua.Utilisateur)
            .WithMany(u => u.UtilisateurActivites)
            .HasForeignKey(ua => ua.IdUtilisateur)
            .OnDelete(DeleteBehavior.Cascade);

        // Relation UtilisateurActivite -> Activite
        builder.HasOne(ua => ua.Activite)
            .WithMany(a => a.UtilisateurActivites)
            .HasForeignKey(ua => ua.IdActivite)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
