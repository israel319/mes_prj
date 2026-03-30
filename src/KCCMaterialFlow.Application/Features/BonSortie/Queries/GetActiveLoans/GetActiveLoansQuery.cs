using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Enums;
using KCCMaterialFlow.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Application.Features.BonSortie.Queries.GetActiveLoans;

// ── DTO ─────────────────────────────────────────────────────────────────
public sealed record PretListDto(
    int IdBon,
    string NumeroReference,
    string NomDemandeur,
    string NomDestinataire,
    string Destination,
    StatutBonSortie Statut,
    DateTime DateRetourPrevue,
    DateTime? DateRetourEffective,
    bool EstRetourne,
    int JoursRetard,
    bool EstEnRetard);

// ── Query ───────────────────────────────────────────────────────────────
public sealed record GetActiveLoansQuery : IRequest<Result<IReadOnlyList<PretListDto>>>;

// ── Handler ─────────────────────────────────────────────────────────────
internal sealed class GetActiveLoansQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetActiveLoansQuery, Result<IReadOnlyList<PretListDto>>>
{
    public async Task<Result<IReadOnlyList<PretListDto>>> Handle(
        GetActiveLoansQuery request, CancellationToken ct)
    {
        var items = await dbContext.Prets
            .AsNoTracking()
            .Where(p => !p.EstRetourne)
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
