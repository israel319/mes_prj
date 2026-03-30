using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Application.Features.BonSortie.Queries.GetActiveLoans;
using KCCMaterialFlow.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Application.Features.BonSortie.Queries.GetOverdueLoans;

// ── Query ───────────────────────────────────────────────────────────────
public sealed record GetOverdueLoansQuery : IRequest<Result<IReadOnlyList<PretListDto>>>;

// ── Handler ─────────────────────────────────────────────────────────────
internal sealed class GetOverdueLoansQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetOverdueLoansQuery, Result<IReadOnlyList<PretListDto>>>
{
    public async Task<Result<IReadOnlyList<PretListDto>>> Handle(
        GetOverdueLoansQuery request, CancellationToken ct)
    {
        var now = DateTime.Now;

        var items = await dbContext.Prets
            .AsNoTracking()
            .Where(p => !p.EstRetourne && p.DateRetourPrevue < now)
            .OrderBy(p => p.DateRetourPrevue)
            .Select(p => new PretListDto(
                p.Id,
                p.NumeroReference,
                p.NomDemandeur,
                p.NomDestinataire,
                p.Destination,
                p.Statut,
                p.DateRetourPrevue,
                p.DateRetourEffective,
                p.EstRetourne,
                p.JoursRetard,
                p.EstEnRetard))
            .ToListAsync(ct);

        return items;
    }
}
