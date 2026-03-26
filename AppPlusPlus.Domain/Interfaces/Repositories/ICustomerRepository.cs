using AppPlusPlus.Domain.Entities.Clients;

namespace AppPlusPlus.Domain.Interfaces.Repositories;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<List<Customer>> GetPermanentCustomersAsync();
    Task<List<Customer>> SearchByNameAsync(string name);
    Task<List<Customer>> GetByTypeAsync(int customerTypeId);
    Task<List<CustomerType>> GetAllTypesAsync();
}
