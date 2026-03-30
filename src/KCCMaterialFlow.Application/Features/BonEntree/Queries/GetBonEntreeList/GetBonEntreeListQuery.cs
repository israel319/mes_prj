using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Application.Common.Models;
using KCCMaterialFlow.Domain.Enums;
using KCCMaterialFlow.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Application.Features.BonEntree.Queries.GetBonEntreeList;

public sealed record BonEntreeListDto
{
    public int IdBon { get; init; }
    public string NumeroReference { get; init; } = string.Empty;
    public DateTime DateCreation { get; init; }
    public StatutBonEntree Statut { get; init; }
    public string NomCompagnie { get; init; } = string.Empty;
    public string NomDemandeur { get; init; } = string.Empty;
    public string Destination { get; init; } = string.Empty;
    public string Provenance { get; init; } = string.Empty;
    public int NombreMateriels { get; init; }
    public DateTime DateExpiration { get; init; }
}

public sealed record GetBonEntreeListQuery(
    string? SearchTerm,
    string? Statut,
    DateTime? DateDebut,
    DateTime? DateFin,
    int PageIndex = 1,
    int PageSize = 20) : IRequest<Result<PaginatedList<BonEntreeListDto>>>;

public sealed class GetBonEntreeListQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetBonEntreeListQuery, Result<PaginatedList<BonEntreeListDto>>>
{
    public async Task<Result<PaginatedList<BonEntreeListDto>>> Handle(
        GetBonEntreeListQuery query, CancellationToken ct)
    {
        var q = dbContext.BonsEntree.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var term = query.SearchTerm.Trim().ToLower();
            q = q.Where(b =>
                b.NumeroReference.ToLower().Contains(term) ||
                b.NomCompagnie.ToLower().Contains(term) ||
                b.NomDemandeur.ToLower().Contains(term) ||
                b.Destination.ToLower().Contains(term) ||
                b.Provenance.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(query.Statut) &&
            Enum.TryParse<StatutBonEntree>(query.Statut, true, out var statut))
        {
            q = q.Where(b => b.Statut == statut);
        }

        if (query.DateDebut.HasValue)
            q = q.Where(b => b.DateCreation >= query.DateDebut.Value);

        if (query.DateFin.HasValue)
            q = q.Where(b => b.DateCreation <= query.DateFin.Value);

        var totalCount = await q.CountAsync(ct);

        var items = await q
            .OrderByDescending(b => b.DateCreation)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
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

        return new PaginatedList<BonEntreeListDto>(items, totalCount, query.PageIndex, query.PageSize);
    }
}
