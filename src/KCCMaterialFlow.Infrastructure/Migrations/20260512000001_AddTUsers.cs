using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_Users",
                schema: "dbo",
                columns: table => new
                {
                    IdUser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Login = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: true),
                    NiveauAdmin = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    EstActif = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Users", x => x.IdUser);
                    table.ForeignKey(
                        name: "FK_T_Users_T_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "dbo",
                        principalTable: "T_Employees",
                        principalColumn: "IdEmployee",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_Users_EmployeeId",
                schema: "dbo",
                table: "T_Users",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_T_Users_Login",
                schema: "dbo",
                table: "T_Users",
                column: "Login",
                unique: true);

            // Migration des données : copier Login + NiveauAdmin + EstActif depuis T_Employees
            // vers T_Users pour les employés qui ont un login Windows renseigné.
            migrationBuilder.Sql("""
                INSERT INTO dbo.T_Users (Login, EmployeeId, NiveauAdmin, EstActif, DateCreation)
                SELECT
                    e.Login,
                    e.Id,
                    e.NiveauAdmin,
                    e.EstActif,
                    GETUTCDATE()
                FROM dbo.T_Employees e
                WHERE e.Login IS NOT NULL
                  AND LTRIM(RTRIM(e.Login)) <> ''
                  AND NOT EXISTS (
                      SELECT 1 FROM dbo.T_Users u
                      WHERE LOWER(u.Login) = LOWER(e.Login)
                  );
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_Users",
                schema: "dbo");
        }
    }
}
