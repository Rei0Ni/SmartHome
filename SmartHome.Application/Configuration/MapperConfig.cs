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
using SmartHome.Dto.DeviceType;
using SmartHome.Dto.DeviceFunction;
using SmartHome.Dto.Device;

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
            CreateMap<Area, AreaDto>().ReverseMap();

            CreateMap<Controller, CreateControllerDto>().ReverseMap();
            CreateMap<Controller, UpdateControllerDto>().ReverseMap();
            CreateMap<Controller, DeleteControllerDto>().ReverseMap();
            CreateMap<Controller, GetControllerDto>().ReverseMap();
            CreateMap<Controller, ControllerDto>().ReverseMap();

            CreateMap<DeviceType, CreateDeviceTypeDto>().ReverseMap();
            CreateMap<DeviceType, UpdateDeviceTypeDto>().ReverseMap();
            CreateMap<DeviceType, DeleteDeviceTypeDto>().ReverseMap();
            CreateMap<DeviceType, GetDeviceTypeDto>().ReverseMap();
            CreateMap<DeviceType, DeviceTypeDto>().ReverseMap();

            CreateMap<DeviceFunction, CreateDeviceFunctionDto>().ReverseMap();
            CreateMap<DeviceFunction, UpdateDeviceFunctionDto>().ReverseMap();
            CreateMap<DeviceFunction, DeleteDeviceFunctionDto>().ReverseMap();
            CreateMap<DeviceFunction, GetDeviceFunctionDto>().ReverseMap();
            CreateMap<DeviceFunction, DeviceFunctionDto>().ReverseMap();

            CreateMap<Device, CreateDeviceDto>().ReverseMap();
            CreateMap<Device, UpdateDeviceDto>().ReverseMap();
            CreateMap<Device, DeleteDeviceDto>().ReverseMap();
            CreateMap<Device, GetDeviceDto>().ReverseMap();
            CreateMap<Device, DeviceDto>().ReverseMap();
        }
    }
}
