using FluentValidation;
using KCCMaterialFlow.Application.Common.Interfaces;

namespace KCCMaterialFlow.Application.Features.BonEntree.Validators;

public class CreateBonEntreeRequestValidator : AbstractValidator<CreateBonEntreeRequest>
{
    public CreateBonEntreeRequestValidator()
    {
        RuleFor(x => x.NomCompagnie)
            .NotEmpty().WithMessage("Le nom de la compagnie est obligatoire")
            .MaximumLength(200);

        RuleFor(x => x.SiteManager)
            .NotEmpty().WithMessage("Le Site Manager est obligatoire")
            .MaximumLength(200);

        RuleFor(x => x.HostDepartment)
            .NotEmpty().WithMessage("Le département hôte est obligatoire")
            .MaximumLength(100);

        RuleFor(x => x.ReasonOnSite)
            .NotEmpty().WithMessage("Le motif de présence est obligatoire")
            .MaximumLength(1000);

        RuleFor(x => x.NomEscorteur)
            .NotEmpty().WithMessage("Le nom de l'escorteur est obligatoire")
            .MaximumLength(200);

        RuleFor(x => x.Provenance)
            .NotEmpty().WithMessage("La provenance est obligatoire")
            .MaximumLength(200);

        RuleFor(x => x.Destination)
            .NotEmpty().WithMessage("La destination est obligatoire")
            .MaximumLength(200);

        RuleFor(x => x.DateExpiration)
            .GreaterThan(DateTime.Today).WithMessage("La date d'expiration doit être dans le futur");

        RuleFor(x => x.Materiels)
            .NotEmpty().WithMessage("Au moins un matériel est requis");

        RuleForEach(x => x.Materiels).SetValidator(new MaterielRequestValidator());
    }
}

public class MaterielRequestValidator : AbstractValidator<MaterielRequest>
{
    public MaterielRequestValidator()
    {
        RuleFor(x => x.CodeProduitSerial)
            .NotEmpty().WithMessage("Le code/série est obligatoire")
            .MaximumLength(100);

        RuleFor(x => x.Designation)
            .NotEmpty().WithMessage("La désignation est obligatoire")
            .MaximumLength(300);

        RuleFor(x => x.Quantite)
            .GreaterThan(0).WithMessage("La quantité doit être positive");
    }
}

public class UpdateBonEntreeRequestValidator : AbstractValidator<UpdateBonEntreeRequest>
{
    public UpdateBonEntreeRequestValidator()
    {
        RuleFor(x => x.IdBon).GreaterThan(0);

        RuleFor(x => x.NomCompagnie)
            .NotEmpty().WithMessage("Le nom de la compagnie est obligatoire")
            .MaximumLength(200);

        RuleFor(x => x.SiteManager)
            .NotEmpty().WithMessage("Le Site Manager est obligatoire");

        RuleFor(x => x.HostDepartment)
            .NotEmpty().WithMessage("Le département hôte est obligatoire");

        RuleFor(x => x.ReasonOnSite)
            .NotEmpty().WithMessage("Le motif est obligatoire");

        RuleFor(x => x.NomEscorteur)
            .NotEmpty().WithMessage("L'escorteur est obligatoire");
    }
}
