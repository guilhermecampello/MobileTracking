using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.Application
{
    public interface ICalibrationService
    {
        Task<Calibration> FindCalibrationById(int calibrationId);

        Task<List<Calibration>> GetCalibrations(CalibrationsQuery query);

        Task<int> CreateCalibrations(CreateCalibrationsCommand command);

        Task<bool> DeleteCalibration(int calibrationId);
    }
}
