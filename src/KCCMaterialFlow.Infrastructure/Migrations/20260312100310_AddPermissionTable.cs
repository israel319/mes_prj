using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPermissionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_ScanEvenements_T_Anomalies_AnomalieIdAnomalie",
                schema: "dbo",
                table: "T_ScanEvenements");

            migrationBuilder.DropIndex(
                name: "IX_T_ScanEvenements_AnomalieIdAnomalie",
                schema: "dbo",
                table: "T_ScanEvenements");

            migrationBuilder.DropColumn(
                name: "AnomalieIdAnomalie",
                schema: "dbo",
                table: "T_ScanEvenements");

            migrationBuilder.DropColumn(
                name: "Permissions",
                schema: "dbo",
                table: "T_Roles");

            migrationBuilder.DropColumn(
                name: "BonSortieInterne_BonEntreeAssocieId",
                schema: "dbo",
                table: "T_BonsSortie");

            migrationBuilder.RenameColumn(
                name: "IdSoldeMateriel",
                schema: "dbo",
                table: "T_SoldesMateriels",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "IdSite",
                schema: "dbo",
                table: "T_Sites",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "IdRaisonSortie",
                schema: "dbo",
                table: "T_RaisonsSortie",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "IdNotificationRejet",
                schema: "dbo",
                table: "T_NotificationsRejet",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "IdEmployee",
                schema: "dbo",
                table: "T_Employees",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "IdContrat",
                schema: "dbo",
                table: "T_Contrats",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "IdCompagnie",
                schema: "dbo",
                table: "T_Compagnies",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "IdCategorieSortie",
                schema: "dbo",
                table: "T_CategoriesSortie",
                newName: "Id");

            migrationBuilder.AddColumn<int>(
                name: "IdRole",
                schema: "dbo",
                table: "T_Utilisateurs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "T_Permissions",
                schema: "dbo",
                columns: table => new
                {
                    IdPermission = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodePermission = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NomPermission = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Categorie = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EstActif = table.Column<bool>(type: "bit", nullable: false),
                    EstSysteme = table.Column<bool>(type: "bit", nullable: false),
                    OrdreAffichage = table.Column<int>(type: "int", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Permissions", x => x.IdPermission);
                });

            migrationBuilder.CreateTable(
                name: "T_RolePermissions",
                schema: "dbo",
                columns: table => new
                {
                    IdRolePermission = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdRole = table.Column<int>(type: "int", nullable: false),
                    IdPermission = table.Column<int>(type: "int", nullable: false),
                    DateAttribution = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_RolePermissions", x => x.IdRolePermission);
                    table.ForeignKey(
                        name: "FK_T_RolePermissions_T_Permissions_IdPermission",
                        column: x => x.IdPermission,
                        principalSchema: "dbo",
                        principalTable: "T_Permissions",
                        principalColumn: "IdPermission",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T_RolePermissions_T_Roles_IdRole",
                        column: x => x.IdRole,
                        principalSchema: "dbo",
                        principalTable: "T_Roles",
                        principalColumn: "IdRole",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "T_Permissions",
                columns: new[] { "IdPermission", "Categorie", "CodePermission", "DateCreation", "Description", "EstActif", "EstSysteme", "NomPermission", "OrdreAffichage" },
                values: new object[,]
                {
                    { 1, "Système", "ALL", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Accès complet à toutes les fonctionnalités", true, true, "Accès complet", 0 },
                    { 2, "Bons", "CREATE_BON", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Créer des bons d'entrée/sortie de matériel", true, true, "Créer des bons", 10 },
                    { 3, "Bons", "VIEW_BON", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Consulter tous les bons du système", true, true, "Voir tous les bons", 20 },
                    { 4, "Bons", "VIEW_OWN_BON", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Consulter uniquement ses propres bons", true, true, "Voir ses propres bons", 25 },
                    { 5, "Bons", "APPROVE_BON", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Approuver les bons de son département", true, true, "Approuver les bons", 30 },
                    { 6, "Bons", "REJECT_BON", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Rejeter les bons de son département", true, true, "Rejeter les bons", 35 },
                    { 7, "Sécurité", "SCAN_BON", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Scanner les QR codes des bons aux barrières", true, true, "Scanner les QR codes", 40 },
                    { 8, "Sécurité", "CREATE_ANOMALIE", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Créer des signalements d'anomalies", true, true, "Signaler des anomalies", 45 },
                    { 9, "Rapports", "VIEW_REPORTS", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Consulter les rapports et statistiques", true, true, "Accéder aux rapports", 50 },
                    { 10, "Administration", "MANAGE_USERS", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ajouter, modifier et désactiver les utilisateurs", true, true, "Gérer les utilisateurs", 60 },
                    { 11, "Administration", "MANAGE_SETTINGS", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Modifier les paramètres système", true, true, "Gérer les paramètres", 70 }
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "T_RolePermissions",
                columns: new[] { "IdRolePermission", "DateAttribution", "IdPermission", "IdRole" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1 },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 2 },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 2 },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 2 },
                    { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 3 },
                    { 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 3 },
                    { 7, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 3 },
                    { 8, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 4 },
                    { 9, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Utilisateurs_IdRole",
                schema: "dbo",
                table: "T_Utilisateurs",
                column: "IdRole");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Categorie",
                schema: "dbo",
                table: "T_Permissions",
                column: "Categorie");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_CodePermission",
                schema: "dbo",
                table: "T_Permissions",
                column: "CodePermission",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_EstActif",
                schema: "dbo",
                table: "T_Permissions",
                column: "EstActif");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_Role_Permission",
                schema: "dbo",
                table: "T_RolePermissions",
                columns: new[] { "IdRole", "IdPermission" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_T_RolePermissions_IdPermission",
                schema: "dbo",
                table: "T_RolePermissions",
                column: "IdPermission");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_Utilisateurs_T_Roles_IdRole",
                schema: "dbo",
                table: "T_Utilisateurs");

            migrationBuilder.DropTable(
                name: "T_RolePermissions",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "T_Permissions",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_Utilisateurs_IdRole",
                schema: "dbo",
                table: "T_Utilisateurs");

            migrationBuilder.DropColumn(
                name: "IdRole",
                schema: "dbo",
                table: "T_Utilisateurs");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "dbo",
                table: "T_SoldesMateriels",
                newName: "IdSoldeMateriel");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "dbo",
                table: "T_Sites",
                newName: "IdSite");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "dbo",
                table: "T_RaisonsSortie",
                newName: "IdRaisonSortie");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "dbo",
                table: "T_NotificationsRejet",
                newName: "IdNotificationRejet");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "dbo",
                table: "T_Employees",
                newName: "IdEmployee");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "dbo",
                table: "T_Contrats",
                newName: "IdContrat");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "dbo",
                table: "T_Compagnies",
                newName: "IdCompagnie");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "dbo",
                table: "T_CategoriesSortie",
                newName: "IdCategorieSortie");

            migrationBuilder.AddColumn<int>(
                name: "AnomalieIdAnomalie",
                schema: "dbo",
                table: "T_ScanEvenements",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Permissions",
                schema: "dbo",
                table: "T_Roles",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BonSortieInterne_BonEntreeAssocieId",
                schema: "dbo",
                table: "T_BonsSortie",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "T_Roles",
                keyColumn: "IdRole",
                keyValue: 1,
                column: "Permissions",
                value: "[\"ALL\"]");

            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "T_Roles",
                keyColumn: "IdRole",
                keyValue: 2,
                column: "Permissions",
                value: "[\"VIEW_BON\",\"APPROVE_BON\",\"REJECT_BON\"]");

            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "T_Roles",
                keyColumn: "IdRole",
                keyValue: 3,
                column: "Permissions",
                value: "[\"VIEW_BON\",\"SCAN_BON\",\"CREATE_ANOMALIE\"]");

            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "T_Roles",
                keyColumn: "IdRole",
                keyValue: 4,
                column: "Permissions",
                value: "[\"CREATE_BON\",\"VIEW_OWN_BON\"]");

            migrationBuilder.CreateIndex(
                name: "IX_T_ScanEvenements_AnomalieIdAnomalie",
                schema: "dbo",
                table: "T_ScanEvenements",
                column: "AnomalieIdAnomalie");

            migrationBuilder.AddForeignKey(
                name: "FK_T_ScanEvenements_T_Anomalies_AnomalieIdAnomalie",
                schema: "dbo",
                table: "T_ScanEvenements",
                column: "AnomalieIdAnomalie",
                principalSchema: "dbo",
                principalTable: "T_Anomalies",
                principalColumn: "IdAnomalie");
        }
    }
}
