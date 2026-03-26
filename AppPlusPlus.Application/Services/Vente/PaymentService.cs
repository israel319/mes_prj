using AppPlusPlus.Domain.Entities.Vente;
using AppPlusPlus.Domain.Interfaces.Repositories;

namespace AppPlusPlus.Application.Services.Vente;

public class PaymentService : IPaymentService
{
    private readonly IFactureRepository _factureRepo;

    public PaymentService(IFactureRepository factureRepo)
    {
        _factureRepo = factureRepo;
    }

    public async Task<List<Payment>> GetPaymentsByFactureAsync(int factId)
        => await _factureRepo.GetPaymentsByFactIdAsync(factId);
}
