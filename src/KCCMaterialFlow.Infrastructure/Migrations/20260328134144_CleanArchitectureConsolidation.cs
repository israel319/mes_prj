using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CleanArchitectureConsolidation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_BonEntreeHistory_T_Bons_BonIdBon",
                schema: "dbo",
                table: "T_BonEntreeHistory");

            migrationBuilder.DropIndex(
                name: "IX_T_BonEntreeHistory_BonIdBon",
                schema: "dbo",
                table: "T_BonEntreeHistory");

            migrationBuilder.DropColumn(
                name: "DatePassagePrevue",
                schema: "dbo",
                table: "T_ItinerairesSortie");

            migrationBuilder.DropColumn(
                name: "Observations",
                schema: "dbo",
                table: "T_ItinerairesSortie");

            migrationBuilder.DropColumn(
                name: "TypeDiscriminator",
                schema: "dbo",
                table: "T_Bons");

            migrationBuilder.DropColumn(
                name: "BonIdBon",
                schema: "dbo",
                table: "T_BonEntreeHistory");

            migrationBuilder.AddColumn<string>(
                name: "Remarque",
                schema: "dbo",
                table: "T_MaterielsSortie",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StatutPassage",
                schema: "dbo",
                table: "T_ItinerairesSortie",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldDefaultValue: "Prévu");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "dbo",
                table: "T_BonsSortie",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "dbo",
                table: "T_BonsSortie",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "dbo",
                table: "T_BonsSortie",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "dbo",
                table: "T_BonsSortie",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "dbo",
                table: "T_BonSortieHistories",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChangementsJson",
                schema: "dbo",
                table: "T_BonSortieHistories",
                type: "nvarchar(max)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                schema: "dbo",
                table: "T_BonSortieHistories",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SiteManager",
                schema: "dbo",
                table: "T_Bons",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReasonOnSite",
                schema: "dbo",
                table: "T_Bons",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "QRCodeBase64",
                schema: "dbo",
                table: "T_Bons",
                type: "nvarchar(max)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NomEscorteur",
                schema: "dbo",
                table: "T_Bons",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NomDemandeur",
                schema: "dbo",
                table: "T_Bons",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NomCompagnie",
                schema: "dbo",
                table: "T_Bons",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HostDepartment",
                schema: "dbo",
                table: "T_Bons",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "EstVerrouillePourSortie",
                schema: "dbo",
                table: "T_Bons",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "dbo",
                table: "T_Bons",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "dbo",
                table: "T_Bons",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "dbo",
                table: "T_Bons",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "dbo",
                table: "T_Bons",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ChangementsJson",
                schema: "dbo",
                table: "T_BonEntreeHistory",
                type: "nvarchar(max)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Remarque",
                schema: "dbo",
                table: "T_MaterielsSortie");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "dbo",
                table: "T_BonsSortie");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "dbo",
                table: "T_BonsSortie");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "dbo",
                table: "T_BonsSortie");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "dbo",
                table: "T_BonsSortie");

            migrationBuilder.DropColumn(
                name: "ChangementsJson",
                schema: "dbo",
                table: "T_BonSortieHistories");

            migrationBuilder.DropColumn(
                name: "Comment",
                schema: "dbo",
                table: "T_BonSortieHistories");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "dbo",
                table: "T_Bons");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "dbo",
                table: "T_Bons");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "dbo",
                table: "T_Bons");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "dbo",
                table: "T_Bons");

            migrationBuilder.AlterColumn<string>(
                name: "StatutPassage",
                schema: "dbo",
                table: "T_ItinerairesSortie",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Prévu",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<DateTime>(
                name: "DatePassagePrevue",
                schema: "dbo",
                table: "T_ItinerairesSortie",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Observations",
                schema: "dbo",
                table: "T_ItinerairesSortie",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "dbo",
                table: "T_BonSortieHistories",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "SiteManager",
                schema: "dbo",
                table: "T_Bons",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "ReasonOnSite",
                schema: "dbo",
                table: "T_Bons",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "QRCodeBase64",
                schema: "dbo",
                table: "T_Bons",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NomEscorteur",
                schema: "dbo",
                table: "T_Bons",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "NomDemandeur",
                schema: "dbo",
                table: "T_Bons",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "NomCompagnie",
                schema: "dbo",
                table: "T_Bons",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "HostDepartment",
                schema: "dbo",
                table: "T_Bons",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<bool>(
                name: "EstVerrouillePourSortie",
                schema: "dbo",
                table: "T_Bons",
                type: "bit",
                nullable: true,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TypeDiscriminator",
                schema: "dbo",
                table: "T_Bons",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "ChangementsJson",
                schema: "dbo",
                table: "T_BonEntreeHistory",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BonIdBon",
                schema: "dbo",
                table: "T_BonEntreeHistory",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_T_BonEntreeHistory_BonIdBon",
                schema: "dbo",
                table: "T_BonEntreeHistory",
                column: "BonIdBon");

            migrationBuilder.AddForeignKey(
                name: "FK_T_BonEntreeHistory_T_Bons_BonIdBon",
                schema: "dbo",
                table: "T_BonEntreeHistory",
                column: "BonIdBon",
                principalSchema: "dbo",
                principalTable: "T_Bons",
                principalColumn: "IdBon");
        }
    }
}
