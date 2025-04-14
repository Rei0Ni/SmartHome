using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Serilog;
using SmartHome.Application.Exceptions;
using SmartHome.Application.Interfaces.IPCameras;
using SmartHome.Domain.Entities;
using SmartHome.Dto.IPCamera;
using Log = Serilog.Log;

namespace SmartHome.Application.Services
{
    public class IPCameraService : IIPCamerasService
    {
        private readonly IIPCamerasRepository _camerasRepository;
        private readonly IMapper _mapper;
        private IValidator<CreateIPCameraDto> _createCameraValidator { get; set; }
        private IValidator<UpdateIPCameraDto> _updateCameraValidator { get; set; }

        //private readonly HttpClient _client;
        //private readonly IHttpContextAccessor _httpContextAccessor;

        public IPCameraService(
            IIPCamerasRepository camerasRepository, 
            //IHttpClientFactory clientFactory
            IMapper mapper,
            //IHttpContextAccessor httpContextAccessor
            IValidator<CreateIPCameraDto> createCameraValidator,
            IValidator<UpdateIPCameraDto> updateCameraValidator
            )
        {
            _camerasRepository = camerasRepository;
            _mapper = mapper;
            _createCameraValidator = createCameraValidator;
            _updateCameraValidator = updateCameraValidator;
            //_client = clientFactory.CreateClient("IPCameraClient");
            //_httpContextAccessor = httpContextAccessor;
        }

        public async Task<IPCameraDto> CreateCameraAsync(CreateIPCameraDto camera)
        {
            // Validate the camera data
            var validationResult = await _createCameraValidator.ValidateAsync(camera);
            if (!validationResult.IsValid)
            {
                throw new FluentValidationException("error", "Validation Error", validationResult.Errors);
            }

            var cameraEntity = _mapper.Map<IPCamera>(camera);
            var addedCamera = await _camerasRepository.CreateCameraAsync(cameraEntity);
            return _mapper.Map<IPCameraDto>(addedCamera);
        }

        public async Task<bool> CameraExistsAsync(Guid id)
        {
            return await _camerasRepository.CameraExistsAsync(id);
        }

        public async Task<bool> DeleteCameraAsync(Guid id)
        {
            return await _camerasRepository.DeleteCameraAsync(id);
        }

        public async Task<List<IPCameraDto>> GetAllCamerasAsync()
        {
            var cameras = await _camerasRepository.GetAllCamerasAsync();
            return _mapper.Map<List<IPCameraDto>>(cameras);
        }

        public async Task<IPCameraDto> GetCameraByIdAsync(Guid id)
        {
            var camera = await _camerasRepository.GetCameraByIdAsync(id);
            return _mapper.Map<IPCameraDto>(camera);
        }

        public async Task<string> GetCameraStreamUrl(Guid id)
        {
            var camera = await _camerasRepository.GetCameraByIdAsync(id);
            if (camera == null)
            {
                throw new Exception("Camera not found");
            }

            //var httpContext = _httpContextAccessor.HttpContext;
            //if (httpContext == null)
            //{
            //    throw new Exception("HttpContext is null");
            //}

            //HttpResponseMessage cameraResponse = await _client.GetAsync(camera.StreamUrl, HttpCompletionOption.ResponseHeadersRead, httpContext.RequestAborted);

            // Handle the stream response as needed
            //if (!cameraResponse.IsSuccessStatusCode)
            //{
            //    httpContext.Response.StatusCode = (int)cameraResponse.StatusCode;
            //    var errorContent = await cameraResponse.Content.ReadAsStringAsync(httpContext.RequestAborted);
            //    await httpContext.Response.WriteAsync($"Error fetching stream from camera ID {id}: {cameraResponse.ReasonPhrase} - {errorContent}");
            //    Log.Warning("Proxy request failed: Camera {CameraId} returned status {StatusCode} from {MjpegUrl}", id, cameraResponse.StatusCode, camera.StreamUrl);
            //    return;
            //}

            // 3. Copy headers (Content-Type is crucial)
            //httpContext.Response.Headers["Content-Type"] = cameraResponse.Content.Headers.ContentType?.ToString();
            //Log.Debug("Proxying stream for camera {CameraId} with Content-Type: {ContentType}", id, httpContext.Response.Headers["Content-Type"]);

            //// 4. Copy the stream content
            //using var cameraStream = await cameraResponse.Content.ReadAsStreamAsync(httpContext.RequestAborted);
            //await cameraStream.CopyToAsync(httpContext.Response.Body, httpContext.RequestAborted);
            //Log.Information("Proxy stream completed for camera {CameraId}.", id);

            return camera.StreamUrl;
        }

        public async Task<IPCameraDto> UpdateCameraAsync(UpdateIPCameraDto camera)
        {

            // Validate the camera data
            var validationResult = await _updateCameraValidator.ValidateAsync(camera);
            if (!validationResult.IsValid)
            {
                throw new FluentValidationException("error", "Validation Error", validationResult.Errors);
            }
            var cameraEntity = _mapper.Map<IPCamera>(camera);
            var updatedCamera = await _camerasRepository.UpdateCameraAsync(cameraEntity);
            return _mapper.Map<IPCameraDto>(updatedCamera);
        }

        public async Task<IPCameraConnectionInfoDto> GetCameraConnectionInfo(Guid id)
        {
            var camera = await _camerasRepository.GetCameraByIdAsync(id);
            if (camera == null)
            {
                throw new Exception("Camera not found");
            }
            var connectionInfo = _mapper.Map<IPCameraConnectionInfoDto>(camera);
            return connectionInfo;
        }
    }
}
