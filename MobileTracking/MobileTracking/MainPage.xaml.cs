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
        SensorSpeed speed = SensorSpeed.UI;
        Canvas Canvas = new Canvas();
        public double OriginLatitude;
        public double OriginLongitude;
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
            Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
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
                var localization = await Geolocation.GetLocationAsync();
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

            calibrate_button.Text = $"Calibrar posição";

            latitudes.Sort();
            longitudes.Sort();

            longitudes.RemoveAt(0);
            longitudes.RemoveAt(8);

            latitudes.RemoveAt(0);
            latitudes.RemoveAt(8);

            OriginLongitude = longitudes.Sum() / 8;
            OriginLatitude = latitudes.Sum() / 8;
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
                var radius = 6371;
                var localizacao = await Geolocation.GetLocationAsync();
                if (OriginLatitude == 0 || OriginLongitude == 0)
                {
                    CalibrateOrigin();
                }
                if (localizacao != null)
                {

                    var dlong = -DgToRadians(OriginLongitude - localizacao.Longitude) * radius * 1000;
                    var dlat = DgToRadians(OriginLatitude - localizacao.Latitude)*radius*1000;
                    var rot = Math.Sqrt(2) / 2;
                    var rdlong = rot * dlong - rot * dlat;
                    var rdlat = rot * dlong + rot * dlat;
                    
                    lblLongitude.Text = rdlong.ToString();
                    lblLatitude.Text = rdlat.ToString();

                    if (rdlat > 2 && rdlong < 1)
                    {
                        comodo.Text = "Quarto Guilherme";
                    }

                    if ((Math.Abs(rdlat) < 1 && Math.Abs(rdlong) < 1) || rdlat < 0)
                    {
                        comodo.Text = "Sala";
                    }

                    if (rdlat > 1 && rdlong > 1)
                    {
                        comodo.Text = "Corredor";
                    }

                    if (rdlat > 2 && rdlong > 2)
                    {
                        comodo.Text = "Cozinha";
                    }

                    if (rdlat > 4)
                    {
                        comodo.Text = "Quarto Daniel";
                    }

                    this.Canvas.SetCoordinates(rdlong, rdlat);
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

        void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            var data = e.Reading;
            aclX.Text = data.Acceleration.X.ToString();
            aclY.Text = data.Acceleration.Y.ToString();
            aclZ.Text = data.Acceleration.Z.ToString();
        }

        public async void acl_btn_clicked(object sender, System.EventArgs e)
        {
            ToggleAccelerometer();
        }

        public void ToggleAccelerometer()
        {
            try
            {
                if (Accelerometer.IsMonitoring)
                    Accelerometer.Stop();
                else
                    Accelerometer.Start(speed);
            }
            catch (FeatureNotSupportedException)
            {
                // Feature not supported on device
                DisplayAlert("Acelerometro nao suportado", "", "");
            }
            catch (Exception ex)
            {
                // Other error has occurred.
                DisplayAlert(ex.Message, "", "");
            }
        }

        private double DgToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }
    }
}
