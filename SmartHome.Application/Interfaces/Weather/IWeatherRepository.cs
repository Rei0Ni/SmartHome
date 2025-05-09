using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Domain.Entities;

namespace SmartHome.Application.Interfaces.Weather
{
    public interface IWeatherRepository
    {
        Task<Domain.Entities.Weather> GetLatestWeatherAsync();
        Task SaveOrUpdateWeatherAsync(Domain.Entities.Weather weather);
    }
}
