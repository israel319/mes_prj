using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.Extensions.Logging;
using Radzen;
using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Application.Features.BonEntree.DTOs;
using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Host.Components.Pages.BonEntree;

public partial class BonEntreeNew
{
    [Inject] private IReferenceDataService ReferenceDataService { get; set; } = default!;
    [Inject] private IRaisonEntreeService RaisonEntreeService { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] private ILogger<BonEntreeNew> Logger { get; set; } = default!;

    private CreateBonEntreeRequest request = new()
    {
        Materiels = new List<MaterielRequest>(),
        DateExpiration = DateTime.Today.AddDays(30)
    };

    private bool isSaving;

    // Listes de reference
    private IEnumerable<Compagnie> compagnies = [];
    private IEnumerable<Contrat> contrats = [];
    private IEnumerable<Departement> departements = [];
    private IEnumerable<Site> sites = [];
    private IReadOnlyList<RaisonEntree> raisonsEntree = [];

    // Selections
    private int? selectedCompagnieId;
    private int? selectedContratId;
    private int? selectedDepartementId;
    private int? selectedProvenanceId;
    private int? selectedDestinationId;
    private int? selectedRaisonEntreeId;
    private string? selectedRaisonEntreeCode;
    private string autreMotifEntreeTexte = string.Empty;

    // Contrats filtrés par compagnie sélectionnée
    private IEnumerable<Contrat> contratsFiltered =>
        selectedCompagnieId.HasValue
            ? contrats.Where(c => c.CompagnieId == selectedCompagnieId.Value)
            : Enumerable.Empty<Contrat>();

    // Liste filtrée des destinations (exclut le site sélectionné en FROM)
    private IEnumerable<Site> availableDestinations =>
        selectedProvenanceId.HasValue
            ? sites.Where(s => s.Id != selectedProvenanceId.Value)
            : sites;

    [Inject] private ICurrentUserService CurrentUserService { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        // Vérifier l'activité BEM_CREER
        if (!CurrentUserService.HasActivite("BEM_CREER"))
        {
            Navigation.NavigateTo("/", replace: true);
            return;
        }

        // Charger les donnees de reference
        compagnies = await ReferenceDataService.GetCompagniesAsync();
        contrats = await ReferenceDataService.GetContratsAsync();
        departements = await ReferenceDataService.GetDepartementsAsync();
        sites = await ReferenceDataService.GetSitesAsync();
        raisonsEntree = await RaisonEntreeService.GetAllActiveAsync();
    }

    private void OnCompagnieChanged(object value)
    {
        // Réinitialiser le contrat sélectionné quand la compagnie change
        selectedContratId = null;
        request.ContratId = null;
        request.NumeroContrat = string.Empty;

        if (value is int compagnieId)
        {
            var compagnie = compagnies.FirstOrDefault(c => c.Id == compagnieId);
            if (compagnie != null)
            {
                request.NomCompagnie = compagnie.Nom;
                request.EmailContractant = compagnie.Email ?? string.Empty;
                request.SiteManager = compagnie.SiteManager ?? string.Empty;
            }
        }
        else
        {
            request.NomCompagnie = string.Empty;
            request.EmailContractant = string.Empty;
            request.SiteManager = string.Empty;
        }
    }

    private void OnContratChanged(object value)
    {
        if (value is int contratId)
        {
            var contrat = contrats.FirstOrDefault(c => c.Id == contratId);
            if (contrat != null)
            {
                request.ContratId = contrat.Id;
                request.NumeroContrat = contrat.PoNumber;
            }
        }
        else
        {
            request.ContratId = null;
            request.NumeroContrat = string.Empty;
        }
    }

    private void OnDepartementChanged(object value)
    {
        if (value is int departementId)
        {
            var dept = departements.FirstOrDefault(d => d.Id == departementId);
            request.HostDepartment = dept?.NomDepartement ?? string.Empty;
            request.DepartementId = dept?.Id;
        }
        else
        {
            request.HostDepartment = string.Empty;
            request.DepartementId = null;
        }
    }

    private void OnRaisonEntreeChanged(object value)
    {
        if (value is int raisonId)
        {
            var raison = raisonsEntree.FirstOrDefault(r => r.Id == raisonId);
            request.RaisonEntreeId = raisonId;
            selectedRaisonEntreeCode = raison?.Code;
            request.ReasonOnSite = raison?.Nom ?? string.Empty;
        }
        else
        {
            request.RaisonEntreeId = null;
            selectedRaisonEntreeCode = null;
            request.ReasonOnSite = string.Empty;
            autreMotifEntreeTexte = string.Empty;
        }
    }

    private void OnAutreMotifChanged(string value)
    {
        autreMotifEntreeTexte = value;
        request.ReasonOnSite = string.IsNullOrWhiteSpace(value)
            ? "Autre"
            : $"Autre: {value}";
    }

    private void OnProvenanceChanged(object value)
    {
        if (value is int siteId)
        {
            var site = sites.FirstOrDefault(s => s.Id == siteId);
            request.Provenance = site?.Nom ?? string.Empty;

            // Si la destination actuelle est la même que la provenance, la réinitialiser
            if (selectedDestinationId == siteId)
            {
                selectedDestinationId = null;
                request.Destination = string.Empty;
            }
        }
        else
        {
            request.Provenance = string.Empty;
        }
        StateHasChanged();
    }

    private void OnDestinationChanged(object value)
    {
        if (value is int siteId)
        {
            var site = sites.FirstOrDefault(s => s.Id == siteId);
            request.Destination = site?.Nom ?? string.Empty;
        }
        else
        {
            request.Destination = string.Empty;
        }
    }

    private void AddMateriel()
    {
        request.Materiels.Add(new MaterielRequest { Quantite = 1 });
        request.Materiels = new List<MaterielRequest>(request.Materiels);
    }

    private void RemoveMateriel(MaterielRequest item)
    {
        request.Materiels.Remove(item);
        request.Materiels = new List<MaterielRequest>(request.Materiels);
    }

    /// <summary>
    /// Vérifie si le formulaire est prêt à être soumis
    /// </summary>
    private bool CanSubmit()
    {
        return !string.IsNullOrWhiteSpace(request.NomCompagnie)
            && !string.IsNullOrWhiteSpace(request.Provenance)
            && !string.IsNullOrWhiteSpace(request.Destination)
            && request.Materiels.Count > 0
            && request.Materiels.All(m => !string.IsNullOrWhiteSpace(m.Designation) && !string.IsNullOrWhiteSpace(m.CodeProduitSerial));
    }

    private async Task HandleRetour()
    {
        await JSRuntime.InvokeVoidAsync("history.back");
    }

    private async Task HandleSubmit()
    {
        await SaveAsync(submitAfterSave: false);
    }

    private async Task HandleSubmitAndSend()
    {
        await SaveAsync(submitAfterSave: true);
    }

    private async Task SaveAsync(bool submitAfterSave)
    {
        // Validation avec messages d'erreur
        var validationErrors = new List<string>();
        if (string.IsNullOrWhiteSpace(request.NomCompagnie)) validationErrors.Add("Compagnie");
        if (string.IsNullOrWhiteSpace(request.Provenance)) validationErrors.Add("Provenance");
        if (string.IsNullOrWhiteSpace(request.Destination)) validationErrors.Add("Destination");
        if (request.Materiels.Count == 0) validationErrors.Add("Au moins un matériel");
        if (request.Materiels.Any(m => string.IsNullOrWhiteSpace(m.Designation) || string.IsNullOrWhiteSpace(m.CodeProduitSerial)))
            validationErrors.Add("Désignation et code série de chaque matériel");

        // Vérifier les doublons de Code/N° Série
        var duplicates = request.Materiels
            .Where(m => !string.IsNullOrWhiteSpace(m.CodeProduitSerial))
            .GroupBy(m => m.CodeProduitSerial!.Trim(), StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();
        if (duplicates.Any())
            validationErrors.Add($"Code/N° série en doublon : {string.Join(", ", duplicates)}");

        if (validationErrors.Any())
        {
            NotificationService.Notify(NotificationSeverity.Warning, "Champs obligatoires manquants",
                string.Join(", ", validationErrors));
            return;
        }

        isSaving = true;
        try
        {
            var result = await BonEntreeService.CreateAsync(request);
            if (!result.Success)
            {
                NotificationService.Notify(NotificationSeverity.Error, "Erreur", string.Join(", ", result.Errors));
                return;
            }

            if (submitAfterSave && result.BonEntree != null)
            {
                var submitResult = await BonEntreeService.SubmitForApprovalAsync(result.BonEntree.Id);
                if (!submitResult.Success)
                {
                    NotificationService.Notify(NotificationSeverity.Warning, "Attention",
                        $"Bon créé mais non soumis: {submitResult.Message}");
                    Navigation.NavigateTo($"/bon-entree/{result.BonEntree.Id}");
                    return;
                }
                NotificationService.Notify(NotificationSeverity.Success, "Succès", "Bon créé et soumis pour approbation");
            }
            else
            {
                NotificationService.Notify(NotificationSeverity.Success, "Succès", result.Message ?? "Bon créé avec succès (brouillon)");
            }

            Navigation.NavigateTo("/liste-bons");
        }
        catch (Exception ex)
        {
            var errorMessage = ex.InnerException?.Message ?? ex.Message;
            Logger.LogError(ex, "Erreur lors de la création du bon d'entrée");
            NotificationService.Notify(NotificationSeverity.Error, "Erreur", errorMessage);
        }
        finally
        {
            isSaving = false;
        }
    }
}
