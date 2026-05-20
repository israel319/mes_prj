using FluentValidation;
using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Domain.Enums;
using KCCMaterialFlow.Domain.Errors;
using MediatR;

namespace KCCMaterialFlow.Application.Features.BonSortie.Commands.CreateBonSortieExterne;

// ── DTO ─────────────────────────────────────────────────────────────────
public sealed record MaterielSortieItemDto(
    string CodeProduitSerial,
    string Designation,
    decimal Quantite,
    int? MaterielEntreeId = null,
    int? BonEntreeId = null,
    string? BonEntreeNumero = null);

// ── Command ─────────────────────────────────────────────────────────────
public sealed record CreateBonSortieExterneCommand(
    string NomDemandeur,
    string FonctionDemandeur,
    string DepartementDemandeur,
    string MotifSortie,
    string Provenance,
    string Destination,
    DateTime DateExpiration,
    string NomDestinataire,
    string? DescriptionMateriel,
    int? BonEntreeAssocieId = null,
    string? RaisonSortieCode = null,
    string? Description = null,
    string? AdresseDestination = null,
    string? NumeroVehicule = null,
    string? NomChauffeur = null,
    string? TelephoneChauffeur = null,
    List<MaterielSortieItemDto>? Materiels = null) : IRequest<Result<int>>;

// ── Validator ───────────────────────────────────────────────────────────
public sealed class CreateBonSortieExterneCommandValidator : AbstractValidator<CreateBonSortieExterneCommand>
{
    public CreateBonSortieExterneCommandValidator()
    {
        RuleFor(x => x.NomDemandeur).NotEmpty().MaximumLength(200);
        RuleFor(x => x.FonctionDemandeur).NotEmpty().MaximumLength(150);
        RuleFor(x => x.DepartementDemandeur).NotEmpty().MaximumLength(100);
        RuleFor(x => x.MotifSortie).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Provenance).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Destination).NotEmpty().MaximumLength(200);
        RuleFor(x => x.NomDestinataire).NotEmpty().MaximumLength(200);
        RuleFor(x => x.DateExpiration).GreaterThan(DateTime.Now);
    }
}

// ── Handler ─────────────────────────────────────────────────────────────
internal sealed class CreateBonSortieExterneCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser)
    : IRequestHandler<CreateBonSortieExterneCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateBonSortieExterneCommand request, CancellationToken ct)
    {
        var result = BonSortieExterne.Create(
            request.NomDemandeur, request.FonctionDemandeur, request.DepartementDemandeur,
            currentUser.GetUserLogin(), request.MotifSortie, request.Provenance, request.Destination,
            request.DateExpiration, request.NomDestinataire, request.DescriptionMateriel,
            request.BonEntreeAssocieId, request.RaisonSortieCode, request.Description,
            request.AdresseDestination, request.NumeroVehicule, request.NomChauffeur,
            request.TelephoneChauffeur);

        if (result.IsFailure)
            return Result.Failure<int>(result.Error);

        var bon = result.Value;

        if (request.Materiels is { Count: > 0 })
        {
            foreach (var m in request.Materiels)
                bon.AjouterMateriel(new MaterielSortie(bon.Id, m.CodeProduitSerial, m.Designation, m.Quantite));
        }

        dbContext.BonsSortieExterne.Add(bon);
        await dbContext.SaveChangesAsync(ct);

        return bon.Id;
    }
}
