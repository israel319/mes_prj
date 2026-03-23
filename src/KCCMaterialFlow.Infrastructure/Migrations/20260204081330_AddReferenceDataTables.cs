using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReferenceDataTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ref");

            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Compagnies",
                schema: "ref",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NumeroContrat = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Telephone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SiteManager = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EstActif = table.Column<bool>(type: "bit", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compagnies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departements",
                schema: "ref",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Responsable = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EstActif = table.Column<bool>(type: "bit", nullable: false),
                    OrdreAffichage = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationsRejet",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BonType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NumeroReference = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EtapeRejet = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApprobateurNom = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ApprobateurLogin = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MotifRejet = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DemandeurNom = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DateRejet = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstLue = table.Column<bool>(type: "bit", nullable: false),
                    DateLecture = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmailEnvoye = table.Column<bool>(type: "bit", nullable: false),
                    DateEnvoiEmail = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationsRejet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sites",
                schema: "ref",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Adresse = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TypeSite = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EstInterne = table.Column<bool>(type: "bit", nullable: false),
                    EstActif = table.Column<bool>(type: "bit", nullable: false),
                    OrdreAffichage = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                schema: "ref",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Matricule = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NomComplet = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Prenom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Fonction = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Telephone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DepartementId = table.Column<int>(type: "int", nullable: true),
                    CompagnieId = table.Column<int>(type: "int", nullable: true),
                    EstInterne = table.Column<bool>(type: "bit", nullable: false),
                    PeutEtreEscorteur = table.Column<bool>(type: "bit", nullable: false),
                    EstActif = table.Column<bool>(type: "bit", nullable: false),
                    Login = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Compagnies_CompagnieId",
                        column: x => x.CompagnieId,
                        principalSchema: "ref",
                        principalTable: "Compagnies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Employees_Departements_DepartementId",
                        column: x => x.DepartementId,
                        principalSchema: "ref",
                        principalTable: "Departements",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CompagnieId",
                schema: "ref",
                table: "Employees",
                column: "CompagnieId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartementId",
                schema: "ref",
                table: "Employees",
                column: "DepartementId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employees",
                schema: "ref");

            migrationBuilder.DropTable(
                name: "NotificationsRejet",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Sites",
                schema: "ref");

            migrationBuilder.DropTable(
                name: "Compagnies",
                schema: "ref");

            migrationBuilder.DropTable(
                name: "Departements",
                schema: "ref");
        }
    }
}
