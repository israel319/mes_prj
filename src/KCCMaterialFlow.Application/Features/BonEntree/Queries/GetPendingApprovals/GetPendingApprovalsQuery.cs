using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Application.Features.BonEntree.Queries.GetBonEntreeList;
using KCCMaterialFlow.Domain.Enums;
using KCCMaterialFlow.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Application.Features.BonEntree.Queries.GetPendingApprovals;

public sealed record GetPendingApprovalsQuery() : IRequest<Result<IReadOnlyList<BonEntreeListDto>>>;

public sealed class GetPendingApprovalsQueryHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetPendingApprovalsQuery, Result<IReadOnlyList<BonEntreeListDto>>>
{
    private static readonly StatutBonEntree[] PendingStatuses =
    [
        StatutBonEntree.PendingSup,
        StatutBonEntree.PendingGM,
        StatutBonEntree.PendingIT,
        StatutBonEntree.PendingEnv,
        StatutBonEntree.PendingOPJ
    ];

    public async Task<Result<IReadOnlyList<BonEntreeListDto>>> Handle(
        GetPendingApprovalsQuery query, CancellationToken ct)
    {
        // Récupérer l'utilisateur actuel
        var currentEmployee = currentUserService.GetCurrentEmployee();
        if (currentEmployee?.Id == null)
        {
            return new List<BonEntreeListDto>();
        }

        // Afficher les bons où cet utilisateur est le prochain approbateur assigné
        var items = await dbContext.BonsEntree
            .AsNoTracking()
            .Where(b => PendingStatuses.Contains(b.Statut) && b.ProchainApprobateurId == currentEmployee.Id)
            .OrderByDescending(b => b.DateCreation)
            .Select(b => new BonEntreeListDto
            {
                IdBon = b.Id,
                NumeroReference = b.NumeroReference,
                DateCreation = b.DateCreation,
                Statut = b.Statut,
                NomCompagnie = b.NomCompagnie,
                NomDemandeur = b.NomDemandeur,
                Destination = b.Destination,
                Provenance = b.Provenance,
                NombreMateriels = b.Materiels.Count,
                DateExpiration = b.DateExpiration
            })
            .ToListAsync(ct);

        return items;
    }
}
