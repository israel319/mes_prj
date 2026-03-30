using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Application.Common.Models;
using KCCMaterialFlow.Application.Features.BonEntree.Queries.GetBonEntreeList;
using KCCMaterialFlow.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Application.Features.BonEntree.Queries.GetMyBonsEntree;

public sealed record GetMyBonsEntreeQuery(
    int Skip = 0,
    int Take = 20) : IRequest<Result<PaginatedList<BonEntreeListDto>>>;

public sealed class GetMyBonsEntreeQueryHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser)
    : IRequestHandler<GetMyBonsEntreeQuery, Result<PaginatedList<BonEntreeListDto>>>
{
    public async Task<Result<PaginatedList<BonEntreeListDto>>> Handle(
        GetMyBonsEntreeQuery query, CancellationToken ct)
    {
        var login = currentUser.GetUserLogin();
        var pageIndex = (query.Skip / query.Take) + 1;

        var q = dbContext.BonsEntree
            .AsNoTracking()
            .Where(b => b.CreatedBy == login);

        var totalCount = await q.CountAsync(ct);

        var items = await q
            .OrderByDescending(b => b.DateCreation)
            .Skip(query.Skip)
            .Take(query.Take)
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

        return new PaginatedList<BonEntreeListDto>(items, totalCount, pageIndex, query.Take);
    }
}
