using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Enums;
using KCCMaterialFlow.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Application.Features.BonSortie.Queries.GetBonSortieStats;

// ── DTO ─────────────────────────────────────────────────────────────────
public sealed record BonSortieStatsDto(
    int TotalBons,
    int Draft,
    int PendingApproval,
    int Approved,
    int InTransit,
    int Completed,
    int Rejected,
    int Returned,
    int ActiveLoans,
    int OverdueLoans);

// ── Query ───────────────────────────────────────────────────────────────
public sealed record GetBonSortieStatsQuery : IRequest<Result<BonSortieStatsDto>>;

// ── Handler ─────────────────────────────────────────────────────────────
internal sealed class GetBonSortieStatsQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetBonSortieStatsQuery, Result<BonSortieStatsDto>>
{
    public async Task<Result<BonSortieStatsDto>> Handle(
        GetBonSortieStatsQuery request, CancellationToken ct)
    {
        var bons = dbContext.BonsSortie.AsNoTracking();
        var now = DateTime.Now;

        var totalBons = await bons.CountAsync(ct);
        var draft = await bons.CountAsync(b => b.Statut == StatutBonSortie.Draft, ct);
        var pendingApproval = await bons.CountAsync(b =>
            b.Statut == StatutBonSortie.PendingSup || b.Statut == StatutBonSortie.PendingGM ||
            b.Statut == StatutBonSortie.PendingIT || b.Statut == StatutBonSortie.PendingEnv ||
            b.Statut == StatutBonSortie.PendingOPJ, ct);
        var approved = await bons.CountAsync(b => b.Statut == StatutBonSortie.Approved, ct);
        var inTransit = await bons.CountAsync(b => b.Statut == StatutBonSortie.InTransit, ct);
        var completed = await bons.CountAsync(b => b.Statut == StatutBonSortie.Completed, ct);
        var rejected = await bons.CountAsync(b => b.Statut == StatutBonSortie.Rejected, ct);
        var returned = await bons.CountAsync(b => b.Statut == StatutBonSortie.Returned, ct);

        var prets = dbContext.Prets.AsNoTracking();
        var activeLoans = await prets.CountAsync(p => !p.EstRetourne, ct);
        var overdueLoans = await prets.CountAsync(p => !p.EstRetourne && p.DateRetourPrevue < now, ct);

        return new BonSortieStatsDto(
            totalBons, draft, pendingApproval, approved, inTransit,
            completed, rejected, returned, activeLoans, overdueLoans);
    }
}
