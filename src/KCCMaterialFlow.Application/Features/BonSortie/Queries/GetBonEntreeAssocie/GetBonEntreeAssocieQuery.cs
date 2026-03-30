using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Application.Features.BonSortie.Queries.GetBonEntreeAssocie;

// ── DTO ─────────────────────────────────────────────────────────────────
public sealed record BonEntreeAssocieDto(
    int IdBon,
    string NumeroReference,
    string NomCompagnie,
    string NomDemandeur,
    string StatutActuel,
    DateTime DateCreation,
    bool EstVerrouillePourSortie);

// ── Query ───────────────────────────────────────────────────────────────
public sealed record GetBonEntreeAssocieQuery(int BonSortieId) : IRequest<Result<BonEntreeAssocieDto>>;

// ── Handler ─────────────────────────────────────────────────────────────
internal sealed class GetBonEntreeAssocieQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetBonEntreeAssocieQuery, Result<BonEntreeAssocieDto>>
{
    public async Task<Result<BonEntreeAssocieDto>> Handle(
        GetBonEntreeAssocieQuery request, CancellationToken ct)
    {
        var bon = await dbContext.BonsSortie
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == request.BonSortieId, ct);

        if (bon is null)
            return Result.Failure<BonEntreeAssocieDto>(Error.NotFound("BonSortie", request.BonSortieId));

        int? bonEntreeId = bon switch
        {
            BonSortieExterne ext => ext.BonEntreeAssocieId,
            BonSortieInterne intBon => intBon.BonEntreeAssocieId,
            _ => null
        };

        if (bonEntreeId is null)
            return Result.Failure<BonEntreeAssocieDto>(
                new Error("BonSortie.PasDeBonEntree", "Ce bon de sortie n'est pas lié à un bon d'entrée."));

        var bonEntree = await dbContext.BonsEntree
            .AsNoTracking()
            .FirstOrDefaultAsync(be => be.Id == bonEntreeId.Value, ct);

        if (bonEntree is null)
            return Result.Failure<BonEntreeAssocieDto>(Error.NotFound("BonEntree", bonEntreeId.Value));

        return new BonEntreeAssocieDto(
            bonEntree.Id,
            bonEntree.NumeroReference,
            bonEntree.NomCompagnie,
            bonEntree.NomDemandeur,
            bonEntree.StatutActuel,
            bonEntree.DateCreation,
            bonEntree.EstVerrouillePourSortie);
    }
}
