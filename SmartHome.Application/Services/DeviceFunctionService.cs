using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Serilog;
using SmartHome.Application.Exceptions;
using SmartHome.Application.Interfaces.DeviceFunction;
using SmartHome.Dto.DeviceFunction;
using SmartHome.Enum;

namespace SmartHome.Application.Services
{
    public class DeviceFunctionService : IDeviceFunctionService
    {
        private IDeviceFunctionRepository _deviceFunctionRepository { get; set; }
        public IValidator<CreateDeviceFunctionDto> _createDeviceFunctionDtoValidator { get; set; }
        public IValidator<UpdateDeviceFunctionDto> _updateDeviceFunctionDtoValidator { get; set; }
        public IValidator<DeleteDeviceFunctionDto> _deleteDeviceFunctionDtoValidator { get; set; }
        public IValidator<GetDeviceFunctionDto> _getDeviceFunctionDtoValidator { get; set; }
        private IMapper _mapper { get; set; }
        public DeviceFunctionService(
            IDeviceFunctionRepository deviceFunctionRepository, 
            IMapper mapper, 
            IValidator<CreateDeviceFunctionDto> createDeviceFunctionDtoValidator, 
            IValidator<UpdateDeviceFunctionDto> updateDeviceFunctionDtoValidator, 
            IValidator<DeleteDeviceFunctionDto> deleteDeviceFunctionDtoValidator, 
            IValidator<GetDeviceFunctionDto> getDeviceFunctionDtoValidator
            )
        {
            _deviceFunctionRepository = deviceFunctionRepository;
            _mapper = mapper;
            _createDeviceFunctionDtoValidator = createDeviceFunctionDtoValidator;
            _updateDeviceFunctionDtoValidator = updateDeviceFunctionDtoValidator;
            _deleteDeviceFunctionDtoValidator = deleteDeviceFunctionDtoValidator;
            _getDeviceFunctionDtoValidator = getDeviceFunctionDtoValidator;
        }


        public async Task CreateDeviceFunction(CreateDeviceFunctionDto createDeviceFunctionDto)
        {
            var validationResult = await _createDeviceFunctionDtoValidator.ValidateAsync(createDeviceFunctionDto);
            if (!validationResult.IsValid)
            {
                Log.Error("Validation failed for CreateDeviceFunctionDto: {Errors}", validationResult.Errors);
                throw new FluentValidationException(ApiResponseStatus.Error.ToString(), "Bad Request Body", validationResult.Errors);
            }

            var deviceFunction = _mapper.Map<Domain.Entities.DeviceFunction>(createDeviceFunctionDto);
            await _deviceFunctionRepository.CreateDeviceFunction(deviceFunction);
        }

        public async Task DeleteDeviceFunction(DeleteDeviceFunctionDto deleteDeviceFunctionDto)
        {
            var validationResult = await _deleteDeviceFunctionDtoValidator.ValidateAsync(deleteDeviceFunctionDto);
            if (!validationResult.IsValid)
            {
                Log.Error("Validation failed for DeleteDeviceFunctionDto: {Errors}", validationResult.Errors);
                throw new FluentValidationException(ApiResponseStatus.Error.ToString(), "Bad Request Body", validationResult.Errors);
            }

            var deviceFunction = await _deviceFunctionRepository.GetDeviceFunction(deleteDeviceFunctionDto.Id);
            if (deviceFunction == null)
            {
                throw new KeyNotFoundException("Device function not found");
            }

            await _deviceFunctionRepository.DeleteDeviceFunction(deviceFunction);
        }

        public async Task<DeviceFunctionDto> GetDeviceFunction(Guid id)
        {
            var deviceFunction = await _deviceFunctionRepository.GetDeviceFunction(id);
            if (deviceFunction == null)
            {
                throw new KeyNotFoundException("Device function not found");
            }

            return _mapper.Map<DeviceFunctionDto>(deviceFunction);
        }

        public async Task<List<DeviceFunctionDto>> GetDeviceFunctions()
        {
            var deviceFunctions = await _deviceFunctionRepository.GetDeviceFunctions();
            return _mapper.Map<List<DeviceFunctionDto>>(deviceFunctions);
        }

        public async Task UpdateDeviceFunction(UpdateDeviceFunctionDto updateDeviceFunctionDto)
        {
            var validationResult = await _updateDeviceFunctionDtoValidator.ValidateAsync(updateDeviceFunctionDto);
            if (!validationResult.IsValid)
            {
                Log.Error("Validation failed for UpdateDeviceFunctionDto: {Errors}", validationResult.Errors);
                throw new FluentValidationException(ApiResponseStatus.Error.ToString(), "Bad Request Body", validationResult.Errors);
            }

            var deviceFunction = _mapper.Map<Domain.Entities.DeviceFunction>(updateDeviceFunctionDto);
            await _deviceFunctionRepository.UpdateDeviceFunction(deviceFunction);
        }
    }
}
