using MobileTracking.Communication.ClientServices;
using MobileTracking.Core.Application;
using MobileTracking.Core.Models;
using MobileTracking.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileTracking.Pages.Locales
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddLocaleForm : ContentPage
    {
        private readonly LocaleProvider localeProvider;

        private readonly ILocaleService localesService;

        public AddLocaleForm(LocaleProvider localeProvider, ILocaleService localesService)
        {
            InitializeComponent();
            this.localeProvider = localeProvider;
            this.localesService = localesService;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (!activityIndicator.IsRunning)
            {
                activityIndicator.IsRunning = true;
                try
                {
                    var coordinates = await Xamarin.Essentials.Geolocation.GetLocationAsync();
                    var command = new CreateOrUpdateLocaleCommand()
                    {
                        Name = name.Text,
                        Description = description.Text,
                        Latitude = coordinates.Latitude,
                        Longitude = coordinates.Longitude
                    };
                    await localesService.CreateLocale(command);
                    await localeProvider.RefreshLocale();
                    await Navigation.PopAsync();
                }
                catch (Exception ex)
                {
                    await DisplayAlert(AppResources.Error, ex.Message, "OK");
                    activityIndicator.IsRunning = false;
                }
                finally
                {
                    activityIndicator.IsRunning = false;
                }
            }
        }
    }
}