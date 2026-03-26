using AppPlusPlus.Domain.Entities.Approvisionnement;

namespace AppPlusPlus.Domain.Interfaces.Repositories;

public interface IApproRepository : IRepository<Appro>
{
    Task<Appro?> GetWithDetailsAsync(int id);
    Task<List<Appro>> GetByDateRangeAsync(DateOnly from, DateOnly to);
    Task<List<Appro>> GetBySupplierAsync(int supplierId);
    Task<List<Appro>> GetByLocalisationAsync(int localisationId);
    Task<List<Appro>> GetByUserAsync(string userLogin);
    Task<List<ApproDetail>> GetDetailsByApproIdAsync(int approId);
}
