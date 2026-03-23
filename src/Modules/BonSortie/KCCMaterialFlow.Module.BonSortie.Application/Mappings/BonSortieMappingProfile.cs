using AutoMapper;
using KCCMaterialFlow.Module.BonSortie.DTOs;
using KCCMaterialFlow.Module.BonSortie.Entities;
using KCCMaterialFlow.Module.BonSortie.Services;

namespace KCCMaterialFlow.Module.BonSortie.Mappings;

/// <summary>
/// BSM-040: Profil AutoMapper pour le module BonSortie.
/// Définit les mappings entre les entités et les DTOs.
/// </summary>
public class BonSortieMappingProfile : Profile
{
    public BonSortieMappingProfile()
    {
        // ===== MAPPINGS BONSORTIEEXTERNE =====
        
        CreateMap<BonSortieExterne, BonSortieViewDto>()
            .ForMember(dest => dest.TypeSortie, opt => opt.MapFrom(src => "Externe"))
            .ForMember(dest => dest.BonEntreeAssocieNumero, opt => opt.MapFrom(src => 
                src.BonEntreeAssocie != null ? src.BonEntreeAssocie.NumeroReference : null))
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
            .ForMember(dest => dest.EstRetourne, opt => opt.MapFrom(src => false));

        CreateMap<BonSortieExterne, BonSortieListDto>()
            .ForMember(dest => dest.TypeSortie, opt => opt.MapFrom(src => "Externe"))
            .ForMember(dest => dest.NombreMateriels, opt => opt.MapFrom(src => src.Materiels.Count))
            .ForMember(dest => dest.BonEntreeAssocieNumero, opt => opt.MapFrom(src => 
                src.BonEntreeAssocie != null ? src.BonEntreeAssocie.NumeroReference : null))
            .ForMember(dest => dest.DateRetourPrevue, opt => opt.Ignore())
            .ForMember(dest => dest.EstRetourne, opt => opt.Ignore());

        // ===== MAPPINGS BONSORTIEINTERNE =====
        
        CreateMap<BonSortieInterne, BonSortieViewDto>()
            .ForMember(dest => dest.TypeSortie, opt => opt.MapFrom(src => "Interne"))
            .ForMember(dest => dest.BonEntreeAssocieId, opt => opt.Ignore())
            .ForMember(dest => dest.BonEntreeAssocieNumero, opt => opt.Ignore())
            .ForMember(dest => dest.NomDestinataire, opt => opt.Ignore())
            .ForMember(dest => dest.AdresseDestination, opt => opt.Ignore())
            .ForMember(dest => dest.NumeroVehicule, opt => opt.Ignore())
            .ForMember(dest => dest.NomChauffeur, opt => opt.Ignore())
            .ForMember(dest => dest.TelephoneChauffeur, opt => opt.Ignore())
            .ForMember(dest => dest.DateRetourPrevue, opt => opt.Ignore())
            .ForMember(dest => dest.DateRetourEffective, opt => opt.Ignore())
            .ForMember(dest => dest.EtatRetour, opt => opt.Ignore())
            .ForMember(dest => dest.EstRetourne, opt => opt.MapFrom(src => false));

        CreateMap<BonSortieInterne, BonSortieListDto>()
            .ForMember(dest => dest.TypeSortie, opt => opt.MapFrom(src => "Interne"))
            .ForMember(dest => dest.NombreMateriels, opt => opt.MapFrom(src => src.Materiels.Count))
            .ForMember(dest => dest.NomDestinataire, opt => opt.MapFrom(src => "Interne"))
            .ForMember(dest => dest.BonEntreeAssocieNumero, opt => opt.Ignore())
            .ForMember(dest => dest.DateRetourPrevue, opt => opt.Ignore())
            .ForMember(dest => dest.EstRetourne, opt => opt.Ignore());

        // ===== MAPPINGS PRET =====
        
        CreateMap<Pret, BonSortieViewDto>()
            .ForMember(dest => dest.TypeSortie, opt => opt.MapFrom(src => "Pret"))
            .ForMember(dest => dest.BonEntreeAssocieNumero, opt => opt.MapFrom(src => 
                src.BonEntreeAssocie != null ? src.BonEntreeAssocie.NumeroReference : null))
            .ForMember(dest => dest.DepartementOrigine, opt => opt.Ignore())
            .ForMember(dest => dest.DepartementDestination, opt => opt.Ignore())
            .ForMember(dest => dest.NomReceveur, opt => opt.Ignore())
            .ForMember(dest => dest.FonctionReceveur, opt => opt.Ignore())
            .ForMember(dest => dest.EmailReceveur, opt => opt.Ignore())
            .ForMember(dest => dest.LocalisationDestination, opt => opt.Ignore())
            .ForMember(dest => dest.DateTransfertPrevue, opt => opt.Ignore());

        CreateMap<Pret, BonSortieListDto>()
            .ForMember(dest => dest.TypeSortie, opt => opt.MapFrom(src => "Pret"))
            .ForMember(dest => dest.NombreMateriels, opt => opt.MapFrom(src => src.Materiels.Count))
            .ForMember(dest => dest.NomDestinataire, opt => opt.MapFrom(src => src.NomDestinataire ?? "Prêt"))
            .ForMember(dest => dest.BonEntreeAssocieNumero, opt => opt.MapFrom(src => 
                src.BonEntreeAssocie != null ? src.BonEntreeAssocie.NumeroReference : null));

        CreateMap<Pret, PretViewDto>()
            .ForMember(dest => dest.BonEntreeAssocieNumero, opt => opt.MapFrom(src => 
                src.BonEntreeAssocie != null ? src.BonEntreeAssocie.NumeroReference : null))
            .ForMember(dest => dest.NombreMateriels, opt => opt.MapFrom(src => src.Materiels.Count))
            .ForMember(dest => dest.QuantiteTotale, opt => opt.MapFrom(src => 
                src.Materiels.Sum(m => m.Quantite)));

        CreateMap<Pret, PretListDto>()
            .ForMember(dest => dest.NombreMateriels, opt => opt.MapFrom(src => src.Materiels.Count));

        // ===== MAPPINGS MATERIEL =====
        
        CreateMap<MaterielSortie, MaterielSortieViewDto>();
        
        CreateMap<MaterielDto, MaterielSortie>()
            .ForMember(dest => dest.BonSortieId, opt => opt.Ignore())
            .ForMember(dest => dest.BonSortie, opt => opt.Ignore())
            .ForMember(dest => dest.IdMateriel, opt => opt.Condition(src => src.IdMateriel > 0));

        CreateMap<MaterielSortieUpdateDto, MaterielSortie>()
            .ForMember(dest => dest.BonSortieId, opt => opt.Ignore())
            .ForMember(dest => dest.BonSortie, opt => opt.Ignore())
            .ForMember(dest => dest.IdMateriel, opt => opt.Condition(src => src.IdMateriel.HasValue && src.IdMateriel > 0));

        // ===== MAPPINGS APPROBATION =====
        
        CreateMap<ApprobationSortie, ApprobationSortieViewDto>();

        // ===== MAPPINGS ITINERAIRE =====
        
        CreateMap<ItineraireSortie, ItineraireSortieViewDto>()
            .ForMember(dest => dest.NomBarriere, opt => opt.MapFrom(src => $"Barrière {src.BarriereId}"))
            .ForMember(dest => dest.LocalisationBarriere, opt => opt.Ignore())
            .ForMember(dest => dest.DatePassage, opt => opt.MapFrom(src => src.DatePassageEffective))
            .ForMember(dest => dest.EstPasse, opt => opt.MapFrom(src => src.DatePassageEffective.HasValue))
            .ForMember(dest => dest.NomAgent, opt => opt.Ignore());

        // ===== MAPPINGS REQUEST -> ENTITÉ =====
        
        CreateMap<CreateBonSortieExterneRequest, BonSortieExterne>()
            .ForMember(dest => dest.IdBon, opt => opt.Ignore())
            .ForMember(dest => dest.NumeroReference, opt => opt.Ignore())
            .ForMember(dest => dest.DateCreation, opt => opt.MapFrom(_ => DateTime.Now))
            .ForMember(dest => dest.StatutActuel, opt => opt.MapFrom(_ => "Draft"))
            .ForMember(dest => dest.EstDefinitif, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.Quantite, opt => opt.MapFrom(src => src.Materiels.Count))
            .ForMember(dest => dest.Materiels, opt => opt.Ignore())
            .ForMember(dest => dest.Approbations, opt => opt.Ignore())
            .ForMember(dest => dest.Itineraires, opt => opt.Ignore())
            .ForMember(dest => dest.Historiques, opt => opt.Ignore())
            .ForMember(dest => dest.QRCodeData, opt => opt.Ignore())
            .ForMember(dest => dest.QRCodeBase64, opt => opt.Ignore())
            .ForMember(dest => dest.QRCodeHash, opt => opt.Ignore())
            .ForMember(dest => dest.DateGenerationQR, opt => opt.Ignore())
            .ForMember(dest => dest.BonEntreeAssocie, opt => opt.Ignore());

        CreateMap<CreateBonSortieInterneRequest, BonSortieInterne>()
            .ForMember(dest => dest.IdBon, opt => opt.Ignore())
            .ForMember(dest => dest.NumeroReference, opt => opt.Ignore())
            .ForMember(dest => dest.DateCreation, opt => opt.MapFrom(_ => DateTime.Now))
            .ForMember(dest => dest.StatutActuel, opt => opt.MapFrom(_ => "Draft"))
            .ForMember(dest => dest.EstDefinitif, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.Quantite, opt => opt.MapFrom(src => src.Materiels.Count))
            .ForMember(dest => dest.Materiels, opt => opt.Ignore())
            .ForMember(dest => dest.Approbations, opt => opt.Ignore())
            .ForMember(dest => dest.Itineraires, opt => opt.Ignore())
            .ForMember(dest => dest.Historiques, opt => opt.Ignore())
            .ForMember(dest => dest.QRCodeData, opt => opt.Ignore())
            .ForMember(dest => dest.QRCodeBase64, opt => opt.Ignore())
            .ForMember(dest => dest.QRCodeHash, opt => opt.Ignore())
            .ForMember(dest => dest.DateGenerationQR, opt => opt.Ignore())
            .ForMember(dest => dest.DateTransfertEffective, opt => opt.Ignore());

        CreateMap<CreatePretRequest, Pret>()
            .ForMember(dest => dest.IdBon, opt => opt.Ignore())
            .ForMember(dest => dest.NumeroReference, opt => opt.Ignore())
            .ForMember(dest => dest.DateCreation, opt => opt.MapFrom(_ => DateTime.Now))
            .ForMember(dest => dest.StatutActuel, opt => opt.MapFrom(_ => "Draft"))
            .ForMember(dest => dest.EstDefinitif, opt => opt.MapFrom(_ => false))
            .ForMember(dest => dest.Quantite, opt => opt.MapFrom(src => src.Materiels.Count))
            .ForMember(dest => dest.EstRetourne, opt => opt.MapFrom(_ => false))
            .ForMember(dest => dest.DateRetourEffective, opt => opt.Ignore())
            .ForMember(dest => dest.EtatRetour, opt => opt.Ignore())
            .ForMember(dest => dest.ReceptionnePar, opt => opt.Ignore())
            .ForMember(dest => dest.Materiels, opt => opt.Ignore())
            .ForMember(dest => dest.Approbations, opt => opt.Ignore())
            .ForMember(dest => dest.Itineraires, opt => opt.Ignore())
            .ForMember(dest => dest.Historiques, opt => opt.Ignore())
            .ForMember(dest => dest.QRCodeData, opt => opt.Ignore())
            .ForMember(dest => dest.QRCodeBase64, opt => opt.Ignore())
            .ForMember(dest => dest.QRCodeHash, opt => opt.Ignore())
            .ForMember(dest => dest.DateGenerationQR, opt => opt.Ignore())
            .ForMember(dest => dest.BonEntreeAssocie, opt => opt.Ignore());
    }
}
