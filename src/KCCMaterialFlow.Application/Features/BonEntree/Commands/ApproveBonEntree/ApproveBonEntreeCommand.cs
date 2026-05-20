using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Enums;
using KCCMaterialFlow.Domain.Errors;
using MediatR;

namespace KCCMaterialFlow.Application.Features.BonEntree.Commands.ApproveBonEntree;

public sealed record ApproveBonEntreeCommand(
    int BonEntreeId,
    string? Commentaire,
    StatutBonEntree ProchainStatut) : IRequest<Result>;

public sealed class ApproveBonEntreeCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser)
    : IRequestHandler<ApproveBonEntreeCommand, Result>
{
    public async Task<Result> Handle(ApproveBonEntreeCommand cmd, CancellationToken ct)
    {
        if (currentUser.NiveauAdmin is NiveauAdmin.Admin or NiveauAdmin.SuperAdmin)
            return Result.Failure(BonEntreeErrors.AdminNonAutorise);

        var bon = await dbContext.BonsEntree.FindAsync([cmd.BonEntreeId], ct);
        if (bon is null)
            return Result.Failure(Error.NotFound("BonEntree", cmd.BonEntreeId));

        if (currentUser.EmployeeId is null || currentUser.EmployeeId.Value != bon.ProchainApprobateurId)
            return Result.Failure(BonEntreeErrors.NonApprobateurEtape);

        var login = currentUser.GetUserLogin();
        var nom = currentUser.GetUserDisplayName();

        var result = bon.Approuver(login, nom, cmd.Commentaire, cmd.ProchainStatut);
        if (result.IsFailure)
            return result;

        await dbContext.SaveChangesAsync(ct);
        return Result.Success();
    }
}
