using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;
using KCCMaterialFlow.Domain.Enums;
using KCCMaterialFlow.Domain.Errors;
using KCCMaterialFlow.Domain.Events;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// BonEntree — Aggregate Root (Rich Domain Model).
/// PK: IdBon (mapped via EF config). Inherits BaseAuditableEntity.Id for domain events.
/// </summary>
public sealed class BonEntree : BaseAuditableEntity
{
    // ── Identité ───────────────────────────────────────────────────
    [MaxLength(20)]
    public string NumeroReference { get; set; } = string.Empty;

    // ── État ──────────────────────────────────────────────────────
    [MaxLength(50)]
    public string StatutActuel { get; set; } = "Draft";
    public StatutBonEntree Statut { get; set; } = StatutBonEntree.Draft;

    // ── Itinéraire ───────────────────────────────────────────────
    [MaxLength(200)]
    public string Provenance { get; set; } = string.Empty;
    [MaxLength(200)]
    public string Destination { get; set; } = string.Empty;
    [MaxLength(1000)]
    public string? Description { get; set; }
    public DateTime DateExpiration { get; set; }
    public DateTime DateCreation { get; set; } = DateTime.Now;
    public int Quantite { get; set; }

    // ── QR Code ──────────────────────────────────────────────────
    [MaxLength(500)]
    public string? QRCodeData { get; set; }
    public string? QRCodeBase64 { get; set; }
    [MaxLength(128)]
    public string? QRCodeHash { get; set; }
    public DateTime? DateGenerationQR { get; set; }

    // ── Demandeur ────────────────────────────────────────────────
    [MaxLength(200)]
    public string NomDemandeur { get; set; } = string.Empty;
    [MaxLength(200)]
    public string NomCompagnie { get; set; } = string.Empty;
    [MaxLength(200)]
    public string? EmailContractant { get; set; }
    [MaxLength(200)]
    public string SiteManager { get; set; } = string.Empty;
    [MaxLength(100)]
    public string HostDepartment { get; set; } = string.Empty;
    [MaxLength(1000)]
    public string ReasonOnSite { get; set; } = string.Empty;
    [MaxLength(200)]
    public string NomEscorteur { get; set; } = string.Empty;
    [MaxLength(150)]
    public string? FonctionEscorteur { get; set; }
    public int? ContratId { get; set; }
    [MaxLength(50)]
    public string? NumeroContrat { get; set; }

    // ── Liaison Sortie ───────────────────────────────────────────
    public bool EstVerrouillePourSortie { get; set; }
    public DateTime? DateVerrouillage { get; set; }
    public int? BonSortieAssocieId { get; set; }
    [MaxLength(20)]
    public string? BonSortieAssocieNumero { get; set; }

    // ── Collections encapsulées ──────────────────────────────────
    private readonly List<Materiel> _materiels = [];
    public IReadOnlyCollection<Materiel> Materiels => _materiels.AsReadOnly();

    private readonly List<Approbation> _approbations = [];
    public IReadOnlyCollection<Approbation> Approbations => _approbations.AsReadOnly();

    private readonly List<ItinerairePrevu> _itinerairesPrevu = [];
    public IReadOnlyCollection<ItinerairePrevu> ItinerairesPrevu => _itinerairesPrevu.AsReadOnly();

    private readonly List<BonEntreeHistory> _historiques = [];
    public IReadOnlyCollection<BonEntreeHistory> Historiques => _historiques.AsReadOnly();

    // ── Constructeur privé (EF Core) ─────────────────────────────
    private BonEntree() { }

    // ════════════════════════════════════════════════════════════════
    // FACTORY METHOD
    // ════════════════════════════════════════════════════════════════

    public static Result<BonEntree> Create(
        string nomDemandeur,
        string nomCompagnie,
        string siteManager,
        string hostDepartment,
        string reasonOnSite,
        string nomEscorteur,
        string provenance,
        string destination,
        DateTime? dateExpiration = null,
        string? description = null,
        string? emailContractant = null,
        string? fonctionEscorteur = null,
        int? contratId = null,
        string? numeroContrat = null)
    {
        if (string.IsNullOrWhiteSpace(nomCompagnie))
            return Result.Failure<BonEntree>(BonEntreeErrors.CompagnieRequise);
        if (string.IsNullOrWhiteSpace(siteManager))
            return Result.Failure<BonEntree>(BonEntreeErrors.SiteManagerRequis);
        if (string.IsNullOrWhiteSpace(provenance) || string.IsNullOrWhiteSpace(destination))
            return Result.Failure<BonEntree>(BonEntreeErrors.ItineraireRequis);
        if (provenance == destination)
            return Result.Failure<BonEntree>(BonEntreeErrors.ProvenanceEgaleDestination);

        var bon = new BonEntree
        {
            Statut = StatutBonEntree.Draft,
            StatutActuel = "Draft",
            NomDemandeur = nomDemandeur,
            NomCompagnie = nomCompagnie,
            SiteManager = siteManager,
            HostDepartment = hostDepartment,
            ReasonOnSite = reasonOnSite,
            NomEscorteur = nomEscorteur,
            Provenance = provenance,
            Destination = destination,
            DateExpiration = dateExpiration ?? DateTime.Now.AddDays(7),
            Description = description,
            EmailContractant = emailContractant,
            FonctionEscorteur = fonctionEscorteur,
            ContratId = contratId,
            NumeroContrat = numeroContrat
        };

        bon.AddDomainEvent(new BonEntreeCreatedEvent(bon.Id));
        return bon;
    }

    // ════════════════════════════════════════════════════════════════
    // COMPORTEMENTS METIER
    // ════════════════════════════════════════════════════════════════

    public Result AjouterMateriel(string codeProduit, string designation, decimal quantite)
    {
        if (!EstModifiable())
            return Result.Failure(BonEntreeErrors.ModificationInterdite(Statut));
        if (quantite <= 0)
            return Result.Failure(BonEntreeErrors.QuantiteInvalide);
        if (_materiels.Any(m => m.CodeProduitSerial == codeProduit))
            return Result.Failure(BonEntreeErrors.MaterielDuplique(codeProduit));

        _materiels.Add(new Materiel(codeProduit, designation, quantite));
        RecalculerQuantite();
        return Result.Success();
    }

    public Result SupprimerMateriel(int materielId)
    {
        if (!EstModifiable())
            return Result.Failure(BonEntreeErrors.ModificationInterdite(Statut));

        var mat = _materiels.FirstOrDefault(m => m.Id == materielId);
        if (mat is null) return Result.Failure(Error.NotFound("Materiel", materielId));

        _materiels.Remove(mat);
        RecalculerQuantite();
        return Result.Success();
    }

    public Result SoumettrePourApprobation()
    {
        if (Statut is not (StatutBonEntree.Draft or StatutBonEntree.Returned))
            return Result.Failure(BonEntreeErrors.SoumissionImpossible(Statut));
        if (_materiels.Count == 0)
            return Result.Failure(BonEntreeErrors.AuMoinsUnMateriel);
        if (DateExpiration <= DateTime.Now)
            return Result.Failure(BonEntreeErrors.DateExpirationPassee);

        SetStatut(StatutBonEntree.PendingSup);
        AddDomainEvent(new BonEntreeSubmittedEvent(Id, NumeroReference));
        return Result.Success();
    }

    public Result Approuver(string login, string nom, string? commentaire, StatutBonEntree prochainStatut)
    {
        if (!EstEnAttenteApprobation())
            return Result.Failure(BonEntreeErrors.ApprobationImpossible(Statut));

        var ancien = Statut;
        SetStatut(prochainStatut);

        AddDomainEvent(new BonEntreeApprovedEvent(Id, NumeroReference, login, ancien, Statut));
        return Result.Success();
    }

    public Result Rejeter(string login, string nom, string motif)
    {
        if (!EstEnAttenteApprobation())
            return Result.Failure(BonEntreeErrors.ApprobationImpossible(Statut));
        if (string.IsNullOrWhiteSpace(motif))
            return Result.Failure(BonEntreeErrors.MotifRejetRequis);

        SetStatut(StatutBonEntree.Rejected);

        AddDomainEvent(new BonEntreeRejectedEvent(Id, NumeroReference, login, motif));
        return Result.Success();
    }

    public Result RetournerPourModification(string login, string nom, string motif)
    {
        if (!EstEnAttenteApprobation())
            return Result.Failure(BonEntreeErrors.ApprobationImpossible(Statut));
        if (string.IsNullOrWhiteSpace(motif))
            return Result.Failure(BonEntreeErrors.MotifRetourRequis);

        SetStatut(StatutBonEntree.Returned);

        AddDomainEvent(new BonEntreeReturnedEvent(Id, NumeroReference, login, motif));
        return Result.Success();
    }

    public Result MettreAJour(
        string nomCompagnie, string siteManager, string hostDepartment,
        string reasonOnSite, string nomEscorteur, string provenance,
        string destination, DateTime dateExpiration, string? description = null)
    {
        if (!EstModifiable())
            return Result.Failure(BonEntreeErrors.ModificationInterdite(Statut));
        if (provenance == destination)
            return Result.Failure(BonEntreeErrors.ProvenanceEgaleDestination);

        NomCompagnie = nomCompagnie;
        SiteManager = siteManager;
        HostDepartment = hostDepartment;
        ReasonOnSite = reasonOnSite;
        NomEscorteur = nomEscorteur;
        Provenance = provenance;
        Destination = destination;
        DateExpiration = dateExpiration;
        Description = description;
        return Result.Success();
    }

    public Result VerrouillerPourSortie(int bonSortieId, string bonSortieNumero)
    {
        if (Statut is not (StatutBonEntree.Approved or StatutBonEntree.InTransit))
            return Result.Failure(BonEntreeErrors.ModificationInterdite(Statut));
        if (EstVerrouillePourSortie)
            return Result.Failure(BonEntreeErrors.DejaVerrouille(BonSortieAssocieNumero!));

        EstVerrouillePourSortie = true;
        DateVerrouillage = DateTime.UtcNow;
        BonSortieAssocieId = bonSortieId;
        BonSortieAssocieNumero = bonSortieNumero;

        AddDomainEvent(new BonEntreeLockedEvent(Id, bonSortieId));
        return Result.Success();
    }

    public void SetQRCode(string data, string base64, string hash)
    {
        QRCodeData = data;
        QRCodeBase64 = base64;
        QRCodeHash = hash;
        DateGenerationQR = DateTime.Now;
    }

    public void SetNumeroReference(string numero) => NumeroReference = numero;

    // ════════════════════════════════════════════════════════════════
    // QUERIES D'ETAT
    // ════════════════════════════════════════════════════════════════

    public bool EstModifiable()
        => Statut is StatutBonEntree.Draft or StatutBonEntree.Returned;

    public bool EstEnAttenteApprobation()
        => Statut is StatutBonEntree.PendingSup or StatutBonEntree.PendingGM
            or StatutBonEntree.PendingIT or StatutBonEntree.PendingEnv
            or StatutBonEntree.PendingOPJ;

    public bool EstExpire()
        => DateExpiration <= DateTime.Now && Statut != StatutBonEntree.Completed;

    private void RecalculerQuantite()
        => Quantite = (int)_materiels.Sum(m => m.Quantite);

    private void SetStatut(StatutBonEntree statut)
    {
        Statut = statut;
        StatutActuel = statut.ToString();
    }
}
