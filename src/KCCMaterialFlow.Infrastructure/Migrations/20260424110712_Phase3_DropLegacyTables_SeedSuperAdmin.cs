using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Phase3_DropLegacyTables_SeedSuperAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ── Wipe données fictives (Compagnies/Contrats/Employees + staging) ──
            // Ré-import Excel requis ensuite pour repeupler avec les vraies données.
            migrationBuilder.Sql("IF OBJECT_ID('dbo.T_WorkflowApprobateurSpecial','U') IS NOT NULL DELETE FROM dbo.T_WorkflowApprobateurSpecial;");
            migrationBuilder.Sql("DELETE FROM dbo.T_Employees;");
            migrationBuilder.Sql("DELETE FROM dbo.T_Contrats;");
            migrationBuilder.Sql("DELETE FROM dbo.T_Compagnies;");
            migrationBuilder.Sql("IF OBJECT_ID('dbo.T_StagingContracts','U') IS NOT NULL DELETE FROM dbo.T_StagingContracts;");
            migrationBuilder.Sql("IF OBJECT_ID('dbo.T_StagingCompanies','U') IS NOT NULL DELETE FROM dbo.T_StagingCompanies;");

            migrationBuilder.DropForeignKey(
                name: "FK_T_Employees_T_Departements_DepartementId",
                schema: "dbo",
                table: "T_Employees");

            migrationBuilder.DropTable(
                name: "T_DepartementRaisonsSortie",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "T_RolePermissions",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "T_UtilisateurActivites",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "T_UtilisateurRoles",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "T_Departements",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "T_Permissions",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "T_Activites",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "T_Utilisateurs",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "T_Roles",
                schema: "dbo");

            migrationBuilder.Sql("IF EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_T_Employees_DepartementId' AND object_id=OBJECT_ID('dbo.T_Employees')) DROP INDEX IX_T_Employees_DepartementId ON dbo.T_Employees;");

            migrationBuilder.DropColumn(
                name: "DepartementId",
                schema: "dbo",
                table: "T_Employees");

            // ── Seed SuperAdmin S09873 (Kasa Kibinga, Israel — Katanga, CD) ──
            migrationBuilder.Sql(@"
INSERT INTO dbo.T_Employees
    (Matricule, NomComplet, DisplayName,
     EstActif, EstInterne, NiveauAdmin, PeutEtreEscorteur,
     DateCreation)
VALUES
    (N'S09873',
     N'Kasa Kibinga, Israel (Katanga - CD)',
     N'Kasa Kibinga, Israel (Katanga - CD)',
     1, 1, 2, 0,
     SYSUTCDATETIME());
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DepartementId",
                schema: "dbo",
                table: "T_Employees",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "T_Activites",
                schema: "dbo",
                columns: table => new
                {
                    IdActivite = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Categorie = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CodeActivite = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EstActif = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    EstSysteme = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Module = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NomActivite = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OrdreAffichage = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Activites", x => x.IdActivite);
                });

            migrationBuilder.CreateTable(
                name: "T_Departements",
                schema: "dbo",
                columns: table => new
                {
                    IdDepartement = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodeDepartement = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModification = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EstActif = table.Column<bool>(type: "bit", nullable: false),
                    NomDepartement = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ResponsableEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ResponsableLogin = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ResponsableNom = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Departements", x => x.IdDepartement);
                });

            migrationBuilder.CreateTable(
                name: "T_Permissions",
                schema: "dbo",
                columns: table => new
                {
                    IdPermission = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Categorie = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CodePermission = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EstActif = table.Column<bool>(type: "bit", nullable: false),
                    EstSysteme = table.Column<bool>(type: "bit", nullable: false),
                    NomPermission = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    OrdreAffichage = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Permissions", x => x.IdPermission);
                });

            migrationBuilder.CreateTable(
                name: "T_Roles",
                schema: "dbo",
                columns: table => new
                {
                    IdRole = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodeRole = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModification = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EstActif = table.Column<bool>(type: "bit", nullable: false),
                    EstSysteme = table.Column<bool>(type: "bit", nullable: false),
                    NiveauPriorite = table.Column<int>(type: "int", nullable: false),
                    NomRole = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Roles", x => x.IdRole);
                });

            migrationBuilder.CreateTable(
                name: "T_DepartementRaisonsSortie",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartementId = table.Column<int>(type: "int", nullable: true),
                    RaisonSortieId = table.Column<int>(type: "int", nullable: false),
                    AutoSelection = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    OrdreAffichage = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_DepartementRaisonsSortie", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T_DepartementRaisonsSortie_T_Departements_DepartementId",
                        column: x => x.DepartementId,
                        principalSchema: "dbo",
                        principalTable: "T_Departements",
                        principalColumn: "IdDepartement",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T_DepartementRaisonsSortie_T_RaisonsSortie_RaisonSortieId",
                        column: x => x.RaisonSortieId,
                        principalSchema: "dbo",
                        principalTable: "T_RaisonsSortie",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_RolePermissions",
                schema: "dbo",
                columns: table => new
                {
                    IdRolePermission = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPermission = table.Column<int>(type: "int", nullable: false),
                    IdRole = table.Column<int>(type: "int", nullable: false),
                    DateAttribution = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_RolePermissions", x => x.IdRolePermission);
                    table.ForeignKey(
                        name: "FK_T_RolePermissions_T_Permissions_IdPermission",
                        column: x => x.IdPermission,
                        principalSchema: "dbo",
                        principalTable: "T_Permissions",
                        principalColumn: "IdPermission",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T_RolePermissions_T_Roles_IdRole",
                        column: x => x.IdRole,
                        principalSchema: "dbo",
                        principalTable: "T_Roles",
                        principalColumn: "IdRole",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_Utilisateurs",
                schema: "dbo",
                columns: table => new
                {
                    IdUtilisateur = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdRole = table.Column<int>(type: "int", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DateModification = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Departement = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DerniereConnexion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EstActif = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Fonction = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Login = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Matricule = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NomComplet = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Telephone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Utilisateurs", x => x.IdUtilisateur);
                    table.ForeignKey(
                        name: "FK_T_Utilisateurs_T_Roles_IdRole",
                        column: x => x.IdRole,
                        principalSchema: "dbo",
                        principalTable: "T_Roles",
                        principalColumn: "IdRole",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "T_UtilisateurActivites",
                schema: "dbo",
                columns: table => new
                {
                    IdUtilisateurActivite = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdActivite = table.Column<int>(type: "int", nullable: false),
                    IdUtilisateur = table.Column<int>(type: "int", nullable: false),
                    AttribueParLogin = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DateAttribution = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
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

            migrationBuilder.CreateTable(
                name: "T_UtilisateurRoles",
                schema: "dbo",
                columns: table => new
                {
                    IdUtilisateurRole = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdRole = table.Column<int>(type: "int", nullable: false),
                    IdUtilisateur = table.Column<int>(type: "int", nullable: false),
                    AttribueParLogin = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DateAttribution = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_UtilisateurRoles", x => x.IdUtilisateurRole);
                    table.ForeignKey(
                        name: "FK_T_UtilisateurRoles_T_Roles_IdRole",
                        column: x => x.IdRole,
                        principalSchema: "dbo",
                        principalTable: "T_Roles",
                        principalColumn: "IdRole",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T_UtilisateurRoles_T_Utilisateurs_IdUtilisateur",
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

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "T_Permissions",
                columns: new[] { "IdPermission", "Categorie", "CodePermission", "DateCreation", "Description", "EstActif", "EstSysteme", "NomPermission", "OrdreAffichage" },
                values: new object[,]
                {
                    { 1, "Système", "ALL", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Accès complet à toutes les fonctionnalités", true, true, "Accès complet", 0 },
                    { 2, "Bons", "CREATE_BON", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Créer des bons d'entrée/sortie de matériel", true, true, "Créer des bons", 10 },
                    { 3, "Bons", "VIEW_BON", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Consulter tous les bons du système", true, true, "Voir tous les bons", 20 },
                    { 4, "Bons", "VIEW_OWN_BON", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Consulter uniquement ses propres bons", true, true, "Voir ses propres bons", 25 },
                    { 5, "Bons", "APPROVE_BON", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Approuver les bons de son département", true, true, "Approuver les bons", 30 },
                    { 6, "Bons", "REJECT_BON", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Rejeter les bons de son département", true, true, "Rejeter les bons", 35 },
                    { 7, "Sécurité", "SCAN_BON", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Scanner les QR codes des bons aux barrières", true, true, "Scanner les QR codes", 40 },
                    { 8, "Sécurité", "CREATE_ANOMALIE", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Créer des signalements d'anomalies", true, true, "Signaler des anomalies", 45 },
                    { 9, "Rapports", "VIEW_REPORTS", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Consulter les rapports et statistiques", true, true, "Accéder aux rapports", 50 },
                    { 10, "Administration", "MANAGE_USERS", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ajouter, modifier et désactiver les utilisateurs", true, true, "Gérer les utilisateurs", 60 },
                    { 11, "Administration", "MANAGE_SETTINGS", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Modifier les paramètres système", true, true, "Gérer les paramètres", 70 }
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "T_Roles",
                columns: new[] { "IdRole", "CodeRole", "DateCreation", "DateModification", "Description", "EstActif", "EstSysteme", "NiveauPriorite", "NomRole" },
                values: new object[,]
                {
                    { 1, "ADMIN", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Accès complet au système", true, true, 100, "Administrateur" },
                    { 2, "APPROBATEUR", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Peut approuver les bons de son département", true, true, 50, "Approbateur" },
                    { 3, "AGENT_SECURITE", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Peut scanner et contrôler les entrées/sorties", true, true, 40, "Agent de sécurité" },
                    { 4, "UTILISATEUR", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Utilisateur standard - peut créer des bons", true, true, 10, "Utilisateur" }
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "T_RolePermissions",
                columns: new[] { "IdRolePermission", "DateAttribution", "IdPermission", "IdRole" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1 },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 2 },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 2 },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 2 },
                    { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 3 },
                    { 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 3 },
                    { 7, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 3 },
                    { 8, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 4 },
                    { 9, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_Employees_DepartementId",
                schema: "dbo",
                table: "T_Employees",
                column: "DepartementId");

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
                name: "IX_DeptRaisonSortie_DeptId",
                schema: "dbo",
                table: "T_DepartementRaisonsSortie",
                column: "DepartementId");

            migrationBuilder.CreateIndex(
                name: "IX_DeptRaisonSortie_DeptId_RaisonId",
                schema: "dbo",
                table: "T_DepartementRaisonsSortie",
                columns: new[] { "DepartementId", "RaisonSortieId" },
                unique: true,
                filter: "[DepartementId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_T_DepartementRaisonsSortie_RaisonSortieId",
                schema: "dbo",
                table: "T_DepartementRaisonsSortie",
                column: "RaisonSortieId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Categorie",
                schema: "dbo",
                table: "T_Permissions",
                column: "Categorie");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_CodePermission",
                schema: "dbo",
                table: "T_Permissions",
                column: "CodePermission",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_EstActif",
                schema: "dbo",
                table: "T_Permissions",
                column: "EstActif");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_Role_Permission",
                schema: "dbo",
                table: "T_RolePermissions",
                columns: new[] { "IdRole", "IdPermission" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_T_RolePermissions_IdPermission",
                schema: "dbo",
                table: "T_RolePermissions",
                column: "IdPermission");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_CodeRole",
                schema: "dbo",
                table: "T_Roles",
                column: "CodeRole",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_EstActif",
                schema: "dbo",
                table: "T_Roles",
                column: "EstActif");

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

            migrationBuilder.CreateIndex(
                name: "IX_T_UtilisateurRoles_IdRole",
                schema: "dbo",
                table: "T_UtilisateurRoles",
                column: "IdRole");

            migrationBuilder.CreateIndex(
                name: "IX_UtilisateurRoles_Utilisateur_Role",
                schema: "dbo",
                table: "T_UtilisateurRoles",
                columns: new[] { "IdUtilisateur", "IdRole" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Utilisateurs_Departement",
                schema: "dbo",
                table: "T_Utilisateurs",
                column: "Departement");

            migrationBuilder.CreateIndex(
                name: "IX_Utilisateurs_EstActif",
                schema: "dbo",
                table: "T_Utilisateurs",
                column: "EstActif");

            migrationBuilder.CreateIndex(
                name: "IX_Utilisateurs_IdRole",
                schema: "dbo",
                table: "T_Utilisateurs",
                column: "IdRole");

            migrationBuilder.CreateIndex(
                name: "IX_Utilisateurs_Login",
                schema: "dbo",
                table: "T_Utilisateurs",
                column: "Login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Utilisateurs_Matricule",
                schema: "dbo",
                table: "T_Utilisateurs",
                column: "Matricule",
                filter: "[Matricule] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_T_Employees_T_Departements_DepartementId",
                schema: "dbo",
                table: "T_Employees",
                column: "DepartementId",
                principalSchema: "dbo",
                principalTable: "T_Departements",
                principalColumn: "IdDepartement");
        }
    }
}
