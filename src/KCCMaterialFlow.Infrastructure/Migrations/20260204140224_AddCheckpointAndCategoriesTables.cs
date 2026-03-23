using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckpointAndCategoriesTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoriesSortie",
                schema: "ref",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EstActif = table.Column<bool>(type: "bit", nullable: false),
                    OrdreAffichage = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriesSortie", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Checkpoints",
                schema: "ref",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiteId = table.Column<int>(type: "int", nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EstActif = table.Column<bool>(type: "bit", nullable: false),
                    OrdreDefaut = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Checkpoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Checkpoints_Sites_SiteId",
                        column: x => x.SiteId,
                        principalSchema: "ref",
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RaisonsSortie",
                schema: "ref",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategorieId = table.Column<int>(type: "int", nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RequiertBonEntree = table.Column<bool>(type: "bit", nullable: false),
                    RequiertDetails = table.Column<bool>(type: "bit", nullable: false),
                    EstActif = table.Column<bool>(type: "bit", nullable: false),
                    OrdreAffichage = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaisonsSortie", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RaisonsSortie_CategoriesSortie_CategorieId",
                        column: x => x.CategorieId,
                        principalSchema: "ref",
                        principalTable: "CategoriesSortie",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PassagesCheckpoint",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeBon = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    BonId = table.Column<int>(type: "int", nullable: false),
                    NumeroReference = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CheckpointId = table.Column<int>(type: "int", nullable: false),
                    OrdrePrevu = table.Column<int>(type: "int", nullable: false),
                    OrdreEffectif = table.Column<int>(type: "int", nullable: true),
                    DatePrevue = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateEffective = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Statut = table.Column<int>(type: "int", nullable: false),
                    ScannePar = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EstAnomalie = table.Column<bool>(type: "bit", nullable: false),
                    TypeAnomalie = table.Column<int>(type: "int", nullable: true),
                    DescriptionAnomalie = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Observations = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CoordonneeGPS = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PassagesCheckpoint", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PassagesCheckpoint_Checkpoints_CheckpointId",
                        column: x => x.CheckpointId,
                        principalSchema: "ref",
                        principalTable: "Checkpoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Checkpoints_SiteId",
                schema: "ref",
                table: "Checkpoints",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_PassagesCheckpoint_CheckpointId",
                schema: "dbo",
                table: "PassagesCheckpoint",
                column: "CheckpointId");

            migrationBuilder.CreateIndex(
                name: "IX_RaisonsSortie_CategorieId",
                schema: "ref",
                table: "RaisonsSortie",
                column: "CategorieId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PassagesCheckpoint",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "RaisonsSortie",
                schema: "ref");

            migrationBuilder.DropTable(
                name: "Checkpoints",
                schema: "ref");

            migrationBuilder.DropTable(
                name: "CategoriesSortie",
                schema: "ref");
        }
    }
}
