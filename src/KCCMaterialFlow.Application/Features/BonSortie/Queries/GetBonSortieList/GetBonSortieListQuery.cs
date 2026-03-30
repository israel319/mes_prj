using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Application.Common.Models;
using KCCMaterialFlow.Domain.Enums;
using KCCMaterialFlow.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Application.Features.BonSortie.Queries.GetBonSortieList;

// ── DTO ─────────────────────────────────────────────────────────────────
public sealed record BonSortieListDto(
    int IdBon,
    string NumeroReference,
    DateTime DateCreation,
    StatutBonSortie Statut,
    string StatutActuel,
    string Destination,
    string NomDemandeur,
    string MotifSortie,
    int NombreMateriels,
    DateTime DateExpiration,
    bool EstDefinitif);

// ── Query ───────────────────────────────────────────────────────────────
public sealed record GetBonSortieListQuery(
    string? SearchTerm = null,
    string? Statut = null,
    string? TypeSortie = null,
    DateTime? DateDebut = null,
    DateTime? DateFin = null,
    int PageIndex = 1,
    int PageSize = 20) : IRequest<Result<PaginatedList<BonSortieListDto>>>;

// ── Handler ─────────────────────────────────────────────────────────────
internal sealed class GetBonSortieListQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetBonSortieListQuery, Result<PaginatedList<BonSortieListDto>>>
{
    public async Task<Result<PaginatedList<BonSortieListDto>>> Handle(
        GetBonSortieListQuery request, CancellationToken ct)
    {
        var query = dbContext.BonsSortie
            .AsNoTracking()
            .Include(b => b.Materiels)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLower();
            query = query.Where(b =>
                b.NumeroReference.ToLower().Contains(term) ||
                b.NomDemandeur.ToLower().Contains(term) ||
                b.Destination.ToLower().Contains(term) ||
                b.MotifSortie.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(request.Statut) &&
            Enum.TryParse<StatutBonSortie>(request.Statut, true, out var statut))
        {
            query = query.Where(b => b.Statut == statut);
        }

        if (!string.IsNullOrWhiteSpace(request.TypeSortie))
        {
            query = request.TypeSortie.ToLower() switch
            {
                "externe" => query.Where(b => b.GetType() == typeof(Domain.Entities.BonSortieExterne)),
                "interne" => query.OfType<Domain.Entities.BonSortieInterne>(),
                "pret" => query.OfType<Domain.Entities.Pret>(),
                _ => query
            };
        }

        if (request.DateDebut.HasValue)
            query = query.Where(b => b.DateCreation >= request.DateDebut.Value);
        if (request.DateFin.HasValue)
            query = query.Where(b => b.DateCreation <= request.DateFin.Value);

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
