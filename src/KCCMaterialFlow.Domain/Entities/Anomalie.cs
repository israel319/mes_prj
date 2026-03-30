using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;
using KCCMaterialFlow.Domain.Errors;
using KCCMaterialFlow.Domain.Events;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Anomalie signalee lors d'un scan ou constatee sur un bon.
/// EF maps Id -> IdAnomalie column.
/// </summary>
public sealed class Anomalie : BaseEntity
{
    [MaxLength(50)]
    public string TypeAnomalie { get; set; } = string.Empty;

    [MaxLength(20)]
    public string NiveauGravite { get; set; } = "Moyen";

    public DateTime DateSignalement { get; set; } = DateTime.Now;

    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    public int? BonId { get; set; }

    [MaxLength(10)]
    public string? TypeBon { get; set; }

    [MaxLength(30)]
    public string? NumeroReferenceBon { get; set; }

    public int? ScanId { get; set; }
    public int? BarriereId { get; set; }

    // Navigation properties
    public ScanEvenement? ScanEvenement { get; set; }
    public Barriere? Barriere { get; set; }

    public bool EstTraitee { get; set; }
    public DateTime? DateTraitement { get; set; }

    [MaxLength(100)]
    public string SignalePar { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? SignaleParNom { get; set; }

    [MaxLength(100)]
    public string? TraitePar { get; set; }

    [MaxLength(2000)]
    public string? Resolution { get; set; }

    [MaxLength(2000)]
    public string? ActionsCorrectives { get; set; }

    private Anomalie() { }

    public Anomalie(
        string typeAnomalie, string description, string signalePar,
        string niveauGravite = "Moyen", int? bonId = null, string? typeBon = null,
        string? numeroReferenceBon = null, int? scanId = null,
        int? barriereId = null, string? signaleParNom = null)
    {
        TypeAnomalie = typeAnomalie;
        Description = description;
        SignalePar = signalePar;
        NiveauGravite = niveauGravite;
        BonId = bonId;
        TypeBon = typeBon;
        NumeroReferenceBon = numeroReferenceBon;
        ScanId = scanId;
        BarriereId = barriereId;
        SignaleParNom = signaleParNom;

        AddDomainEvent(new AnomalieSignaleeEvent(Id, scanId, description));
    }

    public Result Traiter(string traitePar, string resolution, string? actionsCorrectives = null)
    {
        if (EstTraitee)
            return Result.Failure(SecuriteErrors.AnomalieDejaTraitee(Id));
        if (string.IsNullOrWhiteSpace(resolution))
            return Result.Failure(SecuriteErrors.ActionRequise);

        EstTraitee = true;
        DateTraitement = DateTime.Now;
        TraitePar = traitePar;
        Resolution = resolution;
        ActionsCorrectives = actionsCorrectives;

        AddDomainEvent(new AnomalieTraiteeEvent(Id, traitePar, resolution));
        return Result.Success();
    }
}

public static class TypeAnomalieValues
{
    public const string QRCodeInvalide = "QRCodeInvalide";
    public const string QRCodeExpire = "QRCodeExpire";
    public const string QRCodeDejaScan = "QRCodeDejaScan";
    public const string BonNonTrouve = "BonNonTrouve";
    public const string BonExpire = "BonExpire";
    public const string BonAnnule = "BonAnnule";
    public const string MaterielManquant = "MaterielManquant";
    public const string MaterielExcedentaire = "MaterielExcedentaire";
    public const string MaterielNonConforme = "MaterielNonConforme";
    public const string DocumentManquant = "DocumentManquant";
    public const string AutorisationManquante = "AutorisationManquante";
    public const string BarriereNonAutorisee = "BarriereNonAutorisee";
    public const string HorsHoraires = "HorsHoraires";
    public const string Autre = "Autre";
    public const string DocumentExpire = "QRCodeExpire";
    public const string DocumentInexistant = "BonNonTrouve";
    public const string ScanDuplique = "QRCodeDejaScan";
    public const string ItineraireNonRespectee = "BarriereNonAutorisee";
    public const string SignalementManuel = "Autre";
}

public static class NiveauGraviteValues
{
    public const string Faible = "Faible";
    public const string Moyen = "Moyen";
    public const string Eleve = "Eleve";
    public const string Critique = "Critique";
}
