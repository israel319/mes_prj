using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkflowEtapesConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_WorkflowEtapesConfig",
                schema: "dbo",
                columns: table => new
                {
                    IdWorkflowEtapeConfig = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BonType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RaisonSortieCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OrdreEtape = table.Column<int>(type: "int", nullable: false),
                    RoleCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NomEtape = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EstActif = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DateModification = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifieParLogin = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_WorkflowEtapesConfig", x => x.IdWorkflowEtapeConfig);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowEtapesConfig_BonType_Raison_Actif",
                schema: "dbo",
                table: "T_WorkflowEtapesConfig",
                columns: new[] { "BonType", "RaisonSortieCode", "EstActif" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowEtapesConfig_BonType_Raison_Ordre",
                schema: "dbo",
                table: "T_WorkflowEtapesConfig",
                columns: new[] { "BonType", "RaisonSortieCode", "OrdreEtape" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_WorkflowEtapesConfig",
                schema: "dbo");
        }
    }
}
