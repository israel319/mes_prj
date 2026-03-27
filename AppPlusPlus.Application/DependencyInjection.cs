using Microsoft.Extensions.DependencyInjection;
using AppPlusPlus.Application.Services.Administration;
using AppPlusPlus.Application.Services.Localisation;
using AppPlusPlus.Application.Services.Parametres;
using AppPlusPlus.Application.Services.Catalogue;
using AppPlusPlus.Application.Services.Vente;
using AppPlusPlus.Application.Services.Stock;
using AppPlusPlus.Application.Services.Approvisionnement;
using AppPlusPlus.Application.Services.Commandes;
using AppPlusPlus.Application.Services.Finance;
using AppPlusPlus.Application.Services.Dashboard;
using AppPlusPlus.Application.Services.Rapports;
using AppPlusPlus.Application.Services.Clients;
using AppPlusPlus.Application.Services.Fournisseurs;
using AppPlusPlus.Application.Services.Shared;

namespace AppPlusPlus.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IPermissionResolver, PermissionResolver>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<ILocalisationService, LocalisationService>();
        services.AddSingleton<IAppSettingsService, AppSettingsAppService>();
        services.AddSingleton<IShopProfileService, ShopProfileAppService>();
        services.AddScoped<ICatalogueService, CatalogueService>();
        services.AddScoped<IStockService, StockService>();
        // IFacturationService registered in Infrastructure (query service needs DbContext)
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IApproService, ApproService>();
        services.AddScoped<ITransformationService, TransformationService>();
        services.AddScoped<ICommandeService, CommandeService>();
        services.AddScoped<ICmdService, CmdService>();
        // ILivraisonService registered in Infrastructure (query service needs DbContext)
        // IClotureService registered in Infrastructure (query service needs DbContext)
        services.AddScoped<IFinanceService, FinanceService>();
        // IDashboardService is registered in Infrastructure (query service needs DbContext)
        services.AddScoped<IRapportService, RapportService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ISupplierService, SupplierService>();
        services.AddScoped<ILookupService, LookupService>();

        return services;
    }
}
