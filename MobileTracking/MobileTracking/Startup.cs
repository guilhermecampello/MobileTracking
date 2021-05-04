using Microsoft.Extensions.DependencyInjection;
using MobileTracking.Communication;
using MobileTracking.Communication.Services;
using MobileTracking.Services.MagneticField;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MobileTracking
{
    public static class Startup
    {
        public static IServiceProvider ServiceProvider { get; set; } = null!;

        public static IServiceProvider ConfigureServices()
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton<Client>()
                .AddSingleton<CalibrationsService>()
                .AddSingleton(DependencyService.Get<IWifiConnector>())
                .AddSingleton(DependencyService.Get<IBluetoothConnector>())
                .AddSingleton<MagneticFieldSensor>()
                .AddSingleton<MainPage>()
                .BuildServiceProvider();

            ServiceProvider = serviceProvider;

            return serviceProvider;
        }
    }
}
