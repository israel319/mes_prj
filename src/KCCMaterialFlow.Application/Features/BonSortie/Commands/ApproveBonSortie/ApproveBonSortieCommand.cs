using FluentValidation;
using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Enums;
using KCCMaterialFlow.Domain.Errors;
using MediatR;

namespace KCCMaterialFlow.Application.Features.BonSortie.Commands.ApproveBonSortie;

// ── Command ─────────────────────────────────────────────────────────────
public sealed record ApproveBonSortieCommand(
    int BonSortieId,
    string? Commentaire,
    StatutBonSortie ProchainStatut) : IRequest<Result>;

// ── Validator ───────────────────────────────────────────────────────────
public sealed class ApproveBonSortieCommandValidator : AbstractValidator<ApproveBonSortieCommand>
{
    public ApproveBonSortieCommandValidator()
    {
        RuleFor(x => x.BonSortieId).GreaterThan(0);
        RuleFor(x => x.ProchainStatut).IsInEnum();
    }
}

// ── Handler ─────────────────────────────────────────────────────────────
internal sealed class ApproveBonSortieCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser)
    : IRequestHandler<ApproveBonSortieCommand, Result>
{
    public async Task<Result> Handle(ApproveBonSortieCommand request, CancellationToken ct)
    {
        var bon = await dbContext.BonsSortie.FindAsync([request.BonSortieId], ct);
        if (bon is null)
            return Result.Failure(Error.NotFound("BonSortie", request.BonSortieId));

        var result = bon.Approuver(
            currentUser.GetUserLogin(),
            currentUser.GetUserDisplayName(),
            request.Commentaire,
            request.ProchainStatut);

        if (result.IsFailure)
            return result;

        await dbContext.SaveChangesAsync(ct);
        return Result.Success();
    }
}
