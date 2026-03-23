using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddQRCodeColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateGenerationQR",
                schema: "bsm",
                table: "BonsSortie",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QRCodeBase64",
                schema: "bsm",
                table: "BonsSortie",
                type: "nvarchar(max)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QRCodeData",
                schema: "bsm",
                table: "BonsSortie",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QRCodeHash",
                schema: "bsm",
                table: "BonsSortie",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateGenerationQR",
                schema: "bsm",
                table: "BonsSortie");

            migrationBuilder.DropColumn(
                name: "QRCodeBase64",
                schema: "bsm",
                table: "BonsSortie");

            migrationBuilder.DropColumn(
                name: "QRCodeData",
                schema: "bsm",
                table: "BonsSortie");

            migrationBuilder.DropColumn(
                name: "QRCodeHash",
                schema: "bsm",
                table: "BonsSortie");
        }
    }
}
