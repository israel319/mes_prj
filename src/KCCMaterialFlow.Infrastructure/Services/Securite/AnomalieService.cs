using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KCCMaterialFlow.Infrastructure.Services.Securite;

/// <summary>
/// SEC-020: Service métier pour les anomalies.
/// Gère la création, le traitement, les notifications et le suivi des anomalies.
/// </summary>
public class AnomalieService : IAnomalieService
{
    private readonly IAnomalieRepository _anomalieRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ISecuriteEmailService _emailService;
    private readonly SecuriteEmailOptions _emailOptions;
    private readonly ILogger<AnomalieService> _logger;

    public AnomalieService(
        IAnomalieRepository anomalieRepository,
        ICurrentUserService currentUserService,
        ISecuriteEmailService emailService,
        IOptions<SecuriteEmailOptions> emailOptions,
        ILogger<AnomalieService> logger)
    {
        _anomalieRepository = anomalieRepository;
        _currentUserService = currentUserService;
        _emailService = emailService;
        _emailOptions = emailOptions.Value;
        _logger = logger;
    }

    #region CRUD Operations

    public async Task<AnomalieResult> CreateAsync(CreateAnomalieRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Création d'une anomalie de type {Type} pour le bon {Bon}",
                request.TypeAnomalie, request.NumeroReferenceBon);

            var anomalie = new Anomalie(
                typeAnomalie: request.TypeAnomalie,
                description: request.Description,
                signalePar: _currentUserService.GetUserLogin() ?? "system",
                niveauGravite: request.NiveauGravite,
                bonId: request.BonId,
                typeBon: request.TypeBon,
                numeroReferenceBon: request.NumeroReferenceBon,
                scanId: request.ScanId,
                barriereId: request.BarriereId,
                signaleParNom: _currentUserService.GetUserDisplayName());

            await _anomalieRepository.CreateAsync(anomalie, cancellationToken);

            if (request.NiveauGravite == NiveauGraviteValues.Critique ||
                request.NiveauGravite == NiveauGraviteValues.Eleve)
            {
                await NotifyInvestigationAsync(anomalie, cancellationToken);
            }

            _logger.LogInformation("Anomalie {Id} créée avec succès", anomalie.Id);

            return AnomalieResult.Ok(anomalie, "Anomalie signalée avec succès");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la création de l'anomalie");
            return AnomalieResult.Failed($"Erreur: {ex.Message}");
        }
    }

    public async Task<Anomalie?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _anomalieRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<AnomalieResult> UpdateAsync(UpdateAnomalieRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var anomalie = await _anomalieRepository.GetByIdAsync(request.IdAnomalie, cancellationToken);
            if (anomalie == null)
                return AnomalieResult.Failed($"Anomalie {request.IdAnomalie} non trouvée");

            if (anomalie.EstTraitee)
                return AnomalieResult.Failed("Impossible de modifier une anomalie déjà traitée");

            if (!string.IsNullOrWhiteSpace(request.NiveauGravite))
                anomalie.NiveauGravite = request.NiveauGravite;

            if (!string.IsNullOrWhiteSpace(request.Description))
                anomalie.Description = request.Description;

            await _anomalieRepository.UpdateAsync(anomalie, cancellationToken);

            return AnomalieResult.Ok(anomalie, "Anomalie mise à jour");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la mise à jour de l'anomalie {Id}", request.IdAnomalie);
            return AnomalieResult.Failed($"Erreur: {ex.Message}");
        }
    }

    #endregion

    #region Traitement

    public async Task<AnomalieResult> MarkAsTraiteeAsync(
        int id,
        string resolution,
        string? actionsCorrectives = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var anomalie = await _anomalieRepository.GetByIdAsync(id, cancellationToken);
            if (anomalie == null)
                return AnomalieResult.Failed($"Anomalie {id} non trouvée");

            if (anomalie.EstTraitee)
                return AnomalieResult.Failed("Cette anomalie est déjà traitée");

            var traitePar = _currentUserService.GetUserLogin() ?? "system";

            await _anomalieRepository.MarquerCommeTraiteeAsync(
                id, traitePar, resolution, actionsCorrectives, cancellationToken);

            anomalie = await _anomalieRepository.GetByIdAsync(id, cancellationToken);

            _logger.LogInformation("Anomalie {Id} marquée comme traitée par {User}", id, traitePar);

            return AnomalieResult.Ok(anomalie!, "Anomalie traitée avec succès");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du traitement de l'anomalie {Id}", id);
            return AnomalieResult.Failed($"Erreur: {ex.Message}");
        }
    }

    public async Task<AnomalieResult> ReopenAsync(int id, string motif, CancellationToken cancellationToken = default)
    {
        try
        {
            var anomalie = await _anomalieRepository.GetByIdAsync(id, cancellationToken);
            if (anomalie == null)
                return AnomalieResult.Failed($"Anomalie {id} non trouvée");

            if (!anomalie.EstTraitee)
                return AnomalieResult.Failed("Cette anomalie n'est pas encore traitée");

            anomalie.EstTraitee = false;
            anomalie.DateTraitement = null;
            anomalie.Resolution = $"[RÉOUVERT: {motif}] {anomalie.Resolution}";

            await _anomalieRepository.UpdateAsync(anomalie, cancellationToken);

            _logger.LogInformation("Anomalie {Id} réouverte: {Motif}", id, motif);

            return AnomalieResult.Ok(anomalie, "Anomalie réouverte");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la réouverture de l'anomalie {Id}", id);
            return AnomalieResult.Failed($"Erreur: {ex.Message}");
        }
    }

    #endregion

    #region Requêtes

    public async Task<IReadOnlyList<Anomalie>> GetNonTraiteesAsync(CancellationToken cancellationToken = default)
    {
        return await _anomalieRepository.GetNonTraiteesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Anomalie>> GetCritiquesNonTraiteesAsync(CancellationToken cancellationToken = default)
    {
        return await _anomalieRepository.GetNonTraiteesAsync(
            niveauGravite: NiveauGraviteValues.Critique,
            cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyList<Anomalie>> GetByBonAsync(int bonId, string typeBon, CancellationToken cancellationToken = default)
    {
        return await _anomalieRepository.GetByBonAsync(bonId, typeBon, cancellationToken);
    }

    public async Task<IReadOnlyList<Anomalie>> GetByScanAsync(int scanId, CancellationToken cancellationToken = default)
    {
        return await _anomalieRepository.GetByScanAsync(scanId, cancellationToken);
    }

    public async Task<AnomalieSearchResult> SearchAsync(AnomalieFilter filter, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _anomalieRepository.SearchAsync(
            filter.SearchTerm,
            filter.TypeAnomalie,
            filter.NiveauGravite,
            filter.EstTraitee,
            filter.BarriereId,
            filter.DateDebut,
            filter.DateFin,
            filter.Skip,
            filter.Take,
            cancellationToken);

        return new AnomalieSearchResult
        {
            Items = items,
            TotalCount = totalCount,
            PageSize = filter.Take,
            CurrentPage = filter.Skip / filter.Take + 1
        };
    }

    #endregion

    #region Dashboard et Statistiques

    public async Task<AnomalieDashboard> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var nonTraitees = await _anomalieRepository.GetNonTraiteesAsync(cancellationToken);
        var countByGravite = await _anomalieRepository.GetCountByGraviteAsync(true, cancellationToken);

        var dateDebut = DateTime.Now.AddDays(-30);
        var dateFin = DateTime.Now;
        var stats = await _anomalieRepository.GetStatistiquesAsync(dateDebut, dateFin, cancellationToken);

        var (recentes, _) = await _anomalieRepository.SearchAsync(
            dateDebut: DateTime.Now.AddDays(-1),
            dateFin: DateTime.Now,
            take: 10,
            cancellationToken: cancellationToken);

        return new AnomalieDashboard
        {
            TotalNonTraitees = nonTraitees.Count,
            CritiquesNonTraitees = countByGravite.GetValueOrDefault(NiveauGraviteValues.Critique, 0),
            EleveesNonTraitees = countByGravite.GetValueOrDefault(NiveauGraviteValues.Eleve, 0),
            AnomaliesRecentes = recentes,
            ParType = stats.ParType,
            ParGravite = stats.ParGravite,
            TauxResolution = stats.TauxResolution,
            DelaiMoyenResolution = stats.DelaiMoyenResolution
        };
    }

    public async Task<AnomalieStats> GetStatistiquesAsync(
        DateTime dateDebut,
        DateTime dateFin,
        CancellationToken cancellationToken = default)
    {
        return await _anomalieRepository.GetStatistiquesAsync(dateDebut, dateFin, cancellationToken);
    }

    #endregion

    #region Notifications

    public async Task NotifyInvestigationAsync(Anomalie anomalie, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "SEC-042: Notification Investigation pour anomalie {Type} ({Gravite}) - Bon: {Bon}",
                anomalie.TypeAnomalie,
                anomalie.NiveauGravite,
                anomalie.NumeroReferenceBon);

            var emailModel = new AnomalieEmailModel
            {
                AnomalieId = anomalie.Id,
                TypeAnomalie = anomalie.TypeAnomalie,
                NiveauGravite = anomalie.NiveauGravite,
                Description = anomalie.Description ?? string.Empty,
                DateSignalement = anomalie.DateSignalement,
                NumeroReference = anomalie.NumeroReferenceBon,
                TypeBon = anomalie.TypeBon,
                AgentNom = anomalie.SignaleParNom,
                AgentLogin = anomalie.SignalePar,
                LinkToAnomalie = $"{_emailOptions.BaseUrl}/securite/anomalies/{anomalie.Id}"
            };

            if (anomalie.Barriere != null)
            {
                emailModel.NomBarriere = anomalie.Barriere.NomBarriere;
                emailModel.LocalisationBarriere = anomalie.Barriere.Localisation;
            }

            await _emailService.SendAnomalieNotificationAsync(emailModel, cancellationToken);

            _logger.LogInformation("Notification Investigation envoyée pour anomalie {Id}", anomalie.Id);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Impossible d'envoyer la notification Investigation pour anomalie {Id}",
                anomalie.Id);
        }
    }

    #endregion
}
