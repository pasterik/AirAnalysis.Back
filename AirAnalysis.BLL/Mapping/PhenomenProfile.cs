using AirAnalysis.BLL.DTOs.Phenomen;
using AirAnalysis.BLL.DTOs.RecordData;
using AirAnalysis.DAL.Entities;
using AutoMapper;

namespace AirAnalysis.DAL.Mapping
{
    public class PhenomenProfile : Profile
    {
        public PhenomenProfile()
        {
            CreateMap<Phenomen, PhenomenDto>().ReverseMap();
            CreateMap<Phenomen, PhenomenByDateDto>()
                .ForMember(
                    dest => dest.Name,
                    opt => opt.MapFrom(src => src.Name))
                .ForMember(
                    dest => dest.Records,
                    opt => opt.MapFrom(src => src.RecordDatas ?? new List<RecordData>()));
        }
    }
}
