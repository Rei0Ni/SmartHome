using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace SmartHome.Application.Interfaces.MongoDBHealth.Service
{
    public interface IMongoDBHealth
    {
        Task<Dictionary<string, object>> Ping();
    }
}
