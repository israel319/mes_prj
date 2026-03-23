using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTypeSortieColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Departements_DepartementId",
                schema: "ref",
                table: "Employees");

            migrationBuilder.DropTable(
                name: "Departements",
                schema: "ref");

            migrationBuilder.DropIndex(
                name: "IX_Departements_Code",
                schema: "shared",
                table: "Departements");

            migrationBuilder.DropIndex(
                name: "IX_Departements_EstActif",
                schema: "shared",
                table: "Departements");

            migrationBuilder.DropIndex(
                name: "IX_Departements_Responsable",
                schema: "shared",
                table: "Departements");

            migrationBuilder.AlterColumn<bool>(
                name: "EstActif",
                schema: "shared",
                table: "Departements",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreation",
                schema: "shared",
                table: "Departements",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "TypeSortie",
                schema: "bsm",
                table: "BonsSortie",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Departements_DepartementId",
                schema: "ref",
                table: "Employees",
                column: "DepartementId",
                principalSchema: "shared",
                principalTable: "Departements",
                principalColumn: "IdDepartement");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Departements_DepartementId",
                schema: "ref",
                table: "Employees");

            migrationBuilder.AlterColumn<bool>(
                name: "EstActif",
                schema: "shared",
                table: "Departements",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreation",
                schema: "shared",
                table: "Departements",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "TypeSortie",
                schema: "bsm",
                table: "BonsSortie",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.CreateTable(
                name: "Departements",
                schema: "ref",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EstActif = table.Column<bool>(type: "bit", nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OrdreAffichage = table.Column<int>(type: "int", nullable: false),
                    Responsable = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departements", x => x.Id);
                });

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

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Departements_DepartementId",
                schema: "ref",
                table: "Employees",
                column: "DepartementId",
                principalSchema: "ref",
                principalTable: "Departements",
                principalColumn: "Id");
        }
    }
}
