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
    public partial class AddPositionForm : ContentPage
    {
        private readonly LocaleProvider localeProvider;

        private readonly IPositionService positionsService;

        private readonly Zone zone;

        public AddPositionForm(LocaleProvider localeProvider, IPositionService positionsService, Zone zone)
        {
            InitializeComponent();
            this.localeProvider = localeProvider;
            this.positionsService = positionsService;
            this.zone = zone;
            this.BindingContext = zone;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (!activityIndicator.IsRunning)
            {
                var command = new CreateOrUpdatePositionCommand()
                {
                    ZoneId = zone.Id,
                    Name = name.Text                
                };
                try
                {
                    activityIndicator.IsRunning = true;
                    await positionsService.CreatePosition(command);
                    await localeProvider.RefreshLocale();
                    activityIndicator.IsRunning = false;
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