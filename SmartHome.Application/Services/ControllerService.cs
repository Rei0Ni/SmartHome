using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Serilog;
using SmartHome.Application.Exceptions;
using SmartHome.Application.Interfaces.Area;
using SmartHome.Application.Interfaces.Controller;
using SmartHome.Application.Validations.Area;
using SmartHome.Domain.Entities;
using SmartHome.Dto.Area;
using SmartHome.Dto.Controller;
using SmartHome.Enum;
using Log = Serilog.Log;

namespace SmartHome.Application.Services
{

    public class ControllerService : IControllerService
    {
        private readonly IControllerRepository _controllerRepository;
        private readonly IAreaRepository _areaRepository;
        private readonly IValidator<CreateControllerDto> _createControllerDtoValidator;
        private readonly IValidator<UpdateControllerDto> _updateControllerDtoValidator;
        private readonly IValidator<DeleteControllerDto> _deleteControllerDtoValidator;
        private readonly IValidator<GetControllerDto> _getControllerDtoValidator;
        private readonly IMapper _mapper;
        public ControllerService(IControllerRepository controllerRepository, IMapper mapper,
            IValidator<CreateControllerDto> createControllerDtoValidator,
            IValidator<UpdateControllerDto> updateControllerDtoValidator,
            IValidator<DeleteControllerDto> deleteControllerDtoValidator,
            IValidator<GetControllerDto> getControllerDtoValidator, 
            IAreaRepository areaRepository)
        {
            _controllerRepository = controllerRepository;
            _mapper = mapper;
            _createControllerDtoValidator = createControllerDtoValidator;
            _updateControllerDtoValidator = updateControllerDtoValidator;
            _deleteControllerDtoValidator = deleteControllerDtoValidator;
            _getControllerDtoValidator = getControllerDtoValidator;
            _areaRepository = areaRepository;
        }

        public async Task CreateController(CreateControllerDto createControllerDto)
        {
            Log.Information("Creating Controller with name: {Name}", createControllerDto.Name);
            var validationResult = await _createControllerDtoValidator.ValidateAsync(createControllerDto);

            if (!validationResult.IsValid)
            {
                Log.Error("Validation failed for CreateControllerDto: {Errors}", validationResult.Errors);
                throw new FluentValidationException(ApiResponseStatus.Error.ToString(), "Bad Request Body", validationResult.Errors);
            }

            if (createControllerDto == null)
            {
                Log.Error("CreateControllerDto is null");
                throw new ArgumentNullException(nameof(createControllerDto));
            }

            var controller = _mapper.Map<Controller>(createControllerDto);
            controller.Id = Guid.NewGuid();
            controller.LastSeen = DateTime.UtcNow;

            await _controllerRepository.CreateController(controller);
        }

        public async Task DeleteController(DeleteControllerDto deleteController)
        {
            Log.Information("Deleting Controller with Id: {ID}", deleteController.Id);
            var validationResult = await _deleteControllerDtoValidator.ValidateAsync(deleteController);

            if (!validationResult.IsValid)
            {
                Log.Error("Validation failed for DeleteControllerDto: {Errors}", validationResult.Errors);
                throw new FluentValidationException(ApiResponseStatus.Error.ToString(), "Bad Request Body", validationResult.Errors);
            }

            if (deleteController == null)
            {
                Log.Error("DeleteControllerDto is null");
                throw new ArgumentNullException(nameof(deleteController));
            }

            var controller = await _controllerRepository.GetController(deleteController.Id);
            if (controller == null)
            {
                Log.Error("Controller with Id: {ID} not found", deleteController.Id);
                throw new KeyNotFoundException($"Controller with Id: {deleteController.Id} not found");
            }

            var controllerAreas = await _areaRepository.GetAllAreas();
            var areasToDelete = controllerAreas.Where(area => area.ControllerId == deleteController.Id).ToList();

            foreach (var area in areasToDelete)
            {
                await _areaRepository.DeleteArea(area);
            }

            await _controllerRepository.DeleteController(controller);
        }

        public async Task<ControllerDto> GetController(GetControllerDto getController)
        {
            Log.Information("Getting Controller with Id: {ID}", getController.Id);
            var validationResult = await _getControllerDtoValidator.ValidateAsync(getController);

            if (!validationResult.IsValid)
            {
                Log.Error("Validation failed for GetControllerDto: {Errors}", validationResult.Errors);
                throw new FluentValidationException(ApiResponseStatus.Error.ToString(), "Bad Request Body", validationResult.Errors);
            }

            if (getController == null)
            {
                Log.Error("GetControllerDto is null");
                throw new ArgumentNullException(nameof(getController));
            }

            var controller = await _controllerRepository.GetController(getController.Id);
            if (controller == null)
            {
                return null;
            }

            return _mapper.Map<ControllerDto>(controller);
        }

        public async Task<string> GetControllerIpAsync(Guid id)
        {
            var controller = await _controllerRepository.GetController(id);

            if (controller == null)
            {
                throw new KeyNotFoundException($"Controller with Id: {id} not found");
            }

            return controller.IPAddress;
        }

        public async Task<List<ControllerDto>> GetControllers()
        {
            var controllers = await _controllerRepository.GetControllers();
            return _mapper.Map<List<ControllerDto>>(controllers);
        }

        public async Task UpdateController(UpdateControllerDto updateControllerDto)
        {
            Log.Information("Updating Controller with Id: {ID}", updateControllerDto.Id);
            var validationResult = await _updateControllerDtoValidator.ValidateAsync(updateControllerDto);

            if (!validationResult.IsValid)
            {
                Log.Error("Validation failed for UpdateControllerDto: {Errors}", validationResult.Errors);
                throw new FluentValidationException(ApiResponseStatus.Error.ToString(), "Bad Request Body", validationResult.Errors);
            }

            if (updateControllerDto == null)
            {
                Log.Error("UpdateControllerDto is null");
                throw new ArgumentNullException(nameof(updateControllerDto));
            }

            var controller = await _controllerRepository.GetController(updateControllerDto.Id);
            if (controller != null)
            {
                _mapper.Map(updateControllerDto, controller);
                controller.LastSeen = DateTime.UtcNow;

                await _controllerRepository.UpdateController(controller);
            }
        }
    }
}
