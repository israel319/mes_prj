using AutoMapper;
using KCCMaterialFlow.Module.BonEntree.DTOs;
using KCCMaterialFlow.Module.BonEntree.Entities;
using KCCMaterialFlow.Module.BonEntree.Repositories;
using KCCMaterialFlow.Module.BonEntree.Services;

namespace KCCMaterialFlow.Module.BonEntree.Mappings;

/// <summary>
/// Profil AutoMapper pour le module BonEntree - Version simplifiée
/// Entités selon diagramme UML: 8 champs BonEntree + 5 champs Materiel
/// </summary>
public class BonEntreeMappingProfile : Profile
{
    public BonEntreeMappingProfile()
    {
        #region Entity to DTO

        // BonEntree -> BonEntreeViewDto (mapping direct, propriétés identiques)
        CreateMap<Entities.BonEntree, BonEntreeViewDto>();

        // BonEntree -> BonEntreeListDto (mapping direct)
        CreateMap<Entities.BonEntree, BonEntreeListDto>();

        // Materiel -> MaterielDto (5 champs: CodeProduitSerial, Designation, Quantite, Provenance, Destination)
        CreateMap<Materiel, MaterielDto>();

        // Approbation -> ApprobationDto (4 champs: OrdreEtape, Decision, DateAction, ReservesEventuelles)
        CreateMap<Approbation, ApprobationDto>();

        // ItinerairePrevu -> ItinerairePrevuDto (2 champs: OrdrePassage, BarriereId)
        CreateMap<ItinerairePrevu, ItinerairePrevuDto>();

        #endregion

        #region DTO to Entity

        // MaterielDto -> Materiel (pour création)
        CreateMap<MaterielDto, Materiel>()
            .ForMember(d => d.IdMateriel, opt => opt.Ignore())
            .ForMember(d => d.BonId, opt => opt.Ignore())
            .ForMember(d => d.Bon, opt => opt.Ignore());

        // MaterielRequest -> Materiel (pour création via service)
        CreateMap<MaterielRequest, Materiel>()
            .ForMember(d => d.IdMateriel, opt => opt.Ignore())
            .ForMember(d => d.BonId, opt => opt.Ignore())
            .ForMember(d => d.Bon, opt => opt.Ignore());

        #endregion

        #region Search Results

        // BonEntreeSearchResult -> BonEntreeListResultDto
        CreateMap<BonEntreeSearchResult, BonEntreeListResultDto>();

        // BonEntreeFilter mapping
        CreateMap<BonEntreeSearchCriteriaDto, BonEntreeFilter>()
            .ForMember(d => d.Skip, opt => opt.MapFrom(s => s.Skip))
            .ForMember(d => d.Take, opt => opt.MapFrom(s => s.PageSize));

        #endregion
    }
}
