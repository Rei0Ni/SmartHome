using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using SmartHome.Application.Interfaces.IPCameras;
using SmartHome.Domain.Contexts;
using SmartHome.Domain.Entities;

namespace SmartHome.Infrastructure.Repositories
{
    public class IPCameraRepository : IIPCamerasRepository
    {
        private readonly ApplicationDBContext _context;

        public IPCameraRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IPCamera> CreateCameraAsync(IPCamera camera)
        {
            await _context.IPCameras.InsertOneAsync(camera);
            return camera;
        }

        public async Task<bool> CameraExistsAsync(Guid id)
        {
            var filter = Builders<IPCamera>.Filter.Eq(c => c.Id, id);
            var count = await _context.IPCameras.CountDocumentsAsync(filter);
            return count > 0;
        }

        public async Task<bool> DeleteCameraAsync(Guid id)
        {
            var filter = Builders<IPCamera>.Filter.Eq(c => c.Id, id);
            var result = await _context.IPCameras.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }

        public async Task<List<IPCamera>> GetAllCamerasAsync()
        {
            return await _context.IPCameras.Find(_ => true).ToListAsync();
        }

        public async Task<IPCamera> GetCameraByIdAsync(Guid id)
        {
            var filter = Builders<IPCamera>.Filter.Eq(c => c.Id, id);
            return await _context.IPCameras.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<string> GetCameraStreamUrl(Guid id)
        {
            var filter = Builders<IPCamera>.Filter.Eq(c => c.Id, id);
            var camera = await _context.IPCameras.Find(filter).FirstOrDefaultAsync();
            if (camera == null)
            {
                throw new KeyNotFoundException($"Camera with id {id} not found.");
            }
            return camera.StreamUrl;
        }

        public async Task<IPCamera> UpdateCameraAsync(IPCamera camera)
        {
            var filter = Builders<IPCamera>.Filter.Eq(c => c.Id, camera.Id);
            await _context.IPCameras.ReplaceOneAsync(filter, camera);
            return camera;
        }

        public async Task<List<IPCamera>> GetCamerasForAreas(List<Guid> areaIds)
        {
            return await _context.IPCameras
                            .Find(d => areaIds.Contains(d.AreaId))
                            .ToListAsync();
        }
    }
}
