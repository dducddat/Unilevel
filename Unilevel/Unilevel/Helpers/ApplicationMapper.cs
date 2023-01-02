﻿using AutoMapper;
using Microsoft.Extensions.Options;
using Unilevel.Data;
using Unilevel.Models;

namespace Unilevel.Helpers
{
    public class ApplicationMapper : Profile
    {
        public ApplicationMapper()
        {
            CreateMap<Distributor, DistributorDTO>().ReverseMap()
                .ForMember(source => source.Id,
                            options => options.MapFrom(destination => destination.Id))
                .ForMember(source => source.Name,
                            options => options.MapFrom(destination => destination.Name))
                .ForMember(source => source.Email,
                            options => options.MapFrom(destination => destination.Email))
                .ForMember(source => source.Address,
                            options => options.MapFrom(destination => destination.Address))
                .ForMember(source => source.PhoneNumber,
                            options => options.MapFrom(destination => destination.PhoneNumber));
            CreateMap<Role, RoleDTO>().ReverseMap();
        }
    }
}
