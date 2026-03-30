using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixRemainingHistoryActionValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Catch-all: map any remaining French string values to enum member names
            // in both BonSortie and BonEntree history tables

            migrationBuilder.Sql(@"
                -- T_BonSortieHistories: partial/variant French values
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'RetourModification' WHERE TypeAction = N'Retour';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'RetourModification' WHERE TypeAction = N'Retour modification';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'RetourPret'         WHERE TypeAction = N'Retour pret';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'SortieEffective'    WHERE TypeAction = N'Sortie';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'ExtensionPret'      WHERE TypeAction = N'Extension';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'MiseEnInvestigation' WHERE TypeAction = N'Investigation';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'AjoutMateriel'      WHERE TypeAction = N'Ajout';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'SuppressionMateriel' WHERE TypeAction = N'Suppression';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'GenerationQR'       WHERE TypeAction = N'Generation QR';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'ScanBarriere'       WHERE TypeAction = N'Scan';

                -- T_BonEntreeHistory: partial/variant French values
                UPDATE dbo.T_BonEntreeHistory SET [Action] = 'RetourModification' WHERE [Action] = N'Retour';
                UPDATE dbo.T_BonEntreeHistory SET [Action] = 'RetourModification' WHERE [Action] = N'Retour modification';
                UPDATE dbo.T_BonEntreeHistory SET [Action] = 'EntreeEffective'    WHERE [Action] = N'Entrée';
                UPDATE dbo.T_BonEntreeHistory SET [Action] = 'SortieMateriel'     WHERE [Action] = N'Sortie';
                UPDATE dbo.T_BonEntreeHistory SET [Action] = 'AjoutMateriel'      WHERE [Action] = N'Ajout';
                UPDATE dbo.T_BonEntreeHistory SET [Action] = 'SuppressionMateriel' WHERE [Action] = N'Suppression';
                UPDATE dbo.T_BonEntreeHistory SET [Action] = 'GenerationQR'       WHERE [Action] = N'Generation QR';
                UPDATE dbo.T_BonEntreeHistory SET [Action] = 'ScanBarriere'       WHERE [Action] = N'Scan';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
