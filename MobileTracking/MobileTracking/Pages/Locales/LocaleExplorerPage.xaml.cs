using System;
using System.Collections.Generic;
using System.Linq;
using MobileTracking.Core.Models;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Syncfusion.SfChart.XForms;
using System.Collections.ObjectModel;
using MobileTracking.Charts;
using Xamarin.Forms.Internals;
using System.Timers;

namespace MobileTracking.Pages.Locales
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LocaleExplorerPage : ContentPage
    {
        private Timer timer;
        public LocaleExplorerPage(Locale locale)
        {
            InitializeComponent();
            BindingContext = this;
            this.Locale = locale;
            AggregateData();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var stackLayout = new StackLayout() { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center };
            stackLayout.Children.Add(new ActivityIndicator() { IsRunning = true });
            stackLayout.Children.Add(new Label() { HorizontalTextAlignment = TextAlignment.Center, Text = AppResources.Calculating_data, FontAttributes = FontAttributes.Bold, FontSize = 18 });
            stackLayout.Children.Add(new Label() { HorizontalTextAlignment = TextAlignment.Center, Text = $"{SignalNames.Count} {AppResources.Signals_found.ToLower()}", FontAttributes = FontAttributes.Bold, FontSize = 18 });
            ScrollView.Content = stackLayout;
            timer = new Timer(200);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();
            AddCharts();
        }

        public Locale Locale { get; set; }

        public HashSet<string> SignalNames { get; set; } = new HashSet<string>();

        public ObservableCollection<PositionData> LocaleData { get; set; } = new ObservableCollection<PositionData>();

        public ObservableCollection<PositionData> MagneticFieldData { get; set; } = new ObservableCollection<PositionData>();

        private void AggregateData()
        {
            Locale.Zones?.ForEach(zone =>
                zone.Positions?.ForEach(position =>
                {
                    position.Zone = zone;
                    position.PositionData?.ForEach(data =>
                    {
                        data.Position = position;
                        LocaleData.Add(data);
                    });
                }));

            SignalNames = LocaleData
                .Where(data => data.SignalType != SignalType.Magnetometer && !string.IsNullOrEmpty(data.SignalId))
                .GroupBy(data => data.SignalId)
                .Select(data => new { SignalId = data.Key, Samples = data.Sum(positionData => positionData.Samples) })
                .OrderByDescending(signal => signal.Samples)
                .Select(signal => signal.SignalId)
                .ToHashSet();
        }

        private void AddCharts()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var stackLayout = new StackLayout();
                stackLayout.Children.Add(GeneratePositionsMagneticFieldChart());
                AddPositionsSignalsCharts(stackLayout);
                ScrollView.Content = stackLayout;
            });

        }

        private SfChart GeneratePositionsMagneticFieldChart()
        {
            var magneticFieldData = LocaleData.Where(data => data.SignalType == SignalType.Magnetometer).ToList();
            MagneticFieldData = new ObservableCollection<PositionData>(magneticFieldData);
            return ChartsFactory.CreatePositionDataScatterChart(
                AppResources.Magnetic_field,
                AppResources.Magnetic_Y_Intensity,
                AppResources.Magnetic_Z_Intensity,
                MagneticFieldData,
                "Z", "Y", "StandardDeviationZ", "StandardDeviationX",
                ErrorBarMode.Both
                );
        }

        private void AddPositionsSignalsCharts(StackLayout stackLayout)
        {
            SignalNames.ForEach(signalName =>
            {
                var signalData = LocaleData.Where(data => data.SignalId == signalName).ToList();
                var minimum = signalData.Min(signalData => signalData.Strength - signalData.StandardDeviation);
                var maximum = signalData.Max(signalData => signalData.Strength + signalData.StandardDeviation);
                var primaryAxis = new CategoryAxis() { Title = new ChartAxisTitle() { Text = AppResources.Position } };
                var secondaryAxis = new NumericalAxis()
                {
                    Minimum = minimum,
                    Maximum = maximum,
                    LabelStyle = new ChartAxisLabelStyle() { LabelFormat = "##" },
                    Title = new ChartAxisTitle() { Text = AppResources.RSSI_dB }
                };
                var chart = ChartsFactory.CreatePositionDataScatterChart(
                    signalName,
                    AppResources.RSSI_dB,
                    AppResources.Position,
                    signalData,
                    "PositionId",
                    "Strength",
                    "",
                    "StandardDeviation",
                    ErrorBarMode.Vertical,
                    primaryAxis,
                    secondaryAxis);
                stackLayout.Children.Add(chart);
            });
        }
    }
}