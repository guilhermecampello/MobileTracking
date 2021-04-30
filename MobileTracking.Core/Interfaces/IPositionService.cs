using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.Application
{
    public interface IPositionService
    {
        Task<Position> FindPositionById(int zoneId, PositionQuery query = null);

        Task<List<Position>> GetPositions(PositionQuery query);

        Task<Position> CreatePosition(CreateOrUpdatePositionCommand command);

        Task<Position> UpdatePosition(int positionId, CreateOrUpdatePositionCommand command);

        Task<bool> DeletePosition(int positionId);
    }
}
