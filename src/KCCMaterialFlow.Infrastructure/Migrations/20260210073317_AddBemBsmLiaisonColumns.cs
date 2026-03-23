using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBemBsmLiaisonColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BonsSortie_DeptDestination",
                schema: "bsm",
                table: "BonsSortie");

            migrationBuilder.DropIndex(
                name: "IX_BonsSortie_Transfert",
                schema: "bsm",
                table: "BonsSortie");

            migrationBuilder.DropColumn(
                name: "DepartementDestination",
                schema: "bsm",
                table: "BonsSortie");

            migrationBuilder.DropColumn(
                name: "NomReceveur",
                schema: "bsm",
                table: "BonsSortie");

            migrationBuilder.AddColumn<string>(
                name: "Couleur",
                schema: "ref",
                table: "RaisonsSortie",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DureeMaxJours",
                schema: "ref",
                table: "RaisonsSortie",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EstTemporaire",
                schema: "ref",
                table: "RaisonsSortie",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Icone",
                schema: "ref",
                table: "RaisonsSortie",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiertBarrieres",
                schema: "ref",
                table: "RaisonsSortie",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TypeApprobateurSpecial",
                schema: "ref",
                table: "RaisonsSortie",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ValidationSpeciale",
                schema: "ref",
                table: "RaisonsSortie",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "BonEntreeId",
                schema: "bsm",
                table: "MaterielsSortie",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BonEntreeNumero",
                schema: "bsm",
                table: "MaterielsSortie",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaterielEntreeId",
                schema: "bsm",
                table: "MaterielsSortie",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Observations",
                schema: "bsm",
                table: "MaterielsSortie",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "QuantiteDisponible",
                schema: "bsm",
                table: "MaterielsSortie",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiertBarrieres",
                schema: "ref",
                table: "CategoriesSortie",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequiertBonEntree",
                schema: "ref",
                table: "CategoriesSortie",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TypeEntite",
                schema: "ref",
                table: "CategoriesSortie",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SoldesMateriels",
                schema: "bem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaterielEntreeId = table.Column<int>(type: "int", nullable: false),
                    BonEntreeId = table.Column<int>(type: "int", nullable: false),
                    CodeProduitSerial = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    QuantiteInitiale = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QuantiteSortie = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DateDerniereMaj = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DernierBsmNumero = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoldesMateriels", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SoldesMateriels",
                schema: "bem");

            migrationBuilder.DropColumn(
                name: "Couleur",
                schema: "ref",
                table: "RaisonsSortie");

            migrationBuilder.DropColumn(
                name: "DureeMaxJours",
                schema: "ref",
                table: "RaisonsSortie");

            migrationBuilder.DropColumn(
                name: "EstTemporaire",
                schema: "ref",
                table: "RaisonsSortie");

            migrationBuilder.DropColumn(
                name: "Icone",
                schema: "ref",
                table: "RaisonsSortie");

            migrationBuilder.DropColumn(
                name: "RequiertBarrieres",
                schema: "ref",
                table: "RaisonsSortie");

            migrationBuilder.DropColumn(
                name: "TypeApprobateurSpecial",
                schema: "ref",
                table: "RaisonsSortie");

            migrationBuilder.DropColumn(
                name: "ValidationSpeciale",
                schema: "ref",
                table: "RaisonsSortie");

            migrationBuilder.DropColumn(
                name: "BonEntreeId",
                schema: "bsm",
                table: "MaterielsSortie");

            migrationBuilder.DropColumn(
                name: "BonEntreeNumero",
                schema: "bsm",
                table: "MaterielsSortie");

            migrationBuilder.DropColumn(
                name: "MaterielEntreeId",
                schema: "bsm",
                table: "MaterielsSortie");

            migrationBuilder.DropColumn(
                name: "Observations",
                schema: "bsm",
                table: "MaterielsSortie");

            migrationBuilder.DropColumn(
                name: "QuantiteDisponible",
                schema: "bsm",
                table: "MaterielsSortie");

            migrationBuilder.DropColumn(
                name: "RequiertBarrieres",
                schema: "ref",
                table: "CategoriesSortie");

            migrationBuilder.DropColumn(
                name: "RequiertBonEntree",
                schema: "ref",
                table: "CategoriesSortie");

            migrationBuilder.DropColumn(
                name: "TypeEntite",
                schema: "ref",
                table: "CategoriesSortie");

            migrationBuilder.AddColumn<string>(
                name: "DepartementDestination",
                schema: "bsm",
                table: "BonsSortie",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NomReceveur",
                schema: "bsm",
                table: "BonsSortie",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BonsSortie_DeptDestination",
                schema: "bsm",
                table: "BonsSortie",
                column: "DepartementDestination");

            migrationBuilder.CreateIndex(
                name: "IX_BonsSortie_Transfert",
                schema: "bsm",
                table: "BonsSortie",
                columns: new[] { "DepartementOrigine", "DepartementDestination" });
        }
    }
}
