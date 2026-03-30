using AutoMapper;
using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Application.Features.BonEntree.DTOs;
using KCCMaterialFlow.Domain.Entities;

namespace KCCMaterialFlow.Application.Features.BonEntree.Mappings;

public class BonEntreeMappingProfile : Profile
{
    public BonEntreeMappingProfile()
    {
        #region Entity to DTO

        CreateMap<Domain.Entities.BonEntree, BonEntreeViewDto>()
            .ForMember(d => d.IdBon, opt => opt.MapFrom(s => s.Id));

        CreateMap<Domain.Entities.BonEntree, BonEntreeListDto>()
            .ForMember(d => d.IdBon, opt => opt.MapFrom(s => s.Id))
            .ForMember(d => d.NombreMateriels, opt => opt.MapFrom(s => s.Materiels.Count));

        CreateMap<Materiel, BonEntreeMaterielDto>()
            .ForMember(d => d.IdMateriel, opt => opt.MapFrom(s => s.Id));

        CreateMap<Approbation, ApprobationDto>()
            .ForMember(d => d.IdApprobation, opt => opt.MapFrom(s => s.Id));

        CreateMap<ItinerairePrevu, ItinerairePrevuDto>()
            .ForMember(d => d.IdItinerairePrevu, opt => opt.MapFrom(s => s.Id));

        #endregion

        #region DTO to Entity

        CreateMap<BonEntreeMaterielDto, Materiel>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.BonId, opt => opt.Ignore());

        CreateMap<MaterielRequest, Materiel>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.BonId, opt => opt.Ignore());

        #endregion

        #region Search Results

        CreateMap<BonEntreeSearchResult, BonEntreeListResultDto>();

        CreateMap<BonEntreeSearchCriteriaDto, BonEntreeFilter>()
            .ForMember(d => d.Skip, opt => opt.MapFrom(s => s.Skip))
            .ForMember(d => d.Take, opt => opt.MapFrom(s => s.PageSize));

        #endregion
    }
}
