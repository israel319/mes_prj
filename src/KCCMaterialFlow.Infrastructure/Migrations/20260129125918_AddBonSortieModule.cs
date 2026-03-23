using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBonSortieModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "bsm");

            migrationBuilder.CreateTable(
                name: "BonsSortie",
                schema: "bsm",
                columns: table => new
                {
                    IdBon = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroReference = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DateExpiration = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatutActuel = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "Draft"),
                    Destination = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Provenance = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Quantite = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    NomDemandeur = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FonctionDemandeur = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DepartementDemandeur = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MotifSortie = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    EstDefinitif = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    TypeSortie = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    BonEntreeAssocieId = table.Column<int>(type: "int", nullable: true),
                    TypeMateriel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NomDestinataire = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AdresseDestination = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NumeroVehicule = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NomChauffeur = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TelephoneChauffeur = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DateRetourPrevue = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateRetourEffective = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EstRetourne = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    EtatRetour = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ReceptionnePar = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TypeMaterielInterne = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DepartementOrigine = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DepartementDestination = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NomReceveur = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FonctionReceveur = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    EmailReceveur = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LocalisationDestination = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DateTransfertPrevue = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateTransfertEffective = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BonsSortie", x => x.IdBon);
                    table.ForeignKey(
                        name: "FK_BonsSortie_Bons_BonEntreeAssocieId",
                        column: x => x.BonEntreeAssocieId,
                        principalSchema: "bem",
                        principalTable: "Bons",
                        principalColumn: "IdBon",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ApprobationsSortie",
                schema: "bsm",
                columns: table => new
                {
                    IdApprobation = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BonSortieId = table.Column<int>(type: "int", nullable: false),
                    OrdreEtape = table.Column<int>(type: "int", nullable: false),
                    NomEtape = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Decision = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "En attente"),
                    DateAction = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprobateurLogin = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ApprobateurNom = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Commentaire = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprobationsSortie", x => x.IdApprobation);
                    table.ForeignKey(
                        name: "FK_ApprobationsSortie_BonsSortie_BonSortieId",
                        column: x => x.BonSortieId,
                        principalSchema: "bsm",
                        principalTable: "BonsSortie",
                        principalColumn: "IdBon",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BonSortieHistories",
                schema: "bsm",
                columns: table => new
                {
                    IdHistory = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BonSortieId = table.Column<int>(type: "int", nullable: false),
                    TypeAction = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StatutAvant = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StatutApres = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UtilisateurLogin = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UtilisateurNom = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DateAction = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    AdresseIP = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BonSortieHistories", x => x.IdHistory);
                    table.ForeignKey(
                        name: "FK_BonSortieHistories_BonsSortie_BonSortieId",
                        column: x => x.BonSortieId,
                        principalSchema: "bsm",
                        principalTable: "BonsSortie",
                        principalColumn: "IdBon",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItinerairesSortie",
                schema: "bsm",
                columns: table => new
                {
                    IdItineraire = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BonSortieId = table.Column<int>(type: "int", nullable: false),
                    BarriereId = table.Column<int>(type: "int", nullable: false),
                    OrdrePassage = table.Column<int>(type: "int", nullable: false),
                    DatePassagePrevue = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DatePassageEffective = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StatutPassage = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Prévu"),
                    Observations = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItinerairesSortie", x => x.IdItineraire);
                    table.ForeignKey(
                        name: "FK_ItinerairesSortie_BonsSortie_BonSortieId",
                        column: x => x.BonSortieId,
                        principalSchema: "bsm",
                        principalTable: "BonsSortie",
                        principalColumn: "IdBon",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaterielsSortie",
                schema: "bsm",
                columns: table => new
                {
                    IdMateriel = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BonSortieId = table.Column<int>(type: "int", nullable: false),
                    CodeProduitSerial = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Quantite = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 1m),
                    Provenance = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Destination = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterielsSortie", x => x.IdMateriel);
                    table.ForeignKey(
                        name: "FK_MaterielsSortie_BonsSortie_BonSortieId",
                        column: x => x.BonSortieId,
                        principalSchema: "bsm",
                        principalTable: "BonsSortie",
                        principalColumn: "IdBon",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprobationsSortie_BonSortieId",
                schema: "bsm",
                table: "ApprobationsSortie",
                column: "BonSortieId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprobationsSortie_Etape",
                schema: "bsm",
                table: "ApprobationsSortie",
                columns: new[] { "BonSortieId", "OrdreEtape" });

            migrationBuilder.CreateIndex(
                name: "IX_BonSortieHistories_BonSortieId",
                schema: "bsm",
                table: "BonSortieHistories",
                column: "BonSortieId");

            migrationBuilder.CreateIndex(
                name: "IX_BonSortieHistories_DateAction",
                schema: "bsm",
                table: "BonSortieHistories",
                column: "DateAction");

            migrationBuilder.CreateIndex(
                name: "IX_BonSortieHistories_Utilisateur",
                schema: "bsm",
                table: "BonSortieHistories",
                column: "UtilisateurLogin");

            migrationBuilder.CreateIndex(
                name: "IX_BonsSortie_BonEntreeAssocie",
                schema: "bsm",
                table: "BonsSortie",
                column: "BonEntreeAssocieId");

            migrationBuilder.CreateIndex(
                name: "IX_BonsSortie_DateCreation",
                schema: "bsm",
                table: "BonsSortie",
                column: "DateCreation");

            migrationBuilder.CreateIndex(
                name: "IX_BonsSortie_DateRetourPrevue",
                schema: "bsm",
                table: "BonsSortie",
                column: "DateRetourPrevue");

            migrationBuilder.CreateIndex(
                name: "IX_BonsSortie_Departement",
                schema: "bsm",
                table: "BonsSortie",
                column: "DepartementDemandeur");

            migrationBuilder.CreateIndex(
                name: "IX_BonsSortie_DeptDestination",
                schema: "bsm",
                table: "BonsSortie",
                column: "DepartementDestination");

            migrationBuilder.CreateIndex(
                name: "IX_BonsSortie_DeptOrigine",
                schema: "bsm",
                table: "BonsSortie",
                column: "DepartementOrigine");

            migrationBuilder.CreateIndex(
                name: "IX_BonsSortie_Destinataire",
                schema: "bsm",
                table: "BonsSortie",
                column: "NomDestinataire");

            migrationBuilder.CreateIndex(
                name: "IX_BonsSortie_EstRetourne",
                schema: "bsm",
                table: "BonsSortie",
                column: "EstRetourne");

            migrationBuilder.CreateIndex(
                name: "IX_BonsSortie_NumeroReference",
                schema: "bsm",
                table: "BonsSortie",
                column: "NumeroReference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BonsSortie_PretsEnRetard",
                schema: "bsm",
                table: "BonsSortie",
                columns: new[] { "EstRetourne", "DateRetourPrevue" });

            migrationBuilder.CreateIndex(
                name: "IX_BonsSortie_Statut",
                schema: "bsm",
                table: "BonsSortie",
                column: "StatutActuel");

            migrationBuilder.CreateIndex(
                name: "IX_BonsSortie_Transfert",
                schema: "bsm",
                table: "BonsSortie",
                columns: new[] { "DepartementOrigine", "DepartementDestination" });

            migrationBuilder.CreateIndex(
                name: "IX_BonsSortie_TypeMateriel",
                schema: "bsm",
                table: "BonsSortie",
                column: "TypeMateriel");

            migrationBuilder.CreateIndex(
                name: "IX_ItinerairesSortie_BarriereId",
                schema: "bsm",
                table: "ItinerairesSortie",
                column: "BarriereId");

            migrationBuilder.CreateIndex(
                name: "IX_ItinerairesSortie_BonSortieId",
                schema: "bsm",
                table: "ItinerairesSortie",
                column: "BonSortieId");

            migrationBuilder.CreateIndex(
                name: "IX_ItinerairesSortie_Ordre",
                schema: "bsm",
                table: "ItinerairesSortie",
                columns: new[] { "BonSortieId", "OrdrePassage" });

            migrationBuilder.CreateIndex(
                name: "IX_MaterielsSortie_BonSortieId",
                schema: "bsm",
                table: "MaterielsSortie",
                column: "BonSortieId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterielsSortie_CodeProduit",
                schema: "bsm",
                table: "MaterielsSortie",
                column: "CodeProduitSerial");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprobationsSortie",
                schema: "bsm");

            migrationBuilder.DropTable(
                name: "BonSortieHistories",
                schema: "bsm");

            migrationBuilder.DropTable(
                name: "ItinerairesSortie",
                schema: "bsm");

            migrationBuilder.DropTable(
                name: "MaterielsSortie",
                schema: "bsm");

            migrationBuilder.DropTable(
                name: "BonsSortie",
                schema: "bsm");
        }
    }
}
