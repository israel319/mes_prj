using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRaisonEntreeMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RaisonEntreeId",
                schema: "dbo",
                table: "T_Bons",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "T_RaisonsEntree",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EstActif = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    OrdreAffichage = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Icone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Couleur = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_RaisonsEntree", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "T_RaisonEntreeRaisonsSortie",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RaisonEntreeId = table.Column<int>(type: "int", nullable: false),
                    RaisonSortieId = table.Column<int>(type: "int", nullable: false),
                    AutoSelection = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    OrdreAffichage = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_RaisonEntreeRaisonsSortie", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T_RaisonEntreeRaisonsSortie_T_RaisonsEntree_RaisonEntreeId",
                        column: x => x.RaisonEntreeId,
                        principalSchema: "dbo",
                        principalTable: "T_RaisonsEntree",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T_RaisonEntreeRaisonsSortie_T_RaisonsSortie_RaisonSortieId",
                        column: x => x.RaisonSortieId,
                        principalSchema: "dbo",
                        principalTable: "T_RaisonsSortie",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RaisonEntreeRaisonsSortie_EntreeId",
                schema: "dbo",
                table: "T_RaisonEntreeRaisonsSortie",
                column: "RaisonEntreeId");

            migrationBuilder.CreateIndex(
                name: "IX_RaisonEntreeRaisonsSortie_EntreeId_SortieId",
                schema: "dbo",
                table: "T_RaisonEntreeRaisonsSortie",
                columns: new[] { "RaisonEntreeId", "RaisonSortieId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_T_RaisonEntreeRaisonsSortie_RaisonSortieId",
                schema: "dbo",
                table: "T_RaisonEntreeRaisonsSortie",
                column: "RaisonSortieId");

            migrationBuilder.CreateIndex(
                name: "IX_RaisonsEntree_Code",
                schema: "dbo",
                table: "T_RaisonsEntree",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_RaisonEntreeRaisonsSortie",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "T_RaisonsEntree",
                schema: "dbo");

            migrationBuilder.DropColumn(
                name: "RaisonEntreeId",
                schema: "dbo",
                table: "T_Bons");
        }
    }
}
