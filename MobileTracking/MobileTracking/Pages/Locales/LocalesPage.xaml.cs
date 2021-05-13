using MobileTracking.Communication.ClientServices;
using MobileTracking.Core.Application;
using MobileTracking.Core.Models;
using MobileTracking.Pages.Locales;
using MobileTracking.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileTracking
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LocalesPage : ContentPage
    {
        private readonly LocaleProvider localeProvider;

        public ObservableCollection<LocaleView> Locales { get; set; }

        public LocalesPage(LocaleProvider localeProvider)
        {
            InitializeComponent();

            this.localeProvider = localeProvider;
            Locales = new ObservableCollection<LocaleView>(localeProvider.ClosestLocales
                .Select(locale => new LocaleView(locale, locale.Id == localeProvider.Locale?.Id)));
            MyListView.ItemsSource = Locales;
            MyListView.RefreshCommand = RefreshLocales_Command;
        }

        public ICommand RefreshLocales_Command
        {
            get => new Command(async () =>
                    {
                        await RefreshLocales();
                    });
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await RefreshLocales();
        }

        private async Task RefreshLocales()
        {
            MyListView.IsRefreshing = true;
            var locales = await this.localeProvider.TryGetLocalesByCoordinates();
            Device.BeginInvokeOnMainThread(() =>
            {
                Locales.Clear();
                locales.ForEach(locale => Locales.Add(new LocaleView(locale, locale.Id == localeProvider.Locale?.Id)));
            });
            MyListView.IsRefreshing = false;
        }

        private async void ToolbarAddItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddLocaleForm(localeProvider, Startup.ServiceProvider.GetService<ILocaleService>()));
            await RefreshLocales();
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;
            var selectedLocale = (Locale)e.Item;
            localeProvider.Locale = selectedLocale;
            Device.BeginInvokeOnMainThread(() =>
            {
                Locales.FirstOrDefault(locale => locale.IsSelected).IsSelected = false;
                Locales.FirstOrDefault(locale => locale.Id == selectedLocale.Id).IsSelected = true;
                
            });
            await Navigation.PushAsync(new LocaleZonesPage(this.localeProvider));

        }
    }
}
