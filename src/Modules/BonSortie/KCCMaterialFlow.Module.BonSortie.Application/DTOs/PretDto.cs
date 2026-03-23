namespace KCCMaterialFlow.Module.BonSortie.DTOs;

/// <summary>
/// BSM-038: DTO complet pour la gestion des prêts.
/// Étend les informations de BonSortieViewDto avec les spécificités prêt.
/// </summary>
public class PretViewDto
{
    public int IdBon { get; set; }
    public string NumeroReference { get; set; } = string.Empty;
    public DateTime DateCreation { get; set; }
    public DateTime DateExpiration { get; set; }
    public string StatutActuel { get; set; } = string.Empty;
    public string Provenance { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    
    // Demandeur
    public string NomDemandeur { get; set; } = string.Empty;
    public string FonctionDemandeur { get; set; } = string.Empty;
    public string DepartementDemandeur { get; set; } = string.Empty;
    public string MotifSortie { get; set; } = string.Empty;
    
    // Destinataire
    public string NomDestinataire { get; set; } = string.Empty;
    public string? AdresseDestination { get; set; }
    
    // Dates prêt
    public DateTime DateRetourPrevue { get; set; }
    public DateTime? DateRetourEffective { get; set; }
    
    // État du prêt
    public bool EstRetourne { get; set; }
    public string? EtatRetour { get; set; }
    public string? ReceptionnePar { get; set; }
    
    // Compteurs
    public int NombreMateriels { get; set; }
    public decimal QuantiteTotale { get; set; }
    
    // Matériels
    public List<MaterielSortieViewDto> Materiels { get; set; } = [];
    
    // BonEntree associé
    public int? BonEntreeAssocieId { get; set; }
    public string? BonEntreeAssocieNumero { get; set; }
    
    // QR Code
    public string? QRCodeBase64 { get; set; }
    
    // Métadonnées calculées
    public bool EstExpire => DateExpiration < DateTime.Today;
    public bool EstEnRetard => !EstRetourne && DateRetourPrevue < DateTime.Today;
    public int JoursRestants => (DateRetourPrevue.Date - DateTime.Today).Days;
    public int JoursRetard => EstEnRetard ? (DateTime.Today - DateRetourPrevue.Date).Days : 0;
    
    // Badge statut pour l'UI
    public string StatutPretBadgeClass => (EstRetourne, EstEnRetard) switch
    {
        (true, _) => "rz-badge rz-badge-success", // Retourné
        (false, true) => "rz-badge rz-badge-danger", // En retard
        (false, false) when JoursRestants <= 3 => "rz-badge rz-badge-warning", // Bientôt dû
        _ => "rz-badge rz-badge-info" // En cours
    };
    
    public string StatutPretLibelle => (EstRetourne, EstEnRetard) switch
    {
        (true, _) => "Retourné",
        (false, true) => $"En retard ({JoursRetard} j)",
        (false, false) when JoursRestants == 0 => "Dû aujourd'hui",
        (false, false) when JoursRestants == 1 => "Dû demain",
        (false, false) when JoursRestants <= 3 => $"Dû dans {JoursRestants} jours",
        _ => $"En cours ({JoursRestants} j restants)"
    };
}

/// <summary>
/// DTO léger pour les listes de prêts
/// </summary>
public class PretListDto
{
    public int IdBon { get; set; }
    public string NumeroReference { get; set; } = string.Empty;
    public DateTime DateCreation { get; set; }
    public string NomDemandeur { get; set; } = string.Empty;
    public string DepartementDemandeur { get; set; } = string.Empty;
    public string NomDestinataire { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime DateRetourPrevue { get; set; }
    public DateTime? DateRetourEffective { get; set; }
    public bool EstRetourne { get; set; }
    public int NombreMateriels { get; set; }
    public string StatutActuel { get; set; } = string.Empty;
    
    // Métadonnées calculées
    public bool EstEnRetard => !EstRetourne && DateRetourPrevue < DateTime.Today;
    public int JoursRestantsOuRetard => EstEnRetard 
        ? -(DateTime.Today - DateRetourPrevue.Date).Days 
        : (DateRetourPrevue.Date - DateTime.Today).Days;
    
    public string StatutPretLibelle => (EstRetourne, EstEnRetard) switch
    {
        (true, _) => "✓ Retourné",
        (false, true) => $"⚠ Retard {-JoursRestantsOuRetard}j",
        (false, false) when JoursRestantsOuRetard == 0 => "📅 Dû aujourd'hui",
        (false, false) when JoursRestantsOuRetard <= 3 => $"⏰ {JoursRestantsOuRetard}j restants",
        _ => $"📋 {JoursRestantsOuRetard}j"
    };
}
