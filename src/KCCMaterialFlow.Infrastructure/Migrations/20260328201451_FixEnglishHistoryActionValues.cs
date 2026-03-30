using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixEnglishHistoryActionValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Fix English-named action values from legacy code to match enum member names
            migrationBuilder.Sql(@"
                -- T_BonSortieHistories: English variants → enum names
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'GenerationQR'       WHERE TypeAction = 'QRCodeGenerated';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'Creation'            WHERE TypeAction = 'Created';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'Modification'        WHERE TypeAction = 'Modified';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'Soumission'          WHERE TypeAction = 'Submitted';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'Approbation'         WHERE TypeAction = 'Approved';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'Rejet'               WHERE TypeAction = 'Rejected';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'RetourModification'  WHERE TypeAction = 'ReturnedForModification';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'SortieEffective'     WHERE TypeAction = 'ExitEffective';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'RetourPret'          WHERE TypeAction = 'LoanReturned';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'Cloture'             WHERE TypeAction = 'Closed';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'Annulation'          WHERE TypeAction = 'Cancelled';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'ExtensionPret'       WHERE TypeAction = 'LoanExtended';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'MiseEnInvestigation' WHERE TypeAction = 'Investigation';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'Impression'          WHERE TypeAction = 'Printed';
                UPDATE dbo.T_BonSortieHistories SET TypeAction = 'Notification'        WHERE TypeAction = 'NotificationSent';

                -- T_BonEntreeHistory: English variants → enum names
                UPDATE dbo.T_BonEntreeHistory SET [Action] = 'GenerationQR'       WHERE [Action] = 'QRCodeGenerated';
                UPDATE dbo.T_BonEntreeHistory SET [Action] = 'Creation'            WHERE [Action] = 'Created';
                UPDATE dbo.T_BonEntreeHistory SET [Action] = 'Modification'        WHERE [Action] = 'Modified';
                UPDATE dbo.T_BonEntreeHistory SET [Action] = 'Soumission'          WHERE [Action] = 'Submitted';
                UPDATE dbo.T_BonEntreeHistory SET [Action] = 'Approbation'         WHERE [Action] = 'Approved';
                UPDATE dbo.T_BonEntreeHistory SET [Action] = 'Rejet'               WHERE [Action] = 'Rejected';
                UPDATE dbo.T_BonEntreeHistory SET [Action] = 'RetourModification'  WHERE [Action] = 'ReturnedForModification';
                UPDATE dbo.T_BonEntreeHistory SET [Action] = 'EntreeEffective'     WHERE [Action] = 'EntryEffective';
                UPDATE dbo.T_BonEntreeHistory SET [Action] = 'SortieMateriel'      WHERE [Action] = 'MaterialExit';
                UPDATE dbo.T_BonEntreeHistory SET [Action] = 'Cloture'             WHERE [Action] = 'Closed';
                UPDATE dbo.T_BonEntreeHistory SET [Action] = 'Annulation'          WHERE [Action] = 'Cancelled';
                UPDATE dbo.T_BonEntreeHistory SET [Action] = 'Impression'          WHERE [Action] = 'Printed';
                UPDATE dbo.T_BonEntreeHistory SET [Action] = 'Notification'        WHERE [Action] = 'NotificationSent';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
