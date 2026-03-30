using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Evenement de scan QR effectue a une barriere.
/// EF maps Id -> IdScan column.
/// </summary>
public sealed class ScanEvenement : BaseEntity
{
    public DateTime DateHeureScan { get; set; } = DateTime.Now;

    [MaxLength(30)]
    public string StatutScan { get; set; } = "Valid";

    [MaxLength(20)]
    public string TypeMouvement { get; set; } = "Sortie";

    public int? BonId { get; set; }

    [MaxLength(10)]
    public string? TypeBon { get; set; }

    [MaxLength(30)]
    public string? NumeroReferenceBon { get; set; }

    public int BarriereId { get; set; }

    [MaxLength(100)]
    public string AgentLogin { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? AgentNom { get; set; }

    [MaxLength(1000)]
    public string? QRCodeData { get; set; }

    [MaxLength(128)]
    public string? QRCodeHash { get; set; }

    [MaxLength(500)]
    public string? Message { get; set; }

    [MaxLength(1000)]
    public string? Observations { get; set; }

    public bool AnomalieSignalee { get; set; }

    // Navigation property
    public Barriere? Barriere { get; set; }

    [MaxLength(50)]
    public string? NumeroPreuve { get; set; }

    [MaxLength(200)]
    public string? ProvenanceLieu { get; set; }

    [MaxLength(200)]
    public string? DestinationLieu { get; set; }

    private readonly List<Anomalie> _anomalies = [];
    public IReadOnlyCollection<Anomalie> Anomalies => _anomalies.AsReadOnly();

    private ScanEvenement() { }

    public ScanEvenement(
        int barriereId, string agentLogin, string typeMouvement,
        string? qrCodeData = null, string? qrCodeHash = null,
        int? bonId = null, string? typeBon = null,
        string? numeroReferenceBon = null, string? agentNom = null,
        string? provenanceLieu = null, string? destinationLieu = null)
    {
        BarriereId = barriereId;
        AgentLogin = agentLogin;
        TypeMouvement = typeMouvement;
        QRCodeData = qrCodeData;
        QRCodeHash = qrCodeHash;
        BonId = bonId;
        TypeBon = typeBon;
        NumeroReferenceBon = numeroReferenceBon;
        AgentNom = agentNom;
        ProvenanceLieu = provenanceLieu;
        DestinationLieu = destinationLieu;
    }

    public void MarquerValide(string? message = null)
    {
        StatutScan = StatutScanValues.Valid;
        Message = message;
    }

    public void MarquerInvalide(string message)
    {
        StatutScan = StatutScanValues.Invalid;
        Message = message;
    }

    public void MarquerAnomalie()
    {
        AnomalieSignalee = true;
    }

    public void SetNumeroPreuve(string numero)
    {
        NumeroPreuve = numero;
    }

    public void SetObservations(string observations)
    {
        Observations = observations;
    }
}

public static class StatutScanValues
{
    public const string Valid = "Valid";
    public const string Invalid = "Invalid";
    public const string Expired = "Expired";
    public const string AlreadyUsed = "AlreadyUsed";
    public const string NotFound = "NotFound";
    public const string Unauthorized = "Unauthorized";
}

public static class TypeMouvementValues
{
    public const string Entree = "Entree";
    public const string Sortie = "Sortie";
}
