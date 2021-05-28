using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileTracking
{
    public partial class App : Application
    {
        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NDUyMjg4QDMxMzkyZTMxMmUzME40OVZpZVoyVXZCa1c3SnF1V1J5Q0l6Qk8zSGNzWEoweUl6Q2ZkTloxYVk9");
            InitializeComponent();
            Startup.ConfigureServices();
            MainPage = new NavigationPage(Startup.ServiceProvider.GetService<MainPage>());
        }

        protected override void OnStart()
        {
        }
            

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
