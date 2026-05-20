using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMatriculeToUtilisateur : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Matricule",
                schema: "dbo",
                table: "T_Utilisateurs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Utilisateurs_Matricule",
                schema: "dbo",
                table: "T_Utilisateurs",
                column: "Matricule",
                filter: "[Matricule] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Utilisateurs_Matricule",
                schema: "dbo",
                table: "T_Utilisateurs");

            migrationBuilder.DropColumn(
                name: "Matricule",
                schema: "dbo",
                table: "T_Utilisateurs");
        }
    }
}
