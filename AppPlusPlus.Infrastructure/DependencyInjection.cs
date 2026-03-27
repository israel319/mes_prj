using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AppPlusPlus.Infrastructure.Persistence;
using AppPlusPlus.Infrastructure.Persistence.Repositories;
using AppPlusPlus.Infrastructure.ExternalServices;
using AppPlusPlus.Infrastructure.Identity;
using AppPlusPlus.Domain.Interfaces;
using AppPlusPlus.Domain.Interfaces.Repositories;
using AppPlusPlus.Application.Interfaces;
using AppPlusPlus.Application.Services.Dashboard;
using AppPlusPlus.Application.Services.Vente;
using AppPlusPlus.Application.Services.Commandes;
using AppPlusPlus.Application.Services.Finance;
using AppPlusPlus.Infrastructure.QueryServices;
using AppPlusPlus.Infrastructure.QueryServices.Vente;
using AppPlusPlus.Infrastructure.QueryServices.Commandes;
using AppPlusPlus.Infrastructure.QueryServices.Finance;

namespace AppPlusPlus.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        // ── EF Core ──
        services.AddDbContextFactory<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // ── Repositories ──
        services.AddScoped<ICatalogueRepository, CatalogueRepository>();
        services.AddScoped<IFactureRepository, FactureRepository>();
        services.AddScoped<IStockRepository, StockRepository>();
        services.AddScoped<IApproRepository, ApproRepository>();
        services.AddScoped<ICommandeRepository, CommandeRepository>();
        services.AddScoped<ICmdRepository, CmdRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<IFinanceRepository, FinanceRepository>();
        services.AddScoped<IParametresRepository, ParametresRepository>();
        services.AddScoped<ILookupRepository, LookupRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // ── External Services ──
        services.AddScoped<IEmailSender, EmailSender>();
        services.AddSingleton<INumberToWordsConverter, NumberToWordsConverter>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // ── Query Services ──
        services.AddScoped<IDashboardService, QueryServices.DashboardService>();
        services.AddScoped<IFacturationService, FacturationQueryService>();
        services.AddScoped<ILivraisonService, LivraisonQueryService>();
        services.AddScoped<IClotureService, ClotureQueryService>();

        return services;
    }
}
