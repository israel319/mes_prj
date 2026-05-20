using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EmployeeAsUser_AddNiveauAdminAndDisplayName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                schema: "dbo",
                table: "T_Employees",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "NiveauAdmin",
                schema: "dbo",
                table: "T_Employees",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_T_Employees_NiveauAdmin",
                schema: "dbo",
                table: "T_Employees",
                column: "NiveauAdmin");

            // ── Initialiser DisplayName depuis NomComplet pour les rows existantes ──
            migrationBuilder.Sql(@"
                UPDATE [dbo].[T_Employees]
                SET [DisplayName] = ISNULL([NomComplet], '')
                WHERE [DisplayName] = '' OR [DisplayName] IS NULL;
            ");

            // ── WIPE des tables transactionnelles ──
            // Ordre topologique : enfants → parents pour respecter les FK.
            migrationBuilder.Sql(@"
                IF OBJECT_ID('dbo.T_NotificationsRejet','U')   IS NOT NULL DELETE FROM [dbo].[T_NotificationsRejet];
                IF OBJECT_ID('dbo.T_PassagesCheckpoint','U')   IS NOT NULL DELETE FROM [dbo].[T_PassagesCheckpoint];
                IF OBJECT_ID('dbo.T_HistoriqueScans','U')      IS NOT NULL DELETE FROM [dbo].[T_HistoriqueScans];
                IF OBJECT_ID('dbo.T_ScanEvenements','U')       IS NOT NULL DELETE FROM [dbo].[T_ScanEvenements];
                IF OBJECT_ID('dbo.T_Anomalies','U')            IS NOT NULL DELETE FROM [dbo].[T_Anomalies];
                IF OBJECT_ID('dbo.T_MaterielsSortie','U')      IS NOT NULL DELETE FROM [dbo].[T_MaterielsSortie];
                IF OBJECT_ID('dbo.T_ApprobationsSortie','U')   IS NOT NULL DELETE FROM [dbo].[T_ApprobationsSortie];
                IF OBJECT_ID('dbo.T_Approbations','U')         IS NOT NULL DELETE FROM [dbo].[T_Approbations];
                IF OBJECT_ID('dbo.T_BonSortieHistories','U')   IS NOT NULL DELETE FROM [dbo].[T_BonSortieHistories];
                IF OBJECT_ID('dbo.T_BonEntreeHistory','U')     IS NOT NULL DELETE FROM [dbo].[T_BonEntreeHistory];
                IF OBJECT_ID('dbo.T_Prets','U')                IS NOT NULL DELETE FROM [dbo].[T_Prets];
                IF OBJECT_ID('dbo.T_BonsSortie','U')           IS NOT NULL DELETE FROM [dbo].[T_BonsSortie];
                IF OBJECT_ID('dbo.T_SoldesMateriels','U')      IS NOT NULL DELETE FROM [dbo].[T_SoldesMateriels];
                IF OBJECT_ID('dbo.T_Materiels','U')            IS NOT NULL DELETE FROM [dbo].[T_Materiels];
                IF OBJECT_ID('dbo.T_Bons','U')                 IS NOT NULL DELETE FROM [dbo].[T_Bons];
                IF OBJECT_ID('dbo.T_AuditLogs','U')            IS NOT NULL DELETE FROM [dbo].[T_AuditLogs];
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_T_Employees_NiveauAdmin",
                schema: "dbo",
                table: "T_Employees");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                schema: "dbo",
                table: "T_Employees");

            migrationBuilder.DropColumn(
                name: "NiveauAdmin",
                schema: "dbo",
                table: "T_Employees");
        }
    }
}
