using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using MobileTracking.Core.Commands;
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
            var report = new List<LocaleParameters>();

            for (int neighbours = query.MinNeighbours; neighbours <= query.MaxNeighbours; neighbours += query.NeighboursStep)
            {
                for (double wifiWeight = query.MinWifiWeight; wifiWeight <= query.MaxWifiWeight; wifiWeight += query.WifiWeightStep)
                {
                    for (double bleWeight = query.MinBleWeight; bleWeight <= query.MaxWifiWeight; bleWeight += query.BleWeightStep)
                    {
                        for (double magnetometerWeight = query.MinMagnetometerWeight; magnetometerWeight <= query.MaxMagnetometerWeight; magnetometerWeight += query.MagnetometerWeightStep)
                        {
                            for (double unmatchedSignalsWeight = query.MinUnmatchedSignalsWeight; unmatchedSignalsWeight <= query.MaxUnmatchedSignalsWeight; unmatchedSignalsWeight += query.UnmatchedSignalsWeightStep)
                            {
                                var command = new GetPrecisionReportQuery();
                                command.LocaleId = query.LocaleId;
                                command.UnmatchedSignalsWeight = unmatchedSignalsWeight;
                                command.BleWeight = bleWeight;
                                command.WifiWeight = wifiWeight;
                                command.Neighbours = neighbours;
                                command.MagnetometerWeight = magnetometerWeight;
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
                                    Missings = missings
                                };
                                report.Add(parametersAnalysis);

                                databaseContext.Add(parametersAnalysis);
                                Console.WriteLine(JsonSerializer.Serialize(parametersAnalysis));
                            }
                        }
                    }
                }

                databaseContext.SaveChanges();
            }

            report.OrderBy(parameter => parameter.MeanError);

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
        }
    }
}
