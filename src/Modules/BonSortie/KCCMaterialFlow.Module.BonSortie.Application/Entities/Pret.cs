using System.ComponentModel.DataAnnotations;

namespace KCCMaterialFlow.Module.BonSortie.Entities;

/// <summary>
/// Représente un prêt de matériel (sortie temporaire).
/// Étend BonSortieExterne pour les cas où le matériel doit être retourné.
/// </summary>
public class Pret : BonSortieExterne
{
    /// <summary>
    /// Date de retour prévue du matériel
    /// </summary>
    [Required]
    public DateTime DateRetourPrevue { get; set; }

    /// <summary>
    /// Date de retour effective du matériel (renseignée au retour)
    /// </summary>
    public DateTime? DateRetourEffective { get; set; }

    /// <summary>
    /// Indique si le matériel a été retourné
    /// </summary>
    public bool EstRetourne { get; set; } = false;

    /// <summary>
    /// État du matériel au retour (observations)
    /// </summary>
    [MaxLength(1000)]
    public string? EtatRetour { get; set; }

    /// <summary>
    /// Nom de la personne ayant réceptionné le retour
    /// </summary>
    [MaxLength(200)]
    public string? ReceptionnePar { get; set; }

    /// <summary>
    /// Nombre de jours de retard (calculé)
    /// </summary>
    public int JoursRetard => CalculerJoursRetard();

    /// <summary>
    /// Indique si le prêt est en retard
    /// </summary>
    public bool EstEnRetard => !EstRetourne && DateTime.Now > DateRetourPrevue;

    /// <summary>
    /// Calcule le nombre de jours de retard
    /// </summary>
    private int CalculerJoursRetard()
    {
        if (EstRetourne && DateRetourEffective.HasValue)
        {
            // Retard basé sur la date de retour effective
            var retard = (DateRetourEffective.Value - DateRetourPrevue).Days;
            return retard > 0 ? retard : 0;
        }
        else if (!EstRetourne && DateTime.Now > DateRetourPrevue)
        {
            // Retard en cours
            return (DateTime.Now - DateRetourPrevue).Days;
        }
        return 0;
    }
}
