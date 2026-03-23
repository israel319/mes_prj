using KCCMaterialFlow.Application.Interfaces;
using KCCMaterialFlow.Module.BonEntree.Repositories;
using Microsoft.Extensions.Logging;

namespace KCCMaterialFlow.Module.BonEntree.Services;

/// <summary>
/// BSM-031: Implémentation du service de verrouillage des bons d'entrée
/// pour la liaison avec les bons de sortie.
/// Délègue les opérations de données au repository (Clean Architecture).
/// </summary>
public class BonEntreeLockService : IBonEntreeLockService
{
    private readonly IBonEntreeRepository _repository;
    private readonly ILogger<BonEntreeLockService> _logger;

    public BonEntreeLockService(
        IBonEntreeRepository repository,
        ILogger<BonEntreeLockService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Vérifie si un bon d'entrée est disponible pour être lié à un bon de sortie
    /// </summary>
    public async Task<BonEntreeAvailabilityResult> CheckAvailabilityAsync(int bonEntreeId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Charger avec les matériels pour vérifier la quantité disponible
            var bonEntree = await _repository.GetByIdAsync(bonEntreeId, includeMateriels: true, cancellationToken: cancellationToken);

            if (bonEntree == null)
            {
                return BonEntreeAvailabilityResult.NotAvailable($"Bon d'entrée #{bonEntreeId} non trouvé");
            }

            // Vérifier le statut
            if (bonEntree.StatutActuel != "Approved")
            {
                return BonEntreeAvailabilityResult.NotAvailable(
                    $"Bon d'entrée {bonEntree.NumeroReference} n'est pas approuvé (statut: {bonEntree.StatutActuel})");
            }

            // Vérifier l'expiration
            if (bonEntree.DateExpiration < DateTime.Today)
            {
                return BonEntreeAvailabilityResult.NotAvailable(
                    $"Bon d'entrée {bonEntree.NumeroReference} a expiré le {bonEntree.DateExpiration:dd/MM/yyyy}");
            }

            // Vérifier s'il reste de la quantité disponible sur au moins un matériel
            var quantiteTotaleDisponible = bonEntree.Materiels?.Sum(m => m.QuantiteDisponible) ?? 0;
            if (quantiteTotaleDisponible <= 0)
            {
                return BonEntreeAvailabilityResult.NotAvailable(
                    $"Bon d'entrée {bonEntree.NumeroReference} n'a plus de matériel disponible (tout a été sorti)");
            }

            return BonEntreeAvailabilityResult.Available(bonEntree.NumeroReference, bonEntree.NomCompagnie);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la vérification de disponibilité du BEM {Id}", bonEntreeId);
            return BonEntreeAvailabilityResult.NotAvailable($"Erreur: {ex.Message}");
        }
    }

    /// <summary>
    /// Verrouille un bon d'entrée pour un bon de sortie
    /// </summary>
    public async Task LockAsync(int bonEntreeId, int bonSortieId, string bonSortieNumero, CancellationToken cancellationToken = default)
    {
        try
        {
            await _repository.LockForSortieAsync(bonEntreeId, bonSortieId, bonSortieNumero, cancellationToken);
            _logger.LogInformation("BSM-031: BEM {BemId} verrouillé pour BSM {BsmNumero}", bonEntreeId, bonSortieNumero);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du verrouillage du BEM {Id} pour BSM {BsmNumero}", bonEntreeId, bonSortieNumero);
            throw;
        }
    }

    /// <summary>
    /// Déverrouille un bon d'entrée
    /// </summary>
    public async Task UnlockAsync(int bonEntreeId, CancellationToken cancellationToken = default)
    {
        try
        {
            await _repository.UnlockFromSortieAsync(bonEntreeId, cancellationToken);
            _logger.LogInformation("BSM-031: BEM {BemId} déverrouillé", bonEntreeId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du déverrouillage du BEM {Id}", bonEntreeId);
            throw;
        }
    }

    /// <summary>
    /// Récupère les informations de base d'un bon d'entrée
    /// </summary>
    public async Task<BonEntreeBasicInfo?> GetBasicInfoAsync(int bonEntreeId, CancellationToken cancellationToken = default)
    {
        try
        {
            var bonEntree = await _repository.GetByIdAsync(bonEntreeId, includeMateriels: false, cancellationToken: cancellationToken);

            if (bonEntree == null)
                return null;

            return new BonEntreeBasicInfo
            {
                IdBon = bonEntree.IdBon,
                NumeroReference = bonEntree.NumeroReference,
                NomCompagnie = bonEntree.NomCompagnie,
                StatutActuel = bonEntree.StatutActuel,
                DateCreation = bonEntree.DateCreation,
                DateExpiration = bonEntree.DateExpiration,
                EstVerrouille = bonEntree.EstVerrouillePourSortie,
                BonSortieAssocieId = bonEntree.BonSortieAssocieId,
                BonSortieAssocieNumero = bonEntree.BonSortieAssocieNumero
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des infos du BEM {Id}", bonEntreeId);
            return null;
        }
    }

    /// <summary>
    /// Récupère les détails complets d'un bon d'entrée incluant ses matériels
    /// </summary>
    public async Task<BonEntreeDetailsForSortie?> GetDetailsForSortieAsync(int bonEntreeId, CancellationToken cancellationToken = default)
    {
        try
        {
            var bonEntree = await _repository.GetByIdAsync(bonEntreeId, includeMateriels: true, cancellationToken: cancellationToken);

            if (bonEntree == null)
                return null;

            return new BonEntreeDetailsForSortie
            {
                IdBon = bonEntree.IdBon,
                NumeroReference = bonEntree.NumeroReference,
                NomCompagnie = bonEntree.NomCompagnie,
                ContratId = bonEntree.ContratId,
                NumeroContrat = bonEntree.NumeroContrat,
                SiteManager = bonEntree.SiteManager,
                HostDepartment = bonEntree.HostDepartment,
                ReasonOnSite = bonEntree.ReasonOnSite,
                Provenance = bonEntree.Provenance,
                Destination = bonEntree.Destination,
                DateCreation = bonEntree.DateCreation,
                DateExpiration = bonEntree.DateExpiration,
                StatutActuel = bonEntree.StatutActuel,
                Materiels = bonEntree.Materiels.Select(m => new MaterielForSortie
                {
                    IdMateriel = m.IdMateriel,
                    CodeProduitSerial = m.CodeProduitSerial,
                    Designation = m.Designation,
                    QuantiteInitiale = m.Quantite,
                    QuantiteDejaSortie = m.Quantite - m.QuantiteDisponible // Calculé depuis le stock
                }).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des détails du BEM {Id}", bonEntreeId);
            return null;
        }
    }

    /// <summary>
    /// Recherche un bon d'entrée par son numéro de référence
    /// </summary>
    public async Task<BonEntreeDetailsForSortie?> SearchByNumeroReferenceAsync(string numeroReference, CancellationToken cancellationToken = default)
    {
        try
        {
            var bonEntree = await _repository.GetByNumeroAsync(numeroReference, cancellationToken);

            if (bonEntree == null)
                return null;

            // Récupérer avec les matériels
            var bonEntreeComplet = await _repository.GetByIdAsync(bonEntree.IdBon, includeMateriels: true, cancellationToken: cancellationToken);

            if (bonEntreeComplet == null)
                return null;

            return new BonEntreeDetailsForSortie
            {
                IdBon = bonEntreeComplet.IdBon,
                NumeroReference = bonEntreeComplet.NumeroReference,
                NomCompagnie = bonEntreeComplet.NomCompagnie,
                ContratId = bonEntreeComplet.ContratId,
                NumeroContrat = bonEntreeComplet.NumeroContrat,
                SiteManager = bonEntreeComplet.SiteManager,
                HostDepartment = bonEntreeComplet.HostDepartment,
                ReasonOnSite = bonEntreeComplet.ReasonOnSite,
                Provenance = bonEntreeComplet.Provenance,
                Destination = bonEntreeComplet.Destination,
                DateCreation = bonEntreeComplet.DateCreation,
                DateExpiration = bonEntreeComplet.DateExpiration,
                StatutActuel = bonEntreeComplet.StatutActuel,
                Materiels = bonEntreeComplet.Materiels.Select(m => new MaterielForSortie
                {
                    IdMateriel = m.IdMateriel,
                    CodeProduitSerial = m.CodeProduitSerial,
                    Designation = m.Designation,
                    QuantiteInitiale = m.Quantite,
                    QuantiteDejaSortie = m.Quantite - m.QuantiteDisponible // Calculé depuis le stock
                }).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la recherche du BEM par référence {Ref}", numeroReference);
            return null;
        }
    }

    /// <summary>
    /// INT-006: Archive un bon d'entrée lorsque le bon de sortie associé est complété
    /// </summary>
    public async Task ArchiveAfterSortieAsync(int bonEntreeId, string bonSortieNumero, CancellationToken cancellationToken = default)
    {
        try
        {
            var bonEntree = await _repository.GetByIdAsync(bonEntreeId, includeMateriels: false, cancellationToken: cancellationToken);
            
            if (bonEntree == null)
            {
                _logger.LogWarning("INT-006: BEM {Id} non trouvé pour archivage", bonEntreeId);
                return;
            }

            // Vérifier que le BEM est bien verrouillé par ce BSM
            if (!bonEntree.EstVerrouillePourSortie)
            {
                _logger.LogWarning("INT-006: BEM {Ref} n'est pas verrouillé, archivage ignoré", bonEntree.NumeroReference);
                return;
            }

            // Archiver le BEM
            await _repository.ArchiveAsync(bonEntreeId, $"Archivé suite à la complétion du BSM {bonSortieNumero}", cancellationToken);
            
            _logger.LogInformation("INT-006: BEM {Ref} archivé suite à la complétion du BSM {BsmNumero}", 
                bonEntree.NumeroReference, bonSortieNumero);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "INT-006: Erreur lors de l'archivage du BEM {Id} après complétion BSM {BsmNumero}", 
                bonEntreeId, bonSortieNumero);
            // Ne pas relancer l'exception - l'archivage est une opération secondaire
        }
    }

    /// <summary>
    /// Décrémente la quantité disponible des matériels lors de l'approbation finale d'un BSM
    /// </summary>
    public async Task<StockUpdateResult> DecrementStockAsync(IEnumerable<MaterielStockDecrement> materielsASortir, CancellationToken cancellationToken = default)
    {
        try
        {
            var materielsList = materielsASortir.ToList();
            if (!materielsList.Any())
            {
                return StockUpdateResult.Ok();
            }

            _logger.LogInformation("Décrémentation du stock pour {Count} matériels", materielsList.Count);

            var result = await _repository.DecrementMaterielStockAsync(materielsList, cancellationToken);

            if (result.Success)
            {
                _logger.LogInformation("Stock décrémenté avec succès pour {Count} matériels", materielsList.Count);
            }
            else
            {
                _logger.LogWarning("Échec de la décrémentation du stock: {Error}", result.ErrorMessage);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la décrémentation du stock");
            return StockUpdateResult.Fail($"Erreur: {ex.Message}");
        }
    }

    /// <summary>
    /// Récupère tous les bons d'entrée disponibles pour sortie
    /// </summary>
    public async Task<IReadOnlyList<BonEntreeForDropdown>> GetAllAvailableForSortieAsync(string? hostDepartmentFilter = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _repository.GetAllAvailableForSortieAsync(hostDepartmentFilter, cancellationToken);
            _logger.LogInformation("GetAllAvailableForSortieAsync: {Count} BEM disponibles (HostDepartmentFilter={Filter})", result.Count, hostDepartmentFilter ?? "<none>");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des BEM disponibles");
            return Array.Empty<BonEntreeForDropdown>();
        }
    }
}
