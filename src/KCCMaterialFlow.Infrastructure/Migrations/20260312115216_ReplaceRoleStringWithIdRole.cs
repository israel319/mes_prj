using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceRoleStringWithIdRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ── Étape 1 : Insérer dans T_Roles les rôles présents dans T_Utilisateurs.Role 
            //              qui n'existent pas encore (match case-insensitive sur CodeRole) ──
            migrationBuilder.Sql(@"
                INSERT INTO dbo.T_Roles (CodeRole, NomRole, Description, NiveauPriorite, EstActif, EstSysteme, DateCreation)
                SELECT DISTINCT u.Role, u.Role, 'Rôle migré depuis T_Utilisateurs.Role', 10, 1, 0, GETDATE()
                FROM dbo.T_Utilisateurs u
                WHERE u.Role IS NOT NULL
                  AND NOT EXISTS (
                      SELECT 1 FROM dbo.T_Roles r 
                      WHERE UPPER(r.CodeRole) = UPPER(u.Role)
                         OR UPPER(r.NomRole) = UPPER(u.Role)
                  );
            ");

            // ── Étape 2 : Peupler IdRole à partir de la correspondance Role → T_Roles ──
            migrationBuilder.Sql(@"
                UPDATE u
                SET u.IdRole = r.IdRole
                FROM dbo.T_Utilisateurs u
                INNER JOIN dbo.T_Roles r 
                    ON UPPER(r.CodeRole) = UPPER(u.Role)
                    OR UPPER(r.NomRole) = UPPER(u.Role)
                WHERE u.IdRole IS NULL AND u.Role IS NOT NULL;
            ");

            // ── Étape 3 : Pour les éventuels orphelins restants, assigner le rôle par défaut (Demandeur/UTILISATEUR) ──
            migrationBuilder.Sql(@"
                UPDATE dbo.T_Utilisateurs
                SET IdRole = (SELECT TOP 1 IdRole FROM dbo.T_Roles WHERE CodeRole IN ('DEMANDEUR','UTILISATEUR','Demandeur') ORDER BY IdRole)
                WHERE IdRole IS NULL;
            ");

            // ── Étape 4 : Changements de schéma ──
            migrationBuilder.DropForeignKey(
                name: "FK_T_Utilisateurs_T_Roles_IdRole",
                schema: "dbo",
                table: "T_Utilisateurs");

            migrationBuilder.DropIndex(
                name: "IX_Utilisateurs_Role",
                schema: "dbo",
                table: "T_Utilisateurs");

            migrationBuilder.DropColumn(
                name: "Role",
                schema: "dbo",
                table: "T_Utilisateurs");

            migrationBuilder.AlterColumn<int>(
                name: "IdRole",
                schema: "dbo",
                table: "T_Utilisateurs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_T_Utilisateurs_T_Roles_IdRole",
                schema: "dbo",
                table: "T_Utilisateurs",
                column: "IdRole",
                principalSchema: "dbo",
                principalTable: "T_Roles",
                principalColumn: "IdRole",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_Utilisateurs_T_Roles_IdRole",
                schema: "dbo",
                table: "T_Utilisateurs");

            migrationBuilder.AlterColumn<int>(
                name: "IdRole",
                schema: "dbo",
                table: "T_Utilisateurs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                schema: "dbo",
                table: "T_Utilisateurs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Utilisateur");

            migrationBuilder.CreateIndex(
                name: "IX_Utilisateurs_Role",
                schema: "dbo",
                table: "T_Utilisateurs",
                column: "Role");

            migrationBuilder.AddForeignKey(
                name: "FK_T_Utilisateurs_T_Roles_IdRole",
                schema: "dbo",
                table: "T_Utilisateurs",
                column: "IdRole",
                principalSchema: "dbo",
                principalTable: "T_Roles",
                principalColumn: "IdRole",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
