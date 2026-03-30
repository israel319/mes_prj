using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;
using KCCMaterialFlow.Domain.Enums;
using KCCMaterialFlow.Domain.Errors;
using KCCMaterialFlow.Domain.Events;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Bon de Sortie Interne — transfert de matériel entre départements KCC.
/// TPH: inherits BonSortie.
/// </summary>
public sealed class BonSortieInterne : BonSortie
{
    public int? BonEntreeAssocieId { get; private set; }

    public TypeMateriel TypeMateriel { get; private set; } = TypeMateriel.Autre;

    [MaxLength(100)]
    public string? DepartementOrigine { get; private set; }

    [MaxLength(150)]
    public string? FonctionReceveur { get; private set; }

    [MaxLength(200)]
    public string? EmailReceveur { get; private set; }

    [MaxLength(200)]
    public string? LocalisationDestination { get; private set; }

    public DateTime? DateTransfertPrevue { get; private set; }
    public DateTime? DateTransfertEffective { get; private set; }

    private BonSortieInterne() { }

    public static Result<BonSortieInterne> Create(
        string nomDemandeur, string fonctionDemandeur, string departementDemandeur,
        string createdByLogin, string motifSortie, string provenance, string destination,
        DateTime dateExpiration, TypeMateriel typeMateriel,
        int? bonEntreeAssocieId = null, string? raisonSortieCode = null,
        string? description = null, string? departementOrigine = null,
        string? fonctionReceveur = null, string? emailReceveur = null,
        string? localisationDestination = null, DateTime? dateTransfertPrevue = null)
    {
        if (string.IsNullOrWhiteSpace(provenance) || string.IsNullOrWhiteSpace(destination))
            return Result.Failure<BonSortieInterne>(BonSortieErrors.ItineraireRequis);

        var bon = new BonSortieInterne
        {
            BonEntreeAssocieId = bonEntreeAssocieId,
            TypeMateriel = typeMateriel,
            DepartementOrigine = departementOrigine,
            FonctionReceveur = fonctionReceveur,
            EmailReceveur = emailReceveur,
            LocalisationDestination = localisationDestination,
            DateTransfertPrevue = dateTransfertPrevue
        };

        bon.InitialiserBase(nomDemandeur, fonctionDemandeur, departementDemandeur,
            createdByLogin, motifSortie, provenance, destination, dateExpiration,
            raisonSortieCode, description, estDefinitif: true);

        bon.AddDomainEvent(new BonSortieCreatedEvent(bon.Id, "Interne"));
        return bon;
    }

    public void ConfirmerTransfert()
    {
        DateTransfertEffective = DateTime.Now;
    }
}
