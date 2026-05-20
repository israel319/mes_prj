using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;
using KCCMaterialFlow.Domain.Enums;
using KCCMaterialFlow.Domain.Errors;
using KCCMaterialFlow.Domain.Events;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Bon de Sortie Externe — matériel sortant du site KCC.
/// Peut être lié à un BonEntree existant. TPH: inherits BonSortie.
/// </summary>
public class BonSortieExterne : BonSortie
{
    public int? BonEntreeAssocieId { get; private set; }

    [MaxLength(200)]
    public string? DescriptionMateriel { get; private set; }

    [MaxLength(200)]
    public string NomDestinataire { get; private set; } = string.Empty;

    [MaxLength(500)]
    public string? AdresseDestination { get; private set; }

    [MaxLength(50)]
    public string? NumeroVehicule { get; private set; }

    [MaxLength(200)]
    public string? NomChauffeur { get; private set; }

    [MaxLength(50)]
    public string? TelephoneChauffeur { get; private set; }

    protected BonSortieExterne() { }

    public static Result<BonSortieExterne> Create(
        string nomDemandeur, string fonctionDemandeur, string departementDemandeur,
        string createdByLogin, string motifSortie, string provenance, string destination,
        DateTime dateExpiration, string nomDestinataire, string? descriptionMateriel = null,
        int? bonEntreeAssocieId = null, string? raisonSortieCode = null,
        string? description = null, string? adresseDestination = null,
        string? numeroVehicule = null, string? nomChauffeur = null,
        string? telephoneChauffeur = null)
    {
        if (string.IsNullOrWhiteSpace(nomDestinataire))
            return Result.Failure<BonSortieExterne>(BonSortieErrors.DestinataireSortieRequis);
        if (string.IsNullOrWhiteSpace(provenance) || string.IsNullOrWhiteSpace(destination))
            return Result.Failure<BonSortieExterne>(BonSortieErrors.ItineraireRequis);

        var bon = new BonSortieExterne();

        bon.InitialiserExterne(nomDestinataire, descriptionMateriel, bonEntreeAssocieId,
            adresseDestination, numeroVehicule, nomChauffeur, telephoneChauffeur);

        bon.InitialiserBase(nomDemandeur, fonctionDemandeur, departementDemandeur,
            createdByLogin, motifSortie, provenance, destination, dateExpiration,
            raisonSortieCode, description, estDefinitif: true);

        bon.AddDomainEvent(new BonSortieCreatedEvent(bon.Id, "Externe"));
        return bon;
    }

    public void SetBonEntreeAssocie(int bonEntreeId)
    {
        BonEntreeAssocieId = bonEntreeId;
    }

    /// <summary>
    /// Initialise les champs spécifiques à BonSortieExterne.
    /// Utilisé par le factory Create et par les sous-classes (Pret).
    /// </summary>
    protected void InitialiserExterne(
        string nomDestinataire, string? descriptionMateriel,
        int? bonEntreeAssocieId = null, string? adresseDestination = null,
        string? numeroVehicule = null, string? nomChauffeur = null,
        string? telephoneChauffeur = null)
    {
        NomDestinataire = nomDestinataire;
        DescriptionMateriel = descriptionMateriel;
        BonEntreeAssocieId = bonEntreeAssocieId;
        AdresseDestination = adresseDestination;
        NumeroVehicule = numeroVehicule;
        NomChauffeur = nomChauffeur;
        TelephoneChauffeur = telephoneChauffeur;
    }
}
