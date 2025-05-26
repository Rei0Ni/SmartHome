using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using SmartHome.Application.Hubs;
using SmartHome.Application.Interfaces;
using SmartHome.Application.Interfaces.Area;
using SmartHome.Application.Interfaces.Device;
using SmartHome.Application.Interfaces.DeviceType;
using SmartHome.Application.Interfaces.Hubs;
using SmartHome.Domain.Entities;
using SmartHome.Dto.Area;
using SmartHome.Dto.Dashboard;
using Log = Serilog.Log;

namespace SmartHome.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IAreaService _areaService;
        private readonly IDeviceDataService _deviceDataService;
        private readonly IDeviceTypeService _deviceTypeService;
        //private readonly IDeviceFunctionService _deviceFunctionService;
        //private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubState _hubState;
        private readonly IHubContext<OverviewHub> _overviewHubContext;
        private IMapper _mapper;

        public DashboardService(IAreaService areaService, IDeviceDataService deviceDataService, IDeviceTypeService deviceTypeService, IMapper mapper, UserManager<ApplicationUser> userManager, IHubState hubState, IHubContext<OverviewHub> overviewHubContext)
        {
            _areaService = areaService;
            _deviceDataService = deviceDataService; // Inject DeviceDataService
            _deviceTypeService = deviceTypeService;
            //_deviceFunctionService = deviceFunctionService;
            _mapper = mapper;
            //_httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _hubState = hubState;
            _overviewHubContext = overviewHubContext;
        }

        public async Task<OverviewDto> GetDashboardOverview(string UserId)
        {
            var currentUser = await _userManager.FindByIdAsync(UserId);
            if (currentUser == null) throw new UnauthorizedAccessException("User not found");

            // Get filtered areas based on role
            List<AreaDto> areas;
            if (await _userManager.IsInRoleAsync(currentUser, "Admin"))
            {
                areas = await _areaService.GetAllAreas();
            }
            else
            {
                areas = await _areaService.GettAllowedAreas(currentUser.Id);
            }

            var Overview = new OverviewDto();

            // Optimize device loading
            var areaIds = areas.Select(a => a.Id).ToList();
            var allDevices = await _deviceDataService.GetDevicesForAreas(areaIds); //Use DeviceDataService
            var allIPCameras = await _deviceDataService.GetIPCamerasForAreas(areaIds); // Use DeviceDataService for IPCameras
            var deviceTypeIds = allDevices.Select(d => d.DeviceTypeId).Distinct().ToList();

            // Bulk load device types and functions
            var deviceTypes = await _deviceTypeService.GetDeviceTypes();

            foreach (var area in areas)
            {
                var overviewArea = _mapper.Map<OverviewAreaDto>(area);
                var areaDevices = allDevices.Where(d => d.AreaId == area.Id);

                foreach (var device in areaDevices)
                {
                    var overviewDevice = _mapper.Map<OverviewDeviceDto>(device);
                    var deviceType = deviceTypes.FirstOrDefault(dt => dt.Id == device.DeviceTypeId);

                    if (deviceType == null)
                    {
                        Log.Error($"Missing device type {device.DeviceTypeId} for device {device.Id}");
                        continue; // or handle differently
                    }

                    overviewDevice.DeviceType = deviceType;

                    overviewArea.AreaDevices.Add(overviewDevice);
                }

                foreach (var camera in allIPCameras.Where(c => c.AreaId == area.Id))
                {
                    var overviewCamera = _mapper.Map<OverviewCameraDto>(camera);
                    overviewArea.AreaCameras.Add(overviewCamera);
                }

                Overview.Areas.Add(overviewArea);
            }

            return Overview;
        }


        public async Task SendOverviewUpdateToUser(string userId)
        {
            try
            {
                var overviewData = await GetDashboardOverview(userId);
                await _overviewHubContext.Clients.Group(userId).SendAsync("ReceiveOverviewData", overviewData);
            }
            catch (Exception ex)
            {
                Log.Error($"Error sending overview update to user {userId}: {ex.Message}");
            }
        }

        // Method to send overview update to all connected users
        public async Task SendOverviewUpdateToAll()
        {
            List<string> currentConnectedUserIds = _hubState.GetConnectedUsers();

            foreach (var userId in currentConnectedUserIds)
            {
                await SendOverviewUpdateToUser(userId);
            }
        }
    }
}