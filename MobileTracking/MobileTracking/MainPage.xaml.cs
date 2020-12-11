using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MobileTracking
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        Canvas Canvas = new Canvas();
        
        public double OriginLatitude;
        
        public double OriginLongitude;

        public List<Marker> Markers { get; set; } = new List<Marker>();
        
        HttpClient Client = new HttpClient(new HttpClientHandler()
        {
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
            {
                return true;
            },
        });

        public MainPage()
        {
            InitializeComponent();
            this.CanvasView.Content = this.Canvas;
            CalibrateOrigin();
        }

        public async void CalibrateOrigin()
        {
            var n = 0;
            List<double> latitudes = new List<double>();
            List<double> longitudes = new List<double>();
            while (n < 10)
            {
                calibrate_button.Text = $"Calibrar posição ({n})";
                var localization = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.High));
                latitudes.Add(localization.Latitude);
                longitudes.Add(localization.Longitude);
                var stringContent = new StringContent($"{DateTime.Now},{localization.Longitude},{localization.Latitude}");
                try
                {
                    await Client.PostAsync("http://192.168.15.10:5000/origin", stringContent);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                
                n += 1;
            }

            latitudes.Sort();
            longitudes.Sort();

            longitudes.RemoveAt(0);
            longitudes.RemoveAt(8);

            latitudes.RemoveAt(0);
            latitudes.RemoveAt(8);

            OriginLongitude = longitudes.Sum() / 8;
            OriginLatitude = latitudes.Sum() / 8;

            calibrate_button.Text = $"Calibrar posição";
            lblOrigin.Text = $"Origin ({OriginLongitude}, {OriginLatitude})";
        }

        public void calibrate_btn_clicked(object sender, System.EventArgs e)
        {
            CalibrateOrigin();
        }

        public async void btn_clicked(object sender, System.EventArgs e)
        {
            try
            {
                var localizacao = await Geolocation.GetLocationAsync();
                if (localizacao != null)
                {
                    var point = TranslationFromOrigin(localizacao.Longitude, localizacao.Latitude);
                                        
                    lblLongitude.Text = point.X.ToString();
                    lblLatitude.Text = point.Y.ToString();

                    if (point.Y > 2 && point.X < 1)
                    {
                        comodo.Text = "Quarto Guilherme";
                    }

                    if ((Math.Abs(point.Y) < 1 && Math.Abs(point.X) < 1) || point.Y < 0)
                    {
                        comodo.Text = "Sala";
                    }

                    if (point.Y > 1 && point.X > 1)
                    {
                        comodo.Text = "Corredor";
                    }

                    if (point.Y > 2 && point.X > 2)
                    {
                        comodo.Text = "Cozinha";
                    }

                    if (point.Y > 4)
                    {
                        comodo.Text = "Quarto Daniel";
                    }

                    this.Canvas.SetCoordinates(point.X, point.Y);
                    this.CanvasView.Content = this.Canvas;
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Recurso não suportado no device
                await DisplayAlert("Erro ", fnsEx.Message, "Ok");
            }
            catch (PermissionException pEx)
            {
                // Tratando erro de permissão
                await DisplayAlert("Erro: ", pEx.Message, "Ok");
            }
            catch (Exception ex)
            {
                // Não foi possivel obter localização
                await DisplayAlert("Erro : ", ex.Message, "Ok");
            }
        }

        private double DgToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }

        private Point TranslationFromOrigin(double longitude, double latitude)
        {
            var radius = 6371;
            var dlong = -DgToRadians(OriginLongitude - longitude) * radius * 1000;
            var dlat = DgToRadians(OriginLatitude - latitude) * radius * 1000;
            var rot = Math.Sqrt(2) / 2;
            var rdlong = rot * dlong - rot * dlat;
            var rdlat = rot * dlong + rot * dlat;

            return new Point(rdlong, rdlat);
        }

        public async void add_marker_clicked(object sender, EventArgs e)
        {
           await AddMarker();
        }

        private async Task AddMarker()
        {
            var name = await DisplayPromptAsync("New marker", "Input marker name:");
            if (!string.IsNullOrEmpty(name))
            {
                activityIndicator.IsRunning = true;
                var point = await CalibrateCoordinate();
                activityIndicator.IsRunning = false;
                point = TranslationFromOrigin(point.X, point.Y);
                var marker = new Marker(name, point.X, point.Y);
                Markers.Add(marker);
                Canvas.SetMarkers(Markers);
            }
        }

        private async Task<Point> CalibrateCoordinate()
        {
            var n = 0;
            List<double> latitudes = new List<double>();
            List<double> longitudes = new List<double>();
            while (n < 10)
            {
                var localization = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.High));
                latitudes.Add(localization.Latitude);
                longitudes.Add(localization.Longitude);
                n += 1;
            }

            latitudes.Sort();
            longitudes.Sort();

            longitudes.RemoveAt(0);
            longitudes.RemoveAt(8);

            latitudes.RemoveAt(0);
            latitudes.RemoveAt(8);

            return new Point(longitudes.Sum() / 8, latitudes.Sum() / 8);
        }
    }
}
