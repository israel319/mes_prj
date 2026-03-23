using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleCodeToApprobationsSortie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RoleCode",
                schema: "dbo",
                table: "T_ApprobationsSortie",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ApprobationsSortie_RoleCode_Decision",
                schema: "dbo",
                table: "T_ApprobationsSortie",
                columns: new[] { "RoleCode", "Decision" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApprobationsSortie_RoleCode_Decision",
                schema: "dbo",
                table: "T_ApprobationsSortie");

            migrationBuilder.DropColumn(
                name: "RoleCode",
                schema: "dbo",
                table: "T_ApprobationsSortie");
        }
    }
}
