using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MobileTracking.Core.Application;

namespace WebApplication.BackgroundServices
{
    public class PositionSignalDataUpdater : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;

        public PositionSignalDataUpdater(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = this.serviceProvider.CreateScope())
                {
                    var positionSignalDataService = scope.ServiceProvider.GetRequiredService<IPositionSignalDataService>();
                    var query = new PositionSignalDataQuery()
                    {
                        NeedsUpdate = true
                    };
                    await positionSignalDataService.RecalculatePositionSignalData(query);
                    await Task.Delay(300000);
                }
            }
        }
    }
}
