using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace SmartHome.Application.Interfaces.MongoDBHealth.Repository
{
    public interface IMongoDBHealthRepository
    {
        Task<Dictionary<string, object>> Ping();
    }
}
