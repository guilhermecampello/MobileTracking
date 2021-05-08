using MobileTracking.Core.Models;
using MobileTracking.Services;
using System;
using System.Collections.ObjectModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileTracking
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LocalesPage : ContentPage
    {
        private readonly LocaleProvider localeProvider;

        public ObservableCollection<Locale> Locales { get; set; }

        public LocalesPage(LocaleProvider localeProvider)
        {
            InitializeComponent();

            this.localeProvider = localeProvider;
            Locales = new ObservableCollection<Locale>(localeProvider.ClosestLocales);
            MyListView.ItemsSource = Locales;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            var locales = await this.localeProvider.TryGetLocalesByCoordinates();
            Locales.Clear();
            locales.ForEach(locale => Locales.Add(locale));
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;
            var selectedLocale = (Locale)e.Item;
            localeProvider.Locale = selectedLocale;

            //Deselect Item
            ((ListView)sender).SelectedItem = e.Item;
        }
    }
}
