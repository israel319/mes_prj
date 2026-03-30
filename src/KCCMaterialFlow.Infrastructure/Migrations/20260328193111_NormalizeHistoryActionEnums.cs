using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeHistoryActionEnums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ── T_BonEntreeHistory.Action : anciennes valeurs françaises → noms d'enum ──
            migrationBuilder.Sql(@"
                UPDATE dbo.T_BonEntreeHistory SET [Action] = 'Creation'          WHERE [Action] = N'Création';
                UPDATE dbo.T_BonEntreeHistory SET [Action] = 'Modification'      WHERE [Action] = N'Modification';
                UPDATE dbo.T_BonEntreeHistory SET [Action] = 'Soumission'        WHERE [Action] = N'Soumission';
                UPDATE dbo.T_BonEntreeHistory SET [Action] = 'Approbation'       WHERE [Action] = N'Approbation';
                UPDATE dbo.T_BonEntreeHistory SET [Action] = 'Rejet'             WHERE [Action] = N'Rejet';
                UPDATE dbo.T_BonEntreeHistory SET [Action] = 'RetourModification' WHERE [Action] = N'Retour pour modification';
                UPDATE dbo.T_BonEntreeHistory SET [Action] = 'GenerationQR'      WHERE [Action] = N'Génération QR';
                UPDATE dbo.T_BonEntreeHistory SET [Action] = 'ScanBarriere'      WHERE [Action] = N'Scan barrière';
                UPDATE dbo.T_BonEntreeHistory SET [Action] = 'EntreeEffective'   WHERE [Action] = N'Entrée effective';
                UPDATE dbo.T_BonEntreeHistory SET [Action] = 'SortieMateriel'    WHERE [Action] = N'Sortie matériel';
                UPDATE dbo.T_BonEntreeHistory SET [Action] = 'Cloture'           WHERE [Action] = N'Clôture';
                UPDATE dbo.T_BonEntreeHistory SET [Action] = 'Annulation'        WHERE [Action] = N'Annulation';
                UPDATE dbo.T_BonEntreeHistory SET [Action] = 'Prolongation'      WHERE [Action] = N'Prolongation';
                UPDATE dbo.T_BonEntreeHistory SET [Action] = 'AjoutMateriel'     WHERE [Action] = N'Ajout matériel';
                UPDATE dbo.T_BonEntreeHistory SET [Action] = 'SuppressionMateriel' WHERE [Action] = N'Suppression matériel';
                UPDATE dbo.T_BonEntreeHistory SET [Action] = 'Anomalie'          WHERE [Action] = N'Anomalie';
                UPDATE dbo.T_BonEntreeHistory SET [Action] = 'Impression'        WHERE [Action] = N'Impression';
                UPDATE dbo.T_BonEntreeHistory SET [Action] = 'Notification'      WHERE [Action] = N'Notification';
            ");

            // ── T_BonSortieHistories.TypeAction : anciennes valeurs françaises → noms d'enum ──
            migrationBuilder.Sql(@"
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'Creation'          WHERE TypeAction = N'Création';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'Modification'      WHERE TypeAction = N'Modification';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'Soumission'        WHERE TypeAction = N'Soumission';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'Approbation'       WHERE TypeAction = N'Approbation';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'Rejet'             WHERE TypeAction = N'Rejet';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'RetourModification' WHERE TypeAction = N'Retour pour modification';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'GenerationQR'      WHERE TypeAction = N'Génération QR';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'ScanBarriere'      WHERE TypeAction = N'Scan barrière';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'SortieEffective'   WHERE TypeAction = N'Sortie effective';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'RetourPret'        WHERE TypeAction = N'Retour prêt';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'Cloture'           WHERE TypeAction = N'Clôture';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'Annulation'        WHERE TypeAction = N'Annulation';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'Prolongation'      WHERE TypeAction = N'Prolongation';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'AjoutMateriel'     WHERE TypeAction = N'Ajout matériel';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'SuppressionMateriel' WHERE TypeAction = N'Suppression matériel';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'Anomalie'          WHERE TypeAction = N'Anomalie';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'Impression'        WHERE TypeAction = N'Impression';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'Notification'      WHERE TypeAction = N'Notification';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'ExtensionPret'     WHERE TypeAction = N'Extension prêt';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'MiseEnInvestigation' WHERE TypeAction = N'Mise en investigation';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
