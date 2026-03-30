using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Enums;
using KCCMaterialFlow.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Application.Features.BonEntree.Queries.GetBonEntreeStats;

public sealed record BonEntreeStatsDto
{
    public int TotalBons { get; init; }
    public int BonsDraft { get; init; }
    public int BonsEnAttente { get; init; }
    public int BonsApprouves { get; init; }
    public int BonsRejetes { get; init; }
    public int BonsExpires { get; init; }
    public int BonsEnTransit { get; init; }
    public int BonsCompletes { get; init; }
}

public sealed record GetBonEntreeStatsQuery() : IRequest<Result<BonEntreeStatsDto>>;

public sealed class GetBonEntreeStatsQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetBonEntreeStatsQuery, Result<BonEntreeStatsDto>>
{
    private static readonly StatutBonEntree[] PendingStatuses =
    [
        StatutBonEntree.PendingSup,
        StatutBonEntree.PendingGM,
        StatutBonEntree.PendingIT,
        StatutBonEntree.PendingEnv,
        StatutBonEntree.PendingOPJ
    ];

    public async Task<Result<BonEntreeStatsDto>> Handle(
        GetBonEntreeStatsQuery query, CancellationToken ct)
    {
        var bons = await dbContext.BonsEntree
            .AsNoTracking()
            .Select(b => new { b.Statut, b.DateExpiration })
            .ToListAsync(ct);

        var now = DateTime.Now;

        var stats = new BonEntreeStatsDto
        {
            TotalBons = bons.Count,
            BonsDraft = bons.Count(b => b.Statut == StatutBonEntree.Draft),
            BonsEnAttente = bons.Count(b => PendingStatuses.Contains(b.Statut)),
            BonsApprouves = bons.Count(b => b.Statut == StatutBonEntree.Approved),
            BonsRejetes = bons.Count(b => b.Statut == StatutBonEntree.Rejected),
            BonsExpires = bons.Count(b => b.DateExpiration <= now && b.Statut != StatutBonEntree.Completed),
            BonsEnTransit = bons.Count(b => b.Statut == StatutBonEntree.InTransit),
            BonsCompletes = bons.Count(b => b.Statut == StatutBonEntree.Completed)
        };

        return stats;
    }
}
