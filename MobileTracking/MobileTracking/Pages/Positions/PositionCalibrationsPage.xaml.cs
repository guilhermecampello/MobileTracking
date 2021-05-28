using MobileTracking.Core.Application;
using MobileTracking.Core.Models;
using MobileTracking.Pages.Views;
using Plugin.Toast;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace MobileTracking.Pages.Positions
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PositionCalibrationsPage : ContentPage
    {
        public PositionCalibrationsPage(Position position, List<Calibration>? calibrations)
        {
            InitializeComponent();
            this.Position = position;
            this.BindingContext = this;
            Calibrations = new ObservableCollection<CalibrationView>(
                calibrations != null ?
                calibrations.Select(calibration => new CalibrationView(calibration))
                : position.Calibrations.Select(calibration => new CalibrationView(calibration)));
            collectionView.ItemsSource = Calibrations;
        }

        public Position Position { get; set; }

        public ObservableCollection<CalibrationView> Calibrations { get; set; }

        public IList<CalibrationView> SelectedCalibrations { get; set; } = new List<CalibrationView>();
        

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var stackLayout = (StackLayout)sender;
            var calibrationView = (CalibrationView)stackLayout.BindingContext;
            if (SelectedCalibrations.Any(item => ((CalibrationView)item).Id == calibrationView.Id))
            {
                SelectedCalibrations.Remove(calibrationView);
                stackLayout.BackgroundColor = Color.Default;
            }
            else
            {
                stackLayout.BackgroundColor = Color.LightGray;
                SelectedCalibrations.Add(calibrationView);
            }
        }

        private async void DeleteCalibrations_Clicked(object sender, EventArgs e)
        {
            var deleteConfirmation = await DisplayAlert(AppResources.Delete_calibrations, string.Empty ,AppResources.Delete, AppResources.Cancel);
            if (deleteConfirmation)
            {
                var calibrationService = Startup.ServiceProvider.GetService<ICalibrationService>();
                var deleted = await calibrationService.DeleteCalibrations(SelectedCalibrations.Select(calibration => calibration.Id).ToArray());
                if (deleted)
                {
                    CrossToastPopUp.Current.ShowToastError($"{SelectedCalibrations.Count} {AppResources.Calibrations.ToLower()} {AppResources.Deleted.ToLower()}");
                    SelectedCalibrations.ForEach(deleted =>
                    {
                        var item = Calibrations.FirstOrDefault(calibration => calibration.Id == deleted.Id);
                        Calibrations.Remove(item);
                    });
                    SelectedCalibrations.Clear();
                }
            }
        }
    }
}