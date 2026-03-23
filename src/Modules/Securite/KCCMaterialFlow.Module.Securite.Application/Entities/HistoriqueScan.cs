using System.ComponentModel.DataAnnotations;

namespace KCCMaterialFlow.Module.Securite.Entities;

/// <summary>
/// Représente l'historique complet de tous les scans pour identification.
/// Table spéciale pour la vue "Identification" qui montre tous les mouvements.
/// Contient une vue consolidée des mouvements avec détails matériels.
/// </summary>
public class HistoriqueScan
{
    /// <summary>
    /// Identifiant unique de l'entrée d'historique (clé primaire)
    /// </summary>
    [Key]
    public int IdHistorique { get; set; }

    /// <summary>
    /// Identifiant du scan original
    /// </summary>
    public int ScanId { get; set; }

    /// <summary>
    /// Date et heure du mouvement
    /// </summary>
    public DateTime DateHeureMouvement { get; set; } = DateTime.Now;

    /// <summary>
    /// Type de mouvement : Entree, Sortie
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string TypeMouvement { get; set; } = "Sortie";

    /// <summary>
    /// Type de bon : BEM ou BSM
    /// </summary>
    [Required]
    [MaxLength(10)]
    public string TypeBon { get; set; } = "BSM";

    /// <summary>
    /// Numéro de référence du bon
    /// </summary>
    [Required]
    [MaxLength(30)]
    public string NumeroReferenceBon { get; set; } = string.Empty;

    /// <summary>
    /// Code de la barrière de passage
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string CodeBarriere { get; set; } = string.Empty;

    /// <summary>
    /// Nom de la barrière de passage
    /// </summary>
    [MaxLength(100)]
    public string? NomBarriere { get; set; }

    /// <summary>
    /// Direction : EntreeUsine, SortieUsine
    /// </summary>
    [MaxLength(30)]
    public string? Direction { get; set; }

    /// <summary>
    /// Département ou section concerné
    /// </summary>
    [MaxLength(100)]
    public string? Departement { get; set; }

    /// <summary>
    /// Provenance des matériels (FROM)
    /// </summary>
    [MaxLength(200)]
    public string? Provenance { get; set; }

    /// <summary>
    /// Destination des matériels (TO)
    /// </summary>
    [MaxLength(200)]
    public string? Destination { get; set; }

    /// <summary>
    /// Nombre total de matériels dans le bon
    /// </summary>
    public int NombreMateriels { get; set; }

    /// <summary>
    /// Liste résumée des matériels (pour affichage rapide)
    /// </summary>
    [MaxLength(2000)]
    public string? ResumeMateriels { get; set; }

    /// <summary>
    /// Détails des matériels en JSON pour affichage détaillé
    /// </summary>
    public string? MaterielsJson { get; set; }

    /// <summary>
    /// Statut du scan : Valid, Invalid, etc.
    /// </summary>
    [Required]
    [MaxLength(30)]
    public string StatutScan { get; set; } = "Valid";

    /// <summary>
    /// Indique si le passage a été autorisé
    /// </summary>
    public bool PassageAutorise { get; set; } = true;

    /// <summary>
    /// Login de l'agent de sécurité
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string AgentLogin { get; set; } = string.Empty;

    /// <summary>
    /// Nom de l'agent de sécurité
    /// </summary>
    [MaxLength(200)]
    public string? AgentNom { get; set; }

    /// <summary>
    /// Nom du demandeur/transporteur
    /// </summary>
    [MaxLength(200)]
    public string? NomDemandeur { get; set; }

    /// <summary>
    /// Matricule du véhicule (si applicable)
    /// </summary>
    [MaxLength(50)]
    public string? MatriculeVehicule { get; set; }

    /// <summary>
    /// Observations lors du passage
    /// </summary>
    [MaxLength(1000)]
    public string? Observations { get; set; }

    /// <summary>
    /// Indique si une anomalie a été signalée
    /// </summary>
    public bool AnomalieSignalee { get; set; } = false;

    /// <summary>
    /// Nombre d'anomalies associées
    /// </summary>
    public int NombreAnomalies { get; set; } = 0;

    /// <summary>
    /// Navigation vers le scan original
    /// </summary>
    public virtual ScanEvenement? ScanEvenement { get; set; }
}
