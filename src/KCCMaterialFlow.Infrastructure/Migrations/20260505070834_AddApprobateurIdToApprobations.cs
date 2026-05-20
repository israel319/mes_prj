using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddApprobateurIdToApprobations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApprobateurId",
                schema: "dbo",
                table: "T_ApprobationsSortie",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprobateurMatricule",
                schema: "dbo",
                table: "T_ApprobationsSortie",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodeEtape",
                schema: "dbo",
                table: "T_ApprobationsSortie",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApprobateurId",
                schema: "dbo",
                table: "T_Approbations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprobateurLogin",
                schema: "dbo",
                table: "T_Approbations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprobateurMatricule",
                schema: "dbo",
                table: "T_Approbations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodeEtape",
                schema: "dbo",
                table: "T_Approbations",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApprobationsSortie_Approbateur_Decision",
                schema: "dbo",
                table: "T_ApprobationsSortie",
                columns: new[] { "ApprobateurId", "Decision" });

            migrationBuilder.CreateIndex(
                name: "IX_Approbations_Approbateur_Decision",
                schema: "dbo",
                table: "T_Approbations",
                columns: new[] { "ApprobateurId", "Decision" });

            // ---------------------------------------------------------------------
            // Backfill ApprobateurId / CodeEtape pour bons existants (BEM + BSM).
            // Mappage NomEtape -> EmployeeId via la chaîne Glencore + WorkflowApprobateurSpecial.
            // ---------------------------------------------------------------------
            migrationBuilder.Sql(@"
-- ===== BEM : T_Approbations =====
;WITH ctx AS (
    SELECT a.IdApprobation,
           a.NomEtape,
           a.OrdreEtape,
           b.SiteId,
           b.CreatedBy AS CreatorLogin
    FROM dbo.T_Approbations a
    INNER JOIN dbo.T_Bons b ON b.IdBon = a.BonId
    WHERE a.ApprobateurId IS NULL
), creator AS (
    SELECT c.*, e.Id AS CreatorEmployeeId, ge.EmployeeCode AS CreatorEmployeeCode,
           ge.ReportsToEmployeeCode, ge.GmEmployeeCode
    FROM ctx c
    LEFT JOIN dbo.T_Employees e ON LOWER(e.Login) = LOWER(c.CreatorLogin)
    LEFT JOIN dbo.T_GlencoreEmployees ge ON LOWER(ge.UserName) = LOWER(c.CreatorLogin)
)
UPDATE a
   SET a.CodeEtape = CASE LOWER(LTRIM(RTRIM(a.NomEtape)))
                        WHEN 'superviseur'              THEN 'REPORTSTO'
                        WHEN 'general manager'          THEN 'GM'
                        WHEN 'opj'                      THEN 'OPJ'
                        WHEN 'identification'           THEN 'IDENTIFICATION'
                        WHEN 'département it'           THEN 'ITDEPARTMENT'
                        WHEN 'departement it'           THEN 'ITDEPARTMENT'
                        WHEN 'département environnement' THEN 'ENVIRONMENTDEPARTMENT'
                        WHEN 'departement environnement' THEN 'ENVIRONMENTDEPARTMENT'
                        ELSE NULL END,
       a.ApprobateurId = COALESCE(
           CASE WHEN LOWER(c.NomEtape) = 'superviseur' THEN
               (SELECT TOP 1 e2.Id FROM dbo.T_GlencoreEmployees ge2
                  INNER JOIN dbo.T_Employees e2 ON LOWER(e2.Login) = LOWER(ge2.UserName)
                 WHERE ge2.EmployeeCode = c.ReportsToEmployeeCode) END,
           CASE WHEN LOWER(c.NomEtape) = 'general manager' THEN
               (SELECT TOP 1 e2.Id FROM dbo.T_GlencoreEmployees ge2
                  INNER JOIN dbo.T_Employees e2 ON LOWER(e2.Login) = LOWER(ge2.UserName)
                 WHERE ge2.EmployeeCode = c.GmEmployeeCode) END,
           CASE WHEN LOWER(c.NomEtape) = 'opj' THEN
               (SELECT TOP 1 w.EmployeeId FROM dbo.T_WorkflowApprobateurSpecial w
                 WHERE w.Type = 2 AND w.EstActif = 1
                   AND (w.SiteId = c.SiteId OR w.SiteId IS NULL)
                 ORDER BY CASE WHEN w.SiteId = c.SiteId THEN 0 ELSE 1 END, w.Ordre) END,
           CASE WHEN LOWER(c.NomEtape) = 'identification' THEN
               (SELECT TOP 1 w.EmployeeId FROM dbo.T_WorkflowApprobateurSpecial w
                 WHERE w.Type = 3 AND w.EstActif = 1 ORDER BY w.Ordre) END,
           CASE WHEN LOWER(c.NomEtape) IN ('département it','departement it') THEN
               (SELECT TOP 1 w.EmployeeId FROM dbo.T_WorkflowApprobateurSpecial w
                 WHERE w.Type = 5 AND w.EstActif = 1 ORDER BY w.Ordre) END,
           CASE WHEN LOWER(c.NomEtape) IN ('département environnement','departement environnement') THEN
               (SELECT TOP 1 w.EmployeeId FROM dbo.T_WorkflowApprobateurSpecial w
                 WHERE w.Type = 6 AND w.EstActif = 1 ORDER BY w.Ordre) END
       )
  FROM dbo.T_Approbations a
 INNER JOIN creator c ON c.IdApprobation = a.IdApprobation
 WHERE a.ApprobateurId IS NULL;

-- ===== BSM : T_ApprobationsSortie =====
;WITH ctx AS (
    SELECT a.IdApprobation,
           a.NomEtape,
           a.OrdreEtape,
           b.SiteId,
           b.CreatedBy AS CreatorLogin
    FROM dbo.T_ApprobationsSortie a
    INNER JOIN dbo.T_BonsSortie b ON b.IdBon = a.BonSortieId
    WHERE a.ApprobateurId IS NULL
), creator AS (
    SELECT c.*, e.Id AS CreatorEmployeeId, ge.EmployeeCode AS CreatorEmployeeCode,
           ge.ReportsToEmployeeCode, ge.GmEmployeeCode
    FROM ctx c
    LEFT JOIN dbo.T_Employees e ON LOWER(e.Login) = LOWER(c.CreatorLogin)
    LEFT JOIN dbo.T_GlencoreEmployees ge ON LOWER(ge.UserName) = LOWER(c.CreatorLogin)
)
UPDATE a
   SET a.CodeEtape = CASE LOWER(LTRIM(RTRIM(a.NomEtape)))
                        WHEN 'superviseur'              THEN 'REPORTSTO'
                        WHEN 'general manager'          THEN 'GM'
                        WHEN 'opj'                      THEN 'OPJ'
                        WHEN 'identification'           THEN 'IDENTIFICATION'
                        WHEN 'département it'           THEN 'ITDEPARTMENT'
                        WHEN 'departement it'           THEN 'ITDEPARTMENT'
                        WHEN 'département environnement' THEN 'ENVIRONMENTDEPARTMENT'
                        WHEN 'departement environnement' THEN 'ENVIRONMENTDEPARTMENT'
                        ELSE NULL END,
       a.ApprobateurId = COALESCE(
           CASE WHEN LOWER(c.NomEtape) = 'superviseur' THEN
               (SELECT TOP 1 e2.Id FROM dbo.T_GlencoreEmployees ge2
                  INNER JOIN dbo.T_Employees e2 ON LOWER(e2.Login) = LOWER(ge2.UserName)
                 WHERE ge2.EmployeeCode = c.ReportsToEmployeeCode) END,
           CASE WHEN LOWER(c.NomEtape) = 'general manager' THEN
               (SELECT TOP 1 e2.Id FROM dbo.T_GlencoreEmployees ge2
                  INNER JOIN dbo.T_Employees e2 ON LOWER(e2.Login) = LOWER(ge2.UserName)
                 WHERE ge2.EmployeeCode = c.GmEmployeeCode) END,
           CASE WHEN LOWER(c.NomEtape) = 'opj' THEN
               (SELECT TOP 1 w.EmployeeId FROM dbo.T_WorkflowApprobateurSpecial w
                 WHERE w.Type = 2 AND w.EstActif = 1
                   AND (w.SiteId = c.SiteId OR w.SiteId IS NULL)
                 ORDER BY CASE WHEN w.SiteId = c.SiteId THEN 0 ELSE 1 END, w.Ordre) END,
           CASE WHEN LOWER(c.NomEtape) = 'identification' THEN
               (SELECT TOP 1 w.EmployeeId FROM dbo.T_WorkflowApprobateurSpecial w
                 WHERE w.Type = 3 AND w.EstActif = 1 ORDER BY w.Ordre) END,
           CASE WHEN LOWER(c.NomEtape) IN ('département it','departement it') THEN
               (SELECT TOP 1 w.EmployeeId FROM dbo.T_WorkflowApprobateurSpecial w
                 WHERE w.Type = 5 AND w.EstActif = 1 ORDER BY w.Ordre) END,
           CASE WHEN LOWER(c.NomEtape) IN ('département environnement','departement environnement') THEN
               (SELECT TOP 1 w.EmployeeId FROM dbo.T_WorkflowApprobateurSpecial w
                 WHERE w.Type = 6 AND w.EstActif = 1 ORDER BY w.Ordre) END
       )
  FROM dbo.T_ApprobationsSortie a
 INNER JOIN creator c ON c.IdApprobation = a.IdApprobation
 WHERE a.ApprobateurId IS NULL;

-- Synchroniser ApprobateurMatricule + ApprobateurLogin via T_Employees
UPDATE a SET a.ApprobateurMatricule = e.Matricule, a.ApprobateurLogin = e.Login
  FROM dbo.T_Approbations a INNER JOIN dbo.T_Employees e ON e.Id = a.ApprobateurId
 WHERE a.ApprobateurId IS NOT NULL;

UPDATE a SET a.ApprobateurMatricule = e.Matricule, a.ApprobateurLogin = e.Login
  FROM dbo.T_ApprobationsSortie a INNER JOIN dbo.T_Employees e ON e.Id = a.ApprobateurId
 WHERE a.ApprobateurId IS NOT NULL;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApprobationsSortie_Approbateur_Decision",
                schema: "dbo",
                table: "T_ApprobationsSortie");

            migrationBuilder.DropIndex(
                name: "IX_Approbations_Approbateur_Decision",
                schema: "dbo",
                table: "T_Approbations");

            migrationBuilder.DropColumn(
                name: "ApprobateurId",
                schema: "dbo",
                table: "T_ApprobationsSortie");

            migrationBuilder.DropColumn(
                name: "ApprobateurMatricule",
                schema: "dbo",
                table: "T_ApprobationsSortie");

            migrationBuilder.DropColumn(
                name: "CodeEtape",
                schema: "dbo",
                table: "T_ApprobationsSortie");

            migrationBuilder.DropColumn(
                name: "ApprobateurId",
                schema: "dbo",
                table: "T_Approbations");

            migrationBuilder.DropColumn(
                name: "ApprobateurLogin",
                schema: "dbo",
                table: "T_Approbations");

            migrationBuilder.DropColumn(
                name: "ApprobateurMatricule",
                schema: "dbo",
                table: "T_Approbations");

            migrationBuilder.DropColumn(
                name: "CodeEtape",
                schema: "dbo",
                table: "T_Approbations");
        }
    }
}
