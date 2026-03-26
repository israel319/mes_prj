using AppPlusPlus.Domain.Entities.Finance;

namespace AppPlusPlus.Application.Services.Finance;

public interface IFinanceService
{
    Task<List<Taux>> GetTauxAsync();
    Task<List<Periode>> GetPeriodesAsync();
    Task<List<Versement>> GetVersementsAsync();
}
