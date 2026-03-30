using FluentValidation;
using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Application.Features.BonSortie.Commands.CreateBonSortieExterne;
using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Domain.Enums;
using KCCMaterialFlow.Domain.Errors;
using MediatR;

namespace KCCMaterialFlow.Application.Features.BonSortie.Commands.CreatePret;

// ── Command ─────────────────────────────────────────────────────────────
public sealed record CreatePretCommand(
    string NomDemandeur,
    string FonctionDemandeur,
    string DepartementDemandeur,
    string MotifSortie,
    string Provenance,
    string Destination,
    DateTime DateExpiration,
    string NomDestinataire,
    TypeMateriel TypeMateriel,
    DateTime DateRetourPrevue,
    int? BonEntreeAssocieId = null,
    string? RaisonSortieCode = null,
    string? Description = null,
    string? AdresseDestination = null,
    List<MaterielSortieItemDto>? Materiels = null) : IRequest<Result<int>>;

// ── Validator ───────────────────────────────────────────────────────────
public sealed class CreatePretCommandValidator : AbstractValidator<CreatePretCommand>
{
    public CreatePretCommandValidator()
    {
        RuleFor(x => x.NomDemandeur).NotEmpty().MaximumLength(200);
        RuleFor(x => x.FonctionDemandeur).NotEmpty().MaximumLength(150);
        RuleFor(x => x.DepartementDemandeur).NotEmpty().MaximumLength(100);
        RuleFor(x => x.MotifSortie).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Provenance).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Destination).NotEmpty().MaximumLength(200);
        RuleFor(x => x.NomDestinataire).NotEmpty().MaximumLength(200);
        RuleFor(x => x.DateExpiration).GreaterThan(DateTime.Now);
        RuleFor(x => x.DateRetourPrevue).GreaterThan(DateTime.Now);
    }
}

// ── Handler ─────────────────────────────────────────────────────────────
internal sealed class CreatePretCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser)
    : IRequestHandler<CreatePretCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreatePretCommand request, CancellationToken ct)
    {
        var result = Pret.Create(
            request.NomDemandeur, request.FonctionDemandeur, request.DepartementDemandeur,
            currentUser.GetUserLogin(), request.MotifSortie, request.Provenance, request.Destination,
            request.DateExpiration, request.NomDestinataire, request.TypeMateriel,
            request.DateRetourPrevue,
            request.BonEntreeAssocieId, request.RaisonSortieCode, request.Description,
            request.AdresseDestination);

        if (result.IsFailure)
            return Result.Failure<int>(result.Error);

        var pret = result.Value;

        if (request.Materiels is { Count: > 0 })
        {
            foreach (var m in request.Materiels)
                pret.AjouterMateriel(new MaterielSortie(pret.Id, m.CodeProduitSerial, m.Designation, m.Quantite));
        }

        dbContext.Prets.Add(pret);
        await dbContext.SaveChangesAsync(ct);

        return pret.Id;
    }
}
