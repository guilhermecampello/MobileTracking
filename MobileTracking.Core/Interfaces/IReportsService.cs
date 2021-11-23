using System;
using MobileTracking.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using MobileTracking.Core.Queries;

namespace MobileTracking.Core.Interfaces
{
    public interface IReportsService
    {
        public IEnumerable<PrecisionCalculation> GetPrecisionReport(GetPrecisionReportQuery query);
    }
}
