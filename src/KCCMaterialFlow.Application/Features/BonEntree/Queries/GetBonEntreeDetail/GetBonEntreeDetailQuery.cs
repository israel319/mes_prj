using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Enums;
using KCCMaterialFlow.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Application.Features.BonEntree.Queries.GetBonEntreeDetail;

public sealed record MaterielDto(
    int Id,
    string CodeProduitSerial,
    string Designation,
    decimal Quantite,
    decimal QuantiteDisponible);

public sealed record ApprobationDto(
    int Id,
    int OrdreEtape,
    string NomEtape,
    string? ApprobateurNom,
    string? Decision,
    string? Commentaire,
    DateTime? DateDecision);

public sealed record ItinerairePrevuDto(
    int Id,
    int OrdrePassage,
    int BarriereId);

public sealed record HistoriqueDto(
    int Id,
    string Action,
    string? Commentaire,
    string? EffectuePar,
    DateTime DateAction);

public sealed record BonEntreeDetailDto
{
    public int IdBon { get; init; }
    public string NumeroReference { get; init; } = string.Empty;
    public StatutBonEntree Statut { get; init; }
    public string StatutLibelle { get; init; } = string.Empty;
    public DateTime DateCreation { get; init; }
    public DateTime DateExpiration { get; init; }
    public string Provenance { get; init; } = string.Empty;
    public string Destination { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int Quantite { get; init; }
    public string NomDemandeur { get; init; } = string.Empty;
    public string NomCompagnie { get; init; } = string.Empty;
    public string SiteManager { get; init; } = string.Empty;
    public string HostDepartment { get; init; } = string.Empty;
    public string ReasonOnSite { get; init; } = string.Empty;
    public string NomEscorteur { get; init; } = string.Empty;
    public string? FonctionEscorteur { get; init; }
    public string? EmailContractant { get; init; }
    public string? NumeroContrat { get; init; }
    public bool EstModifiable { get; init; }
    public bool EstEnAttenteApprobation { get; init; }
    public bool EstVerrouillePourSortie { get; init; }
    public string? QRCodeBase64 { get; init; }
    public int? BonSortieAssocieId { get; init; }
    public string? BonSortieAssocieNumero { get; init; }
    public List<MaterielDto> Materiels { get; init; } = [];
    public List<ApprobationDto> Approbations { get; init; } = [];
    public List<ItinerairePrevuDto> ItinerairesPrevu { get; init; } = [];
    public List<HistoriqueDto> Historiques { get; init; } = [];
}

public sealed record GetBonEntreeDetailQuery(int BonEntreeId) : IRequest<Result<BonEntreeDetailDto>>;

public sealed class GetBonEntreeDetailQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetBonEntreeDetailQuery, Result<BonEntreeDetailDto>>
{
    public async Task<Result<BonEntreeDetailDto>> Handle(GetBonEntreeDetailQuery query, CancellationToken ct)
    {
        var bon = await dbContext.BonsEntree
            .AsNoTracking()
            .Include(b => b.Materiels)
            .Include(b => b.Approbations)
            .Include(b => b.ItinerairesPrevu)
            .Include(b => b.Historiques)
            .FirstOrDefaultAsync(b => b.Id == query.BonEntreeId, ct);

        if (bon is null)
            return Result.Failure<BonEntreeDetailDto>(Error.NotFound("BonEntree", query.BonEntreeId));

        var dto = new BonEntreeDetailDto
        {
            IdBon = bon.Id,
            NumeroReference = bon.NumeroReference,
            Statut = bon.Statut,
            StatutLibelle = bon.Statut.ToString(),
            DateCreation = bon.DateCreation,
            DateExpiration = bon.DateExpiration,
            Provenance = bon.Provenance,
            Destination = bon.Destination,
            Description = bon.Description,
            Quantite = bon.Quantite,
            NomDemandeur = bon.NomDemandeur,
            NomCompagnie = bon.NomCompagnie,
            SiteManager = bon.SiteManager,
            HostDepartment = bon.HostDepartment,
            ReasonOnSite = bon.ReasonOnSite,
            NomEscorteur = bon.NomEscorteur,
            FonctionEscorteur = bon.FonctionEscorteur,
            EmailContractant = bon.EmailContractant,
            NumeroContrat = bon.NumeroContrat,
            EstModifiable = bon.EstModifiable(),
            EstEnAttenteApprobation = bon.EstEnAttenteApprobation(),
            EstVerrouillePourSortie = bon.EstVerrouillePourSortie,
            QRCodeBase64 = bon.QRCodeBase64,
            BonSortieAssocieId = bon.BonSortieAssocieId,
            BonSortieAssocieNumero = bon.BonSortieAssocieNumero,
            Materiels = bon.Materiels.Select(m => new MaterielDto(
                m.Id, m.CodeProduitSerial, m.Designation, m.Quantite, m.QuantiteDisponible)).ToList(),
            Approbations = bon.Approbations.Select(a => new ApprobationDto(
                a.Id, a.OrdreEtape, a.NomEtape ?? string.Empty, a.NomApprobateur,
                a.Decision, a.ReservesEventuelles, a.DateAction)).ToList(),
            ItinerairesPrevu = bon.ItinerairesPrevu.Select(i => new ItinerairePrevuDto(
                i.Id, i.OrdrePassage, i.BarriereId)).ToList(),
            Historiques = bon.Historiques.Select(h => new HistoriqueDto(
                h.Id, h.Action.ToString(), h.Comment, h.ActionByNom, h.ActionDate)).ToList()
        };

        return dto;
    }
}
