using FluentValidation;
using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Enums;
using KCCMaterialFlow.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Application.Features.BonSortie.Commands.RejectBonSortie;

// ── Command ─────────────────────────────────────────────────────────────
public sealed record RejectBonSortieCommand(
    int BonSortieId,
    string Motif) : IRequest<Result>;

// ── Validator ───────────────────────────────────────────────────────────
public sealed class RejectBonSortieCommandValidator : AbstractValidator<RejectBonSortieCommand>
{
    public RejectBonSortieCommandValidator()
    {
        RuleFor(x => x.BonSortieId).GreaterThan(0);
        RuleFor(x => x.Motif).NotEmpty().MaximumLength(1000);
    }
}

// ── Handler ─────────────────────────────────────────────────────────────
internal sealed class RejectBonSortieCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser)
    : IRequestHandler<RejectBonSortieCommand, Result>
{
    public async Task<Result> Handle(RejectBonSortieCommand request, CancellationToken ct)
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

        var result = bon.Rejeter(
            currentUser.GetUserLogin(),
            currentUser.GetUserDisplayName(),
            request.Motif);

        if (result.IsFailure)
            return result;

        await dbContext.SaveChangesAsync(ct);
        return Result.Success();
    }
}
