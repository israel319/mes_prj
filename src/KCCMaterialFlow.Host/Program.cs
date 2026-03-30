using System.Runtime.Versioning;
using KCCMaterialFlow.Application;
using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Infrastructure;
using KCCMaterialFlow.Infrastructure.Data;
using KCCMaterialFlow.Infrastructure.Services;
using KCCMaterialFlow.Host.Components;
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

    // ── Clean Architecture DI ───────────────────────────────────────────
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    // ── ICurrentUserService (Windows-only + decorator pattern) ───────────
    // Stays in Program.cs due to CA1416 platform guard and decorator wiring
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

    // ── Host-specific services ──────────────────────────────────────────
    builder.Services.AddScoped<KCCMaterialFlow.Host.Services.IUnifiedSearchService, KCCMaterialFlow.Host.Services.UnifiedSearchService>();
    builder.Services.AddScoped<ICrossModuleService, KCCMaterialFlow.Host.Services.CrossModuleService>();
    builder.Services.AddScoped<KCCMaterialFlow.Host.Services.IUserNotificationService, KCCMaterialFlow.Host.Services.UserNotificationService>();
    builder.Services.AddScoped<KCCMaterialFlow.Host.Services.IExcelExportService, KCCMaterialFlow.Host.Services.ExcelExportService>();

    // ── Windows Authentication (Negotiate/Kerberos) ─────────────────────
    builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
        .AddNegotiate();

    // Transformation des claims : injecter les rôles depuis la base de données dans le ClaimsPrincipal
    builder.Services.AddScoped<Microsoft.AspNetCore.Authentication.IClaimsTransformation,
        KCCMaterialFlow.Host.Services.DatabaseRoleClaimsTransformation>();

    // Handler personnalisé qui vérifie les rôles via Claims au lieu de WindowsPrincipal.IsInRole()
    builder.Services.AddSingleton<Microsoft.AspNetCore.Authorization.IAuthorizationHandler,
        KCCMaterialFlow.Host.Services.ClaimsRolesAuthorizationHandler>();

    builder.Services.AddAuthorization(options =>
    {
        // Politique par défaut : utilisateur authentifié
        options.FallbackPolicy = options.DefaultPolicy;

        // Toutes les politiques utilisent RequireAssertion avec les claims
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
        .AddInteractiveServerRenderMode();

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
