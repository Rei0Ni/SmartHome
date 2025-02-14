using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using SmartHome.Application.Interfaces.Area;
using SmartHome.Application.Interfaces.Device;
using SmartHome.Application.Interfaces.DeviceFunction;
using SmartHome.Application.Interfaces.DeviceType;
using SmartHome.Domain.Entities;
using SmartHome.Dto.Device;
using SmartHome.Dto.DeviceFunction;

namespace SmartHome.Application.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IDeviceTypeService _deviceTypeService;
        private readonly IDeviceTypeRepository _deviceTypeRepository;
        private readonly IDeviceFunctionService _deviceFunctionService;
        private readonly IAreaRepository _areaRepository;
        private IValidator<CreateDeviceDto> _createDeviceDtoValidator;
        private IValidator<UpdateDeviceDto> _updateDeviceDtoValidator;
        private IValidator<DeleteDeviceDto> _deleteDeviceDtoValidator;
        private IValidator<GetDeviceDto> _getDeviceDtovalidator;
        private IMapper _mapper;
        public DeviceService(
            IDeviceRepository deviceRepository,
            IValidator<CreateDeviceDto> createDeviceDtoValidator,
            IValidator<UpdateDeviceDto> updateDeviceDtoValidator,
            IValidator<DeleteDeviceDto> deleteDeviceDtoValidator,
            IValidator<GetDeviceDto> getDeviceDtovalidator,
            IMapper mapper,
            IDeviceTypeService deviceTypeService,
            IDeviceFunctionService deviceFunctionService,
            IDeviceTypeRepository deviceTypeRepository,
            IAreaRepository areaRepository)
        {
            _deviceRepository = deviceRepository;
            _createDeviceDtoValidator = createDeviceDtoValidator;
            _updateDeviceDtoValidator = updateDeviceDtoValidator;
            _deleteDeviceDtoValidator = deleteDeviceDtoValidator;
            _getDeviceDtovalidator = getDeviceDtovalidator;
            _mapper = mapper;
            _deviceTypeService = deviceTypeService;
            _deviceFunctionService = deviceFunctionService;
            _deviceTypeRepository = deviceTypeRepository;
            _areaRepository = areaRepository;
        }


        public async Task CreateDevice(CreateDeviceDto createDeviceDto)
        {
            var validationResult = await _createDeviceDtoValidator.ValidateAsync(createDeviceDto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var device = _mapper.Map<Device>(createDeviceDto);
            await _deviceRepository.CreateDevice(device);

            var deviceType = await _deviceTypeRepository.GetDeviceType(device.DeviceTypeId);
            deviceType.Devices.Add(device.Id);
            await _deviceTypeRepository.UpdateDeviceType(deviceType);

            var area = await _areaRepository.GetArea(device.AreaId);
            area.Devices.Add(device.Id);
            await _areaRepository.UpdateArea(area);
        }

        public async Task DeleteDevice(DeleteDeviceDto deleteDeviceDto)
        {
            var validationResult = await _deleteDeviceDtoValidator.ValidateAsync(deleteDeviceDto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
            var device = _mapper.Map<Device>(await _deviceRepository.GetDevice(deleteDeviceDto.Id));

            if (device == null)
            {
                throw new KeyNotFoundException("Device not found");
            }
            await _deviceRepository.DeleteDevice(device);
        }

        public async Task<DeviceDto> GetDevice(Guid id)
        {
            var device = await _deviceRepository.GetDevice(id);
            if (device == null)
            {
                throw new KeyNotFoundException("Device not found");
            }

            

            var deviceType = await _deviceTypeService.GetDeviceType(device.DeviceTypeId);
            if (deviceType == null)
            {
                throw new KeyNotFoundException("Device Type Not Found found");
            }

            var deviceFunctions = new List<DeviceFunctionDto>();

            foreach (var deviceFunctionId in deviceType.Functions)
            {
                var deviceFunction = await _deviceFunctionService.GetDeviceFunction(deviceFunctionId);
                if (deviceFunction == null)
                {
                    throw new Exception("one or more Device Functions not found");
                }
                deviceFunctions.Add(deviceFunction);
            }

            var dto = _mapper.Map<DeviceDto>(device);
            dto.DeviceType = deviceType;
            dto.DeviceFunctions = deviceFunctions;

            return dto;
        }

        public async Task<List<DeviceDto>> GetDevices()
        {
            var devices = await _deviceRepository.GetDevices();
            return _mapper.Map<List<DeviceDto>>(devices);
        }

        public async Task UpdateDevice(UpdateDeviceDto updateDeviceDto)
        {
            var validationResult = await _updateDeviceDtoValidator.ValidateAsync(updateDeviceDto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var device = await _deviceRepository.GetDevice(updateDeviceDto.Id);
            if (device == null)
            {
                throw new KeyNotFoundException("Device not found");
            }

            _mapper.Map(updateDeviceDto, device);
            await _deviceRepository.UpdateDevice(device);
        }
    }
}
