using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "sec");

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    IdAuditLog = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateAction = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UtilisateurLogin = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UtilisateurNom = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TypeAction = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Categorie = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntiteId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EntiteType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Details = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AncienneValeur = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NouvelleValeur = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AdresseIP = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Resultat = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MessageErreur = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Niveau = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DureeMs = table.Column<int>(type: "int", nullable: true),
                    CorrelationId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.IdAuditLog);
                });

            migrationBuilder.CreateTable(
                name: "ParametresSysteme",
                columns: table => new
                {
                    IdParametre = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Valeur = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    TypeDonnee = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Categorie = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Libelle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ValeurDefaut = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ValeursPossibles = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ValeurMin = table.Column<int>(type: "int", nullable: true),
                    ValeurMax = table.Column<int>(type: "int", nullable: true),
                    Unite = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Ordre = table.Column<int>(type: "int", nullable: false),
                    NecessiteRedemarrage = table.Column<bool>(type: "bit", nullable: false),
                    EstVisible = table.Column<bool>(type: "bit", nullable: false),
                    EstModifiable = table.Column<bool>(type: "bit", nullable: false),
                    EstSysteme = table.Column<bool>(type: "bit", nullable: false),
                    ModifieParLogin = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DateModification = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParametresSysteme", x => x.IdParametre);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    IdRole = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodeRole = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NomRole = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Permissions = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    NiveauPriorite = table.Column<int>(type: "int", nullable: false),
                    EstActif = table.Column<bool>(type: "bit", nullable: false),
                    EstSysteme = table.Column<bool>(type: "bit", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModification = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.IdRole);
                });

            migrationBuilder.CreateTable(
                name: "Statuts",
                columns: table => new
                {
                    IdStatut = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodeStatut = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LibelleStatut = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TypeBon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CouleurFond = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CouleurTexte = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Icone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Ordre = table.Column<int>(type: "int", nullable: false),
                    EstFinal = table.Column<bool>(type: "bit", nullable: false),
                    RequiertAction = table.Column<bool>(type: "bit", nullable: false),
                    StatutsSuivants = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EstActif = table.Column<bool>(type: "bit", nullable: false),
                    EstSysteme = table.Column<bool>(type: "bit", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModification = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statuts", x => x.IdStatut);
                });

            migrationBuilder.CreateTable(
                name: "TypesMateriels",
                columns: table => new
                {
                    IdTypeMateriel = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NomType = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Categorie = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Icone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Couleur = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RequiertApprobationDepartement = table.Column<bool>(type: "bit", nullable: false),
                    RequiertApprobationDirection = table.Column<bool>(type: "bit", nullable: false),
                    NiveauxApprobation = table.Column<int>(type: "int", nullable: false),
                    DureeValiditeDefautJours = table.Column<int>(type: "int", nullable: false),
                    DureeMaximumJours = table.Column<int>(type: "int", nullable: false),
                    NumeroSerieObligatoire = table.Column<bool>(type: "bit", nullable: false),
                    PhotoObligatoire = table.Column<bool>(type: "bit", nullable: false),
                    ChampsPersonnalises = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    WorkflowConfig = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Ordre = table.Column<int>(type: "int", nullable: false),
                    EstActif = table.Column<bool>(type: "bit", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModification = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypesMateriels", x => x.IdTypeMateriel);
                });

            migrationBuilder.CreateTable(
                name: "UtilisateurRoles",
                columns: table => new
                {
                    IdUtilisateurRole = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUtilisateur = table.Column<int>(type: "int", nullable: false),
                    IdRole = table.Column<int>(type: "int", nullable: false),
                    DateAttribution = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AttribueParLogin = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UtilisateurRoles", x => x.IdUtilisateurRole);
                    table.ForeignKey(
                        name: "FK_UtilisateurRoles_Roles_IdRole",
                        column: x => x.IdRole,
                        principalTable: "Roles",
                        principalColumn: "IdRole",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UtilisateurRoles_Utilisateurs_IdUtilisateur",
                        column: x => x.IdUtilisateur,
                        principalSchema: "shared",
                        principalTable: "Utilisateurs",
                        principalColumn: "IdUtilisateur",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Anomalies",
                schema: "sec",
                columns: table => new
                {
                    IdAnomalie = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeAnomalie = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NiveauGravite = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Moyen"),
                    DateSignalement = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    BonId = table.Column<int>(type: "int", nullable: true),
                    TypeBon = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    NumeroReferenceBon = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ScanId = table.Column<int>(type: "int", nullable: true),
                    BarriereId = table.Column<int>(type: "int", nullable: true),
                    EstTraitee = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DateTraitement = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SignalePar = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SignaleParNom = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TraitePar = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Resolution = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ActionsCorrectives = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Anomalies", x => x.IdAnomalie);
                    table.ForeignKey(
                        name: "FK_Anomalies_Barrieres_BarriereId",
                        column: x => x.BarriereId,
                        principalSchema: "shared",
                        principalTable: "Barrieres",
                        principalColumn: "IdBarriere",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScanEvenements",
                schema: "sec",
                columns: table => new
                {
                    IdScan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateHeureScan = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    StatutScan = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "Valid"),
                    TypeMouvement = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Sortie"),
                    BonId = table.Column<int>(type: "int", nullable: true),
                    TypeBon = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    NumeroReferenceBon = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    BarriereId = table.Column<int>(type: "int", nullable: false),
                    AgentLogin = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AgentNom = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    QRCodeData = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    QRCodeHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Observations = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AnomalieSignalee = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    NumeroPreuve = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProvenanceLieu = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DestinationLieu = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AnomalieIdAnomalie = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScanEvenements", x => x.IdScan);
                    table.ForeignKey(
                        name: "FK_ScanEvenements_Anomalies_AnomalieIdAnomalie",
                        column: x => x.AnomalieIdAnomalie,
                        principalSchema: "sec",
                        principalTable: "Anomalies",
                        principalColumn: "IdAnomalie");
                    table.ForeignKey(
                        name: "FK_ScanEvenements_Barrieres_BarriereId",
                        column: x => x.BarriereId,
                        principalSchema: "shared",
                        principalTable: "Barrieres",
                        principalColumn: "IdBarriere",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistoriqueScans",
                schema: "sec",
                columns: table => new
                {
                    IdHistorique = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScanId = table.Column<int>(type: "int", nullable: false),
                    DateHeureMouvement = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    TypeMouvement = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TypeBon = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NumeroReferenceBon = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CodeBarriere = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NomBarriere = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Direction = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Departement = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Provenance = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Destination = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NombreMateriels = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ResumeMateriels = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MaterielsJson = table.Column<string>(type: "nvarchar(max)", maxLength: 500, nullable: true),
                    StatutScan = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    PassageAutorise = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    AgentLogin = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AgentNom = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NomDemandeur = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MatriculeVehicule = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Observations = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AnomalieSignalee = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    NombreAnomalies = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoriqueScans", x => x.IdHistorique);
                    table.ForeignKey(
                        name: "FK_HistoriqueScans_ScanEvenements_ScanId",
                        column: x => x.ScanId,
                        principalSchema: "sec",
                        principalTable: "ScanEvenements",
                        principalColumn: "IdScan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ParametresSysteme",
                columns: new[] { "IdParametre", "Categorie", "Cle", "DateModification", "Description", "EstModifiable", "EstSysteme", "EstVisible", "Libelle", "ModifieParLogin", "NecessiteRedemarrage", "Ordre", "TypeDonnee", "Unite", "Valeur", "ValeurDefaut", "ValeurMax", "ValeurMin", "ValeursPossibles" },
                values: new object[,]
                {
                    { 1, "General", "APP_NOM", null, "Nom affiché dans l'en-tête et les emails", true, false, true, "Nom de l'application", null, false, 1, "String", null, "KCC Material Flow", null, null, null, null },
                    { 2, "General", "APP_VERSION", null, "Version actuelle du système", false, true, true, "Version de l'application", null, false, 2, "String", null, "1.0.0", null, null, null, null },
                    { 10, "Workflow", "WORKFLOW_DUREE_VALIDITE_DEFAUT", null, "Durée de validité par défaut pour les bons de sortie", true, false, true, "Durée de validité par défaut (jours)", null, false, 1, "Integer", "jours", "30", "30", 365, 1, null },
                    { 11, "Workflow", "WORKFLOW_DELAI_RAPPEL_EXPIRATION", null, "Nombre de jours avant expiration pour envoyer un rappel", true, false, true, "Délai de rappel avant expiration (jours)", null, false, 2, "Integer", "jours", "3", "3", 30, 1, null },
                    { 12, "Workflow", "WORKFLOW_DELAI_APPROBATION_MAX", null, "Délai maximum pour qu'un approbateur valide un bon", true, false, true, "Délai maximum d'approbation (jours)", null, false, 3, "Integer", "jours", "7", "7", 30, 1, null },
                    { 20, "Email", "EMAIL_ACTIVER_NOTIFICATIONS", null, "Active ou désactive l'envoi des notifications par email", true, false, true, "Activer les notifications email", null, false, 1, "Boolean", null, "true", "true", null, null, "true|false" },
                    { 21, "Email", "EMAIL_EXPEDITEUR", null, "Adresse email utilisée comme expéditeur pour les notifications", true, false, true, "Adresse email expéditeur", null, false, 2, "String", null, "noreply@kccmaterialflow.local", null, null, null, null },
                    { 22, "Email", "EMAIL_ADMIN", null, "Adresse email pour les notifications administratives", true, false, true, "Email administrateur", null, false, 3, "String", null, "admin@kccmaterialflow.local", null, null, null, null },
                    { 30, "Securite", "SECURITE_QRCODE_DUREE_VALIDITE", null, "Durée pendant laquelle un QR Code scanné est valide", true, false, true, "Durée de validité QR Code (minutes)", null, false, 1, "Integer", "minutes", "60", "60", 1440, 5, null },
                    { 31, "Securite", "SECURITE_MAX_SCANS_JOUR", null, "Nombre maximum de scans autorisés par bon par jour", true, false, true, "Nombre maximum de scans par jour", null, false, 2, "Integer", null, "10", "10", 100, 1, null },
                    { 32, "Securite", "SECURITE_DETECTER_ANOMALIES_AUTO", null, "Active la détection automatique des anomalies lors des scans", true, false, true, "Détection automatique des anomalies", null, false, 3, "Boolean", null, "true", "true", null, null, "true|false" },
                    { 40, "Interface", "UI_ITEMS_PAR_PAGE", null, "Nombre d'éléments affichés par page dans les listes", true, false, true, "Éléments par page", null, false, 1, "Integer", null, "20", "20", 100, 10, "10|20|50|100" },
                    { 41, "Interface", "UI_THEME_DEFAUT", null, "Thème visuel par défaut de l'application", true, false, true, "Thème par défaut", null, false, 2, "String", null, "light", "light", null, null, "light|dark|auto" },
                    { 42, "Interface", "UI_LANGUE_DEFAUT", null, "Langue par défaut de l'interface", true, false, true, "Langue par défaut", null, false, 3, "String", null, "fr", "fr", null, null, "fr|en" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "IdRole", "CodeRole", "DateCreation", "DateModification", "Description", "EstActif", "EstSysteme", "NiveauPriorite", "NomRole", "Permissions" },
                values: new object[,]
                {
                    { 1, "ADMIN", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Accès complet au système", true, true, 100, "Administrateur", "[\"ALL\"]" },
                    { 2, "APPROBATEUR", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Peut approuver les bons de son département", true, true, 50, "Approbateur", "[\"VIEW_BON\",\"APPROVE_BON\",\"REJECT_BON\"]" },
                    { 3, "AGENT_SECURITE", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Peut scanner et contrôler les entrées/sorties", true, true, 40, "Agent de sécurité", "[\"VIEW_BON\",\"SCAN_BON\",\"CREATE_ANOMALIE\"]" },
                    { 4, "UTILISATEUR", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Utilisateur standard - peut créer des bons", true, true, 10, "Utilisateur", "[\"CREATE_BON\",\"VIEW_OWN_BON\"]" }
                });

            migrationBuilder.InsertData(
                table: "Statuts",
                columns: new[] { "IdStatut", "CodeStatut", "CouleurFond", "CouleurTexte", "DateCreation", "DateModification", "Description", "EstActif", "EstFinal", "EstSysteme", "Icone", "LibelleStatut", "Ordre", "RequiertAction", "StatutsSuivants", "TypeBon" },
                values: new object[,]
                {
                    { 1, "BROUILLON", "#6c757d", "#ffffff", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Bon en cours de création", true, false, true, "bi-pencil", "Brouillon", 1, true, "2", "Tous" },
                    { 2, "EN_ATTENTE_APPROBATION", "#ffc107", "#212529", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Bon soumis, en attente de validation", true, false, true, "bi-hourglass-split", "En attente d'approbation", 2, true, "3,4", "Tous" },
                    { 3, "APPROUVE", "#28a745", "#ffffff", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Bon approuvé par le responsable", true, false, true, "bi-check-circle", "Approuvé", 3, false, "5,6", "Tous" },
                    { 4, "REJETE", "#dc3545", "#ffffff", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Bon rejeté par le responsable", true, true, true, "bi-x-circle", "Rejeté", 4, false, null, "Tous" },
                    { 5, "EN_COURS", "#17a2b8", "#ffffff", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Matériel en cours d'utilisation/sortie", true, false, true, "bi-arrow-repeat", "En cours", 5, false, "6,7", "BonSortie" },
                    { 6, "TERMINE", "#198754", "#ffffff", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Processus terminé avec succès", true, true, true, "bi-check2-all", "Terminé", 6, false, null, "Tous" },
                    { 7, "EXPIRE", "#fd7e14", "#ffffff", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Bon expiré - matériel non retourné à temps", true, false, true, "bi-exclamation-triangle", "Expiré", 7, true, "6", "BonSortie" },
                    { 8, "ANNULE", "#6c757d", "#ffffff", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Bon annulé par l'utilisateur ou le système", true, true, true, "bi-slash-circle", "Annulé", 8, false, null, "Tous" }
                });

            migrationBuilder.InsertData(
                table: "TypesMateriels",
                columns: new[] { "IdTypeMateriel", "Categorie", "ChampsPersonnalises", "CodeType", "Couleur", "DateCreation", "DateModification", "Description", "DureeMaximumJours", "DureeValiditeDefautJours", "EstActif", "Icone", "NiveauxApprobation", "NomType", "NumeroSerieObligatoire", "Ordre", "PhotoObligatoire", "RequiertApprobationDepartement", "RequiertApprobationDirection", "WorkflowConfig" },
                values: new object[,]
                {
                    { 1, "Matériel roulant", null, "VEHICULE", "#0d6efd", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Véhicules de société (voitures, camions, engins)", 30, 1, true, "bi-truck", 1, "Véhicule", true, 1, false, true, false, null },
                    { 2, "Informatique", null, "EQUIPEMENT_IT", "#6610f2", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Ordinateurs, laptops, serveurs, équipements réseau", 365, 30, true, "bi-laptop", 2, "Équipement informatique", true, 2, true, true, true, null },
                    { 3, "Équipement", null, "OUTILLAGE", "#fd7e14", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Outils et équipements de travail", 90, 7, true, "bi-tools", 1, "Outillage", false, 3, false, true, false, null },
                    { 4, "Documentation", null, "DOCUMENT", "#20c997", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Documents confidentiels, plans, archives", 7, 1, true, "bi-file-earmark-text", 1, "Document", false, 4, false, true, false, null },
                    { 5, "Divers", null, "MATERIEL_DIVERS", "#6c757d", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Autre matériel non catégorisé", 180, 7, true, "bi-box-seam", 1, "Matériel divers", false, 99, false, true, false, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Anomalies_BarriereId",
                schema: "sec",
                table: "Anomalies",
                column: "BarriereId");

            migrationBuilder.CreateIndex(
                name: "IX_Anomalies_BonId",
                schema: "sec",
                table: "Anomalies",
                column: "BonId");

            migrationBuilder.CreateIndex(
                name: "IX_Anomalies_DateSignalement",
                schema: "sec",
                table: "Anomalies",
                column: "DateSignalement");

            migrationBuilder.CreateIndex(
                name: "IX_Anomalies_EstTraitee",
                schema: "sec",
                table: "Anomalies",
                column: "EstTraitee");

            migrationBuilder.CreateIndex(
                name: "IX_Anomalies_EstTraitee_NiveauGravite",
                schema: "sec",
                table: "Anomalies",
                columns: new[] { "EstTraitee", "NiveauGravite" });

            migrationBuilder.CreateIndex(
                name: "IX_Anomalies_NiveauGravite",
                schema: "sec",
                table: "Anomalies",
                column: "NiveauGravite");

            migrationBuilder.CreateIndex(
                name: "IX_Anomalies_ScanId",
                schema: "sec",
                table: "Anomalies",
                column: "ScanId");

            migrationBuilder.CreateIndex(
                name: "IX_Anomalies_SignalePar",
                schema: "sec",
                table: "Anomalies",
                column: "SignalePar");

            migrationBuilder.CreateIndex(
                name: "IX_Anomalies_TypeAnomalie",
                schema: "sec",
                table: "Anomalies",
                column: "TypeAnomalie");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Categorie",
                table: "AuditLogs",
                column: "Categorie");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_CorrelationId",
                table: "AuditLogs",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_DateAction",
                table: "AuditLogs",
                column: "DateAction");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Entite",
                table: "AuditLogs",
                columns: new[] { "EntiteType", "EntiteId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Niveau",
                table: "AuditLogs",
                column: "Niveau");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Reporting",
                table: "AuditLogs",
                columns: new[] { "DateAction", "Categorie", "TypeAction" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_TypeAction",
                table: "AuditLogs",
                column: "TypeAction");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UtilisateurLogin",
                table: "AuditLogs",
                column: "UtilisateurLogin");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriqueScans_AgentLogin",
                schema: "sec",
                table: "HistoriqueScans",
                column: "AgentLogin");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriqueScans_AnomalieSignalee",
                schema: "sec",
                table: "HistoriqueScans",
                column: "AnomalieSignalee");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriqueScans_Barriere_Date",
                schema: "sec",
                table: "HistoriqueScans",
                columns: new[] { "CodeBarriere", "DateHeureMouvement" });

            migrationBuilder.CreateIndex(
                name: "IX_HistoriqueScans_CodeBarriere",
                schema: "sec",
                table: "HistoriqueScans",
                column: "CodeBarriere");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriqueScans_Date_TypeMouvement",
                schema: "sec",
                table: "HistoriqueScans",
                columns: new[] { "DateHeureMouvement", "TypeMouvement" });

            migrationBuilder.CreateIndex(
                name: "IX_HistoriqueScans_DateHeureMouvement",
                schema: "sec",
                table: "HistoriqueScans",
                column: "DateHeureMouvement");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriqueScans_NumeroReferenceBon",
                schema: "sec",
                table: "HistoriqueScans",
                column: "NumeroReferenceBon");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriqueScans_PassageAutorise",
                schema: "sec",
                table: "HistoriqueScans",
                column: "PassageAutorise");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriqueScans_ScanId",
                schema: "sec",
                table: "HistoriqueScans",
                column: "ScanId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriqueScans_Statut_Passage",
                schema: "sec",
                table: "HistoriqueScans",
                columns: new[] { "StatutScan", "PassageAutorise" });

            migrationBuilder.CreateIndex(
                name: "IX_HistoriqueScans_TypeBon",
                schema: "sec",
                table: "HistoriqueScans",
                column: "TypeBon");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriqueScans_TypeMouvement",
                schema: "sec",
                table: "HistoriqueScans",
                column: "TypeMouvement");

            migrationBuilder.CreateIndex(
                name: "IX_ParametresSysteme_Categorie",
                table: "ParametresSysteme",
                column: "Categorie");

            migrationBuilder.CreateIndex(
                name: "IX_ParametresSysteme_Cle",
                table: "ParametresSysteme",
                column: "Cle",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_CodeRole",
                table: "Roles",
                column: "CodeRole",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_EstActif",
                table: "Roles",
                column: "EstActif");

            migrationBuilder.CreateIndex(
                name: "IX_ScanEvenements_AgentLogin",
                schema: "sec",
                table: "ScanEvenements",
                column: "AgentLogin");

            migrationBuilder.CreateIndex(
                name: "IX_ScanEvenements_AnomalieIdAnomalie",
                schema: "sec",
                table: "ScanEvenements",
                column: "AnomalieIdAnomalie");

            migrationBuilder.CreateIndex(
                name: "IX_ScanEvenements_BarriereId",
                schema: "sec",
                table: "ScanEvenements",
                column: "BarriereId");

            migrationBuilder.CreateIndex(
                name: "IX_ScanEvenements_BonId",
                schema: "sec",
                table: "ScanEvenements",
                column: "BonId");

            migrationBuilder.CreateIndex(
                name: "IX_ScanEvenements_DateHeureScan",
                schema: "sec",
                table: "ScanEvenements",
                column: "DateHeureScan");

            migrationBuilder.CreateIndex(
                name: "IX_ScanEvenements_StatutScan",
                schema: "sec",
                table: "ScanEvenements",
                column: "StatutScan");

            migrationBuilder.CreateIndex(
                name: "IX_ScanEvenements_TypeBon_BonId",
                schema: "sec",
                table: "ScanEvenements",
                columns: new[] { "TypeBon", "BonId" });

            migrationBuilder.CreateIndex(
                name: "IX_Statuts_CodeStatut",
                table: "Statuts",
                column: "CodeStatut",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Statuts_TypeBon_EstActif",
                table: "Statuts",
                columns: new[] { "TypeBon", "EstActif" });

            migrationBuilder.CreateIndex(
                name: "IX_TypesMateriels_CodeType",
                table: "TypesMateriels",
                column: "CodeType",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TypesMateriels_EstActif_Ordre",
                table: "TypesMateriels",
                columns: new[] { "EstActif", "Ordre" });

            migrationBuilder.CreateIndex(
                name: "IX_UtilisateurRoles_IdRole",
                table: "UtilisateurRoles",
                column: "IdRole");

            migrationBuilder.CreateIndex(
                name: "IX_UtilisateurRoles_Utilisateur_Role",
                table: "UtilisateurRoles",
                columns: new[] { "IdUtilisateur", "IdRole" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Anomalies_ScanEvenements_ScanId",
                schema: "sec",
                table: "Anomalies",
                column: "ScanId",
                principalSchema: "sec",
                principalTable: "ScanEvenements",
                principalColumn: "IdScan",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Anomalies_ScanEvenements_ScanId",
                schema: "sec",
                table: "Anomalies");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "HistoriqueScans",
                schema: "sec");

            migrationBuilder.DropTable(
                name: "ParametresSysteme");

            migrationBuilder.DropTable(
                name: "Statuts");

            migrationBuilder.DropTable(
                name: "TypesMateriels");

            migrationBuilder.DropTable(
                name: "UtilisateurRoles");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "ScanEvenements",
                schema: "sec");

            migrationBuilder.DropTable(
                name: "Anomalies",
                schema: "sec");
        }
    }
}
