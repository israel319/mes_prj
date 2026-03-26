using AppPlusPlus.Domain.Entities.Finance;
using AppPlusPlus.Domain.Interfaces.Repositories;

namespace AppPlusPlus.Application.Services.Finance;

public class FinanceService : IFinanceService
{
    private readonly IFinanceRepository _financeRepo;

    public FinanceService(IFinanceRepository financeRepo)
    {
        _financeRepo = financeRepo;
    }

    public async Task<List<Taux>> GetTauxAsync()
        => await _financeRepo.GetAllTauxAsync();

    public async Task<List<Periode>> GetPeriodesAsync()
        => await _financeRepo.GetAllPeriodesAsync();

    public async Task<List<Versement>> GetVersementsAsync()
    {
        // Phase 3: Add proper date range or localisation filtering
        // For now return versements for today as a placeholder
        return await _financeRepo.GetVersementsByDateAsync(DateOnly.FromDateTime(DateTime.Today));
    }
}
