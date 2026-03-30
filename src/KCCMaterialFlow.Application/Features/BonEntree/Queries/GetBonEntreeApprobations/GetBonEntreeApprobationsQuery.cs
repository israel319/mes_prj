using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Application.Features.BonEntree.Queries.GetBonEntreeDetail;
using KCCMaterialFlow.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Application.Features.BonEntree.Queries.GetBonEntreeApprobations;

public sealed record GetBonEntreeApprobationsQuery(int BonEntreeId)
    : IRequest<Result<IReadOnlyList<ApprobationDto>>>;

public sealed class GetBonEntreeApprobationsQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetBonEntreeApprobationsQuery, Result<IReadOnlyList<ApprobationDto>>>
{
    public async Task<Result<IReadOnlyList<ApprobationDto>>> Handle(
        GetBonEntreeApprobationsQuery query, CancellationToken ct)
    {
        var bonExists = await dbContext.BonsEntree
            .AsNoTracking()
            .AnyAsync(b => b.Id == query.BonEntreeId, ct);

        if (!bonExists)
            return Result.Failure<IReadOnlyList<ApprobationDto>>(
                Error.NotFound("BonEntree", query.BonEntreeId));

        var approbations = await dbContext.Approbations
            .AsNoTracking()
            .Where(a => a.BonId == query.BonEntreeId)
            .OrderBy(a => a.OrdreEtape)
            .Select(a => new ApprobationDto(
                a.Id,
                a.OrdreEtape,
                a.NomEtape ?? string.Empty,
                a.NomApprobateur,
                a.Decision,
                a.ReservesEventuelles,
                a.DateAction))
            .ToListAsync(ct);

        return approbations;
    }
}
