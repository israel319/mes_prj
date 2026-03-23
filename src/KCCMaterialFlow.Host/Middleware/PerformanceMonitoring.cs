using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace KCCMaterialFlow.Host.Middleware;

/// <summary>
/// Middleware pour mesurer et logger les temps de réponse des requêtes HTTP.
/// INT-025: Tester temps chargement pages - Toutes pages < 2 secondes
/// </summary>
public class PerformanceMonitoringMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PerformanceMonitoringMiddleware> _logger;
    
    /// <summary>
    /// Seuil d'alerte en millisecondes (2 secondes)
    /// </summary>
    private const int WarningThresholdMs = 2000;
    
    /// <summary>
    /// Seuil critique en millisecondes (5 secondes)
    /// </summary>
    private const int CriticalThresholdMs = 5000;

    public PerformanceMonitoringMiddleware(
        RequestDelegate next,
        ILogger<PerformanceMonitoringMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestPath = context.Request.Path.Value ?? "/";
        var requestMethod = context.Request.Method;

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            // Ne pas logger les requêtes statiques (css, js, images)
            if (!IsStaticResource(requestPath))
            {
                LogRequestTiming(requestPath, requestMethod, elapsedMs, context.Response.StatusCode);
            }

            // Ajouter le header de timing (utile pour les tests de performance)
            context.Response.Headers["X-Response-Time-Ms"] = elapsedMs.ToString();
        }
    }

    private void LogRequestTiming(string path, string method, long elapsedMs, int statusCode)
    {
        if (elapsedMs >= CriticalThresholdMs)
        {
            _logger.LogError(
                "CRITICAL PERFORMANCE: {Method} {Path} took {ElapsedMs}ms (Status: {StatusCode}) - Exceeds critical threshold of {Threshold}ms",
                method, path, elapsedMs, statusCode, CriticalThresholdMs);
        }
        else if (elapsedMs >= WarningThresholdMs)
        {
            _logger.LogWarning(
                "SLOW REQUEST: {Method} {Path} took {ElapsedMs}ms (Status: {StatusCode}) - Exceeds target of {Threshold}ms",
                method, path, elapsedMs, statusCode, WarningThresholdMs);
        }
        else
        {
            _logger.LogDebug(
                "Request: {Method} {Path} completed in {ElapsedMs}ms (Status: {StatusCode})",
                method, path, elapsedMs, statusCode);
        }
    }

    private static bool IsStaticResource(string path)
    {
        var staticExtensions = new[] { ".css", ".js", ".png", ".jpg", ".jpeg", ".gif", ".svg", ".ico", ".woff", ".woff2", ".ttf", ".eot" };
        return staticExtensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
            || path.StartsWith("/_blazor", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/_framework", StringComparison.OrdinalIgnoreCase);
    }
}

/// <summary>
/// Extensions pour enregistrer le middleware
/// </summary>
public static class PerformanceMonitoringMiddlewareExtensions
{
    public static IApplicationBuilder UsePerformanceMonitoring(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<PerformanceMonitoringMiddleware>();
    }
}

/// <summary>
/// Service pour collecter les métriques de performance
/// </summary>
public class PerformanceMetricsCollector
{
    private readonly ConcurrentQueue<RequestMetric> _recentMetrics = new();
    private readonly int _maxMetrics = 1000;

    public void RecordMetric(string path, string method, long elapsedMs, int statusCode)
    {
        var metric = new RequestMetric
        {
            Path = path,
            Method = method,
            ElapsedMs = elapsedMs,
            StatusCode = statusCode,
            Timestamp = DateTime.UtcNow
        };

        _recentMetrics.Enqueue(metric);

        // Limiter la taille de la queue
        while (_recentMetrics.Count > _maxMetrics)
        {
            _recentMetrics.TryDequeue(out _);
        }
    }

    public PerformanceSummary GetSummary(TimeSpan? period = null)
    {
        var cutoff = period.HasValue 
            ? DateTime.UtcNow.Subtract(period.Value) 
            : DateTime.MinValue;

        var metrics = _recentMetrics
            .Where(m => m.Timestamp >= cutoff)
            .ToList();

        if (metrics.Count == 0)
        {
            return new PerformanceSummary { TotalRequests = 0 };
        }

        return new PerformanceSummary
        {
            TotalRequests = metrics.Count,
            AverageResponseTimeMs = metrics.Average(m => m.ElapsedMs),
            MaxResponseTimeMs = metrics.Max(m => m.ElapsedMs),
            MinResponseTimeMs = metrics.Min(m => m.ElapsedMs),
            P95ResponseTimeMs = CalculatePercentile(metrics.Select(m => m.ElapsedMs).ToList(), 95),
            P99ResponseTimeMs = CalculatePercentile(metrics.Select(m => m.ElapsedMs).ToList(), 99),
            RequestsOver2Seconds = metrics.Count(m => m.ElapsedMs > 2000),
            ErrorCount = metrics.Count(m => m.StatusCode >= 400),
            PathStats = metrics
                .GroupBy(m => m.Path)
                .Select(g => new PathPerformanceStats
                {
                    Path = g.Key,
                    Count = g.Count(),
                    AverageMs = g.Average(m => m.ElapsedMs),
                    MaxMs = g.Max(m => m.ElapsedMs)
                })
                .OrderByDescending(p => p.AverageMs)
                .Take(10)
                .ToList()
        };
    }

    private static long CalculatePercentile(List<long> values, int percentile)
    {
        if (values.Count == 0) return 0;
        
        values.Sort();
        var index = (int)Math.Ceiling(percentile / 100.0 * values.Count) - 1;
        return values[Math.Max(0, index)];
    }
}

public record RequestMetric
{
    public string Path { get; init; } = string.Empty;
    public string Method { get; init; } = string.Empty;
    public long ElapsedMs { get; init; }
    public int StatusCode { get; init; }
    public DateTime Timestamp { get; init; }
}

public record PerformanceSummary
{
    public int TotalRequests { get; init; }
    public double AverageResponseTimeMs { get; init; }
    public long MaxResponseTimeMs { get; init; }
    public long MinResponseTimeMs { get; init; }
    public long P95ResponseTimeMs { get; init; }
    public long P99ResponseTimeMs { get; init; }
    public int RequestsOver2Seconds { get; init; }
    public int ErrorCount { get; init; }
    public List<PathPerformanceStats> PathStats { get; init; } = [];
}

public record PathPerformanceStats
{
    public string Path { get; init; } = string.Empty;
    public int Count { get; init; }
    public double AverageMs { get; init; }
    public long MaxMs { get; init; }
}
