using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSiteAndRequestedForToBons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_T_WorkflowApprobateurSpecial_Type_EmployeeId",
                schema: "dbo",
                table: "T_WorkflowApprobateurSpecial");

            migrationBuilder.AddColumn<int>(
                name: "SiteId",
                schema: "dbo",
                table: "T_WorkflowApprobateurSpecial",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartementCode",
                schema: "dbo",
                table: "T_MaterielsSortie",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartementCode",
                schema: "dbo",
                table: "T_Materiels",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestedForDepartement",
                schema: "dbo",
                table: "T_BonsSortie",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestedForDisplay",
                schema: "dbo",
                table: "T_BonsSortie",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestedForEmployeeCode",
                schema: "dbo",
                table: "T_BonsSortie",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SiteId",
                schema: "dbo",
                table: "T_BonsSortie",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SiteManager",
                schema: "dbo",
                table: "T_BonsSortie",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestedForDepartement",
                schema: "dbo",
                table: "T_Bons",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestedForDisplay",
                schema: "dbo",
                table: "T_Bons",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestedForEmployeeCode",
                schema: "dbo",
                table: "T_Bons",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SiteId",
                schema: "dbo",
                table: "T_Bons",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_T_WorkflowApprobateurSpecial_SiteId",
                schema: "dbo",
                table: "T_WorkflowApprobateurSpecial",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_T_WorkflowApprobateurSpecial_Type_EmployeeId_SiteId",
                schema: "dbo",
                table: "T_WorkflowApprobateurSpecial",
                columns: new[] { "Type", "EmployeeId", "SiteId" },
                unique: true,
                filter: "[SiteId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_T_BonsSortie_SiteId",
                schema: "dbo",
                table: "T_BonsSortie",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_T_Bons_SiteId",
                schema: "dbo",
                table: "T_Bons",
                column: "SiteId");

            migrationBuilder.AddForeignKey(
                name: "FK_T_Bons_T_Sites_SiteId",
                schema: "dbo",
                table: "T_Bons",
                column: "SiteId",
                principalSchema: "dbo",
                principalTable: "T_Sites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_T_BonsSortie_T_Sites_SiteId",
                schema: "dbo",
                table: "T_BonsSortie",
                column: "SiteId",
                principalSchema: "dbo",
                principalTable: "T_Sites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_T_WorkflowApprobateurSpecial_T_Sites_SiteId",
                schema: "dbo",
                table: "T_WorkflowApprobateurSpecial",
                column: "SiteId",
                principalSchema: "dbo",
                principalTable: "T_Sites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_Bons_T_Sites_SiteId",
                schema: "dbo",
                table: "T_Bons");

            migrationBuilder.DropForeignKey(
                name: "FK_T_BonsSortie_T_Sites_SiteId",
                schema: "dbo",
                table: "T_BonsSortie");

            migrationBuilder.DropForeignKey(
                name: "FK_T_WorkflowApprobateurSpecial_T_Sites_SiteId",
                schema: "dbo",
                table: "T_WorkflowApprobateurSpecial");

            migrationBuilder.DropIndex(
                name: "IX_T_WorkflowApprobateurSpecial_SiteId",
                schema: "dbo",
                table: "T_WorkflowApprobateurSpecial");

            migrationBuilder.DropIndex(
                name: "IX_T_WorkflowApprobateurSpecial_Type_EmployeeId_SiteId",
                schema: "dbo",
                table: "T_WorkflowApprobateurSpecial");

            migrationBuilder.DropIndex(
                name: "IX_T_BonsSortie_SiteId",
                schema: "dbo",
                table: "T_BonsSortie");

            migrationBuilder.DropIndex(
                name: "IX_T_Bons_SiteId",
                schema: "dbo",
                table: "T_Bons");

            migrationBuilder.DropColumn(
                name: "SiteId",
                schema: "dbo",
                table: "T_WorkflowApprobateurSpecial");

            migrationBuilder.DropColumn(
                name: "DepartementCode",
                schema: "dbo",
                table: "T_MaterielsSortie");

            migrationBuilder.DropColumn(
                name: "DepartementCode",
                schema: "dbo",
                table: "T_Materiels");

            migrationBuilder.DropColumn(
                name: "RequestedForDepartement",
                schema: "dbo",
                table: "T_BonsSortie");

            migrationBuilder.DropColumn(
                name: "RequestedForDisplay",
                schema: "dbo",
                table: "T_BonsSortie");

            migrationBuilder.DropColumn(
                name: "RequestedForEmployeeCode",
                schema: "dbo",
                table: "T_BonsSortie");

            migrationBuilder.DropColumn(
                name: "SiteId",
                schema: "dbo",
                table: "T_BonsSortie");

            migrationBuilder.DropColumn(
                name: "SiteManager",
                schema: "dbo",
                table: "T_BonsSortie");

            migrationBuilder.DropColumn(
                name: "RequestedForDepartement",
                schema: "dbo",
                table: "T_Bons");

            migrationBuilder.DropColumn(
                name: "RequestedForDisplay",
                schema: "dbo",
                table: "T_Bons");

            migrationBuilder.DropColumn(
                name: "RequestedForEmployeeCode",
                schema: "dbo",
                table: "T_Bons");

            migrationBuilder.DropColumn(
                name: "SiteId",
                schema: "dbo",
                table: "T_Bons");

            migrationBuilder.CreateIndex(
                name: "IX_T_WorkflowApprobateurSpecial_Type_EmployeeId",
                schema: "dbo",
                table: "T_WorkflowApprobateurSpecial",
                columns: new[] { "Type", "EmployeeId" },
                unique: true);
        }
    }
}
