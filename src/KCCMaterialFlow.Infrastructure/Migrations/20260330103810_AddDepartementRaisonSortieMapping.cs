using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartementRaisonSortieMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DepartementId",
                schema: "dbo",
                table: "T_Bons",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "T_DepartementRaisonsSortie",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartementId = table.Column<int>(type: "int", nullable: true),
                    RaisonSortieId = table.Column<int>(type: "int", nullable: false),
                    AutoSelection = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    OrdreAffichage = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_DepartementRaisonsSortie", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T_DepartementRaisonsSortie_T_Departements_DepartementId",
                        column: x => x.DepartementId,
                        principalSchema: "dbo",
                        principalTable: "T_Departements",
                        principalColumn: "IdDepartement",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T_DepartementRaisonsSortie_T_RaisonsSortie_RaisonSortieId",
                        column: x => x.RaisonSortieId,
                        principalSchema: "dbo",
                        principalTable: "T_RaisonsSortie",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeptRaisonSortie_DeptId",
                schema: "dbo",
                table: "T_DepartementRaisonsSortie",
                column: "DepartementId");

            migrationBuilder.CreateIndex(
                name: "IX_DeptRaisonSortie_DeptId_RaisonId",
                schema: "dbo",
                table: "T_DepartementRaisonsSortie",
                columns: new[] { "DepartementId", "RaisonSortieId" },
                unique: true,
                filter: "[DepartementId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_T_DepartementRaisonsSortie_RaisonSortieId",
                schema: "dbo",
                table: "T_DepartementRaisonsSortie",
                column: "RaisonSortieId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_DepartementRaisonsSortie",
                schema: "dbo");

            migrationBuilder.DropColumn(
                name: "DepartementId",
                schema: "dbo",
                table: "T_Bons");
        }
    }
}
