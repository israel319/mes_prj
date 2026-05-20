$f = "c:\Users\ikasa\source\repos\Materiels\src\KCCMaterialFlow.Host\Resources\SharedResource.fr.resx"
$xml = [xml](Get-Content $f -Raw)
$nodes = $xml.root.data
Write-Host "Total data nodes: $($nodes.Count)"
$dups = $nodes | Group-Object name | Where-Object { $_.Count -gt 1 }
Write-Host "Duplicate keys: $($dups.Count)"
foreach ($d in $dups | Select-Object -First 5) {
    Write-Host "  $($d.Name) appears $($d.Count) times"
    foreach ($n in $d.Group) {
        Write-Host "    -> '$($n.value)'"
    }
}
$bem = $nodes | Where-Object { $_.name -eq "Dash_NewBEM" }
Write-Host "Dash_NewBEM count: $($bem.Count)"
foreach ($n in $bem) { Write-Host "  Value: '$($n.value)'" }
