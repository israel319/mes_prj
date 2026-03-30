using FluentValidation;
using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Errors;
using MediatR;

namespace KCCMaterialFlow.Application.Features.BonEntree.Commands.RejectBonEntree;

public sealed record RejectBonEntreeCommand(int BonEntreeId, string Motif) : IRequest<Result>;

public sealed class RejectBonEntreeCommandValidator : AbstractValidator<RejectBonEntreeCommand>
{
    public RejectBonEntreeCommandValidator()
    {
        RuleFor(x => x.BonEntreeId).GreaterThan(0);
        RuleFor(x => x.Motif).NotEmpty().MaximumLength(2000);
    }
}

public sealed class RejectBonEntreeCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser)
    : IRequestHandler<RejectBonEntreeCommand, Result>
{
    public async Task<Result> Handle(RejectBonEntreeCommand cmd, CancellationToken ct)
    {
        var bon = await dbContext.BonsEntree.FindAsync([cmd.BonEntreeId], ct);
        if (bon is null)
            return Result.Failure(Error.NotFound("BonEntree", cmd.BonEntreeId));

        var login = currentUser.GetUserLogin();
        var nom = currentUser.GetUserDisplayName();

        var result = bon.Rejeter(login, nom, cmd.Motif);
        if (result.IsFailure)
            return result;

        await dbContext.SaveChangesAsync(ct);
        return Result.Success();
    }
}
