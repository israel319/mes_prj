using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Domain.Enums;
using KCCMaterialFlow.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Application.Features.BonSortie.Queries.GetBonSortieDetail;

// ── DTOs ────────────────────────────────────────────────────────────────
public sealed record MaterielSortieDto(
    int Id,
    string CodeProduitSerial,
    string Designation,
    decimal Quantite,
    string? Remarque);

public sealed record ApprobationSortieDto(
    int Id,
    int OrdreEtape,
    string? NomEtape,
    string Decision,
    DateTime? DateAction,
    string? NomApprobateur,
    string? RoleApprobateur,
    string? ReservesEventuelles);

public sealed record ItineraireSortieDto(
    int Id,
    int OrdrePassage,
    int BarriereId);

public sealed record HistoriqueSortieDto(
    int Id,
    ActionBonSortie Action,
    string ActionDescription,
    string ActionBy,
    string? ActionByNom,
    DateTime ActionDate,
    string? Comment,
    string? StatutAvant,
    string? StatutApres);

public sealed record ExterneInfoDto(
    string NomDestinataire,
    string? DescriptionMateriel,
    int? BonEntreeAssocieId,
    string? AdresseDestination,
    string? NumeroVehicule,
    string? NomChauffeur,
    string? TelephoneChauffeur);

public sealed record InterneInfoDto(
    string? DescriptionMateriel,
    int? BonEntreeAssocieId,
    string? DepartementOrigine,
    string? FonctionReceveur,
    string? EmailReceveur,
    string? LocalisationDestination,
    DateTime? DateTransfertPrevue,
    DateTime? DateTransfertEffective);

public sealed record PretInfoDto(
    DateTime DateRetourPrevue,
    DateTime? DateRetourEffective,
    bool EstRetourne,
    string? EtatRetour,
    string? ReceptionnePar,
    int JoursRetard,
    bool EstEnRetard);

public sealed record BonSortieDetailDto(
    int IdBon,
    string NumeroReference,
    DateTime DateCreation,
    DateTime DateExpiration,
    string StatutActuel,
    StatutBonSortie Statut,
    string Destination,
    string Provenance,
    string? Description,
    int Quantite,
    string NomDemandeur,
    string FonctionDemandeur,
    string DepartementDemandeur,
    string CreatedByLogin,
    string MotifSortie,
    string? RaisonSortieCode,
    bool EstDefinitif,
    string? QRCodeBase64,
    string? QRCodeHash,
    DateTime? DateGenerationQR,
    IReadOnlyList<MaterielSortieDto> Materiels,
    IReadOnlyList<ApprobationSortieDto> Approbations,
    IReadOnlyList<ItineraireSortieDto> Itineraires,
    IReadOnlyList<HistoriqueSortieDto> Historiques,
    ExterneInfoDto? ExterneInfo,
    InterneInfoDto? InterneInfo,
    PretInfoDto? PretInfo);

// ── Query ───────────────────────────────────────────────────────────────
public sealed record GetBonSortieDetailQuery(int BonSortieId) : IRequest<Result<BonSortieDetailDto>>;

// ── Handler ─────────────────────────────────────────────────────────────
internal sealed class GetBonSortieDetailQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetBonSortieDetailQuery, Result<BonSortieDetailDto>>
{
    public async Task<Result<BonSortieDetailDto>> Handle(GetBonSortieDetailQuery request, CancellationToken ct)
    {
        var bon = await dbContext.BonsSortie
            .AsNoTracking()
            .Include(b => b.Materiels)
            .Include(b => b.Approbations)
            .Include(b => b.Itineraires)
            .Include(b => b.Historiques)
            .FirstOrDefaultAsync(b => b.Id == request.BonSortieId, ct);

        if (bon is null)
            return Result.Failure<BonSortieDetailDto>(Error.NotFound("BonSortie", request.BonSortieId));

        var materiels = bon.Materiels.Select(m =>
            new MaterielSortieDto(m.Id, m.CodeProduitSerial, m.Designation, m.Quantite, m.Remarque)).ToList();

        var approbations = bon.Approbations.Select(a =>
            new ApprobationSortieDto(a.Id, a.OrdreEtape, a.NomEtape, a.Decision, a.DateAction,
                a.NomApprobateur, a.RoleApprobateur, a.ReservesEventuelles)).ToList();

        var itineraires = bon.Itineraires.Select(i =>
            new ItineraireSortieDto(i.Id, i.OrdrePassage, i.BarriereId)).ToList();

        var historiques = bon.Historiques.Select(h =>
            new HistoriqueSortieDto(h.Id, h.Action, h.ActionDescription, h.ActionBy,
                h.ActionByNom, h.ActionDate, h.Comment, h.StatutAvant, h.StatutApres)).ToList();

        ExterneInfoDto? externeInfo = bon is BonSortieExterne ext
            ? new ExterneInfoDto(ext.NomDestinataire, ext.DescriptionMateriel, ext.BonEntreeAssocieId,
                ext.AdresseDestination, ext.NumeroVehicule, ext.NomChauffeur, ext.TelephoneChauffeur)
            : null;

        InterneInfoDto? interneInfo = bon is BonSortieInterne intBon
            ? new InterneInfoDto(intBon.DescriptionMateriel, intBon.BonEntreeAssocieId,
                intBon.DepartementOrigine, intBon.FonctionReceveur, intBon.EmailReceveur,
                intBon.LocalisationDestination, intBon.DateTransfertPrevue, intBon.DateTransfertEffective)
            : null;

        PretInfoDto? pretInfo = bon is Pret pret
            ? new PretInfoDto(pret.DateRetourPrevue, pret.DateRetourEffective, pret.EstRetourne,
                pret.EtatRetour, pret.ReceptionnePar, pret.JoursRetard, pret.EstEnRetard)
            : null;

        var dto = new BonSortieDetailDto(
            bon.Id, bon.NumeroReference, bon.DateCreation, bon.DateExpiration,
            bon.StatutActuel, bon.Statut, bon.Destination, bon.Provenance,
            bon.Description, bon.Quantite, bon.NomDemandeur, bon.FonctionDemandeur,
            bon.DepartementDemandeur, bon.CreatedByLogin, bon.MotifSortie,
            bon.RaisonSortieCode, bon.EstDefinitif, bon.QRCodeBase64, bon.QRCodeHash,
            bon.DateGenerationQR,
            materiels, approbations, itineraires, historiques,
            externeInfo, interneInfo, pretInfo);

        return dto;
    }
}
