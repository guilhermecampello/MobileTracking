﻿using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileTracking
{
    public partial class App : Application
    {
        public App()
        {
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
