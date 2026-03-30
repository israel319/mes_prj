using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Application.Features.BonSortie.Commands.SubmitBonSortie;

// ── Command ─────────────────────────────────────────────────────────────
public sealed record SubmitBonSortieCommand(int BonSortieId) : IRequest<Result>;

// ── Handler ─────────────────────────────────────────────────────────────
internal sealed class SubmitBonSortieCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<SubmitBonSortieCommand, Result>
{
    public async Task<Result> Handle(SubmitBonSortieCommand request, CancellationToken ct)
    {
        var bon = await dbContext.BonsSortie
            .Include(b => b.Materiels)
            .FirstOrDefaultAsync(b => b.Id == request.BonSortieId, ct);

        if (bon is null)
            return Result.Failure(Error.NotFound("BonSortie", request.BonSortieId));

        var result = bon.SoumettrePourApprobation();
        if (result.IsFailure)
            return result;

        await dbContext.SaveChangesAsync(ct);
        return Result.Success();
    }
}
