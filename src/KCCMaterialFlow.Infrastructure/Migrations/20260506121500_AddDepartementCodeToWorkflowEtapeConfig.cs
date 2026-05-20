using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartementCodeToWorkflowEtapeConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DepartementCode",
                schema: "dbo",
                table: "T_WorkflowEtapesConfig",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowEtapesConfig_BonType_Dept_Actif",
                schema: "dbo",
                table: "T_WorkflowEtapesConfig",
                columns: new[] { "BonType", "DepartementCode", "EstActif" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkflowEtapesConfig_BonType_Dept_Actif",
                schema: "dbo",
                table: "T_WorkflowEtapesConfig");

            migrationBuilder.DropColumn(
                name: "DepartementCode",
                schema: "dbo",
                table: "T_WorkflowEtapesConfig");
        }
    }
}
