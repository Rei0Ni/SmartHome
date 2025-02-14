using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using SmartHome.Application.Interfaces.Controller;
using SmartHome.Domain.Contexts;
using SmartHome.Domain.Entities;
using SmartHome.Dto.Area;

namespace SmartHome.Infrastructure.Repositories
{
    public class ControllerRepository : IControllerRepository
    {
        public ApplicationDBContext _context { get; set; }
        public ControllerRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task CreateController(Controller createControllerDto)
        {
            await _context.Controllers.InsertOneAsync(createControllerDto);
        }

        public async Task DeleteController(Controller deleteController)
        {
            var filter = Builders<Controller>.Filter.Eq(x => x.Id, deleteController.Id);
            await _context.Controllers.DeleteOneAsync(filter);
        }

        public async Task<Controller> GetController(Guid id)
        {
            return await _context.Controllers.Find(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Controller>> GetControllers()
        {
            return await _context.Controllers.Find(_ => true).ToListAsync();
        }

        public async Task UpdateController(Controller updateControllerDto)
        {
            await _context.Controllers.ReplaceOneAsync(c => c.Id == updateControllerDto.Id, updateControllerDto);
        }
    }
}
