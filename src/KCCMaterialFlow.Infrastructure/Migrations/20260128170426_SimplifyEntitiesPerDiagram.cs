using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KCCMaterialFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyEntitiesPerDiagram : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materiels_Bons_BonEntreeId",
                schema: "bem",
                table: "Materiels");

            migrationBuilder.DropIndex(
                name: "IX_Materiels_BonEntree_Ressorti",
                schema: "bem",
                table: "Materiels");

            migrationBuilder.DropIndex(
                name: "IX_Materiels_BonSortie",
                schema: "bem",
                table: "Materiels");

            migrationBuilder.DropIndex(
                name: "IX_Materiels_Ressorti",
                schema: "bem",
                table: "Materiels");

            migrationBuilder.DropIndex(
                name: "IX_Materiels_Type",
                schema: "bem",
                table: "Materiels");

            migrationBuilder.DropIndex(
                name: "IX_ItinerairesPrevu_Bon_Scan_Ordre",
                schema: "bem",
                table: "ItinerairesPrevu");

            migrationBuilder.DropIndex(
                name: "IX_ItinerairesPrevu_Scan",
                schema: "bem",
                table: "ItinerairesPrevu");

            migrationBuilder.DropIndex(
                name: "IX_Bons_BonSortieLie",
                schema: "bem",
                table: "Bons");

            migrationBuilder.DropIndex(
                name: "IX_Bons_Createur",
                schema: "bem",
                table: "Bons");

            migrationBuilder.DropIndex(
                name: "IX_Bons_Ressortie",
                schema: "bem",
                table: "Bons");

            migrationBuilder.DropIndex(
                name: "IX_Bons_SiteManager",
                schema: "bem",
                table: "Bons");

            migrationBuilder.DropIndex(
                name: "IX_Bons_Statut_Type_Date",
                schema: "bem",
                table: "Bons");

            migrationBuilder.DropIndex(
                name: "IX_Bons_Type",
                schema: "bem",
                table: "Bons");

            migrationBuilder.DropIndex(
                name: "IX_Approbations_EtapeCourante",
                schema: "bem",
                table: "Approbations");

            migrationBuilder.DropIndex(
                name: "IX_Approbations_Utilisateur",
                schema: "bem",
                table: "Approbations");

            migrationBuilder.DropIndex(
                name: "IX_Approbations_Utilisateur_Decision_Courante",
                schema: "bem",
                table: "Approbations");

            migrationBuilder.DropColumn(
                name: "BonSortieId",
                schema: "bem",
                table: "Materiels");

            migrationBuilder.DropColumn(
                name: "DateCreation",
                schema: "bem",
                table: "Materiels");

            migrationBuilder.DropColumn(
                name: "DateModification",
                schema: "bem",
                table: "Materiels");

            migrationBuilder.DropColumn(
                name: "DateSortie",
                schema: "bem",
                table: "Materiels");

            migrationBuilder.DropColumn(
                name: "Devise",
                schema: "bem",
                table: "Materiels");

            migrationBuilder.DropColumn(
                name: "EstRessorti",
                schema: "bem",
                table: "Materiels");

            migrationBuilder.DropColumn(
                name: "EtatEntree",
                schema: "bem",
                table: "Materiels");

            migrationBuilder.DropColumn(
                name: "EtatSortie",
                schema: "bem",
                table: "Materiels");

            migrationBuilder.DropColumn(
                name: "Marque",
                schema: "bem",
                table: "Materiels");

            migrationBuilder.DropColumn(
                name: "Modele",
                schema: "bem",
                table: "Materiels");

            migrationBuilder.DropColumn(
                name: "ObservationsEntree",
                schema: "bem",
                table: "Materiels");

            migrationBuilder.DropColumn(
                name: "ObservationsSortie",
                schema: "bem",
                table: "Materiels");

            migrationBuilder.DropColumn(
                name: "PhotoEntree",
                schema: "bem",
                table: "Materiels");

            migrationBuilder.DropColumn(
                name: "PhotoSortie",
                schema: "bem",
                table: "Materiels");

            migrationBuilder.DropColumn(
                name: "TypeMateriel",
                schema: "bem",
                table: "Materiels");

            migrationBuilder.DropColumn(
                name: "Unite",
                schema: "bem",
                table: "Materiels");

            migrationBuilder.DropColumn(
                name: "ValeurEstimee",
                schema: "bem",
                table: "Materiels");

            migrationBuilder.DropColumn(
                name: "AgentLogin",
                schema: "bem",
                table: "ItinerairesPrevu");

            migrationBuilder.DropColumn(
                name: "AgentNom",
                schema: "bem",
                table: "ItinerairesPrevu");

            migrationBuilder.DropColumn(
                name: "BarriereLocalisation",
                schema: "bem",
                table: "ItinerairesPrevu");

            migrationBuilder.DropColumn(
                name: "BarriereNom",
                schema: "bem",
                table: "ItinerairesPrevu");

            migrationBuilder.DropColumn(
                name: "DateCreation",
                schema: "bem",
                table: "ItinerairesPrevu");

            migrationBuilder.DropColumn(
                name: "DatePassage",
                schema: "bem",
                table: "ItinerairesPrevu");

            migrationBuilder.DropColumn(
                name: "EstObligatoire",
                schema: "bem",
                table: "ItinerairesPrevu");

            migrationBuilder.DropColumn(
                name: "Observations",
                schema: "bem",
                table: "ItinerairesPrevu");

            migrationBuilder.DropColumn(
                name: "ScanEffectue",
                schema: "bem",
                table: "ItinerairesPrevu");

            migrationBuilder.DropColumn(
                name: "AgentEntreeLogin",
                schema: "bem",
                table: "Bons");

            migrationBuilder.DropColumn(
                name: "BarriereEntreeId",
                schema: "bem",
                table: "Bons");

            migrationBuilder.DropColumn(
                name: "BarriereEntreeNom",
                schema: "bem",
                table: "Bons");

            migrationBuilder.DropColumn(
                name: "BonSortieLieId",
                schema: "bem",
                table: "Bons");

            migrationBuilder.DropColumn(
                name: "Commentaires",
                schema: "bem",
                table: "Bons");

            migrationBuilder.DropColumn(
                name: "CreateurDepartement",
                schema: "bem",
                table: "Bons");

            migrationBuilder.DropColumn(
                name: "CreateurLogin",
                schema: "bem",
                table: "Bons");

            migrationBuilder.DropColumn(
                name: "CreateurNom",
                schema: "bem",
                table: "Bons");

            migrationBuilder.DropColumn(
                name: "DateAnnulation",
                schema: "bem",
                table: "Bons");

            migrationBuilder.DropColumn(
                name: "DateEntreeEffective",
                schema: "bem",
                table: "Bons");

            migrationBuilder.DropColumn(
                name: "DateEntreePrevue",
                schema: "bem",
                table: "Bons");

            migrationBuilder.DropColumn(
                name: "DateGenerationQR",
                schema: "bem",
                table: "Bons");

            migrationBuilder.DropColumn(
                name: "DateModification",
                schema: "bem",
                table: "Bons");

            migrationBuilder.DropColumn(
                name: "DateToutRessorti",
                schema: "bem",
                table: "Bons");

            migrationBuilder.DropColumn(
                name: "DoitRessortir",
                schema: "bem",
                table: "Bons");

            migrationBuilder.DropColumn(
                name: "EscorteurLogin",
                schema: "bem",
                table: "Bons");

            migrationBuilder.DropColumn(
                name: "EstAnnule",
                schema: "bem",
                table: "Bons");

            migrationBuilder.DropColumn(
                name: "ModificateurLogin",
                schema: "bem",
                table: "Bons");

            migrationBuilder.DropColumn(
                name: "MotifAnnulation",
                schema: "bem",
                table: "Bons");

            migrationBuilder.DropColumn(
                name: "ObservationsEntree",
                schema: "bem",
                table: "Bons");

            migrationBuilder.DropColumn(
                name: "QRCodeData",
                schema: "bem",
                table: "Bons");

            migrationBuilder.DropColumn(
                name: "QRCodePath",
                schema: "bem",
                table: "Bons");

            migrationBuilder.DropColumn(
                name: "SiteManagerLogin",
                schema: "bem",
                table: "Bons");

            migrationBuilder.DropColumn(
                name: "TelephoneContractant",
                schema: "bem",
                table: "Bons");

            migrationBuilder.DropColumn(
                name: "ToutRessorti",
                schema: "bem",
                table: "Bons");

            migrationBuilder.DropColumn(
                name: "TypeBon",
                schema: "bem",
                table: "Bons");

            migrationBuilder.DropColumn(
                name: "ZoneTravail",
                schema: "bem",
                table: "Bons");

            migrationBuilder.DropColumn(
                name: "Commentaire",
                schema: "bem",
                table: "Approbations");

            migrationBuilder.DropColumn(
                name: "DateCreation",
                schema: "bem",
                table: "Approbations");

            migrationBuilder.DropColumn(
                name: "EstEtapeCourante",
                schema: "bem",
                table: "Approbations");

            migrationBuilder.DropColumn(
                name: "NomEtape",
                schema: "bem",
                table: "Approbations");

            migrationBuilder.DropColumn(
                name: "StatutAttendu",
                schema: "bem",
                table: "Approbations");

            migrationBuilder.DropColumn(
                name: "UtilisateurFonction",
                schema: "bem",
                table: "Approbations");

            migrationBuilder.DropColumn(
                name: "UtilisateurLogin",
                schema: "bem",
                table: "Approbations");

            migrationBuilder.DropColumn(
                name: "UtilisateurNom",
                schema: "bem",
                table: "Approbations");

            migrationBuilder.RenameColumn(
                name: "BonEntreeId",
                schema: "bem",
                table: "Materiels",
                newName: "BonId");

            migrationBuilder.RenameIndex(
                name: "IX_Materiels_BonEntree",
                schema: "bem",
                table: "Materiels",
                newName: "IX_Materiels_Bon");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantite",
                schema: "bem",
                table: "Materiels",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 1m,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<string>(
                name: "Decision",
                schema: "bem",
                table: "Approbations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "En attente",
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldDefaultValue: "EnAttente");

            migrationBuilder.AddForeignKey(
                name: "FK_Materiels_Bons_BonId",
                schema: "bem",
                table: "Materiels",
                column: "BonId",
                principalSchema: "bem",
                principalTable: "Bons",
                principalColumn: "IdBon",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materiels_Bons_BonId",
                schema: "bem",
                table: "Materiels");

            migrationBuilder.RenameColumn(
                name: "BonId",
                schema: "bem",
                table: "Materiels",
                newName: "BonEntreeId");

            migrationBuilder.RenameIndex(
                name: "IX_Materiels_Bon",
                schema: "bem",
                table: "Materiels",
                newName: "IX_Materiels_BonEntree");

            migrationBuilder.AlterColumn<int>(
                name: "Quantite",
                schema: "bem",
                table: "Materiels",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldDefaultValue: 1m);

            migrationBuilder.AddColumn<int>(
                name: "BonSortieId",
                schema: "bem",
                table: "Materiels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreation",
                schema: "bem",
                table: "Materiels",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateModification",
                schema: "bem",
                table: "Materiels",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateSortie",
                schema: "bem",
                table: "Materiels",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Devise",
                schema: "bem",
                table: "Materiels",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD");

            migrationBuilder.AddColumn<bool>(
                name: "EstRessorti",
                schema: "bem",
                table: "Materiels",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "EtatEntree",
                schema: "bem",
                table: "Materiels",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EtatSortie",
                schema: "bem",
                table: "Materiels",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Marque",
                schema: "bem",
                table: "Materiels",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Modele",
                schema: "bem",
                table: "Materiels",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObservationsEntree",
                schema: "bem",
                table: "Materiels",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObservationsSortie",
                schema: "bem",
                table: "Materiels",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoEntree",
                schema: "bem",
                table: "Materiels",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoSortie",
                schema: "bem",
                table: "Materiels",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TypeMateriel",
                schema: "bem",
                table: "Materiels",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Autre");

            migrationBuilder.AddColumn<string>(
                name: "Unite",
                schema: "bem",
                table: "Materiels",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "pcs");

            migrationBuilder.AddColumn<decimal>(
                name: "ValeurEstimee",
                schema: "bem",
                table: "Materiels",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AgentLogin",
                schema: "bem",
                table: "ItinerairesPrevu",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AgentNom",
                schema: "bem",
                table: "ItinerairesPrevu",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BarriereLocalisation",
                schema: "bem",
                table: "ItinerairesPrevu",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BarriereNom",
                schema: "bem",
                table: "ItinerairesPrevu",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreation",
                schema: "bem",
                table: "ItinerairesPrevu",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "DatePassage",
                schema: "bem",
                table: "ItinerairesPrevu",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EstObligatoire",
                schema: "bem",
                table: "ItinerairesPrevu",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "Observations",
                schema: "bem",
                table: "ItinerairesPrevu",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ScanEffectue",
                schema: "bem",
                table: "ItinerairesPrevu",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AgentEntreeLogin",
                schema: "bem",
                table: "Bons",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BarriereEntreeId",
                schema: "bem",
                table: "Bons",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BarriereEntreeNom",
                schema: "bem",
                table: "Bons",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BonSortieLieId",
                schema: "bem",
                table: "Bons",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Commentaires",
                schema: "bem",
                table: "Bons",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreateurDepartement",
                schema: "bem",
                table: "Bons",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreateurLogin",
                schema: "bem",
                table: "Bons",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreateurNom",
                schema: "bem",
                table: "Bons",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateAnnulation",
                schema: "bem",
                table: "Bons",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateEntreeEffective",
                schema: "bem",
                table: "Bons",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateEntreePrevue",
                schema: "bem",
                table: "Bons",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateGenerationQR",
                schema: "bem",
                table: "Bons",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateModification",
                schema: "bem",
                table: "Bons",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateToutRessorti",
                schema: "bem",
                table: "Bons",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DoitRessortir",
                schema: "bem",
                table: "Bons",
                type: "bit",
                nullable: true,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "EscorteurLogin",
                schema: "bem",
                table: "Bons",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EstAnnule",
                schema: "bem",
                table: "Bons",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ModificateurLogin",
                schema: "bem",
                table: "Bons",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotifAnnulation",
                schema: "bem",
                table: "Bons",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObservationsEntree",
                schema: "bem",
                table: "Bons",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QRCodeData",
                schema: "bem",
                table: "Bons",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QRCodePath",
                schema: "bem",
                table: "Bons",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SiteManagerLogin",
                schema: "bem",
                table: "Bons",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TelephoneContractant",
                schema: "bem",
                table: "Bons",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ToutRessorti",
                schema: "bem",
                table: "Bons",
                type: "bit",
                nullable: true,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TypeBon",
                schema: "bem",
                table: "Bons",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ZoneTravail",
                schema: "bem",
                table: "Bons",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Decision",
                schema: "bem",
                table: "Approbations",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "EnAttente",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldDefaultValue: "En attente");

            migrationBuilder.AddColumn<string>(
                name: "Commentaire",
                schema: "bem",
                table: "Approbations",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreation",
                schema: "bem",
                table: "Approbations",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<bool>(
                name: "EstEtapeCourante",
                schema: "bem",
                table: "Approbations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NomEtape",
                schema: "bem",
                table: "Approbations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StatutAttendu",
                schema: "bem",
                table: "Approbations",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UtilisateurFonction",
                schema: "bem",
                table: "Approbations",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UtilisateurLogin",
                schema: "bem",
                table: "Approbations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UtilisateurNom",
                schema: "bem",
                table: "Approbations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Materiels_BonEntree_Ressorti",
                schema: "bem",
                table: "Materiels",
                columns: new[] { "BonEntreeId", "EstRessorti" });

            migrationBuilder.CreateIndex(
                name: "IX_Materiels_BonSortie",
                schema: "bem",
                table: "Materiels",
                column: "BonSortieId");

            migrationBuilder.CreateIndex(
                name: "IX_Materiels_Ressorti",
                schema: "bem",
                table: "Materiels",
                column: "EstRessorti");

            migrationBuilder.CreateIndex(
                name: "IX_Materiels_Type",
                schema: "bem",
                table: "Materiels",
                column: "TypeMateriel");

            migrationBuilder.CreateIndex(
                name: "IX_ItinerairesPrevu_Bon_Scan_Ordre",
                schema: "bem",
                table: "ItinerairesPrevu",
                columns: new[] { "BonId", "ScanEffectue", "OrdrePassage" });

            migrationBuilder.CreateIndex(
                name: "IX_ItinerairesPrevu_Scan",
                schema: "bem",
                table: "ItinerairesPrevu",
                column: "ScanEffectue");

            migrationBuilder.CreateIndex(
                name: "IX_Bons_BonSortieLie",
                schema: "bem",
                table: "Bons",
                column: "BonSortieLieId");

            migrationBuilder.CreateIndex(
                name: "IX_Bons_Createur",
                schema: "bem",
                table: "Bons",
                column: "CreateurLogin");

            migrationBuilder.CreateIndex(
                name: "IX_Bons_Ressortie",
                schema: "bem",
                table: "Bons",
                columns: new[] { "DoitRessortir", "ToutRessorti" });

            migrationBuilder.CreateIndex(
                name: "IX_Bons_SiteManager",
                schema: "bem",
                table: "Bons",
                column: "SiteManagerLogin");

            migrationBuilder.CreateIndex(
                name: "IX_Bons_Statut_Type_Date",
                schema: "bem",
                table: "Bons",
                columns: new[] { "StatutActuel", "TypeBon", "DateCreation" });

            migrationBuilder.CreateIndex(
                name: "IX_Bons_Type",
                schema: "bem",
                table: "Bons",
                column: "TypeBon");

            migrationBuilder.CreateIndex(
                name: "IX_Approbations_EtapeCourante",
                schema: "bem",
                table: "Approbations",
                column: "EstEtapeCourante");

            migrationBuilder.CreateIndex(
                name: "IX_Approbations_Utilisateur",
                schema: "bem",
                table: "Approbations",
                column: "UtilisateurLogin");

            migrationBuilder.CreateIndex(
                name: "IX_Approbations_Utilisateur_Decision_Courante",
                schema: "bem",
                table: "Approbations",
                columns: new[] { "UtilisateurLogin", "Decision", "EstEtapeCourante" });

            migrationBuilder.AddForeignKey(
                name: "FK_Materiels_Bons_BonEntreeId",
                schema: "bem",
                table: "Materiels",
                column: "BonEntreeId",
                principalSchema: "bem",
                principalTable: "Bons",
                principalColumn: "IdBon",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
