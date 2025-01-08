using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using SmartHome.Application.DTOs.User;
using SmartHome.Domain.Entities;

namespace SmartHome.Application.Configuration
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<ApplicationUser,
                UserInfoDto>().ReverseMap();
        }
    }
}
