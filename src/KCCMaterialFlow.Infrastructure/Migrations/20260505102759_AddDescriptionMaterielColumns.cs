using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionMaterielColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Conditionally drop index if it exists (migration may be idempotent)
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_BonsSortie_TypeMateriel' AND object_id = OBJECT_ID('dbo.T_BonsSortie'))
                BEGIN
                    DROP INDEX [IX_BonsSortie_TypeMateriel] ON [dbo].[T_BonsSortie];
                END");

            // Conditionally drop columns if they exist
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'T_RaisonsSortie' AND COLUMN_NAME = 'TypeMaterielDefaut')
                BEGIN
                    ALTER TABLE [dbo].[T_RaisonsSortie] DROP COLUMN [TypeMaterielDefaut];
                END");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'T_BonsSortie' AND COLUMN_NAME = 'TypeMateriel')
                BEGIN
                    ALTER TABLE [dbo].[T_BonsSortie] DROP COLUMN [TypeMateriel];
                END");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'T_BonsSortie' AND COLUMN_NAME = 'TypeMaterielInterne')
                BEGIN
                    ALTER TABLE [dbo].[T_BonsSortie] DROP COLUMN [TypeMaterielInterne];
                END");

            // Conditionally add columns if they don't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'T_BonsSortie' AND COLUMN_NAME = 'DescriptionMateriel')
                BEGIN
                    ALTER TABLE [dbo].[T_BonsSortie] ADD [DescriptionMateriel] nvarchar(200) NULL;
                END");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'T_BonsSortie' AND COLUMN_NAME = 'DescriptionMaterielInterne')
                BEGIN
                    ALTER TABLE [dbo].[T_BonsSortie] ADD [DescriptionMaterielInterne] nvarchar(200) NULL;
                END");

            // Conditionally create index if it doesn't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_BonsSortie_DescriptionMateriel' AND object_id = OBJECT_ID('dbo.T_BonsSortie'))
                BEGIN
                    CREATE INDEX [IX_BonsSortie_DescriptionMateriel] ON [dbo].[T_BonsSortie] ([DescriptionMateriel]);
                END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BonsSortie_DescriptionMateriel",
                schema: "dbo",
                table: "T_BonsSortie");

            migrationBuilder.DropColumn(
                name: "DescriptionMateriel",
                schema: "dbo",
                table: "T_BonsSortie");

            migrationBuilder.DropColumn(
                name: "DescriptionMaterielInterne",
                schema: "dbo",
                table: "T_BonsSortie");

            migrationBuilder.AddColumn<int>(
                name: "TypeMaterielDefaut",
                schema: "dbo",
                table: "T_RaisonsSortie",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TypeMateriel",
                schema: "dbo",
                table: "T_BonsSortie",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TypeMaterielInterne",
                schema: "dbo",
                table: "T_BonsSortie",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BonsSortie_TypeMateriel",
                schema: "dbo",
                table: "T_BonsSortie",
                column: "TypeMateriel");
        }
    }
}
