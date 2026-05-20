using KCCMaterialFlow.Domain.Enums;

namespace KCCMaterialFlow.Domain.Errors;

/// <summary>
/// Catalogue des erreurs métier BonSortie.
/// </summary>
public static class BonSortieErrors
{
    public static readonly Error BonEntreeRequis =
        Error.Validation("BonSortie.BonEntreeRequis", "Un bon d'entrée approuvé est requis pour une sortie.");

    public static readonly Error CategorieRequise =
        Error.Validation("BonSortie.CategorieRequise", "La catégorie de sortie est obligatoire.");

    public static readonly Error RaisonRequise =
        Error.Validation("BonSortie.RaisonRequise", "La raison de sortie est obligatoire.");

    public static readonly Error DestinationRequise =
        Error.Validation("BonSortie.DestinationRequise", "La destination est obligatoire.");

    public static readonly Error DateRetourRequise =
        Error.Validation("BonSortie.DateRetourRequise", "La date de retour prévue est obligatoire pour un prêt.");

    public static readonly Error DateRetourPassee =
        Error.Validation("BonSortie.DateRetourPassee", "La date de retour ne peut pas être dans le passé.");

    public static readonly Error MotifRejetRequis =
        Error.Validation("BonSortie.MotifRejetRequis", "Le motif de rejet est obligatoire.");

    public static readonly Error QuantiteRetourInvalide =
        Error.Validation("BonSortie.QuantiteRetourInvalide", "La quantité retournée ne peut pas dépasser la quantité sortie.");

    public static readonly Error AuMoinsUnMateriel =
        Error.Validation("BonSortie.AuMoinsUnMateriel", "Le bon doit contenir au moins un matériel.");

    public static readonly Error DestinataireSortieRequis =
        Error.Validation("BonSortie.DestinataireSortieRequis", "Le nom du destinataire est obligatoire pour une sortie externe.");

    public static readonly Error ItineraireRequis =
        Error.Validation("BonSortie.ItineraireRequis", "La provenance et la destination sont obligatoires.");

    public static readonly Error PretDejaRetourne =
        Error.Conflict("BonSortie.PretDejaRetourne", "Ce prêt a déjà été retourné.");

    public static readonly Error DateProlongationInvalide =
        Error.Validation("BonSortie.DateProlongationInvalide", "La nouvelle date de retour doit être postérieure à la date actuelle.");

    public static Error ModificationInterdite(string statut) =>
        new("BonSortie.ModificationInterdite", $"Modification impossible dans l'état '{statut}'.");

    public static Error ModificationInterdite(StatutBonSortie statut) =>
        ModificationInterdite(statut.ToString());

    public static Error SoumissionImpossible(string statut) =>
        new("BonSortie.SoumissionImpossible", $"Soumission impossible depuis l'état '{statut}'.");

    public static Error SoumissionImpossible(StatutBonSortie statut) =>
        SoumissionImpossible(statut.ToString());

    public static Error ApprobationImpossible(string statut) =>
        new("BonSortie.ApprobationImpossible", $"Approbation/rejet impossible dans l'état '{statut}'.");

    public static Error ApprobationImpossible(StatutBonSortie statut) =>
        ApprobationImpossible(statut.ToString());

    public static Error BonEntreeNonApprouve(string numero) =>
        new("BonSortie.BonEntreeNonApprouve", $"Le bon d'entrée '{numero}' n'est pas approuvé.");

    public static Error DejaRetourne() =>
        Error.Conflict("BonSortie.DejaRetourne", "Ce prêt a déjà été retourné.");

    public static readonly Error NonApprobateurEtape =
        Error.Validation("BonSortie.NonApprobateurEtape", "Vous n'êtes pas l'approbateur désigné de l'étape en cours.");

    public static readonly Error AdminNonAutorise =
        Error.Validation("BonSortie.AdminNonAutorise", "Les rôles Admin et SuperAdmin ne sont pas autorisés à approuver ou rejeter un bon.");
}
