using FluentValidation;
using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Errors;
using MediatR;

namespace KCCMaterialFlow.Application.Features.BonSortie.Commands.ReturnBonSortie;

// ── Command ─────────────────────────────────────────────────────────────
public sealed record ReturnBonSortieCommand(
    int BonSortieId,
    string Motif) : IRequest<Result>;

// ── Validator ───────────────────────────────────────────────────────────
public sealed class ReturnBonSortieCommandValidator : AbstractValidator<ReturnBonSortieCommand>
{
    public ReturnBonSortieCommandValidator()
    {
        RuleFor(x => x.BonSortieId).GreaterThan(0);
        RuleFor(x => x.Motif).NotEmpty().MaximumLength(1000);
    }
}

// ── Handler ─────────────────────────────────────────────────────────────
internal sealed class ReturnBonSortieCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser)
    : IRequestHandler<ReturnBonSortieCommand, Result>
{
    public async Task<Result> Handle(ReturnBonSortieCommand request, CancellationToken ct)
    {
        var bon = await dbContext.BonsSortie.FindAsync([request.BonSortieId], ct);
        if (bon is null)
            return Result.Failure(Error.NotFound("BonSortie", request.BonSortieId));

        var result = bon.RetournerPourModification(
            currentUser.GetUserLogin(),
            request.Motif);

        if (result.IsFailure)
            return result;

        await dbContext.SaveChangesAsync(ct);
        return Result.Success();
    }
}
