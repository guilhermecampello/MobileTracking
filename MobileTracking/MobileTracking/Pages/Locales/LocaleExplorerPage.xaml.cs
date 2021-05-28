using System;
using System.Collections.Generic;
using System.Linq;
using MobileTracking.Core.Models;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Syncfusion.SfChart.XForms;
using System.Collections.ObjectModel;

namespace MobileTracking.Pages.Locales
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LocaleExplorerPage : ContentPage
    {
        public LocaleExplorerPage(Locale locale)
        {
            InitializeComponent();
            BindingContext = this;
            this.Locale = locale;
            GeneratePositionsMagneticFieldChart();
        }

        public Locale Locale { get; set; }

        public ObservableCollection<PositionData> MagneticFieldData { get; set; } = new ObservableCollection<PositionData>();

        private void GeneratePositionsMagneticFieldChart()
        {
            Locale.Zones?.ForEach(zone =>
                zone.Positions?.ForEach(position =>
            {
                var data = position.PositionData.FirstOrDefault(data => data.SignalType == SignalType.Magnetometer);
                if (data != null)
                {
                    position.Zone = zone;
                    data.Position = position;
                    MagneticFieldData.Add(data);
                }
            }));
            Chart.ChartBehaviors.Add(new ChartZoomPanBehavior());
            Chart.Title = new ChartTitle() { Text = AppResources.Magnetic_field };
            Chart.PrimaryAxis = new NumericalAxis() { Title = new ChartAxisTitle() { Text = AppResources.Magnetic_Z_Intensity } };
            Chart.SecondaryAxis = new NumericalAxis() { Title = new ChartAxisTitle() { Text = AppResources.Magnetic_Y_Intensity } };
            var scatterSeries = new ScatterSeries()
            {
                ItemsSource = MagneticFieldData,
                ScatterHeight = 13,
                ScatterWidth = 13,
                ShapeType = ChartScatterShapeType.Ellipse,
                XBindingPath = "Z",
                YBindingPath = "Y",
                DataMarker = new ChartDataMarker()
                {
                    ShowLabel = true,
                    LabelStyle = new DataMarkerLabelStyle() { LabelPosition = DataMarkerLabelPosition.Outer },
                    LabelTemplate = new DataTemplate(() =>
                    {
                        var label = new Label()
                        {
                            VerticalTextAlignment = TextAlignment.Center,
                            FontSize = 13
                        };
                        label.SetBinding(Label.TextProperty, "PositionId");
                        return label;
                    })
                }
            };
            scatterSeries.ColorModel.Palette = ChartColorPalette.Natural;
            scatterSeries.EnableDataPointSelection = true;
            var errorSeries = new ErrorBarSeries()
            {
                ItemsSource = MagneticFieldData,
                XBindingPath = "Z",
                YBindingPath = "Y",
                Type = ErrorBarType.Custom,
                Mode = ErrorBarMode.Both,
                HorizontalCapLineStyle = new ErrorBarCapLineStyle() { StrokeColor = Color.Black.MultiplyAlpha(0.4) },
                VerticalCapLineStyle = new ErrorBarCapLineStyle() { StrokeColor = Color.Black.MultiplyAlpha(0.4) },
                HorizontalLineStyle = new ErrorBarLineStyle() { StrokeColor = Color.Black.MultiplyAlpha(0.4) },
                VerticalLineStyle = new ErrorBarLineStyle() { StrokeColor = Color.Black.MultiplyAlpha(0.4) },
                HorizontalErrorPath = "StandardDeviationZ",
                VerticalErrorPath = "StandardDeviationY"
            };
            Chart.Series.Add(scatterSeries);
            Chart.Series.Add(errorSeries);

            Chart.Legend = new ChartLegend()
            {
                Series = scatterSeries,
                OverflowMode = ChartLegendOverflowMode.Wrap,
                DockPosition = LegendPlacement.Bottom,
                ItemTemplate = new DataTemplate(() =>
                {
                    StackLayout stack = new StackLayout()
                    {
                        Orientation = StackOrientation.Horizontal,
                        WidthRequest = 150
                    };

                    BoxView boxView = new BoxView()
                    {
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        CornerRadius = 7,
                        WidthRequest = 13,
                        HeightRequest = 13
                    };
                    boxView.SetBinding(BoxView.BackgroundColorProperty, "IconColor");

                    Label id = new Label()
                    {
                        VerticalTextAlignment = TextAlignment.Center,
                        FontSize = 13
                    };
                    id.SetBinding(Label.TextProperty, "DataPoint.PositionId");
                    Label dashLabel = new Label()
                    {
                        VerticalTextAlignment = TextAlignment.Center,
                        FontSize = 10,
                        Text = "-"
                    };
                    Label name = new Label()
                    {
                        VerticalTextAlignment = TextAlignment.Center,
                        FontSize = 10
                    };
                    name.SetBinding(Label.TextProperty, "DataPoint.Name");

                    stack.Children.Add(boxView);
                    stack.Children.Add(id);
                    stack.Children.Add(dashLabel);
                    stack.Children.Add(name);
                    return stack;
                })
            };
        }
    }
}