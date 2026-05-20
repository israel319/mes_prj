using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddApprovalChainProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EtapeActuelleApprobation",
                schema: "dbo",
                table: "T_Bons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProchainApprobateurId",
                schema: "dbo",
                table: "T_Bons",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProchainApprobateurNom",
                schema: "dbo",
                table: "T_Bons",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EtapeActuelleApprobation",
                schema: "dbo",
                table: "T_Bons");

            migrationBuilder.DropColumn(
                name: "ProchainApprobateurId",
                schema: "dbo",
                table: "T_Bons");

            migrationBuilder.DropColumn(
                name: "ProchainApprobateurNom",
                schema: "dbo",
                table: "T_Bons");
        }
    }
}
