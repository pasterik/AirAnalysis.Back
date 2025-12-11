using AirAnalysis.BLL.DTOs.Phenomen;
using AirAnalysis.BLL.DTOs.Place;
using AirAnalysis.DAL.Entities;
using AutoMapper;

namespace AirAnalysis.DAL.Mapping
{
    public class PlaceProfile : Profile
    {
        public PlaceProfile()
        {
            CreateMap<Place, PlaceDto>().ReverseMap();
        }
    }
}
