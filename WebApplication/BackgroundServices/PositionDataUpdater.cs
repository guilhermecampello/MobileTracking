using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MobileTracking.Core.Application;

namespace WebApplication.BackgroundServices
{
    public class PositionDataUpdater : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;

        public PositionDataUpdater(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = this.serviceProvider.CreateScope())
                {
                    var positionDataService = scope.ServiceProvider.GetRequiredService<IPositionDataService>();
                    var query = new PositionDataQuery()
                    {
                        NeedsUpdate = true
                    };
                    await positionDataService.RecalculatePositionData(query);
                    await Task.Delay(30000);
                }
            }
        }
    }
}
