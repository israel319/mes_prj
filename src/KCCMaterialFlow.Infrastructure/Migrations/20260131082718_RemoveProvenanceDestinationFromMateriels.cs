using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveProvenanceDestinationFromMateriels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Destination",
                schema: "bsm",
                table: "MaterielsSortie");

            migrationBuilder.DropColumn(
                name: "Provenance",
                schema: "bsm",
                table: "MaterielsSortie");

            migrationBuilder.DropColumn(
                name: "Destination",
                schema: "bem",
                table: "Materiels");

            migrationBuilder.DropColumn(
                name: "Provenance",
                schema: "bem",
                table: "Materiels");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Destination",
                schema: "bsm",
                table: "MaterielsSortie",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Provenance",
                schema: "bsm",
                table: "MaterielsSortie",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Destination",
                schema: "bem",
                table: "Materiels",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Provenance",
                schema: "bem",
                table: "Materiels",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
