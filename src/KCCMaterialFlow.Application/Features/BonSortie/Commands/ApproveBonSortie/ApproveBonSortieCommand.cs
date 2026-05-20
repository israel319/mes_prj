using FluentValidation;
using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Enums;
using KCCMaterialFlow.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
        if (currentUser.NiveauAdmin is NiveauAdmin.Admin or NiveauAdmin.SuperAdmin)
            return Result.Failure(BonSortieErrors.AdminNonAutorise);

        var bon = await dbContext.BonsSortie
            .Include(b => b.Approbations)
            .FirstOrDefaultAsync(b => b.Id == request.BonSortieId, ct);
        if (bon is null)
            return Result.Failure(Error.NotFound("BonSortie", request.BonSortieId));

        var etapeEnCours = bon.Approbations
            .Where(a => a.Decision == "En attente")
            .OrderBy(a => a.OrdreEtape)
            .FirstOrDefault();

        if (etapeEnCours is null || currentUser.EmployeeId is null || currentUser.EmployeeId.Value != etapeEnCours.ApprobateurId)
            return Result.Failure(BonSortieErrors.NonApprobateurEtape);

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
