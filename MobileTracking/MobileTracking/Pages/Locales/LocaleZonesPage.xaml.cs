using MobileTracking.Communication.ClientServices;
using MobileTracking.Core.Application;
using MobileTracking.Core.Models;
using MobileTracking.Pages;
using MobileTracking.Pages.Locales;
using MobileTracking.Services;
using Plugin.Toast;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileTracking
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LocaleZonesPage : ContentPage
    {
        private readonly LocaleProvider localeProvider;

        public ObservableCollection<Zone> Zones { get; set; }

        public LocaleZonesPage(LocaleProvider localeProvider)
        {
            this.localeProvider = localeProvider;
            this.BindingContext = this.localeProvider.Locale;
            InitializeComponent();
            
            Zones = new ObservableCollection<Zone>(this.localeProvider.Locale!.Zones);
            MyListView.ItemsSource = Zones;
            MyListView.RefreshCommand = RefreshZones_Command;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            RefreshZones();
        }

        public ICommand RefreshZones_Command
        {
            get => new Command(() =>
            {
                RefreshZones();
            });
        }

        private void RefreshZones()
        {
            if (this.localeProvider.Locale!.Zones?.Count == 0)
            {
                MyListView.Header = new Label()
                {
                    Text = AppResources.No_zones_message,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 14
                };
            }
            else
            {
                MyListView.Header = null;
            }
            MyListView.IsRefreshing = true;
            Device.BeginInvokeOnMainThread(() =>
            {
                Zones.Clear();
                localeProvider.Locale?.Zones?.ForEach(zone => Zones.Add(zone));
            });
            MyListView.IsRefreshing = false;
        }

        private async void ToolbarAddItem_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new AddZoneForm(localeProvider, Startup.ServiceProvider.GetService<IZoneService>()));
        }

        private async void Button_Clicked(object sender, System.EventArgs e)
        {
            var zone = (Zone)((Button)sender).BindingContext;
            await Navigation.PushAsync(new AddPositionForm(localeProvider, Startup.ServiceProvider.GetService<IPositionService>(), zone));
        }

        private async void Zone_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var zone = (Zone)e.Item;
            var action = await DisplayActionSheet(zone.Name, AppResources.Back, string.Empty, AppResources.Delete_zone.ToUpper(), AppResources.View_data.ToUpper());
            if (action == AppResources.Delete_zone.ToUpper())
            {
                var deleteConfirmation = await DisplayAlert(AppResources.Delete_zone, $"{AppResources.Confirm_delete_zone}  {zone.Name} ?", AppResources.Delete, AppResources.Cancel);
                if (deleteConfirmation)
                {
                    var zonesService = Startup.ServiceProvider.GetService<IZoneService>();
                    MyListView.IsRefreshing = true;
                    if (await zonesService.DeleteZone(zone.Id))
                    {
                        await localeProvider.RefreshLocale();
                        CrossToastPopUp.Current.ShowToastError($"{AppResources.Zone} {zone.Name} {AppResources.Deleted.ToLower()}");
                    }
                    RefreshZones();
                }
            }
        }

        private async void Position_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var position = (Position)e.Item;
            var query = new PositionQuery()
            {
                IncludeZone = true,
                IncludeData = true
            };
            var positionsService = Startup.ServiceProvider.GetService<IPositionService>();
            position = await positionsService.FindPositionById(position.Id, query);
            await Navigation.PushAsync(new PositionPage(position));
        }

        private async void ExploreDataButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                var localeService = Startup.ServiceProvider.GetService<ILocaleService>();
                var query = new LocaleQuery() { IncludeZones = true, IncludePositions = true, IncludePositionsData = true };
                var locale = await localeService.FindLocaleById(localeProvider.Locale!.Id, query);
                await Navigation.PushAsync(new LocaleExplorerPage(locale));
            }
            catch (Exception ex)
            {
                await DisplayAlert(ex.Message, ex.InnerException.Message, "OK");
            }
        }
    }
}