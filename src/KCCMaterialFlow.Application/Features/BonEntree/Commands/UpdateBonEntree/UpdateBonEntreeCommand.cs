using FluentValidation;
using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Errors;
using MediatR;

namespace KCCMaterialFlow.Application.Features.BonEntree.Commands.UpdateBonEntree;

public sealed record UpdateBonEntreeCommand(
    int BonEntreeId,
    string NomCompagnie,
    string SiteManager,
    string HostDepartment,
    string ReasonOnSite,
    string NomEscorteur,
    string Provenance,
    string Destination,
    DateTime DateExpiration,
    string? Description) : IRequest<Result>;

public sealed class UpdateBonEntreeCommandValidator : AbstractValidator<UpdateBonEntreeCommand>
{
    public UpdateBonEntreeCommandValidator()
    {
        RuleFor(x => x.BonEntreeId).GreaterThan(0);
        RuleFor(x => x.NomCompagnie).NotEmpty().MaximumLength(200);
        RuleFor(x => x.SiteManager).NotEmpty().MaximumLength(200);
        RuleFor(x => x.HostDepartment).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ReasonOnSite).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.NomEscorteur).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Provenance).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Destination).NotEmpty().MaximumLength(200);
    }
}

public sealed class UpdateBonEntreeCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<UpdateBonEntreeCommand, Result>
{
    public async Task<Result> Handle(UpdateBonEntreeCommand cmd, CancellationToken ct)
    {
        var bon = await dbContext.BonsEntree.FindAsync([cmd.BonEntreeId], ct);
        if (bon is null)
            return Result.Failure(Error.NotFound("BonEntree", cmd.BonEntreeId));

        var result = bon.MettreAJour(
            cmd.NomCompagnie, cmd.SiteManager, cmd.HostDepartment,
            cmd.ReasonOnSite, cmd.NomEscorteur, cmd.Provenance,
            cmd.Destination, cmd.DateExpiration, cmd.Description);

        if (result.IsFailure)
            return result;

        await dbContext.SaveChangesAsync(ct);
        return Result.Success();
    }
}
