using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddImportStagingAndReportTo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                schema: "dbo",
                table: "T_Compagnies");

            migrationBuilder.DropColumn(
                name: "SiteManager",
                schema: "dbo",
                table: "T_Compagnies");

            migrationBuilder.EnsureSchema(
                name: "stg");

            migrationBuilder.AlterColumn<string>(
                name: "Matricule",
                schema: "dbo",
                table: "T_Employees",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumeroEmploye",
                schema: "dbo",
                table: "T_Employees",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "dbo",
                table: "T_Contrats",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SiteManager",
                schema: "dbo",
                table: "T_Contrats",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Staging_Company",
                schema: "stg",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Actif = table.Column<bool>(type: "bit", nullable: true),
                    DateSys = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateSysRaw = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ImportBatchId = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstMerge = table.Column<bool>(type: "bit", nullable: false),
                    ErreurMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staging_Company", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Staging_Contract",
                schema: "stg",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PoNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ContractDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Actif = table.Column<bool>(type: "bit", nullable: true),
                    DateSys = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateSysRaw = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ImportBatchId = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstMerge = table.Column<bool>(type: "bit", nullable: false),
                    ErreurMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staging_Contract", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Staging_Employee",
                schema: "stg",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeEntity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EmployeeId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EmployeeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Mail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    JobTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TelephoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Departement = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Sources = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Actif = table.Column<bool>(type: "bit", nullable: true),
                    Login = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReportToEmployeeId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ImportBatchId = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstMerge = table.Column<bool>(type: "bit", nullable: false),
                    ErreurMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staging_Employee", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "T_ReportTo",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    ApproverEmployeeId = table.Column<int>(type: "int", nullable: false),
                    NiveauApprobation = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    EstActif = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_ReportTo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T_ReportTo_T_Employees_ApproverEmployeeId",
                        column: x => x.ApproverEmployeeId,
                        principalSchema: "dbo",
                        principalTable: "T_Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_T_ReportTo_T_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "dbo",
                        principalTable: "T_Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_Employees_Matricule",
                schema: "dbo",
                table: "T_Employees",
                column: "Matricule");

            migrationBuilder.CreateIndex(
                name: "IX_T_Employees_NumeroEmploye",
                schema: "dbo",
                table: "T_Employees",
                column: "NumeroEmploye");

            migrationBuilder.CreateIndex(
                name: "IX_T_Compagnies_Code",
                schema: "dbo",
                table: "T_Compagnies",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_Staging_Company_CompanyCode",
                schema: "stg",
                table: "Staging_Company",
                column: "CompanyCode");

            migrationBuilder.CreateIndex(
                name: "IX_Staging_Company_ImportBatchId",
                schema: "stg",
                table: "Staging_Company",
                column: "ImportBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Staging_Contract_CompanyCode",
                schema: "stg",
                table: "Staging_Contract",
                column: "CompanyCode");

            migrationBuilder.CreateIndex(
                name: "IX_Staging_Contract_ImportBatchId",
                schema: "stg",
                table: "Staging_Contract",
                column: "ImportBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Staging_Contract_PoNumber",
                schema: "stg",
                table: "Staging_Contract",
                column: "PoNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Staging_Employee_EmployeeEntity",
                schema: "stg",
                table: "Staging_Employee",
                column: "EmployeeEntity");

            migrationBuilder.CreateIndex(
                name: "IX_Staging_Employee_EmployeeId",
                schema: "stg",
                table: "Staging_Employee",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Staging_Employee_ImportBatchId",
                schema: "stg",
                table: "Staging_Employee",
                column: "ImportBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Staging_Employee_ReportToEmployeeId",
                schema: "stg",
                table: "Staging_Employee",
                column: "ReportToEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_T_ReportTo_ApproverEmployeeId",
                schema: "dbo",
                table: "T_ReportTo",
                column: "ApproverEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_T_ReportTo_EmployeeId_NiveauApprobation",
                schema: "dbo",
                table: "T_ReportTo",
                columns: new[] { "EmployeeId", "NiveauApprobation" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Staging_Company",
                schema: "stg");

            migrationBuilder.DropTable(
                name: "Staging_Contract",
                schema: "stg");

            migrationBuilder.DropTable(
                name: "Staging_Employee",
                schema: "stg");

            migrationBuilder.DropTable(
                name: "T_ReportTo",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_T_Employees_Matricule",
                schema: "dbo",
                table: "T_Employees");

            migrationBuilder.DropIndex(
                name: "IX_T_Employees_NumeroEmploye",
                schema: "dbo",
                table: "T_Employees");

            migrationBuilder.DropIndex(
                name: "IX_T_Compagnies_Code",
                schema: "dbo",
                table: "T_Compagnies");

            migrationBuilder.DropColumn(
                name: "NumeroEmploye",
                schema: "dbo",
                table: "T_Employees");

            migrationBuilder.DropColumn(
                name: "Email",
                schema: "dbo",
                table: "T_Contrats");

            migrationBuilder.DropColumn(
                name: "SiteManager",
                schema: "dbo",
                table: "T_Contrats");

            migrationBuilder.AlterColumn<string>(
                name: "Matricule",
                schema: "dbo",
                table: "T_Employees",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "dbo",
                table: "T_Compagnies",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SiteManager",
                schema: "dbo",
                table: "T_Compagnies",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
