using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "bem");

            migrationBuilder.EnsureSchema(
                name: "shared");

            migrationBuilder.CreateTable(
                name: "Barrieres",
                schema: "shared",
                columns: table => new
                {
                    IdBarriere = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodeBarriere = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NomBarriere = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Localisation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TypeBarriere = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Mixte"),
                    EstActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    HorairesOuverture = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Telephone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DateModification = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Barrieres", x => x.IdBarriere);
                });

            migrationBuilder.CreateTable(
                name: "Bons",
                schema: "bem",
                columns: table => new
                {
                    IdBon = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroReference = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TypeBon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DateExpiration = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatutActuel = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "Draft"),
                    Provenance = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Quantite = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreateurLogin = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreateurNom = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreateurDepartement = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DateModification = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificateurLogin = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    QRCodePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    QRCodeData = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DateGenerationQR = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Commentaires = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    EstAnnule = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    MotifAnnulation = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DateAnnulation = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TypeDiscriminator = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    NumeroContrat = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NomCompagnie = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EmailContractant = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TelephoneContractant = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SiteManager = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SiteManagerLogin = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HostDepartment = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReasonOnSite = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NomEscorteur = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FonctionEscorteur = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    EscorteurLogin = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DateEntreePrevue = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateEntreeEffective = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BarriereEntreeId = table.Column<int>(type: "int", nullable: true),
                    BarriereEntreeNom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AgentEntreeLogin = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DoitRessortir = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    BonSortieLieId = table.Column<int>(type: "int", nullable: true),
                    ToutRessorti = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DateToutRessorti = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ZoneTravail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ObservationsEntree = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bons", x => x.IdBon);
                });

            migrationBuilder.CreateTable(
                name: "Departements",
                schema: "shared",
                columns: table => new
                {
                    IdDepartement = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodeDepartement = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NomDepartement = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ResponsableLogin = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ResponsableNom = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ResponsableEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EstActif = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DateModification = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departements", x => x.IdDepartement);
                });

            migrationBuilder.CreateTable(
                name: "Utilisateurs",
                schema: "shared",
                columns: table => new
                {
                    IdUtilisateur = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Login = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NomComplet = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Fonction = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Departement = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Utilisateur"),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Telephone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EstActif = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DateModification = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DerniereConnexion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilisateurs", x => x.IdUtilisateur);
                });

            migrationBuilder.CreateTable(
                name: "Approbations",
                schema: "bem",
                columns: table => new
                {
                    IdApprobation = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BonId = table.Column<int>(type: "int", nullable: false),
                    OrdreEtape = table.Column<int>(type: "int", nullable: false),
                    NomEtape = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StatutAttendu = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Decision = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "EnAttente"),
                    UtilisateurLogin = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UtilisateurNom = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UtilisateurFonction = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    DateAction = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReservesEventuelles = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Commentaire = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    EstEtapeCourante = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Approbations", x => x.IdApprobation);
                    table.ForeignKey(
                        name: "FK_Approbations_Bons_BonId",
                        column: x => x.BonId,
                        principalSchema: "bem",
                        principalTable: "Bons",
                        principalColumn: "IdBon",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BonEntreeHistory",
                schema: "bem",
                columns: table => new
                {
                    IdHistory = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BonId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ActionDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ActionBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ActionByNom = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    StatutAvant = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    StatutApres = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ChangementsJson = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AdresseIP = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BonEntreeHistory", x => x.IdHistory);
                    table.ForeignKey(
                        name: "FK_BonEntreeHistory_Bons_BonId",
                        column: x => x.BonId,
                        principalSchema: "bem",
                        principalTable: "Bons",
                        principalColumn: "IdBon",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItinerairesPrevu",
                schema: "bem",
                columns: table => new
                {
                    IdItineraire = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BonId = table.Column<int>(type: "int", nullable: false),
                    OrdrePassage = table.Column<int>(type: "int", nullable: false),
                    BarriereId = table.Column<int>(type: "int", nullable: false),
                    BarriereNom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BarriereLocalisation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EstObligatoire = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DatePassage = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AgentLogin = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AgentNom = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Observations = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ScanEffectue = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItinerairesPrevu", x => x.IdItineraire);
                    table.ForeignKey(
                        name: "FK_ItinerairesPrevu_Bons_BonId",
                        column: x => x.BonId,
                        principalSchema: "bem",
                        principalTable: "Bons",
                        principalColumn: "IdBon",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Materiels",
                schema: "bem",
                columns: table => new
                {
                    IdMateriel = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BonEntreeId = table.Column<int>(type: "int", nullable: false),
                    CodeProduitSerial = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    TypeMateriel = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "Autre"),
                    Quantite = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    Unite = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "pcs"),
                    Provenance = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Destination = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Marque = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Modele = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ValeurEstimee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Devise = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    EtatEntree = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ObservationsEntree = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EstRessorti = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DateSortie = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EtatSortie = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ObservationsSortie = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BonSortieId = table.Column<int>(type: "int", nullable: true),
                    PhotoEntree = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PhotoSortie = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DateModification = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materiels", x => x.IdMateriel);
                    table.ForeignKey(
                        name: "FK_Materiels_Bons_BonEntreeId",
                        column: x => x.BonEntreeId,
                        principalSchema: "bem",
                        principalTable: "Bons",
                        principalColumn: "IdBon",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Approbations_Bon",
                schema: "bem",
                table: "Approbations",
                column: "BonId");

            migrationBuilder.CreateIndex(
                name: "IX_Approbations_Bon_Etape",
                schema: "bem",
                table: "Approbations",
                columns: new[] { "BonId", "OrdreEtape" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Approbations_Decision",
                schema: "bem",
                table: "Approbations",
                column: "Decision");

            migrationBuilder.CreateIndex(
                name: "IX_Approbations_EtapeCourante",
                schema: "bem",
                table: "Approbations",
                column: "EstEtapeCourante");

            migrationBuilder.CreateIndex(
                name: "IX_Approbations_Utilisateur",
                schema: "bem",
                table: "Approbations",
                column: "UtilisateurLogin");

            migrationBuilder.CreateIndex(
                name: "IX_Approbations_Utilisateur_Decision_Courante",
                schema: "bem",
                table: "Approbations",
                columns: new[] { "UtilisateurLogin", "Decision", "EstEtapeCourante" });

            migrationBuilder.CreateIndex(
                name: "IX_Barrieres_Code",
                schema: "shared",
                table: "Barrieres",
                column: "CodeBarriere",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Barrieres_EstActive",
                schema: "shared",
                table: "Barrieres",
                column: "EstActive");

            migrationBuilder.CreateIndex(
                name: "IX_Barrieres_Localisation",
                schema: "shared",
                table: "Barrieres",
                column: "Localisation");

            migrationBuilder.CreateIndex(
                name: "IX_Barrieres_Type",
                schema: "shared",
                table: "Barrieres",
                column: "TypeBarriere");

            migrationBuilder.CreateIndex(
                name: "IX_BonEntreeHistory_Action",
                schema: "bem",
                table: "BonEntreeHistory",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_BonEntreeHistory_ActionBy",
                schema: "bem",
                table: "BonEntreeHistory",
                column: "ActionBy");

            migrationBuilder.CreateIndex(
                name: "IX_BonEntreeHistory_ActionDate",
                schema: "bem",
                table: "BonEntreeHistory",
                column: "ActionDate");

            migrationBuilder.CreateIndex(
                name: "IX_BonEntreeHistory_Bon",
                schema: "bem",
                table: "BonEntreeHistory",
                column: "BonId");

            migrationBuilder.CreateIndex(
                name: "IX_BonEntreeHistory_Bon_Date",
                schema: "bem",
                table: "BonEntreeHistory",
                columns: new[] { "BonId", "ActionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Bons_BonSortieLie",
                schema: "bem",
                table: "Bons",
                column: "BonSortieLieId");

            migrationBuilder.CreateIndex(
                name: "IX_Bons_Createur",
                schema: "bem",
                table: "Bons",
                column: "CreateurLogin");

            migrationBuilder.CreateIndex(
                name: "IX_Bons_DateCreation",
                schema: "bem",
                table: "Bons",
                column: "DateCreation");

            migrationBuilder.CreateIndex(
                name: "IX_Bons_HostDepartment",
                schema: "bem",
                table: "Bons",
                column: "HostDepartment");

            migrationBuilder.CreateIndex(
                name: "IX_Bons_NomCompagnie",
                schema: "bem",
                table: "Bons",
                column: "NomCompagnie");

            migrationBuilder.CreateIndex(
                name: "IX_Bons_NumeroReference",
                schema: "bem",
                table: "Bons",
                column: "NumeroReference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bons_Ressortie",
                schema: "bem",
                table: "Bons",
                columns: new[] { "DoitRessortir", "ToutRessorti" });

            migrationBuilder.CreateIndex(
                name: "IX_Bons_SiteManager",
                schema: "bem",
                table: "Bons",
                column: "SiteManagerLogin");

            migrationBuilder.CreateIndex(
                name: "IX_Bons_Statut",
                schema: "bem",
                table: "Bons",
                column: "StatutActuel");

            migrationBuilder.CreateIndex(
                name: "IX_Bons_Statut_Type_Date",
                schema: "bem",
                table: "Bons",
                columns: new[] { "StatutActuel", "TypeBon", "DateCreation" });

            migrationBuilder.CreateIndex(
                name: "IX_Bons_Type",
                schema: "bem",
                table: "Bons",
                column: "TypeBon");

            migrationBuilder.CreateIndex(
                name: "IX_Departements_Code",
                schema: "shared",
                table: "Departements",
                column: "CodeDepartement",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departements_EstActif",
                schema: "shared",
                table: "Departements",
                column: "EstActif");

            migrationBuilder.CreateIndex(
                name: "IX_Departements_Responsable",
                schema: "shared",
                table: "Departements",
                column: "ResponsableLogin");

            migrationBuilder.CreateIndex(
                name: "IX_ItinerairesPrevu_Barriere",
                schema: "bem",
                table: "ItinerairesPrevu",
                column: "BarriereId");

            migrationBuilder.CreateIndex(
                name: "IX_ItinerairesPrevu_Bon",
                schema: "bem",
                table: "ItinerairesPrevu",
                column: "BonId");

            migrationBuilder.CreateIndex(
                name: "IX_ItinerairesPrevu_Bon_Ordre",
                schema: "bem",
                table: "ItinerairesPrevu",
                columns: new[] { "BonId", "OrdrePassage" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItinerairesPrevu_Bon_Scan_Ordre",
                schema: "bem",
                table: "ItinerairesPrevu",
                columns: new[] { "BonId", "ScanEffectue", "OrdrePassage" });

            migrationBuilder.CreateIndex(
                name: "IX_ItinerairesPrevu_Scan",
                schema: "bem",
                table: "ItinerairesPrevu",
                column: "ScanEffectue");

            migrationBuilder.CreateIndex(
                name: "IX_Materiels_BonEntree",
                schema: "bem",
                table: "Materiels",
                column: "BonEntreeId");

            migrationBuilder.CreateIndex(
                name: "IX_Materiels_BonEntree_Ressorti",
                schema: "bem",
                table: "Materiels",
                columns: new[] { "BonEntreeId", "EstRessorti" });

            migrationBuilder.CreateIndex(
                name: "IX_Materiels_BonSortie",
                schema: "bem",
                table: "Materiels",
                column: "BonSortieId");

            migrationBuilder.CreateIndex(
                name: "IX_Materiels_CodeSerial",
                schema: "bem",
                table: "Materiels",
                column: "CodeProduitSerial");

            migrationBuilder.CreateIndex(
                name: "IX_Materiels_Ressorti",
                schema: "bem",
                table: "Materiels",
                column: "EstRessorti");

            migrationBuilder.CreateIndex(
                name: "IX_Materiels_Type",
                schema: "bem",
                table: "Materiels",
                column: "TypeMateriel");

            migrationBuilder.CreateIndex(
                name: "IX_Utilisateurs_Departement",
                schema: "shared",
                table: "Utilisateurs",
                column: "Departement");

            migrationBuilder.CreateIndex(
                name: "IX_Utilisateurs_EstActif",
                schema: "shared",
                table: "Utilisateurs",
                column: "EstActif");

            migrationBuilder.CreateIndex(
                name: "IX_Utilisateurs_Login",
                schema: "shared",
                table: "Utilisateurs",
                column: "Login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Utilisateurs_Role",
                schema: "shared",
                table: "Utilisateurs",
                column: "Role");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Approbations",
                schema: "bem");

            migrationBuilder.DropTable(
                name: "Barrieres",
                schema: "shared");

            migrationBuilder.DropTable(
                name: "BonEntreeHistory",
                schema: "bem");

            migrationBuilder.DropTable(
                name: "Departements",
                schema: "shared");

            migrationBuilder.DropTable(
                name: "ItinerairesPrevu",
                schema: "bem");

            migrationBuilder.DropTable(
                name: "Materiels",
                schema: "bem");

            migrationBuilder.DropTable(
                name: "Utilisateurs",
                schema: "shared");

            migrationBuilder.DropTable(
                name: "Bons",
                schema: "bem");
        }
    }
}
