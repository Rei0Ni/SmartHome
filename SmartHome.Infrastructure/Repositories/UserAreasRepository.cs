using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using SmartHome.Application.Interfaces.UserAreas;
using SmartHome.Domain.Contexts;
using SmartHome.Domain.Entities;

namespace SmartHome.Infrastructure.Repositories
{
    public class UserAreasRepository : IUserAreasRepository
    {
        private readonly ApplicationDBContext _context;

        public UserAreasRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task AddUserAreasAsync(UserAreas userArea)
        {
            await _context.UserAreas.InsertOneAsync(userArea);
        }

        public async Task DeleteUserAreasAsync(Guid id)
        {
            var filter = Builders<UserAreas>.Filter.Eq(ua => ua.UserId, id);
            await _context.UserAreas.DeleteOneAsync(filter);
        }

        public async Task<UserAreas> GetUserAreasByIdAsync(Guid id)
        {
            var filter = Builders<UserAreas>.Filter.Eq(ua => ua.UserId, id);
            var userArea = await _context.UserAreas.Find(filter).FirstOrDefaultAsync();
            return userArea;
        }

        public async Task UpdateUserAreasAsync(UserAreas userArea)
        {
            var filter = Builders<UserAreas>.Filter.Eq(ua => ua.UserId, userArea.UserId);
            var update = Builders<UserAreas>.Update.Set(ua => ua.AllowedAreaIds, userArea.AllowedAreaIds);
            await _context.UserAreas.UpdateOneAsync(filter, update);
        }
    }
}
