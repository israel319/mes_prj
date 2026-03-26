using AppPlusPlus.Domain.Entities.Finance;

namespace AppPlusPlus.Application.Services.Finance;

public interface IClotureService
{
    /// <summary>
    /// Loads clotures (Versements) with their Localisation navigation,
    /// filtered by localisation IDs, ordered by date descending.
    /// </summary>
    Task<List<Versement>> GetCloturesByLocalisationsAsync(List<int> localisationIds);
}
