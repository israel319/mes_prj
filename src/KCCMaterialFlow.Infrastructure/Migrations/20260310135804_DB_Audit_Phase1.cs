using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DB_Audit_Phase1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "dbo",
                table: "T_SoldesMateriels",
                newName: "IdSoldeMateriel");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "dbo",
                table: "T_Sites",
                newName: "IdSite");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "dbo",
                table: "T_RaisonsSortie",
                newName: "IdRaisonSortie");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "dbo",
                table: "T_NotificationsRejet",
                newName: "IdNotificationRejet");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "dbo",
                table: "T_Employees",
                newName: "IdEmployee");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "dbo",
                table: "T_Contrats",
                newName: "IdContrat");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "dbo",
                table: "T_Compagnies",
                newName: "IdCompagnie");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "dbo",
                table: "T_CategoriesSortie",
                newName: "IdCategorieSortie");

            migrationBuilder.AlterColumn<string>(
                name: "MotifRejet",
                schema: "dbo",
                table: "T_NotificationsRejet",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdSoldeMateriel",
                schema: "dbo",
                table: "T_SoldesMateriels",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "IdSite",
                schema: "dbo",
                table: "T_Sites",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "IdRaisonSortie",
                schema: "dbo",
                table: "T_RaisonsSortie",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "IdNotificationRejet",
                schema: "dbo",
                table: "T_NotificationsRejet",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "IdEmployee",
                schema: "dbo",
                table: "T_Employees",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "IdContrat",
                schema: "dbo",
                table: "T_Contrats",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "IdCompagnie",
                schema: "dbo",
                table: "T_Compagnies",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "IdCategorieSortie",
                schema: "dbo",
                table: "T_CategoriesSortie",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "MotifRejet",
                schema: "dbo",
                table: "T_NotificationsRejet",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);
        }
    }
}
