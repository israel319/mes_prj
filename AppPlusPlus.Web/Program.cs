using MudBlazor.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AppPlusPlus.Web.Components;
using AppPlusPlus.Infrastructure;
using AppPlusPlus.Infrastructure.Persistence;
using AppPlusPlus.Application;
using AppPlusPlus.Application.Services.Parametres;
using AppPlusPlus.Application.Services.Administration;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/login";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(12);
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

builder.Services.AddMudServices();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Détails des erreurs Blazor Server (visible dans F12 Console)
builder.Services.AddServerSideBlazor().AddCircuitOptions(o => o.DetailedErrors = true);

var app = builder.Build();

// Charger les paramètres de l'application au démarrage
using (var scope = app.Services.CreateScope())
{
    var settings = scope.ServiceProvider.GetRequiredService<IAppSettingsService>();
    await settings.LoadAsync();
}

// Initialiser les permissions par défaut pour les rôles qui n'en ont pas encore
using (var scope = app.Services.CreateScope())
{
    var roleService = scope.ServiceProvider.GetRequiredService<IRoleService>();
    await roleService.InitializeDefaultPermissionsAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapPost("/auth/login", async (
    [FromForm] string login,
    [FromForm] string password,
    [FromForm] string? returnUrl,
    IDbContextFactory<AppDbContext> dbFactory,
    HttpContext httpContext) =>
{
    var safeReturnUrl = "/";
    if (!string.IsNullOrWhiteSpace(returnUrl) && returnUrl.StartsWith('/'))
    {
        safeReturnUrl = returnUrl;
    }

    await using var ctx = await dbFactory.CreateDbContextAsync();
    var user = await ctx.Users
        .Include(u => u.Role)
        .FirstOrDefaultAsync(u => u.Login == login && u.Password == password && u.Activated == true);

    if (user is null)
    {
        return Results.Redirect($"/login?error=1&returnUrl={Uri.EscapeDataString(safeReturnUrl)}");
    }

    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, user.Login),
        new(ClaimTypes.Name, user.Name ?? user.Login),
        new("login", user.Login),
        new("roleId", (user.RoleId ?? 0).ToString())
    };

    if (!string.IsNullOrWhiteSpace(user.Role?.DescriptionRole))
    {
        claims.Add(new Claim(ClaimTypes.Role, user.Role.DescriptionRole));
    }

    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var principal = new ClaimsPrincipal(identity);

    await httpContext.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        principal,
        new AuthenticationProperties { IsPersistent = true });

    return Results.Redirect(safeReturnUrl);
}).AllowAnonymous();

app.MapGet("/auth/logout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
}).AllowAnonymous();

app.Run();
