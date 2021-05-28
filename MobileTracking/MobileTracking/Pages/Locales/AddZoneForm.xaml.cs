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
    public partial class AddZoneForm : ContentPage
    {
        private readonly LocaleProvider localeProvider;

        private readonly IZoneService zonesService;

        public AddZoneForm(LocaleProvider localeProvider, IZoneService zonesService)
        {
            InitializeComponent();
            this.localeProvider = localeProvider;
            this.zonesService = zonesService;
            this.BindingContext = localeProvider.Locale;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            var command = new CreateOrUpdateZoneCommand()
            {
                LocaleId = localeProvider.Locale!.Id,
                Name = name.Text,
                Description = description.Text,
                Floor = Convert.ToInt32(floor.Text)                
            };
            try
            {
                activityIndicator.IsRunning = true;
                await zonesService.CreateZone(command);
                await localeProvider.RefreshLocale();
                activityIndicator.IsRunning = false;
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert(AppResources.Error, ex.Message, "OK");
                activityIndicator.IsRunning = false;
            }
        }
    }
}