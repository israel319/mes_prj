using Microsoft.EntityFrameworkCore;
using AppPlusPlus.Domain.Entities.Clients;
using AppPlusPlus.Domain.Interfaces.Repositories;

namespace AppPlusPlus.Infrastructure.Persistence.Repositories;

public class CustomerRepository : RepositoryBase<Customer>, ICustomerRepository
{
    public CustomerRepository(IDbContextFactory<AppDbContext> dbFactory) : base(dbFactory) { }

    public async Task<List<Customer>> GetPermanentCustomersAsync()
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.Customers.Where(c => c.IsPermanent == true).ToListAsync();
    }

    public async Task<List<Customer>> SearchByNameAsync(string name)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.Customers
            .Where(c => c.CustomerName != null && c.CustomerName.Contains(name))
            .ToListAsync();
    }

    public async Task<List<Customer>> GetByTypeAsync(int customerTypeId)
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.Customers.Where(c => c.CustomerTypeId == customerTypeId).ToListAsync();
    }

    public async Task<List<CustomerType>> GetAllTypesAsync()
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();
        return await ctx.CustomerTypes.ToListAsync();
    }
}
