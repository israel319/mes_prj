namespace KCCMaterialFlow.Application.Common.Interfaces;

/// <summary>
/// Interface pour le service de génération et validation de QR Codes
/// </summary>
public interface IQRCodeService
{
    /// <summary>
    /// Génère un QR Code pour un bon
    /// </summary>
    QRCodeResult GenerateQRCode(int bonId, string bonType, string numeroReference);

    /// <summary>
    /// Génère un QR Code à partir de données personnalisées
    /// </summary>
    string GenerateQRCode(string data);

    /// <summary>
    /// Génère un QR Code de manière asynchrone et retourne (data, base64, hash).
    /// Utilisé par les handlers CQRS.
    /// </summary>
    Task<(string data, string base64, string hash)> GenerateAsync(string data, CancellationToken ct = default);

    /// <summary>
    /// Valide un QR Code scanné et retourne les informations du bon
    /// </summary>
    Task<QRCodeValidationResult> ValidateQRCodeAsync(string scannedCode);

    /// <summary>
    /// Valide un QR Code contre un hash attendu.
    /// Utilisé par les handlers CQRS.
    /// </summary>
    Task<bool> ValidateAsync(string qrCodeData, string expectedHash, CancellationToken ct = default);

    /// <summary>
    /// Génère un hash sécurisé pour le code unique du bon
    /// </summary>
    /// <param name="bonId">Identifiant du bon</param>
    /// <param name="bonType">Type de bon</param>
    /// <param name="numeroReference">Numéro de référence</param>
    /// <returns>Code haché sécurisé</returns>
    string HashCode(int bonId, string bonType, string numeroReference);

    /// <summary>
    /// Vérifie si un hash correspond aux données d'un bon
    /// </summary>
    /// <param name="hash">Hash à vérifier</param>
    /// <param name="bonId">Identifiant du bon</param>
    /// <param name="bonType">Type de bon</param>
    /// <param name="numeroReference">Numéro de référence</param>
    /// <returns>True si le hash est valide</returns>
    bool VerifyHash(string hash, int bonId, string bonType, string numeroReference);

    /// <summary>
    /// Décode un hash pour retrouver les informations du bon
    /// </summary>
    /// <param name="hash">Hash à décoder</param>
    /// <returns>Informations décodées ou null si invalide</returns>
    QRCodeDecodedInfo? DecodeHash(string hash);
}

/// <summary>
/// Résultat de la génération d'un QR Code
/// </summary>
public class QRCodeResult
{
    /// <summary>
    /// Image du QR Code en base64 (data:image/png;base64,...)
    /// </summary>
    public string QRCodeBase64 { get; set; } = string.Empty;

    /// <summary>
    /// Code haché encapsulé dans le QR Code
    /// </summary>
    public string HashedCode { get; set; } = string.Empty;

    /// <summary>
    /// Date de génération du QR Code
    /// </summary>
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Résultat de la validation d'un QR Code
/// </summary>
public class QRCodeValidationResult
{
    /// <summary>
    /// Indique si le QR Code est valide
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Message d'erreur si invalide
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Code d'erreur pour le type d'anomalie
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Identifiant du bon
    /// </summary>
    public int? BonId { get; set; }

    /// <summary>
    /// Type de bon (BEM, BSM)
    /// </summary>
    public string? BonType { get; set; }

    /// <summary>
    /// Numéro de référence du bon
    /// </summary>
    public string? NumeroReference { get; set; }

    /// <summary>
    /// Statut actuel du bon
    /// </summary>
    public string? BonStatus { get; set; }

    /// <summary>
    /// Date d'expiration du bon
    /// </summary>
    public DateTime? DateExpiration { get; set; }

    /// <summary>
    /// Indique si le bon est expiré
    /// </summary>
    public bool IsExpired { get; set; }

    /// <summary>
    /// Informations supplémentaires sur le porteur/demandeur
    /// </summary>
    public string? PorteurNom { get; set; }
    public string? PorteurFonction { get; set; }
    public string? PorteurDepartement { get; set; }

    /// <summary>
    /// Provenance et destination du matériel
    /// </summary>
    public string? Provenance { get; set; }
    public string? Destination { get; set; }
}

/// <summary>
/// Informations décodées d'un hash QR Code
/// </summary>
public class QRCodeDecodedInfo
{
    /// <summary>
    /// Identifiant du bon
    /// </summary>
    public int BonId { get; set; }

    /// <summary>
    /// Type de bon (BEM, BSM)
    /// </summary>
    public string BonType { get; set; } = string.Empty;

    /// <summary>
    /// Numéro de référence
    /// </summary>
    public string NumeroReference { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp de génération
    /// </summary>
    public DateTime GeneratedAt { get; set; }

    /// <summary>
    /// Hash du QR Code (code original hashé)
    /// </summary>
    public string? HashedCode { get; set; }
}
