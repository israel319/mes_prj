using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Domain.Enums;
using KCCMaterialFlow.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Application.Features.BonSortie.Queries.GetBonsSortieByBonEntree;

// ── DTO ─────────────────────────────────────────────────────────────────
public sealed record BonSortieLieDto(
    int IdBon,
    string NumeroReference,
    StatutBonSortie Statut,
    string StatutActuel,
    DateTime DateCreation,
    string NomDemandeur,
    string Destination,
    bool EstDefinitif);

// ── Query ───────────────────────────────────────────────────────────────
public sealed record GetBonsSortieByBonEntreeQuery(int BonEntreeId)
    : IRequest<Result<IReadOnlyList<BonSortieLieDto>>>;

// ── Handler ─────────────────────────────────────────────────────────────
internal sealed class GetBonsSortieByBonEntreeQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetBonsSortieByBonEntreeQuery, Result<IReadOnlyList<BonSortieLieDto>>>
{
    public async Task<Result<IReadOnlyList<BonSortieLieDto>>> Handle(
        GetBonsSortieByBonEntreeQuery request, CancellationToken ct)
    {
        // Query both Externe (including Pret) and Interne that reference this BonEntree
        var externes = await dbContext.BonsSortieExterne
            .AsNoTracking()
            .Where(b => b.BonEntreeAssocieId == request.BonEntreeId)
            .Select(b => new BonSortieLieDto(
                b.Id, b.NumeroReference, b.Statut, b.StatutActuel,
                b.DateCreation, b.NomDemandeur, b.Destination, b.EstDefinitif))
            .ToListAsync(ct);

        var internes = await dbContext.BonsSortieInterne
            .AsNoTracking()
            .Where(b => b.BonEntreeAssocieId == request.BonEntreeId)
            .Select(b => new BonSortieLieDto(
                b.Id, b.NumeroReference, b.Statut, b.StatutActuel,
                b.DateCreation, b.NomDemandeur, b.Destination, b.EstDefinitif))
            .ToListAsync(ct);

        var result = externes.Concat(internes)
            .OrderByDescending(b => b.DateCreation)
            .ToList();

        return result;
    }
}
