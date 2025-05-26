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

        public IMongoCollection<Area> Areas => 
                _database.GetCollection<Area>("Areas");
        public IMongoCollection<UserAreas> UserAreas =>
                _database.GetCollection<UserAreas>("UserAreas");

        public IMongoCollection<Controller> Controllers =>
                _database.GetCollection<Controller>("Controllers");

        public IMongoCollection<DeviceType> DeviceTypes =>
                _database.GetCollection<DeviceType>("DeviceTypes");

        public IMongoCollection<Device> Devices =>
                _database.GetCollection<Device>("Devices");

        public IMongoCollection<IPCamera> IPCameras =>
                _database.GetCollection<IPCamera>("IPCameras");

        public IMongoCollection<Weather> Weather =>
                _database.GetCollection<Weather>("Weather");
        public IMongoCollection<Log> Logs =>
                _database.GetCollection<Log>("Logs");

        public IMongoCollection<Setting> Settings =>
                _database.GetCollection<Setting>("Settings");

        public IMongoCollection<EmailSetting> EmailSettings =>
                _database.GetCollection<EmailSetting>("EmailSettings");
    }
}
