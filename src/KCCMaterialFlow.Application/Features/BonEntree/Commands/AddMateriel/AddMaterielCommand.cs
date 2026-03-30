using FluentValidation;
using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Errors;
using MediatR;

namespace KCCMaterialFlow.Application.Features.BonEntree.Commands.AddMateriel;

public sealed record AddMaterielCommand(
    int BonEntreeId,
    string CodeProduitSerial,
    string Designation,
    decimal Quantite) : IRequest<Result>;

public sealed class AddMaterielCommandValidator : AbstractValidator<AddMaterielCommand>
{
    public AddMaterielCommandValidator()
    {
        RuleFor(x => x.BonEntreeId).GreaterThan(0);
        RuleFor(x => x.CodeProduitSerial).NotEmpty();
        RuleFor(x => x.Designation).NotEmpty();
        RuleFor(x => x.Quantite).GreaterThan(0);
    }
}

public sealed class AddMaterielCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<AddMaterielCommand, Result>
{
    public async Task<Result> Handle(AddMaterielCommand cmd, CancellationToken ct)
    {
        var bon = await dbContext.BonsEntree.FindAsync([cmd.BonEntreeId], ct);
        if (bon is null)
            return Result.Failure(Error.NotFound("BonEntree", cmd.BonEntreeId));

        var result = bon.AjouterMateriel(cmd.CodeProduitSerial, cmd.Designation, cmd.Quantite);
        if (result.IsFailure)
            return result;

        await dbContext.SaveChangesAsync(ct);
        return Result.Success();
    }
}
