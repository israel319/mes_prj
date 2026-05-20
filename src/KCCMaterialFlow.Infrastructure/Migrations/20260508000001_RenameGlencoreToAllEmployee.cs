using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameGlencoreToAllEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Renommer la PK constraint
            migrationBuilder.Sql(
                "EXEC sp_rename N'dbo.T_GlencoreEmployees.PK_T_GlencoreEmployees', N'PK_T_AllEmployees', N'OBJECT';",
                suppressTransaction: false);

            // 2. Renommer les index
            migrationBuilder.Sql(
                "EXEC sp_rename N'dbo.T_GlencoreEmployees.IX_T_GlencoreEmployees_EmployeeCode', N'IX_T_AllEmployees_EmployeeCode', N'INDEX';",
                suppressTransaction: false);

            migrationBuilder.Sql(
                "EXEC sp_rename N'dbo.T_GlencoreEmployees.IX_T_GlencoreEmployees_UserName', N'IX_T_AllEmployees_UserName', N'INDEX';",
                suppressTransaction: false);

            migrationBuilder.Sql(
                "EXEC sp_rename N'dbo.T_GlencoreEmployees.IX_T_GlencoreEmployees_DepartementCode', N'IX_T_AllEmployees_DepartementCode', N'INDEX';",
                suppressTransaction: false);

            // 3. Renommer la colonne PK
            migrationBuilder.RenameColumn(
                name: "IdGlencoreEmployee",
                schema: "dbo",
                table: "T_GlencoreEmployees",
                newName: "IdAllEmployee");

            // 4. Renommer la table
            migrationBuilder.RenameTable(
                name: "T_GlencoreEmployees",
                schema: "dbo",
                newName: "T_AllEmployees",
                newSchema: "dbo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 4. Rétablir le nom de la table
            migrationBuilder.RenameTable(
                name: "T_AllEmployees",
                schema: "dbo",
                newName: "T_GlencoreEmployees",
                newSchema: "dbo");

            // 3. Rétablir le nom de la colonne PK
            migrationBuilder.RenameColumn(
                name: "IdAllEmployee",
                schema: "dbo",
                table: "T_GlencoreEmployees",
                newName: "IdGlencoreEmployee");

            // 2. Rétablir les index
            migrationBuilder.Sql(
                "EXEC sp_rename N'dbo.T_GlencoreEmployees.IX_T_AllEmployees_DepartementCode', N'IX_T_GlencoreEmployees_DepartementCode', N'INDEX';",
                suppressTransaction: false);

            migrationBuilder.Sql(
                "EXEC sp_rename N'dbo.T_GlencoreEmployees.IX_T_AllEmployees_UserName', N'IX_T_GlencoreEmployees_UserName', N'INDEX';",
                suppressTransaction: false);

            migrationBuilder.Sql(
                "EXEC sp_rename N'dbo.T_GlencoreEmployees.IX_T_AllEmployees_EmployeeCode', N'IX_T_GlencoreEmployees_EmployeeCode', N'INDEX';",
                suppressTransaction: false);

            // 1. Rétablir la PK constraint
            migrationBuilder.Sql(
                "EXEC sp_rename N'dbo.T_GlencoreEmployees.PK_T_AllEmployees', N'PK_T_GlencoreEmployees', N'OBJECT';",
                suppressTransaction: false);
        }
    }
}
