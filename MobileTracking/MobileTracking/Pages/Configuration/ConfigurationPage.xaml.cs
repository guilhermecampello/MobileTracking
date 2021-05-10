using MobileTracking.Communication;
using System;
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
            this.BindingContext = client;
            if (client.IsHealthy)
            {
                clientStatus.BackgroundColor = Color.Green;
            }
            else
            {
                clientStatus.BackgroundColor = Color.Red;
            }
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                await CheckClientHealth();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private async Task<bool> CheckClientHealth()
        {
            if (await this.client.CheckHealth())
            {
                Device.BeginInvokeOnMainThread(() => {
                    urlEditor.Text = client.Hostname;
                    clientStatus.BackgroundColor = Color.Green;
                });
                return true;
            }
            else
            {
                Device.BeginInvokeOnMainThread(() => {
                    clientStatus.BackgroundColor = Color.Red;
                });
            }

            return false;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            var url = await DisplayPromptAsync(
                AppResources.Edit_Hostname,
                string.Empty,
                initialValue: client.Hostname,
                placeholder: "localhost");
            if (!string.IsNullOrEmpty(url))
            {
                this.client.Hostname = url;
                try
                {
                    if (!await CheckClientHealth())
                    {
                        this.client.Hostname = urlEditor.Text;
                        await DisplayAlert(AppResources.Server_not_available, string.Empty, "OK");
                    }
                }
                catch(Exception ex)
                {
                    await DisplayAlert(AppResources.Error, ex.Message, "OK");
                }
            }
            else
            {
                await DisplayAlert(AppResources.Invalid_Hostname,string.Empty,"OK");
            }
        }

        private void Locales_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(Startup.ServiceProvider.GetService<LocalesPage>());
        }
    }
}