using Microsoft.Extensions.DependencyInjection;
using MobileTracking.Communication;
using MobileTracking.Communication.ClientServices;
using MobileTracking.Communication.Services;
using MobileTracking.Core.Application;
using MobileTracking.Core.Models;
using MobileTracking.Services;
using MobileTracking.Services.MagneticField;
using System;
using Xamarin.Forms;

namespace MobileTracking
{
    public static class Startup
    {
        public static IServiceProvider ServiceProvider { get; set; } = null!;

        public static IServiceProvider ConfigureServices()
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton<Configuration>()
                .AddSingleton<Client>()
                .AddSingleton<ILocaleService, LocalesService>()
                .AddSingleton<IZoneService, ZonesService>()
                .AddSingleton<IPositionService, PositionsService>()
                .AddSingleton<ICalibrationService, CalibrationsService>()
                .AddSingleton<IPositionDataService, PositionDatasService>()
                .AddSingleton(DependencyService.Get<IWifiConnector>())
                .AddSingleton(DependencyService.Get<IBluetoothConnector>())
                .AddSingleton<MagneticFieldSensor>()
                // PAGES
                .AddSingleton<MainPage>()
                .AddSingleton<ConfigurationPage>()
                .AddSingleton<LocalesPage>()
                // PROVIDERS
                .AddSingleton<LocaleProvider>()
                .BuildServiceProvider();

            ServiceProvider = serviceProvider;

            return serviceProvider;
        }
    }
}
