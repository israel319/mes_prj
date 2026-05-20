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

    // ── Cache de rôles et tracker de changements ────────────────────────
    builder.Services.AddSingleton<KCCMaterialFlow.Host.Services.UserRoleCacheService>();
    builder.Services.AddSingleton<KCCMaterialFlow.Host.Services.UserRoleChangeTracker>();

    // ── ICurrentUserService (Windows-only) ───────────────────────────────
#pragma warning disable CA1416 // Validate platform compatibility
    if (OperatingSystem.IsWindows())
        builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
#pragma warning restore CA1416 // Validate platform compatibility

    // Bootstrap : promeut au démarrage les Employees listés dans InitialSuperAdminMatricules
    builder.Services.AddHostedService<KCCMaterialFlow.Host.Services.SuperAdminBootstrapService>();

    // DEV ONLY : auto-import DATA.xlsx au démarrage si T_Employees est quasi-vide
    if (builder.Environment.IsDevelopment())
    {
        builder.Services.AddHostedService<KCCMaterialFlow.Host.Services.DevDataAutoImportService>();
    }

    // ── Localisation FR / EN ────────────────────────────────────────────
    builder.Services.AddLocalization();

    // ── Host-specific services ──────────────────────────────────────────
    builder.Services.AddScoped<ICrossModuleService, KCCMaterialFlow.Host.Services.CrossModuleService>();
    builder.Services.AddScoped<KCCMaterialFlow.Host.Services.IUserNotificationService, KCCMaterialFlow.Host.Services.UserNotificationService>();
    builder.Services.AddScoped<KCCMaterialFlow.Host.Services.IExcelExportService, KCCMaterialFlow.Host.Services.ExcelExportService>();
    builder.Services.AddScoped<IImpersonationService, KCCMaterialFlow.Host.Services.ImpersonationService>();

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

        // ── Nouvelles policies (Employee = Utilisateur) ─────────────────────
        // SuperAdminOnly : NiveauAdmin = SuperAdmin
        options.AddPolicy("SuperAdminOnly", policy => policy.RequireAssertion(ctx =>
            ctx.User.HasClaim(c => c.Type == System.Security.Claims.ClaimTypes.Role && c.Value == "SuperAdmin")));

        // AdminOrAbove : Admin OU SuperAdmin (effectif)
        options.AddPolicy("AdminOrAbove", policy => policy.RequireAssertion(ctx =>
            ctx.User.HasClaim(c => c.Type == System.Security.Claims.ClaimTypes.Role &&
                (c.Value == "Admin" || c.Value == "SuperAdmin"))));

        // ── Anciennes policies (rétro-compat — autorisent désormais Admin/SuperAdmin) ──
        // À supprimer en Phase 4 (cleanup des [Authorize(Policy=...)] dans les pages)
        options.AddPolicy("AdminOnly", policy => policy.RequireAssertion(ctx =>
            ctx.User.HasClaim(c => c.Type == System.Security.Claims.ClaimTypes.Role &&
                (c.Value == "Admin" || c.Value == "SuperAdmin"))));
        options.AddPolicy("RealAdminOnly", policy => policy.RequireAssertion(ctx =>
            ctx.User.HasClaim(c => c.Type == System.Security.Claims.ClaimTypes.Role &&
                (c.Value == "Admin" || c.Value == "SuperAdmin"))));
        options.AddPolicy("RealSuperAdminOnly", policy => policy.RequireAssertion(ctx =>
            ctx.User.HasClaim(c => c.Type == System.Security.Claims.ClaimTypes.Role && c.Value == "SuperAdmin")));
        options.AddPolicy("SuperviseurOrAbove", policy => policy.RequireAssertion(ctx =>
            ctx.User.HasClaim(c => c.Type == System.Security.Claims.ClaimTypes.Role &&
                (c.Value == "Admin" || c.Value == "SuperAdmin"))));
        options.AddPolicy("CanApprove", policy => policy.RequireAssertion(ctx =>
            ctx.User.HasClaim(c => c.Type == System.Security.Claims.ClaimTypes.Role &&
                (c.Value == "Admin" || c.Value == "SuperAdmin"))));
        options.AddPolicy("Investigation", policy => policy.RequireAssertion(ctx =>
            ctx.User.HasClaim(c => c.Type == System.Security.Claims.ClaimTypes.Role &&
                (c.Value == "Admin" || c.Value == "SuperAdmin"))));
    });

    var app = builder.Build();

    // Apply pending EF Core migrations on startup
    try
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<KCCMaterialFlowDbContext>();
            Log.Information("Applying pending EF Core migrations...");
            await dbContext.Database.MigrateAsync();
            Log.Information("Database migrations applied successfully");
        }
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Failed to apply database migrations");
        throw;
    }

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

    // ── Fichiers statiques : servis AVANT l'auth pour éviter le 401 Negotiate ──
    app.UseStaticFiles();
    app.UseHttpsRedirection();

    // ── Localisation (cookie .AspNetCore.Culture, défaut = fr) ──────────
    var supportedCultures = new[] { "fr", "en" };
    app.UseRequestLocalization(new Microsoft.AspNetCore.Builder.RequestLocalizationOptions()
        .SetDefaultCulture("fr")
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures));

    // Rediriger les 403 (Forbidden) vers la page Access Denied (AVANT auth pour intercepter)
    app.UseStatusCodePages(context =>
    {
        if (context.HttpContext.Response.StatusCode == 403)
        {
            context.HttpContext.Response.Redirect("/access-denied");
        }
        return Task.CompletedTask;
    });

    app.UseAuthentication();
    app.UseAuthorization();

    // Vérifier en BD si l'utilisateur est actif AVANT de charger Blazor
    app.UseMiddleware<KCCMaterialFlow.Host.Services.UserAccessMiddleware>();

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

    // === Endpoints impersonation ===
    app.MapPost("/api/impersonation/start", async (HttpContext httpCtx) =>
    {
        var form = await httpCtx.Request.ReadFormAsync();
        var matricule = form["matricule"].FirstOrDefault()
                     ?? form["employeeCode"].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(matricule))
            return Results.BadRequest("Matricule manquant.");

        var userLogin  = form["userLogin"].FirstOrDefault() ?? "";
        var realLogin  = httpCtx.User.Identity?.Name ?? "";
        var opts = new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,
            IsEssential = true,
            Path = "/"
        };
        httpCtx.Response.Cookies.Append("imp_active",    "1",        opts);
        httpCtx.Response.Cookies.Append("imp_matricule", matricule,  opts);
        httpCtx.Response.Cookies.Append("imp_login",     userLogin,  opts);
        httpCtx.Response.Cookies.Append("imp_reallogin", realLogin,  opts);

        return Results.Redirect("/");
    }).RequireAuthorization("RealSuperAdminOnly").DisableAntiforgery();

    app.MapPost("/api/impersonation/stop", (HttpContext httpCtx) =>
    {
        httpCtx.Response.Cookies.Delete("imp_active");
        httpCtx.Response.Cookies.Delete("imp_matricule");
        httpCtx.Response.Cookies.Delete("imp_login");
        httpCtx.Response.Cookies.Delete("imp_reallogin");
        return Results.Redirect("/");
    }).RequireAuthorization().DisableAntiforgery();

    // === Changement de langue (FR / EN) ===
    app.MapGet("/api/set-language", (HttpContext httpCtx, string lang, string? returnUrl) =>
    {
        var culture = new[] { "fr", "en" }.Contains(lang, StringComparer.OrdinalIgnoreCase)
            ? lang.ToLowerInvariant()
            : "fr";
        httpCtx.Response.Cookies.Append(
            Microsoft.AspNetCore.Localization.CookieRequestCultureProvider.DefaultCookieName,
            Microsoft.AspNetCore.Localization.CookieRequestCultureProvider.MakeCookieValue(
                new Microsoft.AspNetCore.Localization.RequestCulture(culture)),
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true,
                SameSite = SameSiteMode.Lax,
                HttpOnly = false,
                Path = "/"
            });
        var redirect = string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl;
        return Results.Redirect(redirect);
    }).AllowAnonymous();

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
