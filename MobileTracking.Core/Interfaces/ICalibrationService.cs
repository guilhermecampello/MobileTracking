using System.Collections.Generic;
using System.Threading.Tasks;
using MobileTracking.Core.Models;

namespace MobileTracking.Core.Application
{
    public interface ICalibrationService
    {
        Task<Calibration> FindCalibrationById(int calibrationId);

        Task<List<Calibration>> GetCalibrations(CalibrationsQuery query);

        Task<int> CreateCalibrations(CreateCalibrationsCommand command);

        Task<bool> DeleteCalibrations(CalibrationsQuery query);
    }
}
