using AppPlusPlus.Domain.Entities.Vente;

namespace AppPlusPlus.Application.Services.Vente;

public interface IPaymentService
{
    Task<List<Payment>> GetPaymentsByFactureAsync(int factId);
}
