using MongoDB.Bson;
using MongoDB.Driver;
using SmartHome.Application.Interfaces.MongoDBHealth.Repository;
using SmartHome.Domain.Contexts;

namespace SmartHome.Infrastructure.Repositories
{
    public class MongoDBHealthRepository : IMongoDBHealthRepository
    {
        private readonly ApplicationDBContext _context;

        public MongoDBHealthRepository(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<Dictionary<string, object>> Ping()
        {
            var result = await _context.MongoDB().RunCommandAsync((Command<BsonDocument>)"{ping:1}");
            return result.ToDictionary();
        }
    }
}
