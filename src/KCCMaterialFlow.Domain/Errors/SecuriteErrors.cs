using KCCMaterialFlow.Domain.Errors;

namespace KCCMaterialFlow.Domain.Errors;

/// <summary>
/// Catalogue des erreurs métier Sécurité.
/// </summary>
public static class SecuriteErrors
{
    public static readonly Error QRCodeInvalide =
        Error.Validation("Securite.QRCodeInvalide", "Le QR code scanné est invalide ou expiré.");

    public static readonly Error BarriereRequise =
        Error.Validation("Securite.BarriereRequise", "La barrière est obligatoire pour le scan.");

    public static readonly Error AgentNonAffecte =
        Error.Validation("Securite.AgentNonAffecte", "L'agent n'est pas affecté à cette barrière.");

    public static readonly Error MotifAnomalieRequis =
        Error.Validation("Securite.MotifAnomalieRequis", "Le motif de l'anomalie est obligatoire.");

    public static readonly Error ActionRequise =
        Error.Validation("Securite.ActionRequise", "L'action corrective est obligatoire.");

    public static Error AnomalieDejaTraitee(int id) =>
        Error.Conflict("Securite.AnomalieDejaTraitee", $"L'anomalie #{id} a déjà été traitée.");

    public static Error ScanDuplique(string qrCode, string barriere) =>
        Error.Conflict("Securite.ScanDuplique", $"Le QR code '{qrCode}' a déjà été scanné à la barrière '{barriere}'.");
}
