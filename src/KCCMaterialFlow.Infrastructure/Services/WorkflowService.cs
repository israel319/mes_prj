using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace KCCMaterialFlow.Infrastructure.Services;

/// <summary>
/// Service de gestion du workflow d'approbation dynamique selon le type de matériel
/// </summary>
public class WorkflowService : IWorkflowService
{
    private readonly ILogger<WorkflowService> _logger;

    public WorkflowService(ILogger<WorkflowService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<WorkflowResult> ApproveAsync(int bonId, string bonType, string? comment = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Approbation du bon {BonId} ({BonType})", bonId, bonType);
            
            // TODO: Implémenter la logique d'approbation avec le repository

            await Task.CompletedTask;

            return new WorkflowResult
            {
                Success = true,
                Message = "Bon approuvé avec succès",
                NewStatus = "Approved"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'approbation du bon {BonId}", bonId);
            return new WorkflowResult
            {
                Success = false,
                Message = $"Erreur: {ex.Message}"
            };
        }
    }

    /// <inheritdoc />
    public async Task<WorkflowResult> RejectAsync(int bonId, string bonType, string comment, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Rejet du bon {BonId} ({BonType}). Raison: {Reason}", 
                bonId, bonType, comment);

            await Task.CompletedTask;

            return new WorkflowResult
            {
                Success = true,
                Message = "Bon rejeté",
                NewStatus = "Rejected"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du rejet du bon {BonId}", bonId);
            return new WorkflowResult
            {
                Success = false,
                Message = $"Erreur: {ex.Message}"
            };
        }
    }

    /// <inheritdoc />
    public async Task<WorkflowResult> ReturnAsync(int bonId, string bonType, string comment, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retour du bon {BonId} ({BonType}). Raison: {Reason}", 
                bonId, bonType, comment);

            await Task.CompletedTask;

            return new WorkflowResult
            {
                Success = true,
                Message = "Bon retourné au demandeur pour modification",
                NewStatus = "Returned"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du retour du bon {BonId}", bonId);
            return new WorkflowResult
            {
                Success = false,
                Message = $"Erreur: {ex.Message}"
            };
        }
    }

    /// <inheritdoc />
    public Task<NextApproverInfo> GetNextApproverAsync(int bonId, string bonType, string? typeMateriel = null, CancellationToken cancellationToken = default)
    {
        // TODO: Implémenter la logique de détermination du prochain approbateur
        return Task.FromResult(new NextApproverInfo
        {
            ApproverRole = "Superviseur",
            StepNumber = 1,
            IsFinalStep = false
        });
    }

    /// <inheritdoc />
    public Task<bool> CanApproveAsync(int bonId, string bonType, string userLogin, CancellationToken cancellationToken = default)
    {
        // TODO: Implémenter la vérification des permissions
        return Task.FromResult(false);
    }

    /// <inheritdoc />
    public IEnumerable<WorkflowStep> GetApprovalChain(string typeMateriel)
    {
        var steps = new List<WorkflowStep>();
        int order = 1;

        // Étape 1: Superviseur (toujours)
        steps.Add(new WorkflowStep
        {
            Order = order++,
            RoleCode = "Superviseur",
            StepName = "Validation Superviseur",
            IsOptional = false
        });

        // Étape 2: GM (toujours)
        steps.Add(new WorkflowStep
        {
            Order = order++,
            RoleCode = "GM",
            StepName = "Validation Direction",
            IsOptional = false
        });

        // Étapes conditionnelles selon le type de matériel
        var typeEnum = Enum.TryParse<TypeMateriel>(typeMateriel, out var tm) ? tm : TypeMateriel.Circulaire;

        switch (typeEnum)
        {
            case TypeMateriel.Informatique:
                steps.Add(new WorkflowStep
                {
                    Order = order++,
                    RoleCode = "IT",
                    StepName = "Validation IT",
                    IsOptional = false
                });
                break;

            case TypeMateriel.Residu:
            case TypeMateriel.Radioprotection:
            case TypeMateriel.Modification:
                steps.Add(new WorkflowStep
                {
                    Order = order++,
                    RoleCode = "Environnement",
                    StepName = "Validation Environnement",
                    IsOptional = false
                });
                break;
        }

        // Étape OPJ
        steps.Add(new WorkflowStep
        {
            Order = order++,
            RoleCode = "OPJ",
            StepName = "Validation OPJ",
            IsOptional = false
        });

        // Étape Identification (toujours)
        steps.Add(new WorkflowStep
        {
            Order = order++,
            RoleCode = "Identification",
            StepName = "Vérification Identification",
            IsOptional = false
        });

        return steps;
    }

    /// <summary>
    /// Vérifie si le statut actuel permet l'approbation par un rôle donné
    /// </summary>
    public bool CanRoleApprove(BonStatut currentStatus, RoleUtilisateur role)
    {
        return (currentStatus, role) switch
        {
            (BonStatut.PendingSup, RoleUtilisateur.Superviseur) => true,
            (BonStatut.PendingGM, RoleUtilisateur.GM) => true,
            (BonStatut.PendingIT, RoleUtilisateur.IT) => true,
            (BonStatut.PendingEnv, RoleUtilisateur.Environnement) => true,
            (BonStatut.PendingOPJ, RoleUtilisateur.OPJ) => true,
            (BonStatut.PendingIdentification, RoleUtilisateur.Identification) => true,
            (_, RoleUtilisateur.Admin) => true,
            _ => false
        };
    }
}
