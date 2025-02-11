using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using SmartHome.Application.Interfaces.Health;
using Microsoft.Extensions.Configuration;
using SmartHome.Dto;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;
using Serilog;

namespace SmartHome.Application.Services
{
    public class MongodbHealthCheck : IComponentHealthCheck
    {
        private readonly IConfiguration _configurations;

        public MongodbHealthCheck(IConfiguration configurations)
        {
            _configurations = configurations;
        }

        public async Task<ComponentHealthCheckDto> CheckHealthAsync(CancellationToken cancellationToken)
        {
            try
            {
                var result = await IsMongoHealthy();
                return new ComponentHealthCheckDto()
                {
                    Component = "Mongodb",
                    Status = HealthStatus.Healthy.ToString(),
                    Details = result
                };
            }
            catch (Exception ex)
            {
                Log.Error(this.GetType().Name + " : " + ex.Message);
                return new ComponentHealthCheckDto()
                {
                    Component = "Mongodb",
                    Status = HealthStatus.Unhealthy.ToString(),
                    Details = new Dictionary<string, object>
                    {
                        {"Err",ex.Message}
                    }
                };
            }
        }

        private async Task<Dictionary<string, object>> IsMongoHealthy()
        {
            string connectionString = _configurations["MongoDBConfig:ConnectionURI"]!;
            MongoUrl url = new MongoUrl(connectionString);

            IMongoDatabase dbInstance = new MongoClient(url)
                .GetDatabase(_configurations["MongoDBConfig:DatabaseName"])
                .WithReadPreference(new ReadPreference(ReadPreferenceMode.Secondary));

            // ping the database server
            var result = await dbInstance.RunCommandAsync<BsonDocument>(new BsonDocument { { "ping", 1 } });
            return result.ToDictionary();
        }
    }
}
