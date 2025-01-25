using AutoMapper;
using SalesTracker.Models;
using SalesTracker.Models.DTO;
namespace SalesTracker
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Lead Mapping
            CreateMap<LeadNew, LeadDTO>().ReverseMap();
            //LeadActivity Mapping
            CreateMap<LeadActivity, LeadActivityDTO>().ReverseMap();
            
        }
    }
}
