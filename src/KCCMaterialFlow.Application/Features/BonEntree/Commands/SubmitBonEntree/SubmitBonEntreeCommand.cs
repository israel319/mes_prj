using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Application.Features.BonEntree.Commands.SubmitBonEntree;

public sealed record SubmitBonEntreeCommand(int BonEntreeId) : IRequest<Result>;

public sealed class SubmitBonEntreeCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<SubmitBonEntreeCommand, Result>
{
    public async Task<Result> Handle(SubmitBonEntreeCommand cmd, CancellationToken ct)
    {
        var bon = await dbContext.BonsEntree
            .Include(b => b.Materiels)
            .FirstOrDefaultAsync(b => b.Id == cmd.BonEntreeId, ct);

        if (bon is null)
            return Result.Failure(Error.NotFound("BonEntree", cmd.BonEntreeId));

        var result = bon.SoumettrePourApprobation();
        if (result.IsFailure)
            return result;

        await dbContext.SaveChangesAsync(ct);
        return Result.Success();
    }
}
