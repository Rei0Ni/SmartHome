using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using SmartHome.Dto.Area;
using SmartHome.Dto.User;
using SmartHome.Domain.Entities;

namespace SmartHome.Application.Configuration
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<ApplicationUser, UserInfoDto>().ReverseMap();

            CreateMap<Area, CreateAreaDto>().ReverseMap();
            CreateMap<Area, UpdateAreaDto>().ReverseMap();
            CreateMap<Area, DeleteAreaDto>().ReverseMap();
            CreateMap<Area, GetAreaDto>().ReverseMap();
        }
    }
}
