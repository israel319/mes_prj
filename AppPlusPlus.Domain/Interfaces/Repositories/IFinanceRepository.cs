using AppPlusPlus.Domain.Entities.Finance;
using AppPlusPlus.Domain.Entities.Approvisionnement;

namespace AppPlusPlus.Domain.Interfaces.Repositories;

public interface IFinanceRepository
{
    // Caisse
    Task<Caisse?> GetCaisseByIdAsync(int caisseId);
    Task<List<Caisse>> GetAllCaissesAsync();
    Task AddCaisseAsync(Caisse caisse);
    Task UpdateCaisseAsync(Caisse caisse);

    // Taux
    Task<Taux?> GetCurrentTauxAsync();
    Task<List<Taux>> GetAllTauxAsync();
    Task<List<Taux>> GetTauxByDateRangeAsync(DateOnly from, DateOnly to);
    Task AddTauxAsync(Taux taux);

    // Periode
    Task<Periode?> GetActivePeriodeAsync();
    Task<List<Periode>> GetAllPeriodesAsync();
    Task AddPeriodeAsync(Periode periode);
    Task UpdatePeriodeAsync(Periode periode);

    // Versement
    Task<List<Versement>> GetVersementsByDateAsync(DateOnly date);
    Task<List<Versement>> GetVersementsByLocalisationAsync(int localisationId);
    Task<List<Versement>> GetVersementsByDateRangeAsync(DateOnly from, DateOnly to);
    Task AddVersementAsync(Versement versement);

    // ApproExpense
    Task<List<ApproExpense>> GetApproExpensesAsync();
    Task<List<ApproExpense>> GetApproExpensesByCaisseAsync(int caisseId);
    Task AddApproExpenseAsync(ApproExpense expense);

    // UserCaisse
    Task<List<UserCaisse>> GetUserCaissesAsync(string userLogin);
}
