using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBonEntreeSortieLiaison : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BonSortieAssocieId",
                schema: "bem",
                table: "Bons",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BonSortieAssocieNumero",
                schema: "bem",
                table: "Bons",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateVerrouillage",
                schema: "bem",
                table: "Bons",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EstVerrouillePourSortie",
                schema: "bem",
                table: "Bons",
                type: "bit",
                nullable: true,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Bons_BonSortieAssocieId",
                schema: "bem",
                table: "Bons",
                column: "BonSortieAssocieId",
                filter: "[BonSortieAssocieId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bons_BonSortieAssocieId",
                schema: "bem",
                table: "Bons");

            migrationBuilder.DropColumn(
                name: "BonSortieAssocieId",
                schema: "bem",
                table: "Bons");

            migrationBuilder.DropColumn(
                name: "BonSortieAssocieNumero",
                schema: "bem",
                table: "Bons");

            migrationBuilder.DropColumn(
                name: "DateVerrouillage",
                schema: "bem",
                table: "Bons");

            migrationBuilder.DropColumn(
                name: "EstVerrouillePourSortie",
                schema: "bem",
                table: "Bons");
        }
    }
}
