using AppPlusPlus.Application.DTOs.Finance;
using AppPlusPlus.Domain.Entities.Finance;

namespace AppPlusPlus.Application.Services.Finance;

public interface IClotureService
{
    /// <summary>
    /// Loads clotures (Versements) with their Localisation navigation,
    /// filtered by localisation IDs, ordered by date descending.
    /// </summary>
    Task<List<Versement>> GetCloturesByLocalisationsAsync(List<int> localisationIds);

    /// <summary>
    /// Returns true if a cloture (Versement) already exists for the given date and localisation.
    /// </summary>
    Task<bool> ClotureExistsAsync(DateOnly date, int localisationId);

    /// <summary>
    /// Aggregates paid factures and payments for the given date and localisation
    /// into a summary DTO for the cloture dialog.
    /// </summary>
    Task<ClotureSummaryDto> GetClotureSummaryAsync(DateOnly date, int localisationId);

    /// <summary>
    /// Persists a new cloture (Versement) record.
    /// </summary>
    Task CreateClotureAsync(Versement versement);

    /// <summary>
    /// Returns true if the user has closed today and the closure is not rejected (statut 0 or 1).
    /// </summary>
    Task<bool> HasUserClosedTodayAsync(string userLogin, List<int> localisationIds);

    /// <summary>
    /// Approves or rejects a closure. statut: 1 = approved, 2 = rejected.
    /// </summary>
    Task UpdateClotureStatutAsync(int versementId, int statut, string traitePar, string? motifRejet = null);
}
