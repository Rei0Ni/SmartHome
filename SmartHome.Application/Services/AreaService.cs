using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Serilog;
using SmartHome.Enum;
using SmartHome.Application.Exceptions;
using SmartHome.Application.Interfaces.Area;
using SmartHome.Dto.Area;
using SmartHome.Application.Interfaces.Controller;

namespace SmartHome.Application.Services
{
    public class AreaService : IAreaService
    {
        private readonly IValidator<GetAreaDto> _getAreaDtoValidator;
        private readonly IValidator<UpdateAreaDto> _updateAreaDtoValidator;
        private readonly IValidator<CreateAreaDto> _createAreaDtoValidator;
        private readonly IValidator<DeleteAreaDto> _deleteAreaDtoValidator;
        private readonly IAreaRepository _areaRepository;
        private readonly IControllerRepository _controllerRepository;
        private IMapper _mapper { get; set; }

        public AreaService(
            IAreaRepository areaRepository,
            IMapper mapper,
            IValidator<GetAreaDto> getAreaDtoValidator,
            IValidator<UpdateAreaDto> updateAreaDtoValidator,
            IValidator<CreateAreaDto> createAreaDtoValidator,
            IValidator<DeleteAreaDto> deleteAreaDtoValidator,
            IControllerRepository controllerRepository)
        {
            _areaRepository = areaRepository;
            _mapper = mapper;
            _getAreaDtoValidator = getAreaDtoValidator;
            _updateAreaDtoValidator = updateAreaDtoValidator;
            _createAreaDtoValidator = createAreaDtoValidator;
            _deleteAreaDtoValidator = deleteAreaDtoValidator;
            _controllerRepository = controllerRepository;
        }

        public async Task CreateArea(CreateAreaDto createAreaDto)
        {
            Log.Information("Creating area with name: {Name}", createAreaDto.Name);
            var validationResult = await _createAreaDtoValidator.ValidateAsync(createAreaDto);

            if (!validationResult.IsValid)
            {
                Log.Error("Validation failed for CreateAreaDto: {Errors}", validationResult.Errors);
                throw new FluentValidationException(ApiResponseStatus.Error.ToString(), "Bad Request Body", validationResult.Errors);
            }

            if (createAreaDto == null)
            {
                Log.Error("CreateAreaDto is null");
                throw new ArgumentNullException(nameof(createAreaDto));
            }

            var Controller = await _controllerRepository.GetController(createAreaDto.ControllerId);
            if (Controller == null)
            {
                Log.Error("Controller with ID: {Id} not found", createAreaDto.ControllerId);
                throw new ArgumentNullException(nameof(createAreaDto.ControllerId));
            }

            var newArea = _mapper.Map<Domain.Entities.Area>(createAreaDto);

            await _areaRepository.CreateArea(newArea);

            Controller.Areas.Add(newArea.Id);

            await _controllerRepository.UpdateController(Controller);

            Log.Information("Area created successfully with name: {Name}", createAreaDto.Name);
        }

        public async Task DeleteArea(DeleteAreaDto deleteArea)
        {
            Log.Information("Deleting area with ID: {Id}", deleteArea.Id);
            var validationResult = await _deleteAreaDtoValidator.ValidateAsync(deleteArea);
            if (!validationResult.IsValid)
            {
                Log.Error("Validation failed for DeleteAreaDto: {Errors}", validationResult.Errors);
                throw new FluentValidationException(ApiResponseStatus.Error.ToString(), "Bad Request Body", validationResult.Errors);
            }
            if (deleteArea == null)
            {
                Log.Error("DeleteAreaDto is null");
                throw new ArgumentNullException(nameof(deleteArea));
            }
            var area = await _areaRepository.GetArea(deleteArea.Id);
            if (area == null)
            {
                Log.Error("Area with ID: {Id} not found", deleteArea.Id);
                throw new KeyNotFoundException(nameof(deleteArea.Id));
            }
            await _areaRepository.DeleteArea(area);
            Log.Information("Area deleted successfully with ID: {Id}", deleteArea.Id);
        }

        public async Task<AreaDto> GetArea(GetAreaDto getArea)
        {
            var validationResult = await _getAreaDtoValidator.ValidateAsync(getArea);
            if (!validationResult.IsValid)
            {
                Log.Error("Validation failed for GetAreaDto: {Errors}", validationResult.Errors);
                throw new FluentValidationException(ApiResponseStatus.Error.ToString(), "Bad Request Body", validationResult.Errors);
            }
            if (getArea == null)
            {
                Log.Error("GetAreaDto is null");
                throw new ArgumentNullException(nameof(getArea));
            }
            var area = await _areaRepository.GetArea(getArea.Id);
            var getAreaDto = _mapper.Map<AreaDto>(area);
            return getAreaDto;
        }

        public async Task<List<AreaDto>> GetAreas()
        {
            var areas = await _areaRepository.GetAreas();
            var getAreaDtos = _mapper.Map<List<AreaDto>>(areas);
            return getAreaDtos;
        }

        public async Task UpdateArea(UpdateAreaDto updateAreaDto)
        {
            Log.Information("Updating area with ID: {Id}", updateAreaDto.Id);
            var validationResult = await _updateAreaDtoValidator.ValidateAsync(updateAreaDto);
            if (!validationResult.IsValid)
            {
                Log.Error("Validation failed for UpdateAreaDto: {Errors}", validationResult.Errors);
                throw new FluentValidationException(ApiResponseStatus.Error.ToString(), "Bad Request Body", validationResult.Errors);
            }
            if (updateAreaDto == null)
            {
                Log.Error("UpdateAreaDto is null");
                throw new ArgumentNullException(nameof(updateAreaDto));
            }

            var area = await _areaRepository.GetArea(updateAreaDto.Id);
            if (area == null)
            {
                Log.Error("Area with ID: {Id} not found", updateAreaDto.Id);
                throw new KeyNotFoundException(nameof(updateAreaDto.Id));
            }

            await _areaRepository.UpdateArea(area);
            Log.Information("Area updated successfully with ID: {Id}", updateAreaDto.Id);
        }
    }
}
