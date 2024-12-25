using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Application.Interfaces.MongoDBHealth.Repository;
using SmartHome.Application.Interfaces.MongoDBHealth.Service;
using MongoDB.Bson;

namespace SmartHome.Application.Services
{
    public class MongoDBHealth : IMongoDBHealth
    {
        public readonly IMongoDBHealthRepository MongoDBHealthRepository;

        public MongoDBHealth(IMongoDBHealthRepository healthRepository)
        {
            MongoDBHealthRepository = healthRepository;
        }

        public async Task<Dictionary<string, object>> Ping()
        {
            var result = await MongoDBHealthRepository.Ping();
            return result;
        }
    }
}
