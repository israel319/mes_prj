using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Errors;
using MediatR;

namespace KCCMaterialFlow.Application.Features.BonSortie.Commands.DeleteDraftBonSortie;

// ── Command ─────────────────────────────────────────────────────────────
public sealed record DeleteDraftBonSortieCommand(int BonSortieId) : IRequest<Result>;

// ── Handler ─────────────────────────────────────────────────────────────
internal sealed class DeleteDraftBonSortieCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<DeleteDraftBonSortieCommand, Result>
{
    public async Task<Result> Handle(DeleteDraftBonSortieCommand request, CancellationToken ct)
    {
        var bon = await dbContext.BonsSortie.FindAsync([request.BonSortieId], ct);
        if (bon is null)
            return Result.Failure(Error.NotFound("BonSortie", request.BonSortieId));

        if (!bon.EstModifiable())
            return Result.Failure(BonSortieErrors.ModificationInterdite(bon.Statut));

        dbContext.BonsSortie.Remove(bon);
        await dbContext.SaveChangesAsync(ct);

        return Result.Success();
    }
}
