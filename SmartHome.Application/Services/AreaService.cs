using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Serilog;
using SmartHome.Application.Enums;
using SmartHome.Application.Exceptions;
using SmartHome.Application.Interfaces.Area;
using SmartHome.Dto.Area;

namespace SmartHome.Application.Services
{
    public class AreaService : IAreaService
    {
        private readonly IValidator<GetAreaDto> _getAreaDtoValidator;
        private readonly IValidator<UpdateAreaDto> _updateAreaDtoValidator;
        private readonly IValidator<CreateAreaDto> _createAreaDtoValidator;
        private readonly IValidator<DeleteAreaDto> _deleteAreaDtoValidator;
        private readonly IAreaRepository _areaRepository;
        private IMapper _mapper { get; set; }

        public AreaService(
            IAreaRepository areaRepository,
            IMapper mapper,
            IValidator<GetAreaDto> getAreaDtoValidator,
            IValidator<UpdateAreaDto> updateAreaDtoValidator,
            IValidator<CreateAreaDto> createAreaDtoValidator,
            IValidator<DeleteAreaDto> deleteAreaDtoValidator)
        {
            _areaRepository = areaRepository;
            _mapper = mapper;
            _getAreaDtoValidator = getAreaDtoValidator;
            _updateAreaDtoValidator = updateAreaDtoValidator;
            _createAreaDtoValidator = createAreaDtoValidator;
            _deleteAreaDtoValidator = deleteAreaDtoValidator;
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

            await _areaRepository.CreateArea(new Domain.Entities.Area
            {
                Name = createAreaDto.Name
            });

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
            await _areaRepository.DeleteArea(deleteArea);
            Log.Information("Area deleted successfully with ID: {Id}", deleteArea.Id);
        }

        public async Task<GetAreaDto> GetArea(GetAreaDto getArea)
        {
            Log.Information("Getting area with ID: {Id}", getArea.Id);
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
            var getAreaDto = _mapper.Map<GetAreaDto>(area);
            Log.Information("Area retrieved successfully with ID: {Id}", getArea.Id);
            return getAreaDto;
        }

        public async Task<List<GetAreaDto>> GetAreas()
        {
            Log.Information("Getting all areas");
            var areas = await _areaRepository.GetAreas();
            var getAreaDtos = _mapper.Map<List<GetAreaDto>>(areas);
            Log.Information("Areas retrieved successfully");
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
            await _areaRepository.UpdateArea(updateAreaDto);
            Log.Information("Area updated successfully with ID: {Id}", updateAreaDto.Id);
        }
    }
}
