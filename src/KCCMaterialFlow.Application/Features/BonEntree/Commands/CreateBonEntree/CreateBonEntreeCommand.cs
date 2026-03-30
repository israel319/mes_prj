using FluentValidation;
using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Errors;
using MediatR;

namespace KCCMaterialFlow.Application.Features.BonEntree.Commands.CreateBonEntree;

public sealed record MaterielItemDto(string CodeProduitSerial, string Designation, decimal Quantite);

public sealed record CreateBonEntreeCommand(
    string NomCompagnie,
    string SiteManager,
    string HostDepartment,
    string ReasonOnSite,
    string NomEscorteur,
    string Provenance,
    string Destination,
    DateTime? DateExpiration,
    string? Description,
    string? EmailContractant,
    string? FonctionEscorteur,
    int? ContratId,
    string? NumeroContrat,
    List<MaterielItemDto>? Materiels) : IRequest<Result<int>>;

public sealed class CreateBonEntreeCommandValidator : AbstractValidator<CreateBonEntreeCommand>
{
    public CreateBonEntreeCommandValidator()
    {
        RuleFor(x => x.NomCompagnie).NotEmpty().MaximumLength(200);
        RuleFor(x => x.SiteManager).NotEmpty().MaximumLength(200);
        RuleFor(x => x.HostDepartment).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ReasonOnSite).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.NomEscorteur).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Provenance).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Destination).NotEmpty().MaximumLength(200);
    }
}

public sealed class CreateBonEntreeCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser)
    : IRequestHandler<CreateBonEntreeCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateBonEntreeCommand cmd, CancellationToken ct)
    {
        var result = Domain.Entities.BonEntree.Create(
            nomDemandeur: currentUser.GetUserDisplayName(),
            nomCompagnie: cmd.NomCompagnie,
            siteManager: cmd.SiteManager,
            hostDepartment: cmd.HostDepartment,
            reasonOnSite: cmd.ReasonOnSite,
            nomEscorteur: cmd.NomEscorteur,
            provenance: cmd.Provenance,
            destination: cmd.Destination,
            dateExpiration: cmd.DateExpiration,
            description: cmd.Description,
            emailContractant: cmd.EmailContractant,
            fonctionEscorteur: cmd.FonctionEscorteur,
            contratId: cmd.ContratId,
            numeroContrat: cmd.NumeroContrat);

        if (result.IsFailure)
            return Result.Failure<int>(result.Error);

        var bon = result.Value;

        if (cmd.Materiels is { Count: > 0 })
        {
            foreach (var item in cmd.Materiels)
            {
                var addResult = bon.AjouterMateriel(item.CodeProduitSerial, item.Designation, item.Quantite);
                if (addResult.IsFailure)
                    return Result.Failure<int>(addResult.Error);
            }
        }

        dbContext.BonsEntree.Add(bon);
        await dbContext.SaveChangesAsync(ct);

        return bon.Id;
    }
}
