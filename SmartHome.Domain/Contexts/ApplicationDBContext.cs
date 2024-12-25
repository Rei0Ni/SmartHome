using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace SmartHome.Domain.Contexts
{
    public class ApplicationDBContext
    {
        private readonly IMongoDatabase _database;

        public ApplicationDBContext(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoDatabase MongoDB()
        {
            return _database;
        }
    }
}
