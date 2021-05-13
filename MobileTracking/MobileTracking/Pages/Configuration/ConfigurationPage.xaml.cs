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

        private readonly Configuration configuration;

        public ConfigurationPage(Client client, Configuration configuration)
        {
            InitializeComponent();
            this.client = client;
            this.configuration = configuration;
            this.BindingContext = configuration;
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
                this.configuration.Hostname = url;
                try
                {
                    if (!await CheckClientHealth())
                    {
                        this.configuration.Hostname = urlEditor.Text;
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

        private async void Samples_Button_Clicked(object sender, EventArgs e)
        {
            var result = await DisplayPromptAsync(
                AppResources.Edit_samples,
                AppResources.Provide_samples_per_position,
                initialValue: configuration.SamplesPerPosition.ToString(),
                placeholder: "50",
                keyboard: Keyboard.Numeric);
            if (int.TryParse(result, out int samples) && samples > 0)
            {
                this.configuration.SamplesPerPosition = samples;
                Device.BeginInvokeOnMainThread(() => {
                    samplesPerPosition.Text = result;
                });
            }
            else
            {
                await DisplayAlert(AppResources.Value_must_be_a_positive_integer, string.Empty, "OK");
            }
        }

        private async void Interval_Button_Clicked(object sender, EventArgs e)
        {
            var result = await DisplayPromptAsync(
                AppResources.Edit_interval,
                AppResources.Provide_interval,
                initialValue: configuration.DataAquisitionInterval.ToString(),
                placeholder: "5",
                keyboard: Keyboard.Numeric);
            if (int.TryParse(result, out int interval) && interval > 0)
            {
                this.configuration.DataAquisitionInterval = interval;
                Device.BeginInvokeOnMainThread(() => {
                    dataInterval.Text = result;
                });
            }
            else
            {
                await DisplayAlert(AppResources.Value_must_be_a_positive_integer, string.Empty, "OK");
            }
        }

        private void Locales_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(Startup.ServiceProvider.GetService<LocalesPage>());
        }
    }
}