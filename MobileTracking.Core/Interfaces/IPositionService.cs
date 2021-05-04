using System.Collections.Generic;
using System.Threading.Tasks;
using MobileTracking.Core.Models;

namespace MobileTracking.Core.Application
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
