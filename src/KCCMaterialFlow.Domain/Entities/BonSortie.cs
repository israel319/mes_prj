using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;
using KCCMaterialFlow.Domain.Enums;
using KCCMaterialFlow.Domain.Errors;
using KCCMaterialFlow.Domain.Events;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// BonSortie — Abstract Aggregate Root (Rich Domain Model).
/// Base class for all material exit types (TPH: BonSortie → BonSortieExterne → Pret, BonSortie → BonSortieInterne).
/// EF maps Id → IdBon column.
/// </summary>
public abstract class BonSortie : BaseAuditableEntity
{
    [MaxLength(20)]
    public string NumeroReference { get; set; } = string.Empty;

    public DateTime DateCreation { get; set; } = DateTime.Now;
    public DateTime DateExpiration { get; set; }

    [MaxLength(50)]
    public string StatutActuel { get; set; } = "Draft";
    public StatutBonSortie Statut { get; set; } = StatutBonSortie.Draft;

    [MaxLength(200)]
    public string Destination { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Provenance { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    public int Quantite { get; set; }

    [MaxLength(200)]
    public string NomDemandeur { get; set; } = string.Empty;

    [MaxLength(150)]
    public string FonctionDemandeur { get; set; } = string.Empty;

    [MaxLength(100)]
    public string DepartementDemandeur { get; set; } = string.Empty;

    [MaxLength(100)]
    public string CreatedByLogin { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string MotifSortie { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? RaisonSortieCode { get; set; }

    public bool EstDefinitif { get; set; } = true;

    // ── QR Code ──────────────────────────────────────────────────
    [MaxLength(500)]
    public string? QRCodeData { get; set; }
    public string? QRCodeBase64 { get; set; }
    [MaxLength(128)]
    public string? QRCodeHash { get; set; }
    public DateTime? DateGenerationQR { get; set; }

    // ── Collections encapsulées ──────────────────────────────────
    private readonly List<MaterielSortie> _materiels = [];
    public IReadOnlyCollection<MaterielSortie> Materiels => _materiels.AsReadOnly();

    private readonly List<ApprobationSortie> _approbations = [];
    public IReadOnlyCollection<ApprobationSortie> Approbations => _approbations.AsReadOnly();

    private readonly List<ItineraireSortie> _itineraires = [];
    public IReadOnlyCollection<ItineraireSortie> Itineraires => _itineraires.AsReadOnly();

    private readonly List<BonSortieHistory> _historiques = [];
    public IReadOnlyCollection<BonSortieHistory> Historiques => _historiques.AsReadOnly();

    // ── Constructeur protégé (EF Core + héritage) ────────────────
    protected BonSortie() { }

    // ── Initialisation commune ────────────────────────────────────
    protected void InitialiserBase(
        string nomDemandeur, string fonctionDemandeur, string departementDemandeur,
        string createdByLogin, string motifSortie, string provenance,
        string destination, DateTime dateExpiration,
        string? raisonSortieCode = null, string? description = null,
        bool estDefinitif = true)
    {
        NomDemandeur = nomDemandeur;
        FonctionDemandeur = fonctionDemandeur;
        DepartementDemandeur = departementDemandeur;
        CreatedByLogin = createdByLogin;
        MotifSortie = motifSortie;
        Provenance = provenance;
        Destination = destination;
        DateExpiration = dateExpiration;
        RaisonSortieCode = raisonSortieCode;
        Description = description;
        EstDefinitif = estDefinitif;
    }

    // ── Comportements métier ─────────────────────────────────────
    public Result AjouterMateriel(MaterielSortie materiel)
    {
        if (!EstModifiable())
            return Result.Failure(BonSortieErrors.ModificationInterdite(Statut));
        _materiels.Add(materiel);
        RecalculerQuantite();
        return Result.Success();
    }

    public Result SupprimerMateriel(int materielId)
    {
        if (!EstModifiable())
            return Result.Failure(BonSortieErrors.ModificationInterdite(Statut));
        var mat = _materiels.FirstOrDefault(m => m.Id == materielId);
        if (mat is null) return Result.Failure(Error.NotFound("MaterielSortie", materielId));
        _materiels.Remove(mat);
        RecalculerQuantite();
        return Result.Success();
    }

    public Result SoumettrePourApprobation()
    {
        if (Statut is not (StatutBonSortie.Draft or StatutBonSortie.Returned))
            return Result.Failure(BonSortieErrors.SoumissionImpossible(Statut));
        if (_materiels.Count == 0)
            return Result.Failure(BonSortieErrors.AuMoinsUnMateriel);

        SetStatut(StatutBonSortie.PendingSup);
        AddDomainEvent(new BonSortieSubmittedEvent(Id, NumeroReference));
        return Result.Success();
    }

    public Result Approuver(string login, string nom, string? commentaire, StatutBonSortie prochainStatut)
    {
        if (!EstEnAttenteApprobation())
            return Result.Failure(BonSortieErrors.ApprobationImpossible(Statut));
        SetStatut(prochainStatut);
        return Result.Success();
    }

    public Result Rejeter(string login, string nom, string motif)
    {
        if (!EstEnAttenteApprobation())
            return Result.Failure(BonSortieErrors.ApprobationImpossible(Statut));
        if (string.IsNullOrWhiteSpace(motif))
            return Result.Failure(BonSortieErrors.MotifRejetRequis);
        SetStatut(StatutBonSortie.Rejected);
        AddDomainEvent(new BonSortieRejectedEvent(Id, NumeroReference, login, motif));
        return Result.Success();
    }

    public Result RetournerPourModification(string login, string motif)
    {
        if (!EstEnAttenteApprobation())
            return Result.Failure(BonSortieErrors.ApprobationImpossible(Statut));
        SetStatut(StatutBonSortie.Returned);
        return Result.Success();
    }

    public Result MettreAJour(
        string motifSortie, string provenance, string destination,
        DateTime dateExpiration, string? description = null)
    {
        if (!EstModifiable())
            return Result.Failure(BonSortieErrors.ModificationInterdite(Statut));
        MotifSortie = motifSortie;
        Provenance = provenance;
        Destination = destination;
        DateExpiration = dateExpiration;
        Description = description;
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

    // ── Queries d'état ───────────────────────────────────────────
    public bool EstModifiable()
        => Statut is StatutBonSortie.Draft or StatutBonSortie.Returned;

    public bool EstEnAttenteApprobation()
        => Statut is StatutBonSortie.PendingSup or StatutBonSortie.PendingGM
            or StatutBonSortie.PendingIT or StatutBonSortie.PendingEnv
            or StatutBonSortie.PendingOPJ;

    protected void SetStatut(StatutBonSortie statut)
    {
        Statut = statut;
        StatutActuel = statut.ToString();
    }

    private void RecalculerQuantite()
        => Quantite = (int)_materiels.Sum(m => m.Quantite);
}
