using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCategorieRaisonSortieTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Ajout de OrdreAffichage pour les barrières
            migrationBuilder.AddColumn<int>(
                name: "OrdreAffichage",
                schema: "shared",
                table: "Barrieres",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CategoriesSortie",
                schema: "shared",
                columns: table => new
                {
                    IdCategorie = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Libelle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RequiertApprobationIT = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RequiertApprobationEnvironnement = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    OrdreAffichage = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    EstActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DateModification = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriesSortie", x => x.IdCategorie);
                });

            migrationBuilder.CreateTable(
                name: "RaisonsSortie",
                schema: "shared",
                columns: table => new
                {
                    IdRaison = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategorieId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Libelle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OrdreAffichage = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    EstActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DateModification = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaisonsSortie", x => x.IdRaison);
                    table.ForeignKey(
                        name: "FK_RaisonsSortie_CategoriesSortie_CategorieId",
                        column: x => x.CategorieId,
                        principalSchema: "shared",
                        principalTable: "CategoriesSortie",
                        principalColumn: "IdCategorie",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoriesSortie_Code",
                schema: "shared",
                table: "CategoriesSortie",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CategoriesSortie_Ordre",
                schema: "shared",
                table: "CategoriesSortie",
                column: "OrdreAffichage");

            migrationBuilder.CreateIndex(
                name: "IX_RaisonsSortie_Categorie_Code",
                schema: "shared",
                table: "RaisonsSortie",
                columns: new[] { "CategorieId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RaisonsSortie_Categorie_Ordre",
                schema: "shared",
                table: "RaisonsSortie",
                columns: new[] { "CategorieId", "OrdreAffichage" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RaisonsSortie",
                schema: "shared");

            migrationBuilder.DropTable(
                name: "CategoriesSortie",
                schema: "shared");

            migrationBuilder.DropColumn(
                name: "OrdreAffichage",
                schema: "shared",
                table: "Barrieres");
        }
    }
}
