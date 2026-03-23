using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTypeMaterielDefautToRaisonSortie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TypeMaterielDefaut",
                schema: "dbo",
                table: "T_RaisonsSortie",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TypeMaterielDefaut",
                schema: "dbo",
                table: "T_RaisonsSortie");
        }
    }
}
