using AirAnalysis.BLL.DTOs.RecordData;
using AirAnalysis.DAL.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirAnalysis.BLL.Mapping
{
    public class RecordDataProfile : Profile
    {
        public RecordDataProfile()
        {
            CreateMap<CreateRecordDateDto, RecordData>();
            CreateMap<RecordData, RecordDateDto>();

        }
    }
}
