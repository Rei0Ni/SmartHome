using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using SmartHome.Domain.Entities;

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

        public IMongoCollection<ApplicationUser> Users =>
                _database.GetCollection<ApplicationUser>("Users");
        public IMongoCollection<ApplicationRole> Roles =>
                _database.GetCollection<ApplicationRole>("Roles");
    }
}
