using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Application.Features.BonEntree.Commands.SubmitBonEntree;

public sealed record SubmitBonEntreeCommand(int BonEntreeId) : IRequest<Result>;

public sealed class SubmitBonEntreeCommandHandler(
    IApplicationDbContext dbContext,
    IChaineApprobationService chaineService)
    : IRequestHandler<SubmitBonEntreeCommand, Result>
{
    public async Task<Result> Handle(SubmitBonEntreeCommand cmd, CancellationToken ct)
    {
        var bon = await dbContext.BonsEntree
            .Include(b => b.Materiels)
            .FirstOrDefaultAsync(b => b.Id == cmd.BonEntreeId, ct);

        if (bon is null)
            return Result.Failure(Error.NotFound("BonEntree", cmd.BonEntreeId));

        var result = bon.SoumettrePourApprobation();
        if (result.IsFailure)
            return result;

        // Récupérer l'employé créateur du bon
        // bon.CreatedBy contient le login Windows (DOMAIN\MATRICULE) → extraire le matricule
        var creatorLogin = bon.CreatedBy ?? "";
        var creatorMatricule = creatorLogin.Contains('\\')
            ? creatorLogin[(creatorLogin.LastIndexOf('\\') + 1)..].ToUpperInvariant()
            : creatorLogin.ToUpperInvariant();
        var creatorEmployee = await dbContext.Employees
            .FirstOrDefaultAsync(e => e.Matricule == creatorMatricule, ct);
        
        if (creatorEmployee is null)
            return Result.Failure(Error.Validation("BonEntree.CreatorNotFound", 
                "L'employé créateur du bon n'a pas pu être identifié."));

        // Construire la chaîne d'approbation v2 (Glencore + Site)
        // BonEntree n'est pas typé par TypeMateriel (un bon peut contenir plusieurs types) : null = pas de routage IT/Env.
        try
        {
            var chainResult = await chaineService.ConstruireChaineAsync(
                creatorEmployee.Id, descriptionMateriel: null, siteId: bon.SiteId, ct: ct);
            
            if (chainResult.Etapes.Count == 0)
                return Result.Failure(Error.Validation("BonEntree.NoApprovalChain", 
                    "Aucune étape d'approbation trouvée pour ce bon."));

            // Assigner le premier approbateur
            var firstEtape = chainResult.Etapes.First();
            bon.AssignerProchainApprobateur(firstEtape.EmployeeId, firstEtape.EmployeeNomComplet, 1);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(Error.Validation("BonEntree.ChainBuilderError", ex.Message));
        }

        await dbContext.SaveChangesAsync(ct);
        return Result.Success();
    }
}
