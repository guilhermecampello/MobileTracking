using System.Collections.Generic;
using System.Threading.Tasks;
using MobileTracking.Core.Models;

namespace MobileTracking.Core.Application
{
    public interface IPositionDataService
    {
        Task<List<PositionData>> GetPositionDatas(PositionDataQuery query);

        Task<bool> RecalculatePositionData(PositionDataQuery query);
    }
}
