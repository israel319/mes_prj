using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Application.Features.BonEntree.Queries.GetBonEntreeList;
using KCCMaterialFlow.Domain.Enums;
using KCCMaterialFlow.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Application.Features.BonEntree.Queries.GetPendingApprovals;

public sealed record GetPendingApprovalsQuery() : IRequest<Result<IReadOnlyList<BonEntreeListDto>>>;

public sealed class GetPendingApprovalsQueryHandler(IApplicationDbContext dbContext)
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
        var items = await dbContext.BonsEntree
            .AsNoTracking()
            .Where(b => PendingStatuses.Contains(b.Statut))
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
