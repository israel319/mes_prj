using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    /// <summary>
    /// Migration vide pour synchroniser le snapshot du mod�le EF.
    /// Cette migration ne modifie pas la base de donn�es.
    /// Elle met � jour le snapshot pour correspondre au mod�le actuel.
    /// </summary>
    public partial class SyncSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Migration vide - synchronisation du snapshot uniquement
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Migration vide - synchronisation du snapshot uniquement
        }
    }
}
