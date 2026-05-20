$f = "c:\Users\ikasa\source\repos\Materiels\src\KCCMaterialFlow.Host\Resources\SharedResource.fr.resx"
$xml = [xml](Get-Content $f -Raw)
# Detect English-looking values: contain a-z and a vowel, no French accents
$rxFrench = '[éèêëàâäîïôöùûüç]'
$rxEnglish = '\b(the|and|of|to|for|with|select|search|please|new|edit|all|none|created|status|enter|click|reason|submit|approve|reject|day|month|year|saved|deleted|loading|error|success|warning|by|from|now|empty|invalid|valid|required|optional|pending|approved|rejected|returned|cancelled|completed|history|details|loan|loans|exit|entry|entries|dashboard|home|filter|clear|cancel|save|add|delete|create|update|modify|view|export|import|print|preview|next|previous|back|finish|continue|step|stage|account|user|users|admin|manager|department|company|site|materials?|description|comment|name|email|phone|address|date|time|number|code|type|level|active|inactive|inactive|notification|notifications|approval|approvals|workflow|workflows|configuration|settings)\b'
$enOnly = $xml.root.data | Where-Object {
  $_.value -and ($_.value -notmatch $rxFrench) -and ($_.value -match $rxEnglish)
}
Write-Host "Entries needing translation: $($enOnly.Count)"
$enOnly | ForEach-Object { "$($_.name)`t$($_.value)" } | Out-File -FilePath "c:\Users\ikasa\source\repos\Materiels\src\to_translate.tsv" -Encoding UTF8
Write-Host "Saved to to_translate.tsv"
