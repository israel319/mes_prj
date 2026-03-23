namespace KCCMaterialFlow.Domain.Enums;

/// <summary>
/// Statuts possibles d'un bon dans le workflow
/// </summary>
public enum BonStatut
{
    /// <summary>
    /// Brouillon - Saisie initiale, pas encore soumis
    /// </summary>
    Draft = 0,

    /// <summary>
    /// En attente de validation par le Superviseur
    /// </summary>
    PendingSup = 10,

    /// <summary>
    /// En attente de validation par le General Manager (GM)
    /// </summary>
    PendingGM = 20,

    /// <summary>
    /// En attente de validation par le Département IT (matériel informatique)
    /// </summary>
    PendingIT = 25,

    /// <summary>
    /// En attente de validation par le Département Environnement
    /// </summary>
    PendingEnv = 26,

    /// <summary>
    /// En attente de validation par l'OPJ (Sécurité)
    /// </summary>
    PendingOPJ = 30,

    /// <summary>
    /// En attente de vérification par l'Équipe Identification
    /// </summary>
    PendingIdentification = 40,

    /// <summary>
    /// Approuvé - QR Code généré, prêt pour impression
    /// </summary>
    Approved = 50,

    /// <summary>
    /// En transit - Matériel en mouvement entre barrières
    /// </summary>
    InTransit = 60,

    /// <summary>
    /// Complété - Passage confirmé à toutes les barrières
    /// </summary>
    Completed = 70,

    /// <summary>
    /// Archivé - Bon clôturé (entrée liée à une sortie complétée)
    /// </summary>
    Archived = 80,

    /// <summary>
    /// Rejeté - Refusé à une étape du workflow
    /// </summary>
    Rejected = 90,

    /// <summary>
    /// Retourné - Renvoyé au demandeur pour modification
    /// </summary>
    Returned = 95,

    /// <summary>
    /// Investigation - Anomalie détectée, en cours d'investigation
    /// </summary>
    Investigation = 100
}
