using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using SmartHome.Application.Interfaces.Weather;
using SmartHome.Domain.Contexts;

namespace SmartHome.Infrastructure.Repositories
{
    public class WeatherRepository : IWeatherRepository
    {
        private readonly ApplicationDBContext _context;
        public WeatherRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Domain.Entities.Weather> GetLatestWeatherAsync()
        {
            return await _context.Weather.Find(_ => true).SortByDescending(c => c.Timestamp).FirstOrDefaultAsync();

        }
        public async Task SaveOrUpdateWeatherAsync(Domain.Entities.Weather weather)
        {
            await _context.Weather.DeleteManyAsync(_ => true); // Delete all existing
            await _context.Weather.InsertOneAsync(weather); // Insert the new one
        }
    }
}
