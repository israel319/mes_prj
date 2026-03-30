using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Application.Features.BonEntree.Commands.RemoveMateriel;

public sealed record RemoveMaterielCommand(int BonEntreeId, int MaterielId) : IRequest<Result>;

public sealed class RemoveMaterielCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<RemoveMaterielCommand, Result>
{
    public async Task<Result> Handle(RemoveMaterielCommand cmd, CancellationToken ct)
    {
        var bon = await dbContext.BonsEntree
            .Include(b => b.Materiels)
            .FirstOrDefaultAsync(b => b.Id == cmd.BonEntreeId, ct);

        if (bon is null)
            return Result.Failure(Error.NotFound("BonEntree", cmd.BonEntreeId));

        var result = bon.SupprimerMateriel(cmd.MaterielId);
        if (result.IsFailure)
            return result;

        await dbContext.SaveChangesAsync(ct);
        return Result.Success();
    }
}
