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

            CreateMap<User, UserIdAndNameAndEmail>().ForMember(des => des.UserId,
                act => act.MapFrom(src => src.Id))
                                        .ForMember(des => des.FullName,
                act => act.MapFrom(src => src.FullName))
                                        .ForMember(des => des.Email,
                act => act.MapFrom(src => src.Email))
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

            CreateMap<Notification, NewNotification>().ForMember(des => des.Id,
                act => act.MapFrom(src => src.Id))
                                        .ForMember(des => des.Title,
                act => act.MapFrom(src => src.Title))
                                        .ForMember(des => des.NameUserCreated,
                act => act.MapFrom(src => src.CreateByUser.FullName))
                                        .ForMember(des => des.AvatarUserCreated,
                act => act.MapFrom(src => src.CreateByUser.Avatar))
                                        .ReverseMap();

            CreateMap<Notification, NotificationDetail>().ForMember(des => des.Id,
                act => act.MapFrom(src => src.Id))
                                        .ForMember(des => des.Title,
                act => act.MapFrom(src => src.Title))
                                        .ForMember(des => des.Description,
                act => act.MapFrom(src => src.Description))
                                        .ForMember(des => des.CreateDate,
                act => act.MapFrom(src => src.CreateDate))
                                        .ForMember(des => des.NameUserCreated,
                act => act.MapFrom(src => src.CreateByUser.FullName))
                                        .ForMember(des => des.AvatarUserCreated,
                act => act.MapFrom(src => src.CreateByUser.Avatar))
                                        .ReverseMap();

            CreateMap<Job, JobSummary>().ForMember(des => des.Id,
                act => act.MapFrom(src => src.Id))
                                        .ForMember(des => des.Title,
                act => act.MapFrom(src => src.Title))
                                        .ForMember(des => des.CreateDate,
                act => act.MapFrom(src => src.CreateDate.ToString("dd-MM-yyyy HH:mm:ss")))
                                        .ForMember(des => des.Status,
                act => act.MapFrom(src => src.Status))
                                        .ForMember(des => des.CreateByUser,
                act => act.MapFrom(src => src.CreateByUser.FullName))
                                        .ReverseMap();

            CreateMap<VisitPlan, VisitPlanSummary>().ForMember(des => des.Id,
                act => act.MapFrom(src => src.Id))
                                        .ForMember(des => des.Title,
                act => act.MapFrom(src => src.Title))
                                        .ForMember(des => des.CreateDate,
                act => act.MapFrom(src => src.CreateDate.ToString("dd-MM-yyyy HH:mm:ss")))
                                        .ForMember(des => des.Status,
                act => act.MapFrom(src => src.Status))
                                        .ForMember(des => des.VisitDate,
                act => act.MapFrom(src => src.VisitDate.ToString("dd-MM-yyyy HH:mm:ss")))
                                        .ForMember(des => des.CreateByUser,
                act => act.MapFrom(src => src.User.FullName))
                                        .ReverseMap();
        }
    }
}
