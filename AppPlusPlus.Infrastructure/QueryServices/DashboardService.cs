using Microsoft.EntityFrameworkCore;
using AppPlusPlus.Application.DTOs.Dashboard;
using AppPlusPlus.Application.Services.Dashboard;
using AppPlusPlus.Infrastructure.Persistence;

namespace AppPlusPlus.Infrastructure.QueryServices;

public class DashboardService : IDashboardService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    public DashboardService(IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<DashboardDataDto> GetDashboardDataAsync()
    {
        await using var ctx = await _dbFactory.CreateDbContextAsync();

        var today = DateTime.Today;
        var year = today.Year;
        var month = today.Month;

        var firstOfYear = new DateOnly(year, 1, 1);
        var lastOfYear = new DateOnly(year, 12, 31);
        var firstOfMonth = new DateOnly(year, month, 1);
        var todayDate = DateOnly.FromDateTime(today);

        // --- Factures annee & mois ---
        var factsYear = await ctx.Facts
            .Where(f => f.Date >= firstOfYear && f.Date <= lastOfYear)
            .ToListAsync();

        var factsMonth = factsYear.Where(f => f.Date.Month == month).ToList();
        var factsToday = factsYear.Where(f => f.Date == todayDate).ToList();
        var factsPrevMonth = factsYear.Where(f => f.Date.Month == month - 1).ToList();

        var venteMois = factsMonth.Sum(f => f.Total ?? 0);
        var ventePrevMois = factsPrevMonth.Sum(f => f.Total ?? 0);
        var venteToday = factsToday.Sum(f => f.Total ?? 0);
        var factsMonthCount = factsMonth.Count;

        // --- Appros annee & mois ---
        var approsYear = await ctx.Appros
            .Where(a => a.Date >= firstOfYear && a.Date <= lastOfYear)
            .ToListAsync();
        var approsMois = approsYear.Where(a => a.Date.Month == month).ToList();
        var totalAppros = approsMois.Sum(a => (a.PA ?? 0) * a.Qte);

        // --- Commandes en attente ---
        var cmdsEnAttente = await ctx.Cmds.CountAsync(c => c.Status != null && c.Status == 0);

        // --- Benefice reel = somme des BenTotal des appros du mois ---
        var benefice = approsMois.Sum(a => a.BenTotal ?? 0);

        // --- Tendances ---
        var trendVente = ventePrevMois > 0
            ? ((venteMois - ventePrevMois) / ventePrevMois * 100).ToString("+0;-0") + "%"
            : "";
        var trendVenteUp = venteMois >= ventePrevMois;

        // --- Graphique revenus + appros mensuels ---
        var monthNames = new[] { "Jan", "Fev", "Mar", "Avr", "Mai", "Jun",
                                 "Jul", "Aou", "Sep", "Oct", "Nov", "Dec" };
        var revenueData = Enumerable.Range(1, 12)
            .Select(m => new MonthlyDataDto
            {
                Month = monthNames[m - 1],
                Ventes = factsYear.Where(f => f.Date.Month == m).Sum(f => f.Total ?? 0),
                Appros = approsYear.Where(a => a.Date.Month == m).Sum(a => (a.PA ?? 0) * a.Qte)
            })
            .ToList();

        var totalRevenueYear = factsYear.Sum(f => f.Total ?? 0);

        // --- Donut statuts factures du mois ---
        var nbNew = factsMonth.Count(f => f.Status == 0);
        var nbValid = factsMonth.Count(f => f.Status == 1);
        var nbPaid = factsMonth.Count(f => f.Status == 2);
        var nbCancel = factsMonth.Count(f => f.Status == 3);
        var statusData = new List<StatusDataDto>
        {
            new() { Label = "Nouvelles", Count = nbNew },
            new() { Label = "Validees", Count = nbValid },
            new() { Label = "Payees", Count = nbPaid },
            new() { Label = "Annulees", Count = nbCancel },
        };

        // --- Alertes stock ---
        var articles = await ctx.Articles.ToListAsync();
        var lowStockArticles = articles
            .Where(a => (a.Qte ?? 0) <= a.Soeuil && a.Soeuil > 0)
            .OrderBy(a => a.Qte ?? 0)
            .Take(20)
            .Select(a => new StockAlertDto
            {
                Description = a.Description,
                Qte = a.Qte ?? 0,
                Seuil = a.Soeuil,
                Pct = a.Soeuil > 0 ? (double)((a.Qte ?? 0) / a.Soeuil) * 100 : 0
            })
            .ToList();

        // --- Top articles vendus ce mois ---
        var factIds = factsMonth.Select(f => f.Id).ToHashSet();
        var detailsMonth = await ctx.FactDetails
            .Where(d => d.IdFact != null && factIds.Contains(d.IdFact.Value))
            .ToListAsync();

        var articleDict = articles.ToDictionary(a => a.IdArticle, a => a.Description);
        var topArticles = detailsMonth
            .Where(d => d.IdArticle != null)
            .GroupBy(d => d.IdArticle!)
            .Select(g => new
            {
                IdArticle = g.Key,
                QteVendue = g.Sum(d => d.Qte ?? 0),
                Montant = g.Sum(d => (d.Qte ?? 0) * (d.Pu ?? 0))
            })
            .OrderByDescending(g => g.Montant)
            .Take(8)
            .Select((g, i) => new TopArticleDto
            {
                Rank = i + 1,
                Description = articleDict.GetValueOrDefault(g.IdArticle, g.IdArticle),
                QteVendue = g.QteVendue,
                Montant = g.Montant
            })
            .ToList();

        // --- Dernieres factures ---
        var recentFacts = factsYear
            .OrderByDescending(f => f.DateSys)
            .Take(8)
            .Select(f => new RecentFactDto
            {
                Id = f.Id,
                Client = string.IsNullOrWhiteSpace(f.DescriptionName) ? "\u2014" : f.DescriptionName,
                Date = f.Date,
                Total = f.Total ?? 0,
                StatusLabel = f.Status switch
                {
                    0 => "Nouvelle",
                    1 => "Validee",
                    2 => "Payee",
                    3 => "Annulee",
                    _ => "\u2014"
                }
            })
            .ToList();

        // --- Statistiques rapides ---
        var totalClients = await ctx.Customers.CountAsync();
        var clientsPerm = await ctx.Customers.CountAsync(c => c.IsPermanent == true);
        var totalArticles = articles.Count;
        var totalCommandes = await ctx.Cmds.CountAsync();

        return new DashboardDataDto
        {
            // KPIs
            VentesToday = venteToday,
            FactsTodayCount = factsToday.Count,
            TotalApprosMonth = totalAppros,
            ApprosMonthCount = approsMois.Count,
            Benefice = benefice,
            VentesMois = venteMois,
            TrendVente = trendVente,
            TrendVenteUp = trendVenteUp,
            TotalRevenueYear = totalRevenueYear,
            FactsMonthCount = factsMonthCount,
            CmdsEnAttente = cmdsEnAttente,

            // Chart data
            RevenueData = revenueData,
            StatusData = statusData,

            // Lists
            TopArticles = topArticles,
            LowStockArticles = lowStockArticles,
            RecentFacts = recentFacts,

            // Quick stats
            TotalClients = totalClients,
            ClientsPermanents = clientsPerm,
            TotalArticles = totalArticles,
            TotalCommandes = totalCommandes,
        };
    }
}
