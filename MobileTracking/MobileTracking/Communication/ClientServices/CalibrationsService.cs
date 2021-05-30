using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MobileTracking.Core.Application;
using MobileTracking.Core.Models;

namespace MobileTracking.Communication.Services
{
    class CalibrationsService : ICalibrationService
    {
        private readonly Client client;

        private string calibrationsController = "calibrations";

        public CalibrationsService(Client client)
        {
            this.client = client;
        }

        public Task<int> CreateCalibrations(CreateCalibrationsCommand command)
        {
            return client.Post<int>(calibrationsController, command);
        }

        public Task<bool> DeleteCalibrations(CalibrationsQuery query)
        {
            return client.Delete<bool>(calibrationsController, string.Empty, query);
        }

        public Task<Calibration> FindCalibrationById(int calibrationId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Calibration>> GetCalibrations(CalibrationsQuery query)
        {
            return client.Get<List<Calibration>>(calibrationsController, query);
        }
    }
}
