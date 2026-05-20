using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefonteEmployeesAndWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AlterColumn<string>(
                name: "Fonction",
                schema: "dbo",
                table: "T_Employees",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartementNom",
                schema: "dbo",
                table: "T_Employees",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReportToEmployeeId",
                schema: "dbo",
                table: "T_Employees",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sources",
                schema: "dbo",
                table: "T_Employees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "T_WorkflowApprobateurSpecial",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    Ordre = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    EstActif = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_WorkflowApprobateurSpecial", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T_WorkflowApprobateurSpecial_T_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "dbo",
                        principalTable: "T_Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_Employees_Matricule",
                schema: "dbo",
                table: "T_Employees",
                column: "Matricule",
                unique: true,
                filter: "[Matricule] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_T_Employees_NumeroEmploye",
                schema: "dbo",
                table: "T_Employees",
                column: "NumeroEmploye",
                unique: true,
                filter: "[NumeroEmploye] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_T_Employees_ReportToEmployeeId",
                schema: "dbo",
                table: "T_Employees",
                column: "ReportToEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_T_WorkflowApprobateurSpecial_EmployeeId",
                schema: "dbo",
                table: "T_WorkflowApprobateurSpecial",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_T_WorkflowApprobateurSpecial_Type",
                schema: "dbo",
                table: "T_WorkflowApprobateurSpecial",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_T_WorkflowApprobateurSpecial_Type_EmployeeId",
                schema: "dbo",
                table: "T_WorkflowApprobateurSpecial",
                columns: new[] { "Type", "EmployeeId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_T_Employees_T_Employees_ReportToEmployeeId",
                schema: "dbo",
                table: "T_Employees",
                column: "ReportToEmployeeId",
                principalSchema: "dbo",
                principalTable: "T_Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_Employees_T_Employees_ReportToEmployeeId",
                schema: "dbo",
                table: "T_Employees");

            migrationBuilder.DropTable(
                name: "T_WorkflowApprobateurSpecial",
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
                name: "IX_T_Employees_ReportToEmployeeId",
                schema: "dbo",
                table: "T_Employees");

            migrationBuilder.DropColumn(
                name: "DepartementNom",
                schema: "dbo",
                table: "T_Employees");

            migrationBuilder.DropColumn(
                name: "ReportToEmployeeId",
                schema: "dbo",
                table: "T_Employees");

            migrationBuilder.DropColumn(
                name: "Sources",
                schema: "dbo",
                table: "T_Employees");

            migrationBuilder.AlterColumn<string>(
                name: "Fonction",
                schema: "dbo",
                table: "T_Employees",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Staging_Employee",
                schema: "stg",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Actif = table.Column<bool>(type: "bit", nullable: true),
                    Departement = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EmployeeEntity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EmployeeId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EmployeeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ErreurMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EstMerge = table.Column<bool>(type: "bit", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ImportBatchId = table.Column<DateTime>(type: "datetime2", nullable: false),
                    JobTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Login = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Mail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReportToEmployeeId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Sources = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TelephoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
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
                    ApproverEmployeeId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    EstActif = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    NiveauApprobation = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
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
    }
}
