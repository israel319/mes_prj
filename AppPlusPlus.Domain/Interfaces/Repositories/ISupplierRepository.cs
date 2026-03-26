using AppPlusPlus.Domain.Entities.Fournisseurs;

namespace AppPlusPlus.Domain.Interfaces.Repositories;

public interface ISupplierRepository : IRepository<Supplier>
{
    Task<List<Supplier>> SearchByNameAsync(string name);
    Task<List<Supplier>> GetByServiceAsync(int serviceId);
    Task<List<SupplierService>> GetAllServicesAsync();
}
