using System.Collections.Generic;
using System.Threading.Tasks;
using MobileTracking.Core.Models;

namespace MobileTracking.Core.Application
{
    public interface IPositionSignalDataService
    {
        Task<List<PositionSignalData>> GetPositionSignalDatas(PositionSignalDataQuery query);

        Task<bool> RecalculatePositionSignalData(PositionSignalDataQuery query);
    }
}
