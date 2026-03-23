using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameTablesToDboConvention : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Anomalies_Barrieres_BarriereId",
                schema: "sec",
                table: "Anomalies");

            migrationBuilder.DropForeignKey(
                name: "FK_Anomalies_ScanEvenements_ScanId",
                schema: "sec",
                table: "Anomalies");

            migrationBuilder.DropForeignKey(
                name: "FK_Approbations_Bons_BonId",
                schema: "bem",
                table: "Approbations");

            migrationBuilder.DropForeignKey(
                name: "FK_ApprobationsSortie_BonsSortie_BonSortieId",
                schema: "bsm",
                table: "ApprobationsSortie");

            migrationBuilder.DropForeignKey(
                name: "FK_BonEntreeHistory_Bons_BonId",
                schema: "bem",
                table: "BonEntreeHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_BonEntreeHistory_Bons_BonIdBon",
                schema: "bem",
                table: "BonEntreeHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_BonSortieHistories_BonsSortie_BonSortieId",
                schema: "bsm",
                table: "BonSortieHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_BonsSortie_Bons_BonEntreeAssocieId",
                schema: "bsm",
                table: "BonsSortie");

            migrationBuilder.DropForeignKey(
                name: "FK_Checkpoints_Sites_SiteId",
                schema: "ref",
                table: "Checkpoints");

            migrationBuilder.DropForeignKey(
                name: "FK_Contrats_Compagnies_CompagnieId",
                schema: "ref",
                table: "Contrats");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Compagnies_CompagnieId",
                schema: "ref",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Departements_DepartementId",
                schema: "ref",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_HistoriqueScans_ScanEvenements_ScanId",
                schema: "sec",
                table: "HistoriqueScans");

            migrationBuilder.DropForeignKey(
                name: "FK_ItinerairesPrevu_Bons_BonId",
                schema: "bem",
                table: "ItinerairesPrevu");

            migrationBuilder.DropForeignKey(
                name: "FK_ItinerairesSortie_BonsSortie_BonSortieId",
                schema: "bsm",
                table: "ItinerairesSortie");

            migrationBuilder.DropForeignKey(
                name: "FK_Materiels_Bons_BonId",
                schema: "bem",
                table: "Materiels");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterielsSortie_BonsSortie_BonSortieId",
                schema: "bsm",
                table: "MaterielsSortie");

            migrationBuilder.DropForeignKey(
                name: "FK_PassagesCheckpoint_Checkpoints_CheckpointId",
                schema: "dbo",
                table: "PassagesCheckpoint");

            migrationBuilder.DropForeignKey(
                name: "FK_RaisonsSortie_CategoriesSortie_CategorieId",
                schema: "ref",
                table: "RaisonsSortie");

            migrationBuilder.DropForeignKey(
                name: "FK_ScanEvenements_Anomalies_AnomalieIdAnomalie",
                schema: "sec",
                table: "ScanEvenements");

            migrationBuilder.DropForeignKey(
                name: "FK_ScanEvenements_Barrieres_BarriereId",
                schema: "sec",
                table: "ScanEvenements");

            migrationBuilder.DropForeignKey(
                name: "FK_UtilisateurRoles_Roles_IdRole",
                table: "UtilisateurRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UtilisateurRoles_Utilisateurs_IdUtilisateur",
                table: "UtilisateurRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Utilisateurs",
                schema: "shared",
                table: "Utilisateurs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UtilisateurRoles",
                table: "UtilisateurRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TypesMateriels",
                table: "TypesMateriels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Statuts",
                table: "Statuts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SoldesMateriels",
                schema: "bem",
                table: "SoldesMateriels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sites",
                schema: "ref",
                table: "Sites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ScanEvenements",
                schema: "sec",
                table: "ScanEvenements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Roles",
                table: "Roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RaisonsSortie",
                schema: "ref",
                table: "RaisonsSortie");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PassagesCheckpoint",
                schema: "dbo",
                table: "PassagesCheckpoint");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ParametresSysteme",
                table: "ParametresSysteme");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NotificationsRejet",
                schema: "dbo",
                table: "NotificationsRejet");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MaterielsSortie",
                schema: "bsm",
                table: "MaterielsSortie");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Materiels",
                schema: "bem",
                table: "Materiels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItinerairesSortie",
                schema: "bsm",
                table: "ItinerairesSortie");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItinerairesPrevu",
                schema: "bem",
                table: "ItinerairesPrevu");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HistoriqueScans",
                schema: "sec",
                table: "HistoriqueScans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Employees",
                schema: "ref",
                table: "Employees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Departements",
                schema: "shared",
                table: "Departements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contrats",
                schema: "ref",
                table: "Contrats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Compagnies",
                schema: "ref",
                table: "Compagnies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Checkpoints",
                schema: "ref",
                table: "Checkpoints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoriesSortie",
                schema: "ref",
                table: "CategoriesSortie");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BonsSortie",
                schema: "bsm",
                table: "BonsSortie");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BonSortieHistories",
                schema: "bsm",
                table: "BonSortieHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bons",
                schema: "bem",
                table: "Bons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BonEntreeHistory",
                schema: "bem",
                table: "BonEntreeHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Barrieres",
                schema: "shared",
                table: "Barrieres");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuditLogs",
                table: "AuditLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApprobationsSortie",
                schema: "bsm",
                table: "ApprobationsSortie");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Approbations",
                schema: "bem",
                table: "Approbations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Anomalies",
                schema: "sec",
                table: "Anomalies");

            migrationBuilder.RenameTable(
                name: "Utilisateurs",
                schema: "shared",
                newName: "T_Utilisateurs",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "UtilisateurRoles",
                newName: "T_UtilisateurRoles",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "TypesMateriels",
                newName: "T_TypesMateriels",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Statuts",
                newName: "T_Statuts",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "SoldesMateriels",
                schema: "bem",
                newName: "T_SoldesMateriels",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Sites",
                schema: "ref",
                newName: "T_Sites",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "ScanEvenements",
                schema: "sec",
                newName: "T_ScanEvenements",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Roles",
                newName: "T_Roles",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "RaisonsSortie",
                schema: "ref",
                newName: "T_RaisonsSortie",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "PassagesCheckpoint",
                schema: "dbo",
                newName: "T_PassagesCheckpoint",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "ParametresSysteme",
                newName: "T_ParametresSysteme",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "NotificationsRejet",
                schema: "dbo",
                newName: "T_NotificationsRejet",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "MaterielsSortie",
                schema: "bsm",
                newName: "T_MaterielsSortie",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Materiels",
                schema: "bem",
                newName: "T_Materiels",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "ItinerairesSortie",
                schema: "bsm",
                newName: "T_ItinerairesSortie",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "ItinerairesPrevu",
                schema: "bem",
                newName: "T_ItinerairesPrevu",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "HistoriqueScans",
                schema: "sec",
                newName: "T_HistoriqueScans",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Employees",
                schema: "ref",
                newName: "T_Employees",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Departements",
                schema: "shared",
                newName: "T_Departements",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Contrats",
                schema: "ref",
                newName: "T_Contrats",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Compagnies",
                schema: "ref",
                newName: "T_Compagnies",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Checkpoints",
                schema: "ref",
                newName: "T_Checkpoints",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "CategoriesSortie",
                schema: "ref",
                newName: "T_CategoriesSortie",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "BonsSortie",
                schema: "bsm",
                newName: "T_BonsSortie",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "BonSortieHistories",
                schema: "bsm",
                newName: "T_BonSortieHistories",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Bons",
                schema: "bem",
                newName: "T_Bons",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "BonEntreeHistory",
                schema: "bem",
                newName: "T_BonEntreeHistory",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Barrieres",
                schema: "shared",
                newName: "T_Barrieres",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "AuditLogs",
                newName: "T_AuditLogs",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "ApprobationsSortie",
                schema: "bsm",
                newName: "T_ApprobationsSortie",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Approbations",
                schema: "bem",
                newName: "T_Approbations",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Anomalies",
                schema: "sec",
                newName: "T_Anomalies",
                newSchema: "dbo");

            migrationBuilder.RenameIndex(
                name: "IX_UtilisateurRoles_IdRole",
                schema: "dbo",
                table: "T_UtilisateurRoles",
                newName: "IX_T_UtilisateurRoles_IdRole");

            migrationBuilder.RenameIndex(
                name: "IX_ScanEvenements_AnomalieIdAnomalie",
                schema: "dbo",
                table: "T_ScanEvenements",
                newName: "IX_T_ScanEvenements_AnomalieIdAnomalie");

            migrationBuilder.RenameIndex(
                name: "IX_RaisonsSortie_CategorieId",
                schema: "dbo",
                table: "T_RaisonsSortie",
                newName: "IX_T_RaisonsSortie_CategorieId");

            migrationBuilder.RenameIndex(
                name: "IX_PassagesCheckpoint_CheckpointId",
                schema: "dbo",
                table: "T_PassagesCheckpoint",
                newName: "IX_T_PassagesCheckpoint_CheckpointId");

            migrationBuilder.RenameIndex(
                name: "IX_HistoriqueScans_ScanId",
                schema: "dbo",
                table: "T_HistoriqueScans",
                newName: "IX_T_HistoriqueScans_ScanId");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_DepartementId",
                schema: "dbo",
                table: "T_Employees",
                newName: "IX_T_Employees_DepartementId");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_CompagnieId",
                schema: "dbo",
                table: "T_Employees",
                newName: "IX_T_Employees_CompagnieId");

            migrationBuilder.RenameIndex(
                name: "IX_Contrats_PoNumber",
                schema: "dbo",
                table: "T_Contrats",
                newName: "IX_T_Contrats_PoNumber");

            migrationBuilder.RenameIndex(
                name: "IX_Contrats_CompagnieId",
                schema: "dbo",
                table: "T_Contrats",
                newName: "IX_T_Contrats_CompagnieId");

            migrationBuilder.RenameIndex(
                name: "IX_Checkpoints_SiteId",
                schema: "dbo",
                table: "T_Checkpoints",
                newName: "IX_T_Checkpoints_SiteId");

            migrationBuilder.RenameIndex(
                name: "IX_BonEntreeHistory_BonIdBon",
                schema: "dbo",
                table: "T_BonEntreeHistory",
                newName: "IX_T_BonEntreeHistory_BonIdBon");

            migrationBuilder.RenameIndex(
                name: "IX_Anomalies_BarriereId",
                schema: "dbo",
                table: "T_Anomalies",
                newName: "IX_T_Anomalies_BarriereId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_Utilisateurs",
                schema: "dbo",
                table: "T_Utilisateurs",
                column: "IdUtilisateur");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_UtilisateurRoles",
                schema: "dbo",
                table: "T_UtilisateurRoles",
                column: "IdUtilisateurRole");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_TypesMateriels",
                schema: "dbo",
                table: "T_TypesMateriels",
                column: "IdTypeMateriel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_Statuts",
                schema: "dbo",
                table: "T_Statuts",
                column: "IdStatut");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_SoldesMateriels",
                schema: "dbo",
                table: "T_SoldesMateriels",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_Sites",
                schema: "dbo",
                table: "T_Sites",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_ScanEvenements",
                schema: "dbo",
                table: "T_ScanEvenements",
                column: "IdScan");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_Roles",
                schema: "dbo",
                table: "T_Roles",
                column: "IdRole");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_RaisonsSortie",
                schema: "dbo",
                table: "T_RaisonsSortie",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_PassagesCheckpoint",
                schema: "dbo",
                table: "T_PassagesCheckpoint",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_ParametresSysteme",
                schema: "dbo",
                table: "T_ParametresSysteme",
                column: "IdParametre");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_NotificationsRejet",
                schema: "dbo",
                table: "T_NotificationsRejet",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_MaterielsSortie",
                schema: "dbo",
                table: "T_MaterielsSortie",
                column: "IdMateriel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_Materiels",
                schema: "dbo",
                table: "T_Materiels",
                column: "IdMateriel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_ItinerairesSortie",
                schema: "dbo",
                table: "T_ItinerairesSortie",
                column: "IdItineraire");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_ItinerairesPrevu",
                schema: "dbo",
                table: "T_ItinerairesPrevu",
                column: "IdItineraire");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_HistoriqueScans",
                schema: "dbo",
                table: "T_HistoriqueScans",
                column: "IdHistorique");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_Employees",
                schema: "dbo",
                table: "T_Employees",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_Departements",
                schema: "dbo",
                table: "T_Departements",
                column: "IdDepartement");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_Contrats",
                schema: "dbo",
                table: "T_Contrats",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_Compagnies",
                schema: "dbo",
                table: "T_Compagnies",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_Checkpoints",
                schema: "dbo",
                table: "T_Checkpoints",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_CategoriesSortie",
                schema: "dbo",
                table: "T_CategoriesSortie",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_BonsSortie",
                schema: "dbo",
                table: "T_BonsSortie",
                column: "IdBon");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_BonSortieHistories",
                schema: "dbo",
                table: "T_BonSortieHistories",
                column: "IdHistory");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_Bons",
                schema: "dbo",
                table: "T_Bons",
                column: "IdBon");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_BonEntreeHistory",
                schema: "dbo",
                table: "T_BonEntreeHistory",
                column: "IdHistory");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_Barrieres",
                schema: "dbo",
                table: "T_Barrieres",
                column: "IdBarriere");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_AuditLogs",
                schema: "dbo",
                table: "T_AuditLogs",
                column: "IdAuditLog");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_ApprobationsSortie",
                schema: "dbo",
                table: "T_ApprobationsSortie",
                column: "IdApprobation");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_Approbations",
                schema: "dbo",
                table: "T_Approbations",
                column: "IdApprobation");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_Anomalies",
                schema: "dbo",
                table: "T_Anomalies",
                column: "IdAnomalie");

            migrationBuilder.AddForeignKey(
                name: "FK_T_Anomalies_T_Barrieres_BarriereId",
                schema: "dbo",
                table: "T_Anomalies",
                column: "BarriereId",
                principalSchema: "dbo",
                principalTable: "T_Barrieres",
                principalColumn: "IdBarriere",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_T_Anomalies_T_ScanEvenements_ScanId",
                schema: "dbo",
                table: "T_Anomalies",
                column: "ScanId",
                principalSchema: "dbo",
                principalTable: "T_ScanEvenements",
                principalColumn: "IdScan",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_T_Approbations_T_Bons_BonId",
                schema: "dbo",
                table: "T_Approbations",
                column: "BonId",
                principalSchema: "dbo",
                principalTable: "T_Bons",
                principalColumn: "IdBon",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_T_ApprobationsSortie_T_BonsSortie_BonSortieId",
                schema: "dbo",
                table: "T_ApprobationsSortie",
                column: "BonSortieId",
                principalSchema: "dbo",
                principalTable: "T_BonsSortie",
                principalColumn: "IdBon",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_T_BonEntreeHistory_T_Bons_BonId",
                schema: "dbo",
                table: "T_BonEntreeHistory",
                column: "BonId",
                principalSchema: "dbo",
                principalTable: "T_Bons",
                principalColumn: "IdBon",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_T_BonEntreeHistory_T_Bons_BonIdBon",
                schema: "dbo",
                table: "T_BonEntreeHistory",
                column: "BonIdBon",
                principalSchema: "dbo",
                principalTable: "T_Bons",
                principalColumn: "IdBon");

            migrationBuilder.AddForeignKey(
                name: "FK_T_BonSortieHistories_T_BonsSortie_BonSortieId",
                schema: "dbo",
                table: "T_BonSortieHistories",
                column: "BonSortieId",
                principalSchema: "dbo",
                principalTable: "T_BonsSortie",
                principalColumn: "IdBon",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_T_BonsSortie_T_Bons_BonEntreeAssocieId",
                schema: "dbo",
                table: "T_BonsSortie",
                column: "BonEntreeAssocieId",
                principalSchema: "dbo",
                principalTable: "T_Bons",
                principalColumn: "IdBon",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_T_Checkpoints_T_Sites_SiteId",
                schema: "dbo",
                table: "T_Checkpoints",
                column: "SiteId",
                principalSchema: "dbo",
                principalTable: "T_Sites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_T_Contrats_T_Compagnies_CompagnieId",
                schema: "dbo",
                table: "T_Contrats",
                column: "CompagnieId",
                principalSchema: "dbo",
                principalTable: "T_Compagnies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_T_Employees_T_Compagnies_CompagnieId",
                schema: "dbo",
                table: "T_Employees",
                column: "CompagnieId",
                principalSchema: "dbo",
                principalTable: "T_Compagnies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_T_Employees_T_Departements_DepartementId",
                schema: "dbo",
                table: "T_Employees",
                column: "DepartementId",
                principalSchema: "dbo",
                principalTable: "T_Departements",
                principalColumn: "IdDepartement");

            migrationBuilder.AddForeignKey(
                name: "FK_T_HistoriqueScans_T_ScanEvenements_ScanId",
                schema: "dbo",
                table: "T_HistoriqueScans",
                column: "ScanId",
                principalSchema: "dbo",
                principalTable: "T_ScanEvenements",
                principalColumn: "IdScan",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_T_ItinerairesPrevu_T_Bons_BonId",
                schema: "dbo",
                table: "T_ItinerairesPrevu",
                column: "BonId",
                principalSchema: "dbo",
                principalTable: "T_Bons",
                principalColumn: "IdBon",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_T_ItinerairesSortie_T_BonsSortie_BonSortieId",
                schema: "dbo",
                table: "T_ItinerairesSortie",
                column: "BonSortieId",
                principalSchema: "dbo",
                principalTable: "T_BonsSortie",
                principalColumn: "IdBon",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_T_Materiels_T_Bons_BonId",
                schema: "dbo",
                table: "T_Materiels",
                column: "BonId",
                principalSchema: "dbo",
                principalTable: "T_Bons",
                principalColumn: "IdBon",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_T_MaterielsSortie_T_BonsSortie_BonSortieId",
                schema: "dbo",
                table: "T_MaterielsSortie",
                column: "BonSortieId",
                principalSchema: "dbo",
                principalTable: "T_BonsSortie",
                principalColumn: "IdBon",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_T_PassagesCheckpoint_T_Checkpoints_CheckpointId",
                schema: "dbo",
                table: "T_PassagesCheckpoint",
                column: "CheckpointId",
                principalSchema: "dbo",
                principalTable: "T_Checkpoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_T_RaisonsSortie_T_CategoriesSortie_CategorieId",
                schema: "dbo",
                table: "T_RaisonsSortie",
                column: "CategorieId",
                principalSchema: "dbo",
                principalTable: "T_CategoriesSortie",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_T_ScanEvenements_T_Anomalies_AnomalieIdAnomalie",
                schema: "dbo",
                table: "T_ScanEvenements",
                column: "AnomalieIdAnomalie",
                principalSchema: "dbo",
                principalTable: "T_Anomalies",
                principalColumn: "IdAnomalie");

            migrationBuilder.AddForeignKey(
                name: "FK_T_ScanEvenements_T_Barrieres_BarriereId",
                schema: "dbo",
                table: "T_ScanEvenements",
                column: "BarriereId",
                principalSchema: "dbo",
                principalTable: "T_Barrieres",
                principalColumn: "IdBarriere",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_T_UtilisateurRoles_T_Roles_IdRole",
                schema: "dbo",
                table: "T_UtilisateurRoles",
                column: "IdRole",
                principalSchema: "dbo",
                principalTable: "T_Roles",
                principalColumn: "IdRole",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_T_UtilisateurRoles_T_Utilisateurs_IdUtilisateur",
                schema: "dbo",
                table: "T_UtilisateurRoles",
                column: "IdUtilisateur",
                principalSchema: "dbo",
                principalTable: "T_Utilisateurs",
                principalColumn: "IdUtilisateur",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_Anomalies_T_Barrieres_BarriereId",
                schema: "dbo",
                table: "T_Anomalies");

            migrationBuilder.DropForeignKey(
                name: "FK_T_Anomalies_T_ScanEvenements_ScanId",
                schema: "dbo",
                table: "T_Anomalies");

            migrationBuilder.DropForeignKey(
                name: "FK_T_Approbations_T_Bons_BonId",
                schema: "dbo",
                table: "T_Approbations");

            migrationBuilder.DropForeignKey(
                name: "FK_T_ApprobationsSortie_T_BonsSortie_BonSortieId",
                schema: "dbo",
                table: "T_ApprobationsSortie");

            migrationBuilder.DropForeignKey(
                name: "FK_T_BonEntreeHistory_T_Bons_BonId",
                schema: "dbo",
                table: "T_BonEntreeHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_T_BonEntreeHistory_T_Bons_BonIdBon",
                schema: "dbo",
                table: "T_BonEntreeHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_T_BonSortieHistories_T_BonsSortie_BonSortieId",
                schema: "dbo",
                table: "T_BonSortieHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_T_BonsSortie_T_Bons_BonEntreeAssocieId",
                schema: "dbo",
                table: "T_BonsSortie");

            migrationBuilder.DropForeignKey(
                name: "FK_T_Checkpoints_T_Sites_SiteId",
                schema: "dbo",
                table: "T_Checkpoints");

            migrationBuilder.DropForeignKey(
                name: "FK_T_Contrats_T_Compagnies_CompagnieId",
                schema: "dbo",
                table: "T_Contrats");

            migrationBuilder.DropForeignKey(
                name: "FK_T_Employees_T_Compagnies_CompagnieId",
                schema: "dbo",
                table: "T_Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_T_Employees_T_Departements_DepartementId",
                schema: "dbo",
                table: "T_Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_T_HistoriqueScans_T_ScanEvenements_ScanId",
                schema: "dbo",
                table: "T_HistoriqueScans");

            migrationBuilder.DropForeignKey(
                name: "FK_T_ItinerairesPrevu_T_Bons_BonId",
                schema: "dbo",
                table: "T_ItinerairesPrevu");

            migrationBuilder.DropForeignKey(
                name: "FK_T_ItinerairesSortie_T_BonsSortie_BonSortieId",
                schema: "dbo",
                table: "T_ItinerairesSortie");

            migrationBuilder.DropForeignKey(
                name: "FK_T_Materiels_T_Bons_BonId",
                schema: "dbo",
                table: "T_Materiels");

            migrationBuilder.DropForeignKey(
                name: "FK_T_MaterielsSortie_T_BonsSortie_BonSortieId",
                schema: "dbo",
                table: "T_MaterielsSortie");

            migrationBuilder.DropForeignKey(
                name: "FK_T_PassagesCheckpoint_T_Checkpoints_CheckpointId",
                schema: "dbo",
                table: "T_PassagesCheckpoint");

            migrationBuilder.DropForeignKey(
                name: "FK_T_RaisonsSortie_T_CategoriesSortie_CategorieId",
                schema: "dbo",
                table: "T_RaisonsSortie");

            migrationBuilder.DropForeignKey(
                name: "FK_T_ScanEvenements_T_Anomalies_AnomalieIdAnomalie",
                schema: "dbo",
                table: "T_ScanEvenements");

            migrationBuilder.DropForeignKey(
                name: "FK_T_ScanEvenements_T_Barrieres_BarriereId",
                schema: "dbo",
                table: "T_ScanEvenements");

            migrationBuilder.DropForeignKey(
                name: "FK_T_UtilisateurRoles_T_Roles_IdRole",
                schema: "dbo",
                table: "T_UtilisateurRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_T_UtilisateurRoles_T_Utilisateurs_IdUtilisateur",
                schema: "dbo",
                table: "T_UtilisateurRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_Utilisateurs",
                schema: "dbo",
                table: "T_Utilisateurs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_UtilisateurRoles",
                schema: "dbo",
                table: "T_UtilisateurRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_TypesMateriels",
                schema: "dbo",
                table: "T_TypesMateriels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_Statuts",
                schema: "dbo",
                table: "T_Statuts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_SoldesMateriels",
                schema: "dbo",
                table: "T_SoldesMateriels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_Sites",
                schema: "dbo",
                table: "T_Sites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_ScanEvenements",
                schema: "dbo",
                table: "T_ScanEvenements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_Roles",
                schema: "dbo",
                table: "T_Roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_RaisonsSortie",
                schema: "dbo",
                table: "T_RaisonsSortie");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_PassagesCheckpoint",
                schema: "dbo",
                table: "T_PassagesCheckpoint");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_ParametresSysteme",
                schema: "dbo",
                table: "T_ParametresSysteme");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_NotificationsRejet",
                schema: "dbo",
                table: "T_NotificationsRejet");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_MaterielsSortie",
                schema: "dbo",
                table: "T_MaterielsSortie");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_Materiels",
                schema: "dbo",
                table: "T_Materiels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_ItinerairesSortie",
                schema: "dbo",
                table: "T_ItinerairesSortie");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_ItinerairesPrevu",
                schema: "dbo",
                table: "T_ItinerairesPrevu");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_HistoriqueScans",
                schema: "dbo",
                table: "T_HistoriqueScans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_Employees",
                schema: "dbo",
                table: "T_Employees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_Departements",
                schema: "dbo",
                table: "T_Departements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_Contrats",
                schema: "dbo",
                table: "T_Contrats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_Compagnies",
                schema: "dbo",
                table: "T_Compagnies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_Checkpoints",
                schema: "dbo",
                table: "T_Checkpoints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_CategoriesSortie",
                schema: "dbo",
                table: "T_CategoriesSortie");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_BonsSortie",
                schema: "dbo",
                table: "T_BonsSortie");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_BonSortieHistories",
                schema: "dbo",
                table: "T_BonSortieHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_Bons",
                schema: "dbo",
                table: "T_Bons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_BonEntreeHistory",
                schema: "dbo",
                table: "T_BonEntreeHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_Barrieres",
                schema: "dbo",
                table: "T_Barrieres");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_AuditLogs",
                schema: "dbo",
                table: "T_AuditLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_ApprobationsSortie",
                schema: "dbo",
                table: "T_ApprobationsSortie");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_Approbations",
                schema: "dbo",
                table: "T_Approbations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_Anomalies",
                schema: "dbo",
                table: "T_Anomalies");

            migrationBuilder.EnsureSchema(
                name: "sec");

            migrationBuilder.EnsureSchema(
                name: "bem");

            migrationBuilder.EnsureSchema(
                name: "bsm");

            migrationBuilder.EnsureSchema(
                name: "shared");

            migrationBuilder.EnsureSchema(
                name: "ref");

            migrationBuilder.RenameTable(
                name: "T_Utilisateurs",
                schema: "dbo",
                newName: "Utilisateurs",
                newSchema: "shared");

            migrationBuilder.RenameTable(
                name: "T_UtilisateurRoles",
                schema: "dbo",
                newName: "UtilisateurRoles");

            migrationBuilder.RenameTable(
                name: "T_TypesMateriels",
                schema: "dbo",
                newName: "TypesMateriels");

            migrationBuilder.RenameTable(
                name: "T_Statuts",
                schema: "dbo",
                newName: "Statuts");

            migrationBuilder.RenameTable(
                name: "T_SoldesMateriels",
                schema: "dbo",
                newName: "SoldesMateriels",
                newSchema: "bem");

            migrationBuilder.RenameTable(
                name: "T_Sites",
                schema: "dbo",
                newName: "Sites",
                newSchema: "ref");

            migrationBuilder.RenameTable(
                name: "T_ScanEvenements",
                schema: "dbo",
                newName: "ScanEvenements",
                newSchema: "sec");

            migrationBuilder.RenameTable(
                name: "T_Roles",
                schema: "dbo",
                newName: "Roles");

            migrationBuilder.RenameTable(
                name: "T_RaisonsSortie",
                schema: "dbo",
                newName: "RaisonsSortie",
                newSchema: "ref");

            migrationBuilder.RenameTable(
                name: "T_PassagesCheckpoint",
                schema: "dbo",
                newName: "PassagesCheckpoint",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "T_ParametresSysteme",
                schema: "dbo",
                newName: "ParametresSysteme");

            migrationBuilder.RenameTable(
                name: "T_NotificationsRejet",
                schema: "dbo",
                newName: "NotificationsRejet",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "T_MaterielsSortie",
                schema: "dbo",
                newName: "MaterielsSortie",
                newSchema: "bsm");

            migrationBuilder.RenameTable(
                name: "T_Materiels",
                schema: "dbo",
                newName: "Materiels",
                newSchema: "bem");

            migrationBuilder.RenameTable(
                name: "T_ItinerairesSortie",
                schema: "dbo",
                newName: "ItinerairesSortie",
                newSchema: "bsm");

            migrationBuilder.RenameTable(
                name: "T_ItinerairesPrevu",
                schema: "dbo",
                newName: "ItinerairesPrevu",
                newSchema: "bem");

            migrationBuilder.RenameTable(
                name: "T_HistoriqueScans",
                schema: "dbo",
                newName: "HistoriqueScans",
                newSchema: "sec");

            migrationBuilder.RenameTable(
                name: "T_Employees",
                schema: "dbo",
                newName: "Employees",
                newSchema: "ref");

            migrationBuilder.RenameTable(
                name: "T_Departements",
                schema: "dbo",
                newName: "Departements",
                newSchema: "shared");

            migrationBuilder.RenameTable(
                name: "T_Contrats",
                schema: "dbo",
                newName: "Contrats",
                newSchema: "ref");

            migrationBuilder.RenameTable(
                name: "T_Compagnies",
                schema: "dbo",
                newName: "Compagnies",
                newSchema: "ref");

            migrationBuilder.RenameTable(
                name: "T_Checkpoints",
                schema: "dbo",
                newName: "Checkpoints",
                newSchema: "ref");

            migrationBuilder.RenameTable(
                name: "T_CategoriesSortie",
                schema: "dbo",
                newName: "CategoriesSortie",
                newSchema: "ref");

            migrationBuilder.RenameTable(
                name: "T_BonsSortie",
                schema: "dbo",
                newName: "BonsSortie",
                newSchema: "bsm");

            migrationBuilder.RenameTable(
                name: "T_BonSortieHistories",
                schema: "dbo",
                newName: "BonSortieHistories",
                newSchema: "bsm");

            migrationBuilder.RenameTable(
                name: "T_Bons",
                schema: "dbo",
                newName: "Bons",
                newSchema: "bem");

            migrationBuilder.RenameTable(
                name: "T_BonEntreeHistory",
                schema: "dbo",
                newName: "BonEntreeHistory",
                newSchema: "bem");

            migrationBuilder.RenameTable(
                name: "T_Barrieres",
                schema: "dbo",
                newName: "Barrieres",
                newSchema: "shared");

            migrationBuilder.RenameTable(
                name: "T_AuditLogs",
                schema: "dbo",
                newName: "AuditLogs");

            migrationBuilder.RenameTable(
                name: "T_ApprobationsSortie",
                schema: "dbo",
                newName: "ApprobationsSortie",
                newSchema: "bsm");

            migrationBuilder.RenameTable(
                name: "T_Approbations",
                schema: "dbo",
                newName: "Approbations",
                newSchema: "bem");

            migrationBuilder.RenameTable(
                name: "T_Anomalies",
                schema: "dbo",
                newName: "Anomalies",
                newSchema: "sec");

            migrationBuilder.RenameIndex(
                name: "IX_T_UtilisateurRoles_IdRole",
                table: "UtilisateurRoles",
                newName: "IX_UtilisateurRoles_IdRole");

            migrationBuilder.RenameIndex(
                name: "IX_T_ScanEvenements_AnomalieIdAnomalie",
                schema: "sec",
                table: "ScanEvenements",
                newName: "IX_ScanEvenements_AnomalieIdAnomalie");

            migrationBuilder.RenameIndex(
                name: "IX_T_RaisonsSortie_CategorieId",
                schema: "ref",
                table: "RaisonsSortie",
                newName: "IX_RaisonsSortie_CategorieId");

            migrationBuilder.RenameIndex(
                name: "IX_T_PassagesCheckpoint_CheckpointId",
                schema: "dbo",
                table: "PassagesCheckpoint",
                newName: "IX_PassagesCheckpoint_CheckpointId");

            migrationBuilder.RenameIndex(
                name: "IX_T_HistoriqueScans_ScanId",
                schema: "sec",
                table: "HistoriqueScans",
                newName: "IX_HistoriqueScans_ScanId");

            migrationBuilder.RenameIndex(
                name: "IX_T_Employees_DepartementId",
                schema: "ref",
                table: "Employees",
                newName: "IX_Employees_DepartementId");

            migrationBuilder.RenameIndex(
                name: "IX_T_Employees_CompagnieId",
                schema: "ref",
                table: "Employees",
                newName: "IX_Employees_CompagnieId");

            migrationBuilder.RenameIndex(
                name: "IX_T_Contrats_PoNumber",
                schema: "ref",
                table: "Contrats",
                newName: "IX_Contrats_PoNumber");

            migrationBuilder.RenameIndex(
                name: "IX_T_Contrats_CompagnieId",
                schema: "ref",
                table: "Contrats",
                newName: "IX_Contrats_CompagnieId");

            migrationBuilder.RenameIndex(
                name: "IX_T_Checkpoints_SiteId",
                schema: "ref",
                table: "Checkpoints",
                newName: "IX_Checkpoints_SiteId");

            migrationBuilder.RenameIndex(
                name: "IX_T_BonEntreeHistory_BonIdBon",
                schema: "bem",
                table: "BonEntreeHistory",
                newName: "IX_BonEntreeHistory_BonIdBon");

            migrationBuilder.RenameIndex(
                name: "IX_T_Anomalies_BarriereId",
                schema: "sec",
                table: "Anomalies",
                newName: "IX_Anomalies_BarriereId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Utilisateurs",
                schema: "shared",
                table: "Utilisateurs",
                column: "IdUtilisateur");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UtilisateurRoles",
                table: "UtilisateurRoles",
                column: "IdUtilisateurRole");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TypesMateriels",
                table: "TypesMateriels",
                column: "IdTypeMateriel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Statuts",
                table: "Statuts",
                column: "IdStatut");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SoldesMateriels",
                schema: "bem",
                table: "SoldesMateriels",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sites",
                schema: "ref",
                table: "Sites",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScanEvenements",
                schema: "sec",
                table: "ScanEvenements",
                column: "IdScan");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Roles",
                table: "Roles",
                column: "IdRole");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RaisonsSortie",
                schema: "ref",
                table: "RaisonsSortie",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PassagesCheckpoint",
                schema: "dbo",
                table: "PassagesCheckpoint",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ParametresSysteme",
                table: "ParametresSysteme",
                column: "IdParametre");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NotificationsRejet",
                schema: "dbo",
                table: "NotificationsRejet",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaterielsSortie",
                schema: "bsm",
                table: "MaterielsSortie",
                column: "IdMateriel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Materiels",
                schema: "bem",
                table: "Materiels",
                column: "IdMateriel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItinerairesSortie",
                schema: "bsm",
                table: "ItinerairesSortie",
                column: "IdItineraire");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItinerairesPrevu",
                schema: "bem",
                table: "ItinerairesPrevu",
                column: "IdItineraire");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HistoriqueScans",
                schema: "sec",
                table: "HistoriqueScans",
                column: "IdHistorique");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Employees",
                schema: "ref",
                table: "Employees",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Departements",
                schema: "shared",
                table: "Departements",
                column: "IdDepartement");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contrats",
                schema: "ref",
                table: "Contrats",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Compagnies",
                schema: "ref",
                table: "Compagnies",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Checkpoints",
                schema: "ref",
                table: "Checkpoints",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoriesSortie",
                schema: "ref",
                table: "CategoriesSortie",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BonsSortie",
                schema: "bsm",
                table: "BonsSortie",
                column: "IdBon");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BonSortieHistories",
                schema: "bsm",
                table: "BonSortieHistories",
                column: "IdHistory");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bons",
                schema: "bem",
                table: "Bons",
                column: "IdBon");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BonEntreeHistory",
                schema: "bem",
                table: "BonEntreeHistory",
                column: "IdHistory");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Barrieres",
                schema: "shared",
                table: "Barrieres",
                column: "IdBarriere");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuditLogs",
                table: "AuditLogs",
                column: "IdAuditLog");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApprobationsSortie",
                schema: "bsm",
                table: "ApprobationsSortie",
                column: "IdApprobation");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Approbations",
                schema: "bem",
                table: "Approbations",
                column: "IdApprobation");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Anomalies",
                schema: "sec",
                table: "Anomalies",
                column: "IdAnomalie");

            migrationBuilder.AddForeignKey(
                name: "FK_Anomalies_Barrieres_BarriereId",
                schema: "sec",
                table: "Anomalies",
                column: "BarriereId",
                principalSchema: "shared",
                principalTable: "Barrieres",
                principalColumn: "IdBarriere",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Anomalies_ScanEvenements_ScanId",
                schema: "sec",
                table: "Anomalies",
                column: "ScanId",
                principalSchema: "sec",
                principalTable: "ScanEvenements",
                principalColumn: "IdScan",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Approbations_Bons_BonId",
                schema: "bem",
                table: "Approbations",
                column: "BonId",
                principalSchema: "bem",
                principalTable: "Bons",
                principalColumn: "IdBon",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApprobationsSortie_BonsSortie_BonSortieId",
                schema: "bsm",
                table: "ApprobationsSortie",
                column: "BonSortieId",
                principalSchema: "bsm",
                principalTable: "BonsSortie",
                principalColumn: "IdBon",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BonEntreeHistory_Bons_BonId",
                schema: "bem",
                table: "BonEntreeHistory",
                column: "BonId",
                principalSchema: "bem",
                principalTable: "Bons",
                principalColumn: "IdBon",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BonEntreeHistory_Bons_BonIdBon",
                schema: "bem",
                table: "BonEntreeHistory",
                column: "BonIdBon",
                principalSchema: "bem",
                principalTable: "Bons",
                principalColumn: "IdBon");

            migrationBuilder.AddForeignKey(
                name: "FK_BonSortieHistories_BonsSortie_BonSortieId",
                schema: "bsm",
                table: "BonSortieHistories",
                column: "BonSortieId",
                principalSchema: "bsm",
                principalTable: "BonsSortie",
                principalColumn: "IdBon",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BonsSortie_Bons_BonEntreeAssocieId",
                schema: "bsm",
                table: "BonsSortie",
                column: "BonEntreeAssocieId",
                principalSchema: "bem",
                principalTable: "Bons",
                principalColumn: "IdBon",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Checkpoints_Sites_SiteId",
                schema: "ref",
                table: "Checkpoints",
                column: "SiteId",
                principalSchema: "ref",
                principalTable: "Sites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contrats_Compagnies_CompagnieId",
                schema: "ref",
                table: "Contrats",
                column: "CompagnieId",
                principalSchema: "ref",
                principalTable: "Compagnies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Compagnies_CompagnieId",
                schema: "ref",
                table: "Employees",
                column: "CompagnieId",
                principalSchema: "ref",
                principalTable: "Compagnies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Departements_DepartementId",
                schema: "ref",
                table: "Employees",
                column: "DepartementId",
                principalSchema: "shared",
                principalTable: "Departements",
                principalColumn: "IdDepartement");

            migrationBuilder.AddForeignKey(
                name: "FK_HistoriqueScans_ScanEvenements_ScanId",
                schema: "sec",
                table: "HistoriqueScans",
                column: "ScanId",
                principalSchema: "sec",
                principalTable: "ScanEvenements",
                principalColumn: "IdScan",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItinerairesPrevu_Bons_BonId",
                schema: "bem",
                table: "ItinerairesPrevu",
                column: "BonId",
                principalSchema: "bem",
                principalTable: "Bons",
                principalColumn: "IdBon",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItinerairesSortie_BonsSortie_BonSortieId",
                schema: "bsm",
                table: "ItinerairesSortie",
                column: "BonSortieId",
                principalSchema: "bsm",
                principalTable: "BonsSortie",
                principalColumn: "IdBon",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Materiels_Bons_BonId",
                schema: "bem",
                table: "Materiels",
                column: "BonId",
                principalSchema: "bem",
                principalTable: "Bons",
                principalColumn: "IdBon",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterielsSortie_BonsSortie_BonSortieId",
                schema: "bsm",
                table: "MaterielsSortie",
                column: "BonSortieId",
                principalSchema: "bsm",
                principalTable: "BonsSortie",
                principalColumn: "IdBon",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PassagesCheckpoint_Checkpoints_CheckpointId",
                schema: "dbo",
                table: "PassagesCheckpoint",
                column: "CheckpointId",
                principalSchema: "ref",
                principalTable: "Checkpoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RaisonsSortie_CategoriesSortie_CategorieId",
                schema: "ref",
                table: "RaisonsSortie",
                column: "CategorieId",
                principalSchema: "ref",
                principalTable: "CategoriesSortie",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScanEvenements_Anomalies_AnomalieIdAnomalie",
                schema: "sec",
                table: "ScanEvenements",
                column: "AnomalieIdAnomalie",
                principalSchema: "sec",
                principalTable: "Anomalies",
                principalColumn: "IdAnomalie");

            migrationBuilder.AddForeignKey(
                name: "FK_ScanEvenements_Barrieres_BarriereId",
                schema: "sec",
                table: "ScanEvenements",
                column: "BarriereId",
                principalSchema: "shared",
                principalTable: "Barrieres",
                principalColumn: "IdBarriere",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UtilisateurRoles_Roles_IdRole",
                table: "UtilisateurRoles",
                column: "IdRole",
                principalTable: "Roles",
                principalColumn: "IdRole",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UtilisateurRoles_Utilisateurs_IdUtilisateur",
                table: "UtilisateurRoles",
                column: "IdUtilisateur",
                principalSchema: "shared",
                principalTable: "Utilisateurs",
                principalColumn: "IdUtilisateur",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
