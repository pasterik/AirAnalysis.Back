using AirAnalysis.BLL.DTOs.Phenomen;
using AirAnalysis.DAL.Entities;
using AirAnalysis.DAL.Repositories.Interfaces;
using AutoMapper;
using MediatR;
using AirAnalysis.DAL.Options;

namespace AirAnalysis.BLL.Queries.Phenomen.GetAll
{
    public class GetAllPhenomenHandler : IRequestHandler<GetAllPhenomenQuery, List<PhenomenDto>>
    {
        private readonly IUnitOfWork _phenomenRepository;
        private readonly IMapper _mapper;
        public GetAllPhenomenHandler(IUnitOfWork phenomenRepository, IMapper mapper)
        {
            _phenomenRepository = phenomenRepository;
            _mapper = mapper;
        }
        public async Task<List<PhenomenDto>> Handle(GetAllPhenomenQuery request, CancellationToken cancellationToken)
        {
            var options = new QueryOptions<DAL.Entities.Phenomen>
            {
                AsNoTracking = true,
            };
            var phenomens = await _phenomenRepository.Phenomen.GetAllAsync(options);

            return _mapper.Map<List<PhenomenDto>>(phenomens);
        }
    }
}
