using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddApprobationDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NomApprobateur",
                schema: "bem",
                table: "Approbations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NomEtape",
                schema: "bem",
                table: "Approbations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoleApprobateur",
                schema: "bem",
                table: "Approbations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NomApprobateur",
                schema: "bem",
                table: "Approbations");

            migrationBuilder.DropColumn(
                name: "NomEtape",
                schema: "bem",
                table: "Approbations");

            migrationBuilder.DropColumn(
                name: "RoleApprobateur",
                schema: "bem",
                table: "Approbations");
        }
    }
}
