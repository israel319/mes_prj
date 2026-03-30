using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Application.Features.BonSortie.Queries.GetBonEntreeForSortie;

// ── DTOs ────────────────────────────────────────────────────────────────
public sealed record MaterielDisponibleDto(
    int Id,
    string CodeProduitSerial,
    string Designation,
    decimal Quantite);

public sealed record BonEntreeDetailsPourSortieDto(
    int IdBon,
    string NumeroReference,
    string NomCompagnie,
    string NomDemandeur,
    string StatutActuel,
    string Provenance,
    string Destination,
    bool EstVerrouillePourSortie,
    int? BonSortieAssocieId,
    string? BonSortieAssocieNumero,
    IReadOnlyList<MaterielDisponibleDto> Materiels);

// ── Query ───────────────────────────────────────────────────────────────
public sealed record GetBonEntreeForSortieQuery(int BonEntreeId)
    : IRequest<Result<BonEntreeDetailsPourSortieDto>>;

// ── Handler ─────────────────────────────────────────────────────────────
internal sealed class GetBonEntreeForSortieQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetBonEntreeForSortieQuery, Result<BonEntreeDetailsPourSortieDto>>
{
    public async Task<Result<BonEntreeDetailsPourSortieDto>> Handle(
        GetBonEntreeForSortieQuery request, CancellationToken ct)
    {
        var bonEntree = await dbContext.BonsEntree
            .AsNoTracking()
            .Include(be => be.Materiels)
            .FirstOrDefaultAsync(be => be.Id == request.BonEntreeId, ct);

        if (bonEntree is null)
            return Result.Failure<BonEntreeDetailsPourSortieDto>(
                Error.NotFound("BonEntree", request.BonEntreeId));

        var materiels = bonEntree.Materiels.Select(m =>
            new MaterielDisponibleDto(m.Id, m.CodeProduitSerial, m.Designation, m.Quantite)).ToList();

        return new BonEntreeDetailsPourSortieDto(
            bonEntree.Id,
            bonEntree.NumeroReference,
            bonEntree.NomCompagnie,
            bonEntree.NomDemandeur,
            bonEntree.StatutActuel,
            bonEntree.Provenance,
            bonEntree.Destination,
            bonEntree.EstVerrouillePourSortie,
            bonEntree.BonSortieAssocieId,
            bonEntree.BonSortieAssocieNumero,
            materiels);
    }
}
