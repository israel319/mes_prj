using FluentValidation;
using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Application.Features.BonSortie.Commands.CreateBonSortieExterne;
using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Domain.Enums;
using KCCMaterialFlow.Domain.Errors;
using MediatR;

namespace KCCMaterialFlow.Application.Features.BonSortie.Commands.CreateBonSortieInterne;

// ── Command ─────────────────────────────────────────────────────────────
public sealed record CreateBonSortieInterneCommand(
    string NomDemandeur,
    string FonctionDemandeur,
    string DepartementDemandeur,
    string MotifSortie,
    string Provenance,
    string Destination,
    DateTime DateExpiration,
    TypeMateriel TypeMateriel,
    int? BonEntreeAssocieId = null,
    string? RaisonSortieCode = null,
    string? Description = null,
    string? DepartementOrigine = null,
    string? FonctionReceveur = null,
    string? EmailReceveur = null,
    string? LocalisationDestination = null,
    DateTime? DateTransfertPrevue = null,
    List<MaterielSortieItemDto>? Materiels = null) : IRequest<Result<int>>;

// ── Validator ───────────────────────────────────────────────────────────
public sealed class CreateBonSortieInterneCommandValidator : AbstractValidator<CreateBonSortieInterneCommand>
{
    public CreateBonSortieInterneCommandValidator()
    {
        RuleFor(x => x.NomDemandeur).NotEmpty().MaximumLength(200);
        RuleFor(x => x.FonctionDemandeur).NotEmpty().MaximumLength(150);
        RuleFor(x => x.DepartementDemandeur).NotEmpty().MaximumLength(100);
        RuleFor(x => x.MotifSortie).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Provenance).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Destination).NotEmpty().MaximumLength(200);
        RuleFor(x => x.DateExpiration).GreaterThan(DateTime.Now);
    }
}

// ── Handler ─────────────────────────────────────────────────────────────
internal sealed class CreateBonSortieInterneCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser)
    : IRequestHandler<CreateBonSortieInterneCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateBonSortieInterneCommand request, CancellationToken ct)
    {
        var result = BonSortieInterne.Create(
            request.NomDemandeur, request.FonctionDemandeur, request.DepartementDemandeur,
            currentUser.GetUserLogin(), request.MotifSortie, request.Provenance, request.Destination,
            request.DateExpiration, request.TypeMateriel,
            request.BonEntreeAssocieId, request.RaisonSortieCode, request.Description,
            request.DepartementOrigine, request.FonctionReceveur, request.EmailReceveur,
            request.LocalisationDestination, request.DateTransfertPrevue);

        if (result.IsFailure)
            return Result.Failure<int>(result.Error);

        var bon = result.Value;

        if (request.Materiels is { Count: > 0 })
        {
            foreach (var m in request.Materiels)
                bon.AjouterMateriel(new MaterielSortie(bon.Id, m.CodeProduitSerial, m.Designation, m.Quantite));
        }

        dbContext.BonsSortieInterne.Add(bon);
        await dbContext.SaveChangesAsync(ct);

        return bon.Id;
    }
}
