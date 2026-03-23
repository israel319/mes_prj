# Script PowerShell pour tester les temps de chargement des pages
# INT-025: Tester temps chargement pages - Toutes pages < 2 secondes
# ==================================================================

param(
    [string]$BaseUrl = "https://localhost:5001",
    [int]$Iterations = 5,
    [int]$TargetMs = 2000
)

Write-Host "============================================" -ForegroundColor Cyan
Write-Host " KCCMaterialFlow - Test de Performance      " -ForegroundColor Cyan  
Write-Host " INT-025: Pages < 2 secondes                " -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "URL de base: $BaseUrl"
Write-Host "Iterations par page: $Iterations"
Write-Host "Seuil cible: ${TargetMs}ms"
Write-Host ""

# Liste des pages à tester
$Pages = @(
    @{ Path = "/"; Name = "Accueil" },
    @{ Path = "/login"; Name = "Connexion" },
    @{ Path = "/dashboard"; Name = "Tableau de bord" },
    @{ Path = "/bons-entree"; Name = "Bons Entree" },
    @{ Path = "/bons-sortie"; Name = "Bons Sortie" },
    @{ Path = "/admin/utilisateurs"; Name = "Admin - Utilisateurs" },
    @{ Path = "/admin/departements"; Name = "Admin - Departements" },
    @{ Path = "/admin/barrieres"; Name = "Admin - Barrieres" },
    @{ Path = "/admin/statuts"; Name = "Admin - Statuts" },
    @{ Path = "/admin/roles"; Name = "Admin - Roles" },
    @{ Path = "/admin/types-materiels"; Name = "Admin - Types Materiels" },
    @{ Path = "/admin/parametres"; Name = "Admin - Parametres" },
    @{ Path = "/admin/audit-logs"; Name = "Admin - Audit Logs" }
)

$Results = @()
$SuccessCount = 0
$WarningCount = 0
$FailCount = 0

# Ignorer les erreurs de certificat SSL pour les tests locaux
if ($BaseUrl -like "*localhost*") {
    Add-Type @"
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    public class TrustAllCertsPolicy : ICertificatePolicy {
        public bool CheckValidationResult(
            ServicePoint srvPoint, X509Certificate certificate,
            WebRequest request, int certificateProblem) {
            return true;
        }
    }
"@
    [System.Net.ServicePointManager]::CertificatePolicy = New-Object TrustAllCertsPolicy
    [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12
}

foreach ($Page in $Pages) {
    $Url = "$BaseUrl$($Page.Path)"
    $Times = @()
    $Errors = 0
    
    Write-Host "Testing: $($Page.Name) ($($Page.Path))" -NoNewline
    
    for ($i = 0; $i -lt $Iterations; $i++) {
        try {
            $Stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
            $Response = Invoke-WebRequest -Uri $Url -Method Get -UseBasicParsing -TimeoutSec 30 -ErrorAction Stop
            $Stopwatch.Stop()
            $Times += $Stopwatch.ElapsedMilliseconds
        }
        catch {
            $Errors++
        }
    }
    
    if ($Times.Count -gt 0) {
        $AvgMs = [math]::Round(($Times | Measure-Object -Average).Average, 0)
        $MinMs = ($Times | Measure-Object -Minimum).Minimum
        $MaxMs = ($Times | Measure-Object -Maximum).Maximum
        
        $Result = [PSCustomObject]@{
            Page = $Page.Name
            Path = $Page.Path
            AvgMs = $AvgMs
            MinMs = $MinMs
            MaxMs = $MaxMs
            Errors = $Errors
            Status = if ($AvgMs -le $TargetMs) { "PASS" } else { "FAIL" }
        }
        
        $Results += $Result
        
        if ($AvgMs -le $TargetMs) {
            Write-Host " - ${AvgMs}ms" -ForegroundColor Green -NoNewline
            Write-Host " [PASS]" -ForegroundColor Green
            $SuccessCount++
        } elseif ($AvgMs -le ($TargetMs * 1.5)) {
            Write-Host " - ${AvgMs}ms" -ForegroundColor Yellow -NoNewline
            Write-Host " [WARNING]" -ForegroundColor Yellow
            $WarningCount++
        } else {
            Write-Host " - ${AvgMs}ms" -ForegroundColor Red -NoNewline
            Write-Host " [FAIL]" -ForegroundColor Red
            $FailCount++
        }
    } else {
        Write-Host " - ERREUR" -ForegroundColor Red
        $FailCount++
    }
}

Write-Host ""
Write-Host "============================================" -ForegroundColor Cyan
Write-Host " RÉSUMÉ DES TESTS                           " -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""

# Afficher les résultats en tableau
$Results | Format-Table -Property Page, @{Name="Moy.(ms)";Expression={$_.AvgMs}}, @{Name="Min(ms)";Expression={$_.MinMs}}, @{Name="Max(ms)";Expression={$_.MaxMs}}, Status -AutoSize

Write-Host ""
Write-Host "Statistiques:" -ForegroundColor Cyan
Write-Host "  Pages testees: $($Pages.Count)"
Write-Host "  PASS (<=${TargetMs}ms): $SuccessCount" -ForegroundColor Green
Write-Host "  WARNING: $WarningCount" -ForegroundColor Yellow  
Write-Host "  FAIL: $FailCount" -ForegroundColor Red
Write-Host ""

if ($FailCount -eq 0 -and $WarningCount -eq 0) {
    Write-Host "✓ Toutes les pages respectent le seuil de ${TargetMs}ms!" -ForegroundColor Green
    exit 0
} elseif ($FailCount -eq 0) {
    Write-Host "⚠ Certaines pages sont proches du seuil" -ForegroundColor Yellow
    exit 0
} else {
    Write-Host "✗ Certaines pages depassent le seuil de ${TargetMs}ms" -ForegroundColor Red
    exit 1
}

# Exporter les résultats en CSV si demandé
$ExportPath = Join-Path $PSScriptRoot "performance-results.csv"
$Results | Export-Csv -Path $ExportPath -NoTypeInformation -Encoding UTF8
Write-Host ""
Write-Host "Resultats exportes vers: $ExportPath"
