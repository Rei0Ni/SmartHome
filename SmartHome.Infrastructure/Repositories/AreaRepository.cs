using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using SmartHome.Dto.Area;
using SmartHome.Application.Interfaces.Area;
using SmartHome.Domain.Contexts;
using SmartHome.Domain.Entities;

namespace SmartHome.Infrastructure.Repositories
{
    public class AreaRepository : IAreaRepository
    {
        public ApplicationDBContext _context { get; set; }
        public AreaRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task CreateArea(Area Area)
        {
            await _context.Areas.InsertOneAsync(Area);
        }

        public async Task DeleteArea(DeleteAreaDto deleteArea)
        {
            var filter = Builders<Area>.Filter.Eq(x => x.Id, deleteArea.Id);
            await _context.Areas.DeleteOneAsync(filter);
        }

        public async Task<Area> GetArea(Guid id)
        {
            var Area = await _context.Areas.Find(x => x.Id == id).FirstOrDefaultAsync();
            return Area;
        }

        public async Task<List<Area>> GetAreas()
        {
            var Areas = await _context.Areas.Find(x => true).ToListAsync();
            return Areas;
        }

        public async Task UpdateArea(UpdateAreaDto updateAreaDto)
        {
            var filter = Builders<Area>.Filter.Eq(x => x.Id, updateAreaDto.Id);
            var update = Builders<Area>.Update.Set(x => x.Name, updateAreaDto.Name);
            await _context.Areas.UpdateOneAsync(filter, update);
        }
    }
}
