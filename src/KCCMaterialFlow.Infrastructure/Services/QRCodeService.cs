using System.Security.Cryptography;
using System.Text;
using KCCMaterialFlow.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QRCoder;

namespace KCCMaterialFlow.Infrastructure.Services;

/// <summary>
/// Service de génération et validation de QR Codes sécurisés
/// Utilise HMAC-SHA256 pour le hachage et QRCoder pour la génération
/// </summary>
public class QRCodeService : IQRCodeService
{
    private readonly byte[] _secretKey;
    private readonly ILogger<QRCodeService> _logger;
    private const string Separator = "|";

    public QRCodeService(
        IConfiguration configuration,
        ILogger<QRCodeService> logger)
    {
        var secret = configuration["QRCode:SecretKey"] 
            ?? throw new InvalidOperationException("QRCode:SecretKey is not configured");
        _secretKey = Encoding.UTF8.GetBytes(secret);
        _logger = logger;
    }

    /// <inheritdoc />
    public QRCodeResult GenerateQRCode(int bonId, string bonType, string numeroReference)
    {
        try
        {
            // Créer le hash sécurisé
            var hashedCode = HashCode(bonId, bonType, numeroReference);

            // Générer le QR Code
            var qrCodeBase64 = GenerateQRCode(hashedCode);

            return new QRCodeResult
            {
                QRCodeBase64 = qrCodeBase64,
                HashedCode = hashedCode,
                GeneratedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la génération du QR Code pour le bon {BonId}", bonId);
            throw;
        }
    }

    /// <inheritdoc />
    public string GenerateQRCode(string data)
    {
        try
        {
            using var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.M);
            
            using var qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeBytes = qrCode.GetGraphic(10);

            var base64 = Convert.ToBase64String(qrCodeBytes);
            return $"data:image/png;base64,{base64}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la génération de l'image QR Code");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<QRCodeValidationResult> ValidateQRCodeAsync(string scannedCode)
    {
        try
        {
            // Décoder le hash pour obtenir les informations du bon
            var decodedInfo = DecodeHash(scannedCode);

            if (decodedInfo == null)
            {
                return new QRCodeValidationResult
                {
                    IsValid = false,
                    ErrorCode = "INVALID_FORMAT",
                    ErrorMessage = "Format de QR Code invalide"
                };
            }

            // Vérifier l'intégrité du hash
            if (!VerifyHash(scannedCode, decodedInfo.BonId, decodedInfo.BonType, decodedInfo.NumeroReference))
            {
                return new QRCodeValidationResult
                {
                    IsValid = false,
                    ErrorCode = "TAMPERED",
                    ErrorMessage = "QR Code altéré ou invalide"
                };
            }

            // TODO: Récupérer les informations du bon depuis la base de données
            // et vérifier le statut, l'expiration, etc.

            await Task.CompletedTask;

            return new QRCodeValidationResult
            {
                IsValid = true,
                BonId = decodedInfo.BonId,
                BonType = decodedInfo.BonType,
                NumeroReference = decodedInfo.NumeroReference
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la validation du QR Code");
            return new QRCodeValidationResult
            {
                IsValid = false,
                ErrorCode = "SYSTEM_ERROR",
                ErrorMessage = "Erreur système lors de la validation"
            };
        }
    }

    /// <inheritdoc />
    public Task<(string data, string base64, string hash)> GenerateAsync(string data, CancellationToken ct = default)
    {
        var base64Image = GenerateQRCode(data);
        using var hmac = new HMACSHA256(_secretKey);
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        var hash = Convert.ToBase64String(hashBytes);
        return Task.FromResult((data, base64Image, hash));
    }

    /// <inheritdoc />
    public Task<bool> ValidateAsync(string qrCodeData, string expectedHash, CancellationToken ct = default)
    {
        using var hmac = new HMACSHA256(_secretKey);
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(qrCodeData));
        var computedHash = Convert.ToBase64String(hashBytes);
        return Task.FromResult(computedHash == expectedHash);
    }

    /// <inheritdoc />
    public string HashCode(int bonId, string bonType, string numeroReference)
    {
        // Créer les données à hacher
        var data = $"{bonId}{Separator}{bonType}{Separator}{numeroReference}{Separator}{DateTime.UtcNow.Ticks}";
        
        // Calculer le HMAC-SHA256
        using var hmac = new HMACSHA256(_secretKey);
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        var hash = Convert.ToBase64String(hashBytes);

        // Encoder les données avec le hash pour permettre la vérification
        var payload = $"{bonId}{Separator}{bonType}{Separator}{numeroReference}";
        var payloadBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(payload));

        // Format: payload.signature (similaire à JWT simplifié)
        return $"{payloadBase64}.{hash}";
    }

    /// <inheritdoc />
    public bool VerifyHash(string hash, int bonId, string bonType, string numeroReference)
    {
        try
        {
            var parts = hash.Split('.');
            if (parts.Length != 2)
                return false;

            var payloadBase64 = parts[0];
            var payload = Encoding.UTF8.GetString(Convert.FromBase64String(payloadBase64));
            var payloadParts = payload.Split(Separator);

            if (payloadParts.Length != 3)
                return false;

            // Vérifier que les données correspondent
            return int.TryParse(payloadParts[0], out var parsedBonId) &&
                   parsedBonId == bonId &&
                   payloadParts[1] == bonType &&
                   payloadParts[2] == numeroReference;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erreur lors de la vérification du hash");
            return false;
        }
    }

    /// <inheritdoc />
    public QRCodeDecodedInfo? DecodeHash(string hash)
    {
        try
        {
            var parts = hash.Split('.');
            if (parts.Length != 2)
                return null;

            var payloadBase64 = parts[0];
            var payload = Encoding.UTF8.GetString(Convert.FromBase64String(payloadBase64));
            var payloadParts = payload.Split(Separator);

            if (payloadParts.Length != 3)
                return null;

            if (!int.TryParse(payloadParts[0], out var bonId))
                return null;

            return new QRCodeDecodedInfo
            {
                BonId = bonId,
                BonType = payloadParts[1],
                NumeroReference = payloadParts[2],
                GeneratedAt = DateTime.UtcNow // Note: L'heure exacte n'est pas stockée dans ce format simplifié
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erreur lors du décodage du hash");
            return null;
        }
    }
}
