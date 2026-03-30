using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Enums;
using KCCMaterialFlow.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Application.Features.BonEntree.Queries.GetReturnedBonsEntree;

public sealed record ReturnedBonEntreeDto
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
    public string? MotifRetour { get; init; }
    public string? RetournePar { get; init; }
    public DateTime? DateRetour { get; init; }
}

public sealed record GetReturnedBonsEntreeQuery() : IRequest<Result<IReadOnlyList<ReturnedBonEntreeDto>>>;

public sealed class GetReturnedBonsEntreeQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetReturnedBonsEntreeQuery, Result<IReadOnlyList<ReturnedBonEntreeDto>>>
{
    public async Task<Result<IReadOnlyList<ReturnedBonEntreeDto>>> Handle(
        GetReturnedBonsEntreeQuery query, CancellationToken ct)
    {
        var items = await dbContext.BonsEntree
            .AsNoTracking()
            .Where(b => b.Statut == StatutBonEntree.Returned)
            .Include(b => b.Historiques)
            .OrderByDescending(b => b.DateCreation)
            .Select(b => new ReturnedBonEntreeDto
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
                DateExpiration = b.DateExpiration,
                MotifRetour = b.Historiques
                    .Where(h => h.Action == ActionBonEntree.RetourModification)
                    .OrderByDescending(h => h.ActionDate)
                    .Select(h => h.Comment)
                    .FirstOrDefault(),
                RetournePar = b.Historiques
                    .Where(h => h.Action == ActionBonEntree.RetourModification)
                    .OrderByDescending(h => h.ActionDate)
                    .Select(h => h.ActionByNom)
                    .FirstOrDefault(),
                DateRetour = b.Historiques
                    .Where(h => h.Action == ActionBonEntree.RetourModification)
                    .OrderByDescending(h => h.ActionDate)
                    .Select(h => (DateTime?)h.ActionDate)
                    .FirstOrDefault()
            })
            .ToListAsync(ct);

        return items;
    }
}
