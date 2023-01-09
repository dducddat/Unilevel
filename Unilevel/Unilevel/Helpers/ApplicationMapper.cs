using AutoMapper;
using Microsoft.Extensions.Options;
using Unilevel.Data;
using Unilevel.Models;

namespace Unilevel.Helpers
{
    public class ApplicationMapper : Profile
    {
        public ApplicationMapper()
        {
            CreateMap<Role, RoleDetail>().ReverseMap();
            CreateMap<Area, AreaInfor>().ForMember(des => des.AreaCode,
                act => act.MapFrom(src => src.AreaCode))
                                        .ForMember(des => des.Name,
                act => act.MapFrom(src => src.Name))
                                        .ReverseMap();
        }
    }
}
