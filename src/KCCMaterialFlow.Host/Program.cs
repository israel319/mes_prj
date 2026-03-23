using System.Reflection;
using System.Runtime.Versioning;
using KCCMaterialFlow.Application.Interfaces;
using KCCMaterialFlow.Infrastructure.Data;
using KCCMaterialFlow.Infrastructure.Services;
using KCCMaterialFlow.Module.Shared.Services;
using KCCMaterialFlow.Host.Components;
using KCCMaterialFlow.Infrastructure.Repositories.BonEntree;
using KCCMaterialFlow.Infrastructure.Repositories.BonSortie;
using KCCMaterialFlow.Infrastructure.Repositories.Securite;
using KCCMaterialFlow.Module.BonEntree.Repositories;
using KCCMaterialFlow.Module.BonSortie.Repositories;
using KCCMaterialFlow.Module.Securite.Repositories;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Radzen;
using Serilog;

// Configuration Serilog avant la création du builder
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Démarrage de KCC Material Flow...");

    var builder = WebApplication.CreateBuilder(args);

    // Configuration Serilog depuis appsettings.json
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", "KCCMaterialFlow"));

    // Add services to the container.
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents(options =>
        {
            // Gestion gracieuse des déconnexions de circuit Blazor Server
            // Évite les ObjectDisposedException quand le WebSocket se ferme pendant un rendu
            options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(3);
            options.DisconnectedCircuitMaxRetained = 100;
            options.MaxBufferedUnacknowledgedRenderBatches = 10;
        });

    // Memory Cache (requis par les services)
    builder.Services.AddMemoryCache();

    // Radzen components
    builder.Services.AddRadzenComponents();

    // CircuitHandler pour gestion gracieuse des déconnexions Blazor Server
    builder.Services.AddScoped<Microsoft.AspNetCore.Components.Server.Circuits.CircuitHandler, 
        KCCMaterialFlow.Host.Services.GracefulCircuitHandler>();

    // HttpContextAccessor pour CurrentUserService
    builder.Services.AddHttpContextAccessor();

    // Définition des assemblies de modules pour EF Core
    var moduleAssemblies = new Assembly[]
    {
        typeof(KCCMaterialFlow.Module.Shared.ModuleInfo).Assembly,
        typeof(KCCMaterialFlow.Module.BonEntree.ModuleInfo).Assembly,
        typeof(KCCMaterialFlow.Module.BonSortie.ModuleInfo).Assembly,
        typeof(KCCMaterialFlow.Module.Securite.ModuleInfo).Assembly
    };
    builder.Services.AddSingleton<IEnumerable<Assembly>>(moduleAssemblies);
    
    // Configurer les assemblies pour le DbContext (utilisé par la factory)
    KCCMaterialFlowDbContext.ConfiguredModuleAssemblies = moduleAssemblies;

    // Configuration Entity Framework avec SQL Server
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    
    // Enregistrer la DbContextFactory pour les opérations thread-safe
    builder.Services.AddDbContextFactory<KCCMaterialFlowDbContext>((sp, optionsBuilder) =>
    {
        optionsBuilder.UseSqlServer(connectionString, sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null);
            sqlOptions.CommandTimeout(30);
            // Utiliser SplitQuery pour éviter les warnings de performance sur les Include multiples
            sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        });

        // En développement, activer les logs détaillés
        if (builder.Environment.IsDevelopment())
        {
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
        }
    });
    
    // Enregistrer aussi le DbContext scopé pour les services qui en ont besoin
    builder.Services.AddScoped(sp => sp.GetRequiredService<IDbContextFactory<KCCMaterialFlowDbContext>>().CreateDbContext());
    
    // Factory abstraite pour découpler les modules de l'Infrastructure
    builder.Services.AddScoped<IAppDbContextFactory, AppDbContextFactory>();

    // Enregistrement des services Core
#pragma warning disable CA1416 // Validate platform compatibility
    if (OperatingSystem.IsWindows())
    {
        // Provider d'identité pour découpler Core de Microsoft.AspNetCore.Http
        builder.Services.AddScoped<IIdentityProvider, HttpContextIdentityProvider>();
        
        // Service de base pour Windows Authentication
        builder.Services.AddScoped<CurrentUserService>();
        
        // Service de simulation de rôles pour le développement (Singleton pour persister entre les requêtes)
        builder.Services.AddSingleton<KCCMaterialFlow.Host.Services.DevRoleSwitcherService>();
        
        // Décorateur qui ajoute les rôles de la base de données et le DevRoleSwitcher
        builder.Services.AddScoped<ICurrentUserService>(sp =>
        {
            CurrentUserService baseService = sp.GetRequiredService<CurrentUserService>();
            IDbContextFactory<KCCMaterialFlowDbContext> dbContextFactory = sp.GetRequiredService<IDbContextFactory<KCCMaterialFlowDbContext>>();
            IMemoryCache cache = sp.GetRequiredService<IMemoryCache>();
            var devRoleSwitcher = sp.GetRequiredService<KCCMaterialFlow.Host.Services.DevRoleSwitcherService>();
            ILogger<KCCMaterialFlow.Host.Services.DatabaseRoleEnricherService> logger = sp.GetRequiredService<ILogger<KCCMaterialFlow.Host.Services.DatabaseRoleEnricherService>>();
            return new KCCMaterialFlow.Host.Services.DatabaseRoleEnricherService(baseService, dbContextFactory, cache, devRoleSwitcher, logger);
        });
    }
#pragma warning restore CA1416 // Validate platform compatibility
    builder.Services.AddScoped<IWorkflowService, WorkflowService>();
    builder.Services.AddScoped<IEmailNotificationService, EmailNotificationService>();
    builder.Services.AddScoped<INotificationRejetService, NotificationRejetService>();
    builder.Services.AddScoped<IReferenceDataService, ReferenceDataService>();
    builder.Services.AddScoped<IQRCodeService, QRCodeService>();
    
    // Services partagés (implémentations Infrastructure - Clean Architecture)
    builder.Services.AddScoped<IUtilisateurService, KCCMaterialFlow.Infrastructure.Services.Shared.UtilisateurService>();
    builder.Services.AddScoped<IDepartementService, KCCMaterialFlow.Infrastructure.Services.Shared.DepartementService>();
    builder.Services.AddScoped<IBarriereService, KCCMaterialFlow.Infrastructure.Services.Shared.BarriereService>();
    builder.Services.AddScoped<IRoleService, KCCMaterialFlow.Infrastructure.Services.Shared.RoleService>();
    builder.Services.AddScoped<IPermissionService, KCCMaterialFlow.Infrastructure.Services.Shared.PermissionService>();
    builder.Services.AddScoped<IActiviteService, KCCMaterialFlow.Infrastructure.Services.Shared.ActiviteService>();
    builder.Services.AddScoped<IStatutService, KCCMaterialFlow.Infrastructure.Services.Shared.StatutService>();
    builder.Services.AddScoped<ITypeMaterielService, KCCMaterialFlow.Infrastructure.Services.Shared.TypeMaterielService>();
    builder.Services.AddScoped<IParametreSystemeService, KCCMaterialFlow.Infrastructure.Services.Shared.ParametreSystemeService>();
    builder.Services.AddScoped<IAuditLogService, KCCMaterialFlow.Infrastructure.Services.Shared.AuditLogService>();
    builder.Services.AddScoped<ICategorieSortieService, KCCMaterialFlow.Infrastructure.Services.Shared.CategorieSortieService>();
    builder.Services.AddScoped<IWorkflowConfigService, KCCMaterialFlow.Infrastructure.Services.Shared.WorkflowConfigService>();
    
    // INT-003: Service de recherche unifiée
    builder.Services.AddScoped<KCCMaterialFlow.Host.Services.IUnifiedSearchService, KCCMaterialFlow.Host.Services.UnifiedSearchService>();
    
    // INT-005: Service cross-module
    builder.Services.AddScoped<KCCMaterialFlow.Application.Interfaces.ICrossModuleService, KCCMaterialFlow.Host.Services.CrossModuleService>();

    // Service de notifications utilisateur
    builder.Services.AddScoped<KCCMaterialFlow.Host.Services.IUserNotificationService, KCCMaterialFlow.Host.Services.UserNotificationService>();

    // Service d'export Excel
    builder.Services.AddScoped<KCCMaterialFlow.Host.Services.IExcelExportService, KCCMaterialFlow.Host.Services.ExcelExportService>();

    // Enregistrement des modules
    var sharedModule = new KCCMaterialFlow.Module.Shared.SharedModule();
    var bonEntreeModule = new KCCMaterialFlow.Module.BonEntree.BonEntreeModule();
    var bonSortieModule = new KCCMaterialFlow.Module.BonSortie.BonSortieModule();
    var securiteModule = new KCCMaterialFlow.Module.Securite.SecuriteModule();
    
    sharedModule.ConfigureServices(builder.Services);
    bonEntreeModule.ConfigureServices(builder.Services);
    bonSortieModule.ConfigureServices(builder.Services);
    securiteModule.ConfigureServices(builder.Services);
    
    // Enregistrement des repositories (Clean Architecture - Host comme composition root)
    builder.Services.AddScoped<IBonEntreeRepository, BonEntreeRepository>();
    builder.Services.AddScoped<IBonSortieRepository, BonSortieRepository>();
    builder.Services.AddScoped<IScanRepository, ScanRepository>();
    builder.Services.AddScoped<IAnomalieRepository, AnomalieRepository>();

    // Windows Authentication (Negotiate/Kerberos)
    builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
        .AddNegotiate();

    // Transformation des claims : injecter les rôles depuis la base de données dans le ClaimsPrincipal
    // Cela permet à [Authorize(Roles = "...")] de fonctionner sans interroger Active Directory
    builder.Services.AddScoped<Microsoft.AspNetCore.Authentication.IClaimsTransformation, 
        KCCMaterialFlow.Host.Services.DatabaseRoleClaimsTransformation>();

    // Handler personnalisé qui vérifie les rôles via Claims au lieu de WindowsPrincipal.IsInRole()
    // Résout "The trust relationship between this workstation and the primary domain failed"
    builder.Services.AddSingleton<Microsoft.AspNetCore.Authorization.IAuthorizationHandler,
        KCCMaterialFlow.Host.Services.ClaimsRolesAuthorizationHandler>();

    builder.Services.AddAuthorization(options =>
    {
        // Politique par défaut : utilisateur authentifié
        options.FallbackPolicy = options.DefaultPolicy;

        // Toutes les politiques utilisent RequireAssertion avec les claims
        // (les rôles sont injectés dans le ClaimsPrincipal par DatabaseRoleClaimsTransformation)
        options.AddPolicy("AdminOnly", policy => policy.RequireAssertion(ctx => 
            ctx.User.HasClaim(c => c.Type == System.Security.Claims.ClaimTypes.Role && c.Value == "Admin")));
        options.AddPolicy("SuperviseurOrAbove", policy => policy.RequireAssertion(ctx => 
            ctx.User.HasClaim(c => c.Type == System.Security.Claims.ClaimTypes.Role && 
                (c.Value == "Admin" || c.Value == "Superviseur" || c.Value == "GM"))));
        options.AddPolicy("CanApprove", policy => policy.RequireAssertion(ctx => 
            ctx.User.HasClaim(c => c.Type == System.Security.Claims.ClaimTypes.Role && 
                (c.Value == "Admin" || c.Value == "Superviseur" || c.Value == "GM" || 
                 c.Value == "IT" || c.Value == "Environnement" || c.Value == "OPJ" || c.Value == "Identification"))));
        options.AddPolicy("Investigation", policy => policy.RequireAssertion(ctx => 
            ctx.User.HasClaim(c => c.Type == System.Security.Claims.ClaimTypes.Role && 
                (c.Value == "Admin" || c.Value == "Investigation"))));
    });

    var app = builder.Build();

    // Configure Serilog request logging
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    });

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        app.UseHsts();
    }

    app.UseHttpsRedirection();

    // Les fichiers statiques doivent être servis AVANT l'authentification
    app.UseStaticFiles();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseAntiforgery();

    app.MapStaticAssets().AllowAnonymous();
    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode()
        .AddAdditionalAssemblies(
            typeof(KCCMaterialFlow.Module.Shared.ModuleInfo).Assembly,
            typeof(KCCMaterialFlow.Module.BonEntree.Presentation.PresentationInfo).Assembly,
            typeof(KCCMaterialFlow.Module.BonSortie.Presentation.PresentationInfo).Assembly,
            typeof(KCCMaterialFlow.Module.Securite.Presentation.PresentationInfo).Assembly);

    // === API Endpoints pour l'export Excel ===
    app.MapGet("/api/export/bons-entree", async (KCCMaterialFlow.Host.Services.IExcelExportService exportService, CancellationToken ct) =>
    {
        var bytes = await exportService.ExportBonsEntreeAsync(ct);
        return Results.File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"BonsEntree_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
    }).RequireAuthorization();

    app.MapGet("/api/export/bons-sortie", async (KCCMaterialFlow.Host.Services.IExcelExportService exportService, CancellationToken ct) =>
    {
        var bytes = await exportService.ExportBonsSortieAsync(ct);
        return Results.File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"BonsSortie_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
    }).RequireAuthorization();

    app.MapGet("/api/export/historique", async (KCCMaterialFlow.Host.Services.IExcelExportService exportService, CancellationToken ct) =>
    {
        var bytes = await exportService.ExportHistoriqueAsync(ct);
        return Results.File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Historique_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
    }).RequireAuthorization();

    Log.Information("KCC Material Flow démarré avec succès");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "L'application a échoué au démarrage");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
