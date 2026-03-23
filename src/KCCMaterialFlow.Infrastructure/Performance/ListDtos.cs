namespace KCCMaterialFlow.Infrastructure.Performance;

/// <summary>
/// DTOs optimisés pour les listes et projections.
/// INT-023: Optimiser queries listes - Projection DTO dans queries
/// Ces DTOs permettent de ne charger que les données nécessaires à l'affichage,
/// réduisant significativement la charge sur la base de données.
/// </summary>

// === BON ENTREE DTOs ===

/// <summary>
/// DTO allégé pour les listes de bons d'entrée
/// </summary>
public record BonEntreeListDto
{
    public int IdBon { get; init; }
    public string NumeroReference { get; init; } = string.Empty;
    public DateTime DateCreation { get; init; }
    public DateTime? DateExpiration { get; init; }
    public string StatutActuel { get; init; } = string.Empty;
    public string? NomCompagnie { get; init; }
    public string? HostDepartment { get; init; }
    public int Quantite { get; init; }
    public string? NomEscorteur { get; init; }
    public bool EstExpire => DateExpiration.HasValue && DateExpiration.Value < DateTime.Now;
}

/// <summary>
/// DTO pour les dropdowns de sélection de bon d'entrée
/// </summary>
public record BonEntreeLookupDto
{
    public int IdBon { get; init; }
    public string NumeroReference { get; init; } = string.Empty;
    public string? NomCompagnie { get; init; }
    public string Display => $"{NumeroReference} - {NomCompagnie}";
}

// === BON SORTIE DTOs ===

/// <summary>
/// DTO allégé pour les listes de bons de sortie
/// </summary>
public record BonSortieListDto
{
    public int IdBon { get; init; }
    public string NumeroReference { get; init; } = string.Empty;
    public string TypeBon { get; init; } = string.Empty; // "Interne" ou "Externe"
    public DateTime DateCreation { get; init; }
    public DateTime? DateExpiration { get; init; }
    public string StatutActuel { get; init; } = string.Empty;
    public string? NomDemandeur { get; init; }
    public string? DepartementDemandeur { get; init; }
    public int Quantite { get; init; }
    public bool EstDefinitif { get; init; }
    public bool EstExpire => DateExpiration.HasValue && DateExpiration.Value < DateTime.Now;
}

// === UTILISATEUR DTOs ===

/// <summary>
/// DTO allégé pour les listes d'utilisateurs
/// </summary>
public record UtilisateurListDto
{
    public int IdUtilisateur { get; init; }
    public string Login { get; init; } = string.Empty;
    public string NomComplet { get; init; } = string.Empty;
    public string? Matricule { get; init; }
    public string? Email { get; init; }
    public string? NomDepartement { get; init; }
    public bool EstActif { get; init; }
    public DateTime? DateDerniereConnexion { get; init; }
}

/// <summary>
/// DTO pour les dropdowns de sélection d'utilisateur
/// </summary>
public record UtilisateurLookupDto
{
    public int IdUtilisateur { get; init; }
    public string Login { get; init; } = string.Empty;
    public string NomComplet { get; init; } = string.Empty;
    public string Display => $"{NomComplet} ({Login})";
}

// === DEPARTEMENT DTOs ===

/// <summary>
/// DTO allégé pour les listes de départements
/// </summary>
public record DepartementListDto
{
    public int IdDepartement { get; init; }
    public string CodeDepartement { get; init; } = string.Empty;
    public string NomDepartement { get; init; } = string.Empty;
    public string? NomResponsable { get; init; }
    public int NombreUtilisateurs { get; init; }
    public bool EstActif { get; init; }
}

/// <summary>
/// DTO pour les dropdowns de sélection de département
/// </summary>
public record DepartementLookupDto
{
    public int IdDepartement { get; init; }
    public string CodeDepartement { get; init; } = string.Empty;
    public string NomDepartement { get; init; } = string.Empty;
    public string Display => $"{CodeDepartement} - {NomDepartement}";
}

// === SCAN DTOs ===

/// <summary>
/// DTO allégé pour les listes de scans
/// </summary>
public record ScanListDto
{
    public long IdScan { get; init; }
    public DateTime DateScan { get; init; }
    public string TypeScan { get; init; } = string.Empty; // "Entree" ou "Sortie"
    public string StatutScan { get; init; } = string.Empty;
    public string? NomBarriere { get; init; }
    public string? NumeroReferenceBon { get; init; }
    public string? NomAgent { get; init; }
}

// === AUDIT LOG DTOs ===

/// <summary>
/// DTO allégé pour les listes d'audit
/// </summary>
public record AuditLogListDto
{
    public long IdAuditLog { get; init; }
    public DateTime DateAction { get; init; }
    public string UtilisateurLogin { get; init; } = string.Empty;
    public string? UtilisateurNom { get; init; }
    public string TypeAction { get; init; } = string.Empty;
    public string Categorie { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Niveau { get; init; } = string.Empty;
    public bool HasDetails { get; init; }
}

// === MATERIEL DTOs ===

/// <summary>
/// DTO allégé pour les listes de matériels
/// </summary>
public record MaterielListDto
{
    public int IdMateriel { get; init; }
    public string CodeProduitSerial { get; init; } = string.Empty;
    public string Designation { get; init; } = string.Empty;
    public decimal Quantite { get; init; }
    public string? NumeroReferenceBon { get; init; }
}

// === STATISTIQUES DTOs ===

/// <summary>
/// DTO pour les statistiques du tableau de bord
/// </summary>
public record DashboardStatsDto
{
    public int TotalBonsEntreeActifs { get; init; }
    public int TotalBonsSortieActifs { get; init; }
    public int BonsEnAttenteApprobation { get; init; }
    public int BonsExpires { get; init; }
    public int ScansAujourdHui { get; init; }
    public int UtilisateursActifs { get; init; }
}

/// <summary>
/// DTO pour les statistiques par statut
/// </summary>
public record StatutCountDto
{
    public string Statut { get; init; } = string.Empty;
    public int Count { get; init; }
}

/// <summary>
/// DTO pour les statistiques par période
/// </summary>
public record PeriodStatsDto
{
    public DateTime Date { get; init; }
    public int BonsEntree { get; init; }
    public int BonsSortie { get; init; }
    public int Scans { get; init; }
}
