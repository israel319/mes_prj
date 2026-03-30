using AutoMapper;
using KCCMaterialFlow.Application.Features.BonSortie.DTOs;
using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Application.Features.BonSortie.Mappings;

public class BonSortieMappingProfile : Profile
{
    public BonSortieMappingProfile()
    {
        // ===== MAPPINGS BONSORTIEEXTERNE =====

        CreateMap<BonSortieExterne, BonSortieViewDto>()
            .ForMember(dest => dest.IdBon, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.TypeSortie, opt => opt.MapFrom(_ => "Externe"))
            .ForMember(dest => dest.BonEntreeAssocieNumero, opt => opt.Ignore())
            .ForMember(dest => dest.DepartementOrigine, opt => opt.Ignore())
            .ForMember(dest => dest.DepartementDestination, opt => opt.Ignore())
            .ForMember(dest => dest.NomReceveur, opt => opt.Ignore())
            .ForMember(dest => dest.FonctionReceveur, opt => opt.Ignore())
            .ForMember(dest => dest.EmailReceveur, opt => opt.Ignore())
            .ForMember(dest => dest.LocalisationDestination, opt => opt.Ignore())
            .ForMember(dest => dest.DateTransfertPrevue, opt => opt.Ignore())
            .ForMember(dest => dest.DateRetourPrevue, opt => opt.Ignore())
            .ForMember(dest => dest.DateRetourEffective, opt => opt.Ignore())
            .ForMember(dest => dest.EtatRetour, opt => opt.Ignore())
            .ForMember(dest => dest.EstRetourne, opt => opt.MapFrom(_ => false));

        CreateMap<BonSortieExterne, BonSortieListDto>()
            .ForMember(dest => dest.IdBon, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.TypeSortie, opt => opt.MapFrom(_ => "Externe"))
            .ForMember(dest => dest.NombreMateriels, opt => opt.MapFrom(src => src.Materiels.Count))
            .ForMember(dest => dest.BonEntreeAssocieNumero, opt => opt.Ignore())
            .ForMember(dest => dest.DateRetourPrevue, opt => opt.Ignore())
            .ForMember(dest => dest.EstRetourne, opt => opt.Ignore());

        // ===== MAPPINGS BONSORTIEINTERNE =====

        CreateMap<BonSortieInterne, BonSortieViewDto>()
            .ForMember(dest => dest.IdBon, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.TypeSortie, opt => opt.MapFrom(_ => "Interne"))
            .ForMember(dest => dest.TypeMateriel, opt => opt.MapFrom(src => src.TypeMateriel))
            .ForMember(dest => dest.BonEntreeAssocieId, opt => opt.MapFrom(src => src.BonEntreeAssocieId))
            .ForMember(dest => dest.BonEntreeAssocieNumero, opt => opt.Ignore())
            .ForMember(dest => dest.NomDestinataire, opt => opt.Ignore())
            .ForMember(dest => dest.AdresseDestination, opt => opt.Ignore())
            .ForMember(dest => dest.NumeroVehicule, opt => opt.Ignore())
            .ForMember(dest => dest.NomChauffeur, opt => opt.Ignore())
            .ForMember(dest => dest.TelephoneChauffeur, opt => opt.Ignore())
            .ForMember(dest => dest.DateRetourPrevue, opt => opt.Ignore())
            .ForMember(dest => dest.DateRetourEffective, opt => opt.Ignore())
            .ForMember(dest => dest.EtatRetour, opt => opt.Ignore())
            .ForMember(dest => dest.EstRetourne, opt => opt.MapFrom(_ => false));

        CreateMap<BonSortieInterne, BonSortieListDto>()
            .ForMember(dest => dest.IdBon, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.TypeSortie, opt => opt.MapFrom(_ => "Interne"))
            .ForMember(dest => dest.NombreMateriels, opt => opt.MapFrom(src => src.Materiels.Count))
            .ForMember(dest => dest.NomDestinataire, opt => opt.MapFrom(_ => "Interne"))
            .ForMember(dest => dest.BonEntreeAssocieNumero, opt => opt.Ignore())
            .ForMember(dest => dest.DateRetourPrevue, opt => opt.Ignore())
            .ForMember(dest => dest.EstRetourne, opt => opt.Ignore());

        // ===== MAPPINGS PRET =====

        CreateMap<Pret, BonSortieViewDto>()
            .ForMember(dest => dest.IdBon, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.TypeSortie, opt => opt.MapFrom(_ => "Pret"))
            .ForMember(dest => dest.BonEntreeAssocieNumero, opt => opt.Ignore())
            .ForMember(dest => dest.DepartementOrigine, opt => opt.Ignore())
            .ForMember(dest => dest.DepartementDestination, opt => opt.Ignore())
            .ForMember(dest => dest.NomReceveur, opt => opt.Ignore())
            .ForMember(dest => dest.FonctionReceveur, opt => opt.Ignore())
            .ForMember(dest => dest.EmailReceveur, opt => opt.Ignore())
            .ForMember(dest => dest.LocalisationDestination, opt => opt.Ignore())
            .ForMember(dest => dest.DateTransfertPrevue, opt => opt.Ignore());

        CreateMap<Pret, BonSortieListDto>()
            .ForMember(dest => dest.IdBon, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.TypeSortie, opt => opt.MapFrom(_ => "Pret"))
            .ForMember(dest => dest.NombreMateriels, opt => opt.MapFrom(src => src.Materiels.Count))
            .ForMember(dest => dest.NomDestinataire, opt => opt.MapFrom(src => src.NomDestinataire ?? "Prêt"))
            .ForMember(dest => dest.BonEntreeAssocieNumero, opt => opt.Ignore());

        CreateMap<Pret, PretViewDto>()
            .ForMember(dest => dest.IdBon, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.BonEntreeAssocieNumero, opt => opt.Ignore())
            .ForMember(dest => dest.NombreMateriels, opt => opt.MapFrom(src => src.Materiels.Count))
            .ForMember(dest => dest.QuantiteTotale, opt => opt.MapFrom(src =>
                src.Materiels.Sum(m => m.Quantite)));

        CreateMap<Pret, PretListDto>()
            .ForMember(dest => dest.IdBon, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.NombreMateriels, opt => opt.MapFrom(src => src.Materiels.Count));

        // ===== MAPPINGS MATERIEL =====

        CreateMap<MaterielSortie, MaterielSortieViewDto>()
            .ForMember(dest => dest.IdMateriel, opt => opt.MapFrom(src => src.Id));

        // ===== MAPPINGS APPROBATION =====

        CreateMap<ApprobationSortie, ApprobationSortieViewDto>()
            .ForMember(dest => dest.IdApprobation, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ApprobateurNom, opt => opt.MapFrom(src => src.NomApprobateur))
            .ForMember(dest => dest.ApprobateurFonction, opt => opt.MapFrom(src => src.RoleApprobateur))
            .ForMember(dest => dest.DateApprobation, opt => opt.MapFrom(src => src.DateAction))
            .ForMember(dest => dest.Statut, opt => opt.MapFrom(src => src.Decision))
            .ForMember(dest => dest.Commentaire, opt => opt.MapFrom(src => src.ReservesEventuelles));

        // ===== MAPPINGS ITINERAIRE =====

        CreateMap<ItineraireSortie, ItineraireSortieViewDto>()
            .ForMember(dest => dest.IdItineraire, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.NomBarriere, opt => opt.MapFrom(src => $"Barrière {src.BarriereId}"))
            .ForMember(dest => dest.LocalisationBarriere, opt => opt.Ignore())
            .ForMember(dest => dest.DatePassage, opt => opt.MapFrom(src => src.DatePassageEffective))
            .ForMember(dest => dest.EstPasse, opt => opt.MapFrom(src => src.DatePassageEffective.HasValue))
            .ForMember(dest => dest.NomAgent, opt => opt.Ignore());
    }
}
