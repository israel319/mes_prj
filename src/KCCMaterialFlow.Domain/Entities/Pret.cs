using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;
using KCCMaterialFlow.Domain.Enums;
using KCCMaterialFlow.Domain.Errors;
using KCCMaterialFlow.Domain.Events;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Prêt de matériel — sortie temporaire avec date de retour.
/// TPH: inherits BonSortieExterne.
/// </summary>
public sealed class Pret : BonSortieExterne
{
    public DateTime DateRetourPrevue { get; private set; }
    public DateTime? DateRetourEffective { get; private set; }
    public bool EstRetourne { get; private set; }

    [MaxLength(1000)]
    public string? EtatRetour { get; private set; }

    [MaxLength(200)]
    public string? ReceptionnePar { get; private set; }

    public int JoursRetard => CalculerJoursRetard();
    public bool EstEnRetard => !EstRetourne && DateTime.Now > DateRetourPrevue;

    private Pret() { }

    public static Result<Pret> Create(
        string nomDemandeur, string fonctionDemandeur, string departementDemandeur,
        string createdByLogin, string motifSortie, string provenance, string destination,
        DateTime dateExpiration, string nomDestinataire, string? descriptionMateriel,
        DateTime dateRetourPrevue,
        int? bonEntreeAssocieId = null, string? raisonSortieCode = null,
        string? description = null, string? adresseDestination = null)
    {
        if (dateRetourPrevue <= DateTime.Now)
            return Result.Failure<Pret>(BonSortieErrors.DateRetourPassee);
        if (string.IsNullOrWhiteSpace(nomDestinataire))
            return Result.Failure<Pret>(BonSortieErrors.DestinataireSortieRequis);
        if (string.IsNullOrWhiteSpace(provenance) || string.IsNullOrWhiteSpace(destination))
            return Result.Failure<Pret>(BonSortieErrors.ItineraireRequis);

        var pret = new Pret
        {
            DateRetourPrevue = dateRetourPrevue,
            EstRetourne = false
        };

        pret.InitialiserExterne(nomDestinataire, descriptionMateriel, bonEntreeAssocieId, adresseDestination);

        pret.InitialiserBase(nomDemandeur, fonctionDemandeur, departementDemandeur,
            createdByLogin, motifSortie, provenance, destination, dateExpiration,
            raisonSortieCode, description, estDefinitif: false);

        pret.AddDomainEvent(new BonSortieCreatedEvent(pret.Id, "Pret"));
        return pret;
    }

    public Result EnregistrerRetour(string receptionnePar, string? etatRetour = null)
    {
        if (EstRetourne)
            return Result.Failure(BonSortieErrors.PretDejaRetourne);

        EstRetourne = true;
        DateRetourEffective = DateTime.Now;
        ReceptionnePar = receptionnePar;
        EtatRetour = etatRetour;

        AddDomainEvent(new PretRetourneEvent(Id, NumeroReference, DateRetourEffective.Value));
        return Result.Success();
    }

    public Result ProlongerPret(DateTime nouvelleDateRetour)
    {
        if (EstRetourne)
            return Result.Failure(BonSortieErrors.PretDejaRetourne);
        if (nouvelleDateRetour <= DateRetourPrevue)
            return Result.Failure(BonSortieErrors.DateProlongationInvalide);

        DateRetourPrevue = nouvelleDateRetour;
        return Result.Success();
    }

    private int CalculerJoursRetard()
    {
        if (EstRetourne && DateRetourEffective.HasValue)
        {
            var retard = (DateRetourEffective.Value - DateRetourPrevue).Days;
            return retard > 0 ? retard : 0;
        }
        if (!EstRetourne && DateTime.Now > DateRetourPrevue)
            return (DateTime.Now - DateRetourPrevue).Days;
        return 0;
    }
}
