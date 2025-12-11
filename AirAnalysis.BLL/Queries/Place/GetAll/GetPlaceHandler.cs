using AirAnalysis.BLL.DTOs.Phenomen;
using AirAnalysis.DAL.Entities;
using AirAnalysis.DAL.Repositories.Interfaces;
using AutoMapper;
using MediatR;
using AirAnalysis.DAL.Options;
using AirAnalysis.BLL.DTOs.Place;

namespace AirAnalysis.BLL.Queries.Place.GetAll
{
    public class GetPlaceHandler : IRequestHandler<GetAllPlaceQuery, List<PlaceDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetPlaceHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<PlaceDto>> Handle(GetAllPlaceQuery request, CancellationToken cancellationToken)
        {
            var options = new QueryOptions<DAL.Entities.Place>
            {
                AsNoTracking = true,
            };
            var places = await _unitOfWork.Place.GetAllAsync(options);

            return _mapper.Map<List<PlaceDto>>(places);
        }
    }
}
