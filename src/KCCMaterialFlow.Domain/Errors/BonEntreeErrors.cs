using KCCMaterialFlow.Domain.Enums;
using KCCMaterialFlow.Domain.Errors;

namespace KCCMaterialFlow.Domain.Errors;

/// <summary>
/// Catalogue complet des erreurs métier BonEntree.
/// </summary>
public static class BonEntreeErrors
{
    public static readonly Error CompagnieRequise =
        Error.Validation("BonEntree.CompagnieRequise", "Le nom de la compagnie est obligatoire.");

    public static readonly Error SiteManagerRequis =
        Error.Validation("BonEntree.SiteManagerRequis", "Le site manager est obligatoire.");

    public static readonly Error ItineraireRequis =
        Error.Validation("BonEntree.ItineraireRequis", "La provenance et la destination sont obligatoires.");

    public static readonly Error ProvenanceEgaleDestination =
        Error.Validation("BonEntree.MemeItineraire", "La provenance et la destination ne peuvent pas être identiques.");

    public static readonly Error AuMoinsUnMateriel =
        Error.Validation("BonEntree.AuMoinsUnMateriel", "Le bon doit contenir au moins un matériel.");

    public static readonly Error DateExpirationPassee =
        Error.Validation("BonEntree.DateExpiree", "La date d'expiration ne peut pas être dans le passé.");

    public static readonly Error QuantiteInvalide =
        Error.Validation("BonEntree.QuantiteInvalide", "La quantité doit être supérieure à zéro.");

    public static readonly Error MotifRejetRequis =
        Error.Validation("BonEntree.MotifRejetRequis", "Le motif de rejet est obligatoire.");

    public static readonly Error MotifRetourRequis =
        Error.Validation("BonEntree.MotifRetourRequis", "Le motif de retour est obligatoire.");

    public static Error ModificationInterdite(StatutBonEntree statut) =>
        new("BonEntree.ModificationInterdite", $"Modification impossible dans l'état '{statut}'.");

    public static Error SoumissionImpossible(StatutBonEntree statut) =>
        new("BonEntree.SoumissionImpossible", $"Soumission impossible depuis l'état '{statut}'.");

    public static Error ApprobationImpossible(StatutBonEntree statut) =>
        new("BonEntree.ApprobationImpossible", $"Approbation/rejet impossible dans l'état '{statut}'.");

    public static Error MaterielDuplique(string code) =>
        Error.Conflict("BonEntree.MaterielDuplique", $"Un matériel avec le code '{code}' existe déjà.");

    public static Error DejaVerrouille(string bonSortieNumero) =>
        Error.Conflict("BonEntree.DejaVerrouille", $"Déjà verrouillé par le bon de sortie '{bonSortieNumero}'.");

    public static readonly Error NonApprobateurEtape =
        Error.Validation("BonEntree.NonApprobateurEtape", "Vous n'êtes pas l'approbateur désigné de l'étape en cours.");

    public static readonly Error AdminNonAutorise =
        Error.Validation("BonEntree.AdminNonAutorise", "Les rôles Admin et SuperAdmin ne sont pas autorisés à approuver ou rejeter un bon.");
}
