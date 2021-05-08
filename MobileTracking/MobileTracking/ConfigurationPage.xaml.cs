using MobileTracking.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileTracking
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfigurationPage : ContentPage
    {
        private readonly Client client; 
        public ConfigurationPage(Client client)
        {
            InitializeComponent();
            this.client = client;
            urlEditor.IsReadOnly = true;
            this.BindingContext = client;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            var url = await DisplayPromptAsync(
                "Edit server URL",
                "Please provide server URL",
                initialValue: client.BaseAddress,
                placeholder: "https://localhost:50001/api");
            if (!string.IsNullOrEmpty(url) && Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                this.client.BaseAddress = url;
                Device.BeginInvokeOnMainThread(() => urlEditor.Text = url);
            }
            else
            {
                await DisplayAlert("Invalid URL","Please provide a valid URL","OK");
            }
        }

        private void Locales_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(Startup.ServiceProvider.GetService<LocalesPage>());
        }
    }
}