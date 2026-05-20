using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAuthColumnsFromEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Supprimer l'index sur NiveauAdmin (créé dans la migration initiale)
            migrationBuilder.DropIndex(
                name: "IX_T_Employees_NiveauAdmin",
                schema: "dbo",
                table: "T_Employees");

            // Supprimer les colonnes d'authentification de T_Employees
            // (désormais portées par T_Users / AppUser)
            migrationBuilder.DropColumn(
                name: "Login",
                schema: "dbo",
                table: "T_Employees");

            migrationBuilder.DropColumn(
                name: "EstActif",
                schema: "dbo",
                table: "T_Employees");

            migrationBuilder.DropColumn(
                name: "NiveauAdmin",
                schema: "dbo",
                table: "T_Employees");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Login",
                schema: "dbo",
                table: "T_Employees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EstActif",
                schema: "dbo",
                table: "T_Employees",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "NiveauAdmin",
                schema: "dbo",
                table: "T_Employees",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_T_Employees_NiveauAdmin",
                schema: "dbo",
                table: "T_Employees",
                column: "NiveauAdmin");
        }
    }
}
