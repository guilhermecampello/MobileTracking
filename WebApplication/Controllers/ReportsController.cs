using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using MobileTracking.Core.Commands;
using MobileTracking.Core.Interfaces;
using MobileTracking.Core.Models;
using MobileTracking.Core.Queries;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("api/reports")]
    public class ReportsController : ControllerBase
    {
        [HttpPost]
        public void GetPrecisionReports(
            [FromServices] IReportsService reportsService,
            [FromQuery] GetPrecisionReportQuery query)
        {
            var report = reportsService.GetPrecisionReport(query);

            var csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                IgnoreReferences = true
            };

            var formattedDate = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            var formattedFilename = $"PrecisionReport-{query.LocaleId}-N{query.Neighbours}-IND{query.IgnoreNotDetectedSignals}-_{formattedDate}.csv";

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
