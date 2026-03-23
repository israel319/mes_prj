using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBonEntreeAssocieIdToInterne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "QuantiteSortie",
                schema: "bem",
                table: "SoldesMateriels",
                type: "decimal(18,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "QuantiteInitiale",
                schema: "bem",
                table: "SoldesMateriels",
                type: "decimal(18,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "QuantiteDisponible",
                schema: "bsm",
                table: "MaterielsSortie",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantite",
                schema: "bsm",
                table: "MaterielsSortie",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 1m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldDefaultValue: 1m);

            migrationBuilder.AddColumn<decimal>(
                name: "QuantiteInitialeBem",
                schema: "bsm",
                table: "MaterielsSortie",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "QuantiteDisponible",
                schema: "bem",
                table: "Materiels",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "BonSortieInterne_BonEntreeAssocieId",
                schema: "bsm",
                table: "BonsSortie",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaterielsSortie_MaterielEntreeId",
                schema: "bsm",
                table: "MaterielsSortie",
                column: "MaterielEntreeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MaterielsSortie_MaterielEntreeId",
                schema: "bsm",
                table: "MaterielsSortie");

            migrationBuilder.DropColumn(
                name: "QuantiteInitialeBem",
                schema: "bsm",
                table: "MaterielsSortie");

            migrationBuilder.DropColumn(
                name: "QuantiteDisponible",
                schema: "bem",
                table: "Materiels");

            migrationBuilder.DropColumn(
                name: "BonSortieInterne_BonEntreeAssocieId",
                schema: "bsm",
                table: "BonsSortie");

            migrationBuilder.AlterColumn<decimal>(
                name: "QuantiteSortie",
                schema: "bem",
                table: "SoldesMateriels",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "QuantiteInitiale",
                schema: "bem",
                table: "SoldesMateriels",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "QuantiteDisponible",
                schema: "bsm",
                table: "MaterielsSortie",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantite",
                schema: "bsm",
                table: "MaterielsSortie",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 1m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldDefaultValue: 1m);
        }
    }
}
