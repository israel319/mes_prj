using FluentValidation;
using KCCMaterialFlow.Domain.Enums;
using KCCMaterialFlow.Module.BonSortie.Services;

namespace KCCMaterialFlow.Module.BonSortie.Validators;

/// <summary>
/// BSM-039: Validateur pour la création d'un bon de sortie externe.
/// Règles de validation métier pour les bons de sortie vers l'extérieur de l'entreprise.
/// </summary>
public class CreateBonSortieExterneRequestValidator : AbstractValidator<CreateBonSortieExterneRequest>
{
    public CreateBonSortieExterneRequestValidator()
    {
        RuleFor(x => x.NomDemandeur)
            .NotEmpty().WithMessage("Le nom du demandeur est obligatoire.")
            .MaximumLength(100).WithMessage("Le nom du demandeur ne peut pas dépasser 100 caractères.");

        RuleFor(x => x.FonctionDemandeur)
            .NotEmpty().WithMessage("La fonction du demandeur est obligatoire.")
            .MaximumLength(100).WithMessage("La fonction du demandeur ne peut pas dépasser 100 caractères.");

        RuleFor(x => x.DepartementDemandeur)
            .NotEmpty().WithMessage("Le département du demandeur est obligatoire.")
            .MaximumLength(100).WithMessage("Le département ne peut pas dépasser 100 caractères.");

        RuleFor(x => x.MotifSortie)
            .NotEmpty().WithMessage("Le motif de sortie est obligatoire.")
            .MaximumLength(500).WithMessage("Le motif de sortie ne peut pas dépasser 500 caractères.");

        RuleFor(x => x.Provenance)
            .NotEmpty().WithMessage("La provenance est obligatoire.")
            .MaximumLength(200).WithMessage("La provenance ne peut pas dépasser 200 caractères.");

        RuleFor(x => x.Destination)
            .NotEmpty().WithMessage("La destination est obligatoire.")
            .MaximumLength(200).WithMessage("La destination ne peut pas dépasser 200 caractères.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("La description ne peut pas dépasser 2000 caractères.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.DateExpiration)
            .NotEmpty().WithMessage("La date d'expiration est obligatoire.")
            .GreaterThan(DateTime.Today).WithMessage("La date d'expiration doit être dans le futur.");

        RuleFor(x => x.NomDestinataire)
            .NotEmpty().WithMessage("Le nom du destinataire est obligatoire.")
            .MaximumLength(100).WithMessage("Le nom du destinataire ne peut pas dépasser 100 caractères.");

        RuleFor(x => x.AdresseDestination)
            .MaximumLength(300).WithMessage("L'adresse de destination ne peut pas dépasser 300 caractères.")
            .When(x => !string.IsNullOrEmpty(x.AdresseDestination));

        RuleFor(x => x.NumeroVehicule)
            .MaximumLength(50).WithMessage("Le numéro du véhicule ne peut pas dépasser 50 caractères.")
            .When(x => !string.IsNullOrEmpty(x.NumeroVehicule));

        RuleFor(x => x.NomChauffeur)
            .MaximumLength(100).WithMessage("Le nom du chauffeur ne peut pas dépasser 100 caractères.")
            .When(x => !string.IsNullOrEmpty(x.NomChauffeur));

        RuleFor(x => x.TelephoneChauffeur)
            .MaximumLength(20).WithMessage("Le téléphone du chauffeur ne peut pas dépasser 20 caractères.")
            .Matches(@"^[\d\s\+\-\(\)]+$").WithMessage("Format de téléphone invalide.")
            .When(x => !string.IsNullOrEmpty(x.TelephoneChauffeur));

        RuleFor(x => x.TypeMateriel)
            .IsInEnum().WithMessage("Le type de matériel est invalide.");

        RuleFor(x => x.Materiels)
            .NotEmpty().WithMessage("Au moins un matériel doit être spécifié.");

        RuleForEach(x => x.Materiels)
            .SetValidator(new MaterielDtoValidator());

        RuleFor(x => x.BarrieresIds)
            .NotEmpty().WithMessage("Au moins une barrière de passage doit être spécifiée.");
    }
}

/// <summary>
/// Validateur pour la création d'un bon de sortie interne.
/// </summary>
public class CreateBonSortieInterneRequestValidator : AbstractValidator<CreateBonSortieInterneRequest>
{
    public CreateBonSortieInterneRequestValidator()
    {
        RuleFor(x => x.NomDemandeur)
            .NotEmpty().WithMessage("Le nom du demandeur est obligatoire.")
            .MaximumLength(100).WithMessage("Le nom du demandeur ne peut pas dépasser 100 caractères.");

        RuleFor(x => x.FonctionDemandeur)
            .NotEmpty().WithMessage("La fonction du demandeur est obligatoire.")
            .MaximumLength(100).WithMessage("La fonction du demandeur ne peut pas dépasser 100 caractères.");

        RuleFor(x => x.DepartementDemandeur)
            .NotEmpty().WithMessage("Le département du demandeur est obligatoire.")
            .MaximumLength(100).WithMessage("Le département ne peut pas dépasser 100 caractères.");

        RuleFor(x => x.MotifSortie)
            .NotEmpty().WithMessage("Le motif de sortie est obligatoire.")
            .MaximumLength(500).WithMessage("Le motif de sortie ne peut pas dépasser 500 caractères.");

        RuleFor(x => x.DepartementOrigine)
            .MaximumLength(100).WithMessage("Le département d'origine ne peut pas dépasser 100 caractères.")
            .When(x => !string.IsNullOrEmpty(x.DepartementOrigine));

        RuleFor(x => x.FonctionReceveur)
            .MaximumLength(100).WithMessage("La fonction du receveur ne peut pas dépasser 100 caractères.")
            .When(x => !string.IsNullOrEmpty(x.FonctionReceveur));

        RuleFor(x => x.EmailReceveur)
            .EmailAddress().WithMessage("L'adresse email du receveur n'est pas valide.")
            .When(x => !string.IsNullOrEmpty(x.EmailReceveur));

        RuleFor(x => x.LocalisationDestination)
            .MaximumLength(200).WithMessage("La localisation de destination ne peut pas dépasser 200 caractères.")
            .When(x => !string.IsNullOrEmpty(x.LocalisationDestination));

        RuleFor(x => x.DateTransfertPrevue)
            .GreaterThan(DateTime.Today).WithMessage("La date de transfert prévue doit être dans le futur.")
            .When(x => x.DateTransfertPrevue.HasValue);

        RuleFor(x => x.DateExpiration)
            .NotEmpty().WithMessage("La date d'expiration est obligatoire.")
            .GreaterThan(DateTime.Today).WithMessage("La date d'expiration doit être dans le futur.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("La description ne peut pas dépasser 2000 caractères.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.TypeMateriel)
            .IsInEnum().WithMessage("Le type de matériel est invalide.");

        RuleFor(x => x.Materiels)
            .NotEmpty().WithMessage("Au moins un matériel doit être spécifié.");

        RuleForEach(x => x.Materiels)
            .SetValidator(new MaterielDtoValidator());
    }
}

/// <summary>
/// Validateur pour la création d'un prêt.
/// </summary>
public class CreatePretRequestValidator : AbstractValidator<CreatePretRequest>
{
    public CreatePretRequestValidator()
    {
        // Inclure toutes les règles de base du bon de sortie externe
        Include(new CreateBonSortieExterneRequestValidator());

        RuleFor(x => x.DateRetourPrevue)
            .NotEmpty().WithMessage("La date de retour prévue est obligatoire.")
            .GreaterThan(DateTime.Today).WithMessage("La date de retour prévue doit être dans le futur.")
            .LessThanOrEqualTo(x => x.DateExpiration)
                .WithMessage("La date de retour prévue ne peut pas dépasser la date d'expiration.");
    }
}

/// <summary>
/// Validateur pour la mise à jour d'un bon de sortie.
/// </summary>
public class UpdateBonSortieRequestValidator : AbstractValidator<UpdateBonSortieRequest>
{
    public UpdateBonSortieRequestValidator()
    {
        RuleFor(x => x.IdBon)
            .GreaterThan(0).WithMessage("L'identifiant du bon est invalide.");

        RuleFor(x => x.MotifSortie)
            .MaximumLength(500).WithMessage("Le motif de sortie ne peut pas dépasser 500 caractères.")
            .When(x => !string.IsNullOrEmpty(x.MotifSortie));

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("La description ne peut pas dépasser 2000 caractères.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.DateExpiration)
            .GreaterThan(DateTime.Today).WithMessage("La date d'expiration doit être dans le futur.")
            .When(x => x.DateExpiration.HasValue);

        RuleForEach(x => x.Materiels)
            .SetValidator(new MaterielDtoValidator())
            .When(x => x.Materiels != null && x.Materiels.Count > 0);
    }
}

/// <summary>
/// Validateur pour le DTO de matériel.
/// </summary>
public class MaterielDtoValidator : AbstractValidator<MaterielDto>
{
    public MaterielDtoValidator()
    {
        RuleFor(x => x.IdMateriel)
            .GreaterThan(0).WithMessage("L'identifiant du matériel est invalide.")
            .When(x => x.IdMateriel > 0);

        RuleFor(x => x.CodeProduitSerial)
            .NotEmpty().WithMessage("Le code produit/série est obligatoire.")
            .MaximumLength(50).WithMessage("Le code produit/série ne peut pas dépasser 50 caractères.");

        RuleFor(x => x.Designation)
            .NotEmpty().WithMessage("La désignation est obligatoire.")
            .MaximumLength(200).WithMessage("La désignation ne peut pas dépasser 200 caractères.");

        RuleFor(x => x.Quantite)
            .GreaterThan(0).WithMessage("La quantité doit être supérieure à 0.");

        RuleFor(x => x.Provenance)
            .MaximumLength(200).WithMessage("La provenance ne peut pas dépasser 200 caractères.")
            .When(x => !string.IsNullOrEmpty(x.Provenance));

        RuleFor(x => x.Destination)
            .MaximumLength(200).WithMessage("La destination ne peut pas dépasser 200 caractères.")
            .When(x => !string.IsNullOrEmpty(x.Destination));
    }
}

/// <summary>
/// Validateur pour les approbations.
/// </summary>
public class ApprovalRequestValidator : AbstractValidator<ApprovalRequest>
{
    public ApprovalRequestValidator()
    {
        RuleFor(x => x.IdBon)
            .GreaterThan(0).WithMessage("L'identifiant du bon est invalide.");

        RuleFor(x => x.Commentaire)
            .MaximumLength(500).WithMessage("Le commentaire ne peut pas dépasser 500 caractères.")
            .When(x => !string.IsNullOrEmpty(x.Commentaire));
    }
}

/// <summary>
/// Validateur pour les rejets.
/// </summary>
public class RejectRequestValidator : AbstractValidator<RejectRequest>
{
    public RejectRequestValidator()
    {
        RuleFor(x => x.IdBon)
            .GreaterThan(0).WithMessage("L'identifiant du bon est invalide.");

        RuleFor(x => x.Motif)
            .NotEmpty().WithMessage("Le motif de rejet est obligatoire.")
            .MaximumLength(500).WithMessage("Le motif de rejet ne peut pas dépasser 500 caractères.");
    }
}

/// <summary>
/// Validateur pour les retours de prêt.
/// </summary>
public class ReturnLoanRequestValidator : AbstractValidator<ReturnLoanRequest>
{
    public ReturnLoanRequestValidator()
    {
        RuleFor(x => x.IdBon)
            .GreaterThan(0).WithMessage("L'identifiant du bon est invalide.");

        RuleFor(x => x.EtatRetour)
            .MaximumLength(100).WithMessage("L'état de retour ne peut pas dépasser 100 caractères.")
            .When(x => !string.IsNullOrEmpty(x.EtatRetour));

        RuleFor(x => x.ReceptionnePar)
            .MaximumLength(100).WithMessage("Le nom du réceptionnaire ne peut pas dépasser 100 caractères.")
            .When(x => !string.IsNullOrEmpty(x.ReceptionnePar));
    }
}
