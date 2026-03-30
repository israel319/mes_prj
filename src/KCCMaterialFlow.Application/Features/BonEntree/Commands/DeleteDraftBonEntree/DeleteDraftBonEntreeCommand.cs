using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Enums;
using KCCMaterialFlow.Domain.Errors;
using MediatR;

namespace KCCMaterialFlow.Application.Features.BonEntree.Commands.DeleteDraftBonEntree;

public sealed record DeleteDraftBonEntreeCommand(int BonEntreeId) : IRequest<Result>;

public sealed class DeleteDraftBonEntreeCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<DeleteDraftBonEntreeCommand, Result>
{
    public async Task<Result> Handle(DeleteDraftBonEntreeCommand cmd, CancellationToken ct)
    {
        var bon = await dbContext.BonsEntree.FindAsync([cmd.BonEntreeId], ct);
        if (bon is null)
            return Result.Failure(Error.NotFound("BonEntree", cmd.BonEntreeId));

        if (!bon.EstModifiable())
            return Result.Failure(BonEntreeErrors.ModificationInterdite(bon.Statut));

        dbContext.BonsEntree.Remove(bon);
        await dbContext.SaveChangesAsync(ct);
        return Result.Success();
    }
}
