using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGlencoreEmployeeReferential : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_GlencoreEmployees",
                schema: "dbo",
                columns: table => new
                {
                    IdGlencoreEmployee = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DepartementCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Departement = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Mail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReportsToEmployeeCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReportsToEmployeeDisplay = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SuperIntendentEmployeeCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SuperIntendentEmployeeDisplay = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ManagerHodEmployeeCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ManagerHodEmployeeDisplay = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    GmEmployeeCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GmEmployeeDisplay = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DateImport = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_GlencoreEmployees", x => x.IdGlencoreEmployee);
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_GlencoreEmployees_DepartementCode",
                schema: "dbo",
                table: "T_GlencoreEmployees",
                column: "DepartementCode");

            migrationBuilder.CreateIndex(
                name: "IX_T_GlencoreEmployees_EmployeeCode",
                schema: "dbo",
                table: "T_GlencoreEmployees",
                column: "EmployeeCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_T_GlencoreEmployees_UserName",
                schema: "dbo",
                table: "T_GlencoreEmployees",
                column: "UserName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_GlencoreEmployees",
                schema: "dbo");
        }
    }
}
