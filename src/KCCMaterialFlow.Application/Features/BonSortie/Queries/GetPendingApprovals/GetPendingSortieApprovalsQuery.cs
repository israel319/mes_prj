using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Application.Features.BonSortie.Queries.GetBonSortieList;
using KCCMaterialFlow.Domain.Enums;
using KCCMaterialFlow.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Application.Features.BonSortie.Queries.GetPendingApprovals;

// ── Query ───────────────────────────────────────────────────────────────
public sealed record GetPendingSortieApprovalsQuery
    : IRequest<Result<IReadOnlyList<BonSortieListDto>>>;

// ── Handler ─────────────────────────────────────────────────────────────
internal sealed class GetPendingSortieApprovalsQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetPendingSortieApprovalsQuery, Result<IReadOnlyList<BonSortieListDto>>>
{
    private static readonly StatutBonSortie[] PendingStatuses =
    [
        StatutBonSortie.PendingSup,
        StatutBonSortie.PendingGM,
        StatutBonSortie.PendingIT,
        StatutBonSortie.PendingEnv,
        StatutBonSortie.PendingOPJ
    ];

    public async Task<Result<IReadOnlyList<BonSortieListDto>>> Handle(
        GetPendingSortieApprovalsQuery request, CancellationToken ct)
    {
        var items = await dbContext.BonsSortie
            .AsNoTracking()
            .Include(b => b.Materiels)
            .Where(b => PendingStatuses.Contains(b.Statut))
            .OrderBy(b => b.DateCreation)
            .Select(b => new BonSortieListDto(
                b.Id,
                b.NumeroReference,
                b.DateCreation,
                b.Statut,
                b.StatutActuel,
                b.Destination,
                b.NomDemandeur,
                b.MotifSortie,
                b.Materiels.Count,
                b.DateExpiration,
                b.EstDefinitif))
            .ToListAsync(ct);

        return items;
    }
}
