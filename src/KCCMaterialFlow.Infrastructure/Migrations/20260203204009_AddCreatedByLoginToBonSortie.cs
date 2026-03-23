using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedByLoginToBonSortie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByLogin",
                schema: "bsm",
                table: "BonsSortie",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "BonIdBon",
                schema: "bem",
                table: "BonEntreeHistory",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BonEntreeHistory_BonIdBon",
                schema: "bem",
                table: "BonEntreeHistory",
                column: "BonIdBon");

            migrationBuilder.AddForeignKey(
                name: "FK_BonEntreeHistory_Bons_BonIdBon",
                schema: "bem",
                table: "BonEntreeHistory",
                column: "BonIdBon",
                principalSchema: "bem",
                principalTable: "Bons",
                principalColumn: "IdBon");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BonEntreeHistory_Bons_BonIdBon",
                schema: "bem",
                table: "BonEntreeHistory");

            migrationBuilder.DropIndex(
                name: "IX_BonEntreeHistory_BonIdBon",
                schema: "bem",
                table: "BonEntreeHistory");

            migrationBuilder.DropColumn(
                name: "CreatedByLogin",
                schema: "bsm",
                table: "BonsSortie");

            migrationBuilder.DropColumn(
                name: "BonIdBon",
                schema: "bem",
                table: "BonEntreeHistory");
        }
    }
}
