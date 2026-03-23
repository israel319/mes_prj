using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddActiviteTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_Activites",
                schema: "dbo",
                columns: table => new
                {
                    IdActivite = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodeActivite = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NomActivite = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Module = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Categorie = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OrdreAffichage = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    EstActif = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    EstSysteme = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Activites", x => x.IdActivite);
                });

            migrationBuilder.CreateTable(
                name: "T_UtilisateurActivites",
                schema: "dbo",
                columns: table => new
                {
                    IdUtilisateurActivite = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUtilisateur = table.Column<int>(type: "int", nullable: false),
                    IdActivite = table.Column<int>(type: "int", nullable: false),
                    DateAttribution = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    AttribueParLogin = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EstActif = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_UtilisateurActivites", x => x.IdUtilisateurActivite);
                    table.ForeignKey(
                        name: "FK_T_UtilisateurActivites_T_Activites_IdActivite",
                        column: x => x.IdActivite,
                        principalSchema: "dbo",
                        principalTable: "T_Activites",
                        principalColumn: "IdActivite",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T_UtilisateurActivites_T_Utilisateurs_IdUtilisateur",
                        column: x => x.IdUtilisateur,
                        principalSchema: "dbo",
                        principalTable: "T_Utilisateurs",
                        principalColumn: "IdUtilisateur",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "T_Activites",
                columns: new[] { "IdActivite", "Categorie", "CodeActivite", "DateCreation", "Description", "EstActif", "EstSysteme", "Module", "NomActivite", "OrdreAffichage" },
                values: new object[,]
                {
                    { 1, "Création", "BEM_CREER", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Saisir et enregistrer un nouveau bon d'entrée matériel", true, true, "BonEntree", "Créer un Bon d'Entrée", 10 },
                    { 2, "Modification", "BEM_MODIFIER", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Éditer un bon d'entrée en brouillon", true, true, "BonEntree", "Modifier un Bon d'Entrée", 20 },
                    { 3, "Workflow", "BEM_SOUMETTRE", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Envoyer un bon d'entrée en approbation", true, true, "BonEntree", "Soumettre un Bon d'Entrée", 30 },
                    { 4, "Approbation", "BEM_APPROUVER", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Valider et approuver un bon d'entrée soumis", true, true, "BonEntree", "Approuver un Bon d'Entrée", 40 },
                    { 5, "Approbation", "BEM_REJETER", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Rejeter un bon d'entrée avec motif obligatoire", true, true, "BonEntree", "Rejeter un Bon d'Entrée", 50 },
                    { 6, "Approbation", "BEM_RETOURNER", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Renvoyer un bon d'entrée au demandeur pour corrections", true, true, "BonEntree", "Retourner un Bon d'Entrée", 60 },
                    { 7, "Suppression", "BEM_SUPPRIMER", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Supprimer un bon d'entrée en statut brouillon", true, true, "BonEntree", "Supprimer un brouillon BEM", 70 },
                    { 8, "Export", "BEM_IMPRIMER", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Imprimer ou télécharger le PDF d'un bon d'entrée", true, true, "BonEntree", "Imprimer / Exporter PDF un BEM", 80 },
                    { 10, "Création", "BSM_CREER", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Saisir et enregistrer un nouveau bon de sortie matériel", true, true, "BonSortie", "Créer un Bon de Sortie", 100 },
                    { 11, "Modification", "BSM_MODIFIER", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Éditer un bon de sortie en brouillon", true, true, "BonSortie", "Modifier un Bon de Sortie", 110 },
                    { 12, "Workflow", "BSM_SOUMETTRE", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Envoyer un bon de sortie dans la chaîne d'approbation", true, true, "BonSortie", "Soumettre un Bon de Sortie", 120 },
                    { 13, "Approbation", "BSM_APPROUVER", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Valider et approuver un bon de sortie à l'étape courante", true, true, "BonSortie", "Approuver un Bon de Sortie", 130 },
                    { 14, "Approbation", "BSM_REJETER", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Rejeter un bon de sortie avec motif obligatoire", true, true, "BonSortie", "Rejeter un Bon de Sortie", 140 },
                    { 15, "Approbation", "BSM_RETOURNER", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Renvoyer un bon de sortie au demandeur pour corrections", true, true, "BonSortie", "Retourner un Bon de Sortie", 150 },
                    { 16, "Suppression", "BSM_SUPPRIMER", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Supprimer un bon de sortie en statut brouillon", true, true, "BonSortie", "Supprimer un brouillon BSM", 160 },
                    { 17, "Export", "BSM_IMPRIMER", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Imprimer ou télécharger le PDF d'un bon de sortie", true, true, "BonSortie", "Imprimer / Exporter PDF un BSM", 170 },
                    { 18, "Prêts", "PRET_RETOUR", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Confirmer le retour d'un matériel prêté", true, true, "BonSortie", "Enregistrer un retour de prêt", 180 },
                    { 19, "Prêts", "PRET_EXTENSION", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Prolonger la date de retour d'un prêt en cours", true, true, "BonSortie", "Demander une extension de prêt", 190 },
                    { 20, "Scan", "SEC_SCANNER", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Scanner un QR code à la barrière pour contrôler un passage", true, true, "Securite", "Scanner un QR Code", 200 },
                    { 21, "Scan", "SEC_CONFIRMER_PASSAGE", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Valider le passage d'un matériel à la barrière après scan", true, true, "Securite", "Confirmer un passage", 210 },
                    { 22, "Anomalies", "SEC_SIGNALER_ANOMALIE", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Signaler manuellement une anomalie lors d'un contrôle", true, true, "Securite", "Signaler une anomalie", 220 },
                    { 23, "Anomalies", "SEC_TRAITER_ANOMALIE", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Résoudre une anomalie avec commentaire et action corrective", true, true, "Securite", "Traiter une anomalie", 230 },
                    { 24, "Anomalies", "SEC_REOUVRIR_ANOMALIE", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Réouvrir une anomalie traitée pour investigation complémentaire", true, true, "Securite", "Réouvrir une anomalie", 240 },
                    { 25, "Consultation", "SEC_VOIR_HISTORIQUE", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Voir l'historique complet des scans QR et passages", true, true, "Securite", "Consulter l'historique des scans", 250 },
                    { 26, "Administration", "SEC_GERER_BARRIERES", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Créer, modifier, activer/désactiver les barrières de contrôle", true, true, "Securite", "Gérer les barrières", 260 },
                    { 27, "Administration", "SEC_GERER_ITINERAIRES", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Configurer les itinéraires et séquences de checkpoints", true, true, "Securite", "Gérer les itinéraires", 270 },
                    { 28, "Administration", "SEC_GERER_AGENTS", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Affecter et gérer les agents aux barrières", true, true, "Securite", "Gérer les agents de barrière", 280 },
                    { 30, "Administration", "ADMIN_UTILISATEURS", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Créer, modifier, activer/désactiver les utilisateurs", true, true, "Admin", "Gérer les utilisateurs", 300 },
                    { 31, "Administration", "ADMIN_ROLES", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Créer, modifier les rôles et leurs permissions", true, true, "Admin", "Gérer les rôles", 310 },
                    { 32, "Administration", "ADMIN_DEPARTEMENTS", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Créer, modifier, activer/désactiver les départements", true, true, "Admin", "Gérer les départements", 320 },
                    { 33, "Administration", "ADMIN_TYPES_MATERIELS", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Créer, modifier les types de matériel", true, true, "Admin", "Gérer les types de matériel", 330 },
                    { 34, "Administration", "ADMIN_STATUTS", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Créer, modifier les statuts de workflow", true, true, "Admin", "Gérer les statuts", 340 },
                    { 35, "Administration", "ADMIN_PARAMETRES", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Configurer les paramètres globaux de l'application", true, true, "Admin", "Gérer les paramètres système", 350 },
                    { 36, "Administration", "ADMIN_AUDIT", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Voir les logs d'audit des actions système", true, true, "Admin", "Consulter le journal d'audit", 360 },
                    { 37, "Administration", "ADMIN_ACTIVITES", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Assigner et retirer des activités aux utilisateurs", true, true, "Admin", "Gérer les activités utilisateurs", 370 },
                    { 40, "Consultation", "VOIR_TOUS_BONS", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Consulter la liste complète de tous les bons du système", true, true, "Transversal", "Voir tous les bons", 400 },
                    { 41, "Approbation", "VOIR_APPROBATIONS", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Consulter la liste des bons en attente d'approbation", true, true, "Transversal", "Voir les approbations en attente", 410 },
                    { 42, "Export", "EXPORT_EXCEL", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Exporter les listes de bons et l'historique au format Excel", true, true, "Transversal", "Exporter les données en Excel", 420 },
                    { 43, "Consultation", "VOIR_HISTORIQUE", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Consulter l'historique complet des bons et mouvements", true, true, "Transversal", "Consulter l'historique", 430 },
                    { 44, "Consultation", "VOIR_TABLEAU_BORD", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Accéder au tableau de bord avec statistiques et raccourcis", true, true, "Transversal", "Voir le tableau de bord", 440 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activites_Categorie",
                schema: "dbo",
                table: "T_Activites",
                column: "Categorie");

            migrationBuilder.CreateIndex(
                name: "IX_Activites_CodeActivite",
                schema: "dbo",
                table: "T_Activites",
                column: "CodeActivite",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Activites_EstActif",
                schema: "dbo",
                table: "T_Activites",
                column: "EstActif");

            migrationBuilder.CreateIndex(
                name: "IX_Activites_Module",
                schema: "dbo",
                table: "T_Activites",
                column: "Module");

            migrationBuilder.CreateIndex(
                name: "IX_UtilisateurActivites_IdActivite",
                schema: "dbo",
                table: "T_UtilisateurActivites",
                column: "IdActivite");

            migrationBuilder.CreateIndex(
                name: "IX_UtilisateurActivites_IdUtilisateur",
                schema: "dbo",
                table: "T_UtilisateurActivites",
                column: "IdUtilisateur");

            migrationBuilder.CreateIndex(
                name: "IX_UtilisateurActivites_Utilisateur_Activite",
                schema: "dbo",
                table: "T_UtilisateurActivites",
                columns: new[] { "IdUtilisateur", "IdActivite" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_UtilisateurActivites",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "T_Activites",
                schema: "dbo");
        }
    }
}
