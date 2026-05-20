using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTypeMaterielSortieWorkflowRoutage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Colonne WorkflowRoutage sur T_TypesMateriels
            migrationBuilder.AddColumn<int>(
                name: "WorkflowRoutage",
                schema: "dbo",
                table: "T_TypesMateriels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // 2. Colonne TypeMaterielSortieId sur T_BonsSortie
            migrationBuilder.AddColumn<int>(
                name: "TypeMaterielSortieId",
                schema: "dbo",
                table: "T_BonsSortie",
                type: "int",
                nullable: true);

            // 3. Index sur la FK
            migrationBuilder.CreateIndex(
                name: "IX_T_BonsSortie_TypeMaterielSortieId",
                schema: "dbo",
                table: "T_BonsSortie",
                column: "TypeMaterielSortieId");

            // 4. Contrainte FK
            migrationBuilder.AddForeignKey(
                name: "FK_T_BonsSortie_T_TypesMateriels_TypeMaterielSortieId",
                schema: "dbo",
                table: "T_BonsSortie",
                column: "TypeMaterielSortieId",
                principalSchema: "dbo",
                principalTable: "T_TypesMateriels",
                principalColumn: "IdTypeMateriel",
                onDelete: ReferentialAction.SetNull);

            // 5. Seed des 7 nouveaux types matériel (workflow sortie)
            migrationBuilder.InsertData(
                schema: "dbo",
                table: "T_TypesMateriels",
                columns: new[]
                {
                    "IdTypeMateriel",
                    "Categorie",
                    "CodeType",
                    "Couleur",
                    "DateCreation",
                    "Description",
                    "DureeMaximumJours",
                    "DureeValiditeDefautJours",
                    "EstActif",
                    "Icone",
                    "NiveauxApprobation",
                    "NomType",
                    "NumeroSerieObligatoire",
                    "Ordre",
                    "PhotoObligatoire",
                    "RequiertApprobationDepartement",
                    "RequiertApprobationDirection",
                    "WorkflowRoutage"
                },
                values: new object[,]
                {
                    { 10, "Workflow Sortie", "BSM_CIRCULANT", "#0d6efd", new DateTime(2025, 1, 1), "Matériel circulant entre sites — workflow standard.", 365, 30, true, "bi-arrow-repeat", 1, "Circulant", false, 10, false, true, false, 0 },
                    { 11, "Workflow Sortie", "BSM_EQUIPEMENT_IT", "#6610f2", new DateTime(2025, 1, 1), "Équipement informatique — passe par le département IT.", 365, 30, true, "bi-laptop", 2, "Équipement IT", true, 11, false, true, true, 1 },
                    { 12, "Workflow Sortie", "BSM_FIN_PROJET", "#20c997", new DateTime(2025, 1, 1), "Sortie en fin de chantier / projet — workflow standard.", 365, 30, true, "bi-flag", 1, "Fin de projet", false, 12, false, true, false, 0 },
                    { 13, "Workflow Sortie", "BSM_RESIDU_DECHET", "#fd7e14", new DateTime(2025, 1, 1), "Résidu ou déchet industriel — passe par le département Environnement.", 365, 30, true, "bi-trash", 1, "Résidu / Déchet", false, 13, false, true, false, 2 },
                    { 14, "Workflow Sortie", "BSM_RADIOPROTECTION", "#dc3545", new DateTime(2025, 1, 1), "Matériel de radioprotection — passe par le département Environnement.", 365, 30, true, "bi-radioactive", 1, "Radioprotection", true, 14, false, true, false, 2 },
                    { 15, "Workflow Sortie", "BSM_MODIFICATION", "#ffc107", new DateTime(2025, 1, 1), "Matériel sorti pour modification — passe par le département Environnement.", 365, 30, true, "bi-tools", 1, "Modification", false, 15, false, true, false, 2 },
                    { 16, "Workflow Sortie", "BSM_PRET_MATERIEL", "#6c757d", new DateTime(2025, 1, 1), "Matériel prêté temporairement — workflow standard.", 180, 30, true, "bi-box-arrow-up-right", 1, "Matériel prêté", false, 16, false, true, false, 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_BonsSortie_T_TypesMateriels_TypeMaterielSortieId",
                schema: "dbo",
                table: "T_BonsSortie");

            migrationBuilder.DropIndex(
                name: "IX_T_BonsSortie_TypeMaterielSortieId",
                schema: "dbo",
                table: "T_BonsSortie");

            for (int id = 10; id <= 16; id++)
            {
                migrationBuilder.DeleteData(
                    schema: "dbo",
                    table: "T_TypesMateriels",
                    keyColumn: "IdTypeMateriel",
                    keyValue: id);
            }

            migrationBuilder.DropColumn(
                name: "TypeMaterielSortieId",
                schema: "dbo",
                table: "T_BonsSortie");

            migrationBuilder.DropColumn(
                name: "WorkflowRoutage",
                schema: "dbo",
                table: "T_TypesMateriels");
        }
    }
}
