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
            CreateMap<Category, CategoryDetail>().ForMember(des => des.Id,
                act => act.MapFrom(src => src.Id))
                                        .ForMember(des => des.Name,
                act => act.MapFrom(src => src.Name))
                                        .ReverseMap();

            CreateMap<Menu, MenuModel>().ReverseMap();

            CreateMap<User, UserIdAndFullName>().ForMember(des => des.UserId,
                act => act.MapFrom(src => src.Id))
                                        .ForMember(des => des.FullName,
                act => act.MapFrom(src => src.FullName))
                                        .ReverseMap();

            CreateMap<Notification, ListNotification>().ForMember(des => des.Id,
                act => act.MapFrom(src => src.Id))
                                        .ForMember(des => des.Title,
                act => act.MapFrom(src => src.Title))
                                        .ForMember(des => des.CreateDate,
                act => act.MapFrom(src => src.CreateDate))
                                        .ForMember(des => des.CreateByUser,
                act => act.MapFrom(src => src.CreateByUser.FullName))
                                        .ForMember(des => des.View,
                act => act.MapFrom(src => src.View))
                                        .ReverseMap();
        }
    }
}
