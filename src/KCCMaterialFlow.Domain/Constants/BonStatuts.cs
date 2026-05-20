namespace KCCMaterialFlow.Domain.Constants;

/// <summary>
/// Constantes pour les statuts des bons (évite les magic strings)
/// </summary>
public static class BonStatuts
{
    public const string Draft = "Draft";
    public const string PendingSup = "PendingSup";
    public const string PendingGM = "PendingGM";
    public const string PendingOPJ = "PendingOPJ";
    public const string Approved = "Approved";
    public const string Rejected = "Rejected";
    public const string InTransit = "InTransit";
    public const string Completed = "Completed";
    public const string Cancelled = "Cancelled";

    /// <summary>
    /// Libellé français du statut
    /// </summary>
    public static string GetLabel(string statut) => statut switch
    {
        Draft => "Brouillon",
        PendingSup => "Chez le Manager",
        PendingGM => "Att. GM",
        PendingOPJ => "Att. OPJ",
        Approved => "Approuvé",
        Rejected => "Rejeté",
        InTransit => "En transit",
        Completed => "Terminé",
        Cancelled => "Annulé",
        _ => statut
    };

    /// <summary>
    /// Vérifie si le statut est en attente d'approbation
    /// </summary>
    public static bool IsPending(string statut) => 
        statut is PendingSup or PendingGM or PendingOPJ;

    /// <summary>
    /// Vérifie si le bon peut être modifié
    /// </summary>
    public static bool CanEdit(string statut) => statut == Draft;

    /// <summary>
    /// Vérifie si le bon peut être annulé
    /// </summary>
    public static bool CanCancel(string statut) => 
        statut is Draft or PendingSup or PendingGM or PendingOPJ;
}

/// <summary>
/// Constantes pour les décisions d'approbation
/// </summary>
public static class ApprobationDecisions
{
    public const string EnAttente = "En attente";
    public const string Approuve = "Approuvé";
    public const string Rejete = "Rejeté";
}
