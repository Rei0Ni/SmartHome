using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using SmartHome.Dto.Area;
using SmartHome.Dto.User;
using SmartHome.Domain.Entities;
using SmartHome.Dto.Controller;

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
            CreateMap<Area, AreaDto>().ReverseMap();

            CreateMap<Controller, CreateControllerDto>().ReverseMap();
            CreateMap<Controller, UpdateControllerDto>().ReverseMap();
            CreateMap<Controller, DeleteControllerDto>().ReverseMap();
            CreateMap<Controller, ControllerDto>().ReverseMap();
        }
    }
}
