using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContratTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumeroContrat",
                schema: "ref",
                table: "Compagnies");

            migrationBuilder.AddColumn<int>(
                name: "ContratId",
                schema: "bem",
                table: "Bons",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Contrats",
                schema: "ref",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PoNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContratDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CompagnieId = table.Column<int>(type: "int", nullable: false),
                    EstActif = table.Column<bool>(type: "bit", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contrats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contrats_Compagnies_CompagnieId",
                        column: x => x.CompagnieId,
                        principalSchema: "ref",
                        principalTable: "Compagnies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contrats_CompagnieId",
                schema: "ref",
                table: "Contrats",
                column: "CompagnieId");

            migrationBuilder.CreateIndex(
                name: "IX_Contrats_PoNumber",
                schema: "ref",
                table: "Contrats",
                column: "PoNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contrats",
                schema: "ref");

            migrationBuilder.DropColumn(
                name: "ContratId",
                schema: "bem",
                table: "Bons");

            migrationBuilder.AddColumn<string>(
                name: "NumeroContrat",
                schema: "ref",
                table: "Compagnies",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
