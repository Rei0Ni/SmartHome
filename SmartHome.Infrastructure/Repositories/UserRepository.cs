using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using Serilog;
using SmartHome.Application.Interfaces.User;
using SmartHome.Domain.Contexts;
using SmartHome.Domain.Entities;
using Log = Serilog.Log;

namespace SmartHome.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDBContext _context;

        public UserRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<bool> UpdateLastLoginValue(ApplicationUser user)
        {
            try
            {
                var filter = Builders<ApplicationUser>.Filter.Eq(u => u.Id, user.Id);
                var update = Builders<ApplicationUser>.Update.Set(u => u.LastLogin, DateTime.UtcNow);

                var result = await _context.Users.UpdateOneAsync(filter, update);

                return result.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                // Log exception or handle accordingly
                Log.Error("Couldn't Update a User's LastLogin Proberty", ex.Message);
                return false;
            }
        }
    }
}
