using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using MobileTracking.Core.Interfaces;
using MobileTracking.Core.Models;
using MobileTracking.Core.Queries;
using WebApplication.Infrastructure;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("api/reports")]
    public class ReportsController : ControllerBase
    {
        [HttpGet]
        public void GetPrecisionReports(
            [FromServices] IReportsService reportsService,
            [FromQuery] GetPrecisionReportQuery query)
        {
            var report = reportsService.GetPrecisionReport(query);

            var csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
            };

            var formattedDate = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            var formattedFilename = $"PrecisionPerLocalization-{query.LocaleId}-N{query.Neighbours}-USW{query.UnmatchedSignalsWeight}-_{formattedDate}.csv";

            this.HttpContext.Response.StatusCode = 200;
            this.HttpContext.Response.Headers.Add("Content-Type", "text/csv");
            this.HttpContext.Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{formattedFilename}\"");

            var streamWriter = new StreamWriter(
                this.HttpContext.Response.BodyWriter.AsStream());

            using var csvWriter = new CsvWriter(streamWriter, csvConfiguration);

            csvWriter.WriteRecords(report);
        }

        [HttpGet("byPosition")]
        public void GetPrecisionReportByPosition(
            [FromServices] IReportsService reportsService,
            [FromQuery] GetPrecisionReportQuery query)
        {
            var report = reportsService.GetPrecisionReport(query).ToList()
                .GroupBy(data => new { X = data.RealX, Y = data.RealY })
                .Select(position => new
                {
                    position.Key.X,
                    position.Key.Y,
                    Error = position.Average(position => position.Error),
                    MinError = position.Min(position => position.Error),
                    MaxError = position.Max(position => position.Error)
                });

            var csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
            };

            var formattedDate = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            var formattedFilename = $"PrecisionPerPosition-{query.LocaleId}-N{query.Neighbours}-USW{query.UnmatchedSignalsWeight}-_{formattedDate}.csv";

            this.HttpContext.Response.StatusCode = 200;
            this.HttpContext.Response.Headers.Add("Content-Type", "text/csv");
            this.HttpContext.Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{formattedFilename}\"");

            var streamWriter = new StreamWriter(
                this.HttpContext.Response.BodyWriter.AsStream());

            using var csvWriter = new CsvWriter(streamWriter, csvConfiguration);

            csvWriter.WriteRecords(report);
        }

        [HttpGet("parameters")]
        public void AnalyzeParametersReport(
            [FromServices] IReportsService reportsService,
            [FromServices] DatabaseContext databaseContext,
            [FromQuery] GetParametersReportQuery query)
        {
            var report = this.EnumerateParametersReport(reportsService, databaseContext, query);

            var csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                IgnoreReferences = true
            };

            var formattedDate = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            var formattedFilename = $"ParametersAnalysis-{query.LocaleId}-_{formattedDate}.csv";

            this.HttpContext.Response.StatusCode = 200;
            this.HttpContext.Response.Headers.Add("Content-Type", "text/csv");
            this.HttpContext.Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{formattedFilename}\"");

            var streamWriter = new StreamWriter(
                this.HttpContext.Response.BodyWriter.AsStream());

            using var csvWriter = new CsvWriter(streamWriter, csvConfiguration);

            csvWriter.WriteRecords(report);

            var bestParameters = databaseContext.LocaleParameters
                .OrderBy(parameters => parameters.Missings)
                .ThenBy(parameter => parameter.MeanError)
                .FirstOrDefault(param => param.LocaleId == query.LocaleId && param.CreatedAt > DateTime.Now.AddDays(-30));

            if (bestParameters != null && query.ReplaceActiveParameters)
            {
                var oldActiveParameters = databaseContext.LocaleParameters.Where(param => param.LocaleId == query.LocaleId && param.IsActive).ToList();
                if (oldActiveParameters != null)
                {
                    oldActiveParameters.ForEach(param => param.IsActive = false);
                }

                bestParameters.IsActive = true;
                databaseContext.SaveChanges();
            }
        }

        private IEnumerable<LocaleParameters> EnumerateParametersReport(
           IReportsService reportsService,
           DatabaseContext databaseContext,
           GetParametersReportQuery query)
        {
            for (int neighbours = query.MinNeighbours; neighbours <= query.MaxNeighbours; neighbours += query.NeighboursStep)
            {
                for (double wifiWeight = query.MinWifiWeight; wifiWeight <= query.MaxWifiWeight; wifiWeight += query.WifiWeightStep)
                {
                    for (double bleWeight = query.MinBleWeight; bleWeight <= query.MaxBleWeight; bleWeight += query.BleWeightStep)
                    {
                        for (double magnetometerWeight = query.MinMagnetometerWeight; magnetometerWeight <= query.MaxMagnetometerWeight; magnetometerWeight += query.MagnetometerWeightStep)
                        {
                            for (double unmatchedSignalsWeight = query.MinUnmatchedSignalsWeight; unmatchedSignalsWeight <= query.MaxUnmatchedSignalsWeight; unmatchedSignalsWeight += query.UnmatchedSignalsWeightStep)
                            {
                                for (double standardDeviationFactor = query.MinStandardDeviationFactor; standardDeviationFactor <= query.MaxStandardDeviationFactor; standardDeviationFactor += query.StandardDeviationFactorStep)
                                {
                                    var command = new GetPrecisionReportQuery();
                                    command.LocaleId = query.LocaleId;
                                    command.UnmatchedSignalsWeight = unmatchedSignalsWeight;
                                    command.BleWeight = bleWeight;
                                    command.WifiWeight = wifiWeight;
                                    command.Neighbours = neighbours;
                                    command.MagnetometerWeight = magnetometerWeight;
                                    command.UseDistance = query.UseDistance;
                                    command.StandardDeviationFactor = standardDeviationFactor;
                                    var data = reportsService.GetPrecisionReport(command);
                                    var error = 0.0;
                                    var n = 0;
                                    var missings = 0;
                                    foreach (var estimation in data)
                                    {
                                        if (estimation.CalculatedX != 0 && estimation.CalculatedY != 0)
                                        {
                                            error += estimation.Error;
                                            n++;
                                        }
                                        else
                                        {
                                            missings++;
                                        }
                                    }

                                    var parametersAnalysis = new LocaleParameters()
                                    {
                                        LocaleId = command.LocaleId,
                                        BleWeight = command.BleWeight,
                                        WifiWeight = command.WifiWeight,
                                        MagnetometerWeight = command.MagnetometerWeight,
                                        MeanError = error / n,
                                        Neighbours = command.Neighbours,
                                        UnmatchedSignalsWeight = command.UnmatchedSignalsWeight,
                                        StandardDeviationFactor = command.StandardDeviationFactor,
                                        Missings = missings,
                                        UseDistance = command.UseDistance
                                    };

                                    databaseContext.Add(parametersAnalysis);
                                    Console.WriteLine(JsonSerializer.Serialize(parametersAnalysis));

                                    yield return parametersAnalysis;
                                }
                            }
                        }
                    }
                }

                databaseContext.SaveChanges();
            }
        }
    }
}
