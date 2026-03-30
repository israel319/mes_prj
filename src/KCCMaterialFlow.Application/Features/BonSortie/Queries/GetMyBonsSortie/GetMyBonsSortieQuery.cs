using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Application.Common.Models;
using KCCMaterialFlow.Application.Features.BonSortie.Queries.GetBonSortieList;
using KCCMaterialFlow.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Application.Features.BonSortie.Queries.GetMyBonsSortie;

// ── Query ───────────────────────────────────────────────────────────────
public sealed record GetMyBonsSortieQuery(
    int PageIndex = 1,
    int PageSize = 20) : IRequest<Result<PaginatedList<BonSortieListDto>>>;

// ── Handler ─────────────────────────────────────────────────────────────
internal sealed class GetMyBonsSortieQueryHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser)
    : IRequestHandler<GetMyBonsSortieQuery, Result<PaginatedList<BonSortieListDto>>>
{
    public async Task<Result<PaginatedList<BonSortieListDto>>> Handle(
        GetMyBonsSortieQuery request, CancellationToken ct)
    {
        var login = currentUser.GetUserLogin();

        var query = dbContext.BonsSortie
            .AsNoTracking()
            .Include(b => b.Materiels)
            .Where(b => b.CreatedByLogin == login);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(b => b.DateCreation)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
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

        return new PaginatedList<BonSortieListDto>(items, totalCount, request.PageIndex, request.PageSize);
    }
}
