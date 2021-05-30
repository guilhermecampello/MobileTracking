using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MobileTracking.Services;
using MobileTracking.Core.Models;
using System.Collections.Generic;
using MobileTracking.Pages;

namespace MobileTracking
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private readonly LocaleProvider localeProvider;


        public MainPage(LocaleProvider localeProvider)
        {
            InitializeComponent();

            this.localeProvider = localeProvider;
            this.BindingContext = localeProvider.LocaleView;
        }

        private void Configuration_Clicked(object sender, EventArgs e)
        {
            var configurationsPage = Startup.ServiceProvider.GetService<ConfigurationPage>();
            Navigation.PushAsync(configurationsPage);
        }

        private async void Locale_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LocaleZonesPage(this.localeProvider));
        }

        private async void EstimateButton_Clicked(object sender, EventArgs e)
        {
            var locale = this.localeProvider.Locale;
            await Navigation.PushAsync(new PositionEstimationPage(locale!));
        }
    }
}
