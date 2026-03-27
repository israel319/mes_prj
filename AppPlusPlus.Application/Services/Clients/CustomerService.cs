using AppPlusPlus.Domain.Entities.Clients;
using AppPlusPlus.Application.Interfaces.Repositories;

namespace AppPlusPlus.Application.Services.Clients;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepo;

    public CustomerService(ICustomerRepository customerRepo)
    {
        _customerRepo = customerRepo;
    }

    public async Task<List<Customer>> GetAllAsync()
        => await _customerRepo.GetAllAsync();

    public async Task<Customer?> GetByIdAsync(int id)
        => await _customerRepo.GetByIdAsync(id);

    public async Task AddAsync(Customer customer)
        => await _customerRepo.AddAsync(customer);

    public async Task UpdateAsync(Customer customer)
        => await _customerRepo.UpdateAsync(customer);

    public async Task DeleteAsync(int id)
    {
        var customer = await _customerRepo.GetByIdAsync(id);
        if (customer != null)
            await _customerRepo.DeleteAsync(customer);
    }
}
