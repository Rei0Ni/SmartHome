using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Serilog;
using SmartHome.Application.Exceptions;
using SmartHome.Application.Interfaces.DeviceType;
using SmartHome.Domain.Entities;
using SmartHome.Dto.DeviceType;
using SmartHome.Enum;
using Log = Serilog.Log;

namespace SmartHome.Application.Services
{
    public class DeviceTypeService : IDeviceTypeService
    {
        public IDeviceTypeRepository _deviceTypeRepository { get; set; }
        public IValidator<CreateDeviceTypeDto> _createDeviceTypeDtoValidator { get; set; }
        public IValidator<UpdateDeviceTypeDto> _updateDeviceTypeDtoValidator { get; set; }
        public IValidator<DeleteDeviceTypeDto> _deleteDeviceTypeDtoValidator { get; set; }
        public IValidator<GetDeviceTypeDto> _getDeviceTypeDtoValidator { get; set; }
        public IMapper _mapper { get; set; }
        public DeviceTypeService(
            IDeviceTypeRepository deviceTypeRepository,
            IMapper mapper,
            IValidator<CreateDeviceTypeDto> createDeviceTypeDtoValidator,
            IValidator<UpdateDeviceTypeDto> updateDeviceTypeDtoValidator,
            IValidator<DeleteDeviceTypeDto> deleteDeviceTypeDtoValidator,
            IValidator<GetDeviceTypeDto> getDeviceTypeDtoValidator)
        {
            _deviceTypeRepository = deviceTypeRepository;
            _mapper = mapper;
            _createDeviceTypeDtoValidator = createDeviceTypeDtoValidator;
            _updateDeviceTypeDtoValidator = updateDeviceTypeDtoValidator;
            _deleteDeviceTypeDtoValidator = deleteDeviceTypeDtoValidator;
            _getDeviceTypeDtoValidator = getDeviceTypeDtoValidator;
        }

        public async Task CreateDeviceType(CreateDeviceTypeDto createDeviceTypeDto)
        {
            var validationResult = await _createDeviceTypeDtoValidator.ValidateAsync(createDeviceTypeDto);
            if (!validationResult.IsValid)
            {
                Log.Error("Validation failed for CreateDeviceTypeDto: {Errors}", validationResult.Errors);
                throw new FluentValidationException(ApiResponseStatus.Error.ToString(), "Bad Request Body", validationResult.Errors);
            }

            var deviceType = _mapper.Map<DeviceType>(createDeviceTypeDto);
            await _deviceTypeRepository.CreateDeviceType(deviceType);
        }

        public async Task DeleteDeviceType(DeleteDeviceTypeDto deleteDeviceType)
        {
            var validationResult = await _deleteDeviceTypeDtoValidator.ValidateAsync(deleteDeviceType);
            if (!validationResult.IsValid)
            {
                Log.Error("Validation failed for DeleteDeviceTypeDto: {Errors}", validationResult.Errors);
                throw new FluentValidationException(ApiResponseStatus.Error.ToString(), "Bad Request Body", validationResult.Errors);
            }

            var deviceType = await _deviceTypeRepository.GetDeviceType(deleteDeviceType.Id);
            if (deviceType != null)
            {
                await _deviceTypeRepository.DeleteDeviceType(deviceType);
            }
        }

        public async Task<DeviceTypeDto> GetDeviceType(Guid id)
        {
            var getDeviceTypeDto = new GetDeviceTypeDto { Id = id };
            var validationResult = await _getDeviceTypeDtoValidator.ValidateAsync(getDeviceTypeDto);
            if (!validationResult.IsValid)
            {
                Log.Error("Validation failed for GetDeviceTypeDto: {Errors}", validationResult.Errors);
                throw new FluentValidationException(ApiResponseStatus.Error.ToString(), "Bad Request Body", validationResult.Errors);
            }

            var deviceType = await _deviceTypeRepository.GetDeviceType(id);
            if (deviceType == null)
            {
                return null;
            }
            return _mapper.Map<DeviceTypeDto>(deviceType);
        }

        public async Task<List<DeviceTypeDto>> GetDeviceTypes()
        {
            var deviceTypes = await _deviceTypeRepository.GetDeviceTypes();
            return _mapper.Map<List<DeviceTypeDto>>(deviceTypes);
        }

        public async Task UpdateDeviceType(UpdateDeviceTypeDto updateDeviceTypeDto)
        {
            var validationResult = await _updateDeviceTypeDtoValidator.ValidateAsync(updateDeviceTypeDto);
            if (!validationResult.IsValid)
            {
                Log.Error("Validation failed for UpdateDeviceTypeDto: {Errors}", validationResult.Errors);
                throw new FluentValidationException(ApiResponseStatus.Error.ToString(), "Bad Request Body", validationResult.Errors);
            }

            var deviceType = await _deviceTypeRepository.GetDeviceType(updateDeviceTypeDto.Id);
            if (deviceType != null)
            {
                deviceType.Name = updateDeviceTypeDto.Name;
                await _deviceTypeRepository.UpdateDeviceType(deviceType);
            }
        }
    }
}
