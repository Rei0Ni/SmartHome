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
using SmartHome.Dto.Dashboard;
using SmartHome.Dto.Log;
using SmartHome.Dto.IPCamera;

namespace SmartHome.Application.Configuration
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<ApplicationUser, UserAuthenticationState>().ReverseMap();

            CreateMap<Area, OverviewAreaDto>().ReverseMap();
            CreateMap<Area, CreateAreaDto>().ReverseMap();
            CreateMap<Area, UpdateAreaDto>().ReverseMap();
            CreateMap<Area, DeleteAreaDto>().ReverseMap();
            CreateMap<Area, GetAreaDto>().ReverseMap();
            CreateMap<Area, AreaDto>().ReverseMap();

            CreateMap<AreaDto, OverviewAreaDto>().ReverseMap();

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

            CreateMap<Device, OverviewDeviceDto>().ReverseMap();
            CreateMap<Device, CreateDeviceDto>().ReverseMap();
            CreateMap<Device, UpdateDeviceDto>().ReverseMap();
            CreateMap<Device, DeleteDeviceDto>().ReverseMap();
            CreateMap<Device, GetDeviceDto>().ReverseMap();
            CreateMap<Device, DeviceDto>().ReverseMap();

            CreateMap<IPCamera, CreateIPCameraDto>().ReverseMap();
            CreateMap<IPCamera, UpdateIPCameraDto>().ReverseMap();
            CreateMap<IPCamera, IPCameraConnectionInfoDto>().ReverseMap();
            CreateMap<IPCamera, IPCameraDto>().ReverseMap();
            

            CreateMap<Log, LogDto>().ReverseMap();

            CreateMap<DeviceDto, OverviewDeviceDto>().ReverseMap();
            CreateMap<IPCameraDto, OverviewCameraDto>().ReverseMap();
        }
    }
}
