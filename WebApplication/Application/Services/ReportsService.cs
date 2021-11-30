﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MobileTracking.Core.Application;
using MobileTracking.Core.Commands;
using MobileTracking.Core.Interfaces;
using MobileTracking.Core.Models;
using MobileTracking.Core.Queries;
using WebApplication.Infrastructure;

namespace WebApplication.Application.Services
{
    public class ReportsService : IReportsService
    {
        private readonly IPositionEstimationService positionEstimationService;

        private readonly DatabaseContext databaseContext;

        public ReportsService(IPositionEstimationService positionEstimationService, DatabaseContext databaseContext)
        {
            this.positionEstimationService = positionEstimationService;
            this.databaseContext = databaseContext;
        }

        public IEnumerable<PrecisionCalculation> GetPrecisionReport(GetPrecisionReportQuery query)
        {
            var userLocalizations = this.databaseContext.UserLocalizations
                .Where(query.LocaleId, localeId => localization => localization.LocaleId == localeId)
                .Where(query.AfterDate, afterDate => localization => localization.DateTime > afterDate)
                .Where(query.BeforeDate, beforeDate => localization => localization.DateTime < beforeDate)
                .Include(userLocalization => userLocalization.LocalizationMeasurements)
                .ToList();

            foreach (var localization in userLocalizations)
            {
                var command = new EstimatePositionCommand()
                {
                    UseDistance = query.UseDistance,
                    LocaleId = localization.LocaleId,
                    Measurements = localization.LocalizationMeasurements!.Select(measurement => new Measurement(measurement)).ToList(),
                    BleWeight = query.BleWeight,
                    WifiWeight = query.WifiWeight,
                    MagnetometerWeight = query.MagnetometerWeight,
                    UnmatchedSignalsWeight = query.UnmatchedSignalsWeight,
                    StandardDeviationFactor = query.StandardDeviationFactor,
                    Neighbours = query.Neighbours
                };

                var calculatedPosition = this.positionEstimationService.EstimatePosition(command).Result;

                yield return new PrecisionCalculation(localization, calculatedPosition);
            }
        }
    }
}