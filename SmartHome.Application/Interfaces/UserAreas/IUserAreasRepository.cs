using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Domain.Entities;

namespace SmartHome.Application.Interfaces.UserAreas
{
    public interface IUserAreasRepository
    {
        Task<Domain.Entities.UserAreas> GetUserAreasByIdAsync(Guid id);
        Task AddUserAreasAsync(Domain.Entities.UserAreas userArea);
        Task UpdateUserAreasAsync(Domain.Entities.UserAreas userArea);
        Task DeleteUserAreasAsync(Guid id);
    }
}
