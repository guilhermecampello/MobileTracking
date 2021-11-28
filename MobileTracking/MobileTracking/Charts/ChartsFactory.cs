using Syncfusion.SfChart.XForms;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MobileTracking.Charts
{
    public class ChartsFactory
    {
        public static SfChart CreatePositionSignalDataScatterChart(
            string title,
            string yTitle,
            string xTitle,
            object data,
            string xBindingPath,
            string yBindingPath,
            string errorXBindingPath = "",
            string errorYBindingPath = "",
            ErrorBarMode? errorBarMode = null,
            ChartAxis? primaryAxis = null,
            RangeAxisBase? secondaryAxis = null,
            bool showError = true,
            List<ScatterSeries>? secondarySeries = null,
            string markerLabel = "PositionId",
            double height = 800
            )
        {
            var chart = new SfChart() { VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand, MinimumHeightRequest = 400, HeightRequest = height };
            chart.ChartBehaviors.Add(new ChartZoomPanBehavior());
            chart.Title = new ChartTitle() { Text = title };
            chart.PrimaryAxis = primaryAxis ?? new NumericalAxis() { Title = new ChartAxisTitle() { Text = xTitle } };
            chart.SecondaryAxis = secondaryAxis ?? new NumericalAxis() { Title = new ChartAxisTitle() { Text = yTitle } };
            var scatterSeries = new ScatterSeries()
            {
                ItemsSource = data,
                ScatterHeight = 13,
                ScatterWidth = 13,
                ShapeType = ChartScatterShapeType.Ellipse,
                XBindingPath = xBindingPath,
                YBindingPath = yBindingPath,
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
                        label.SetBinding(Label.TextProperty, markerLabel);
                        return label;
                    }),
                }
            };
            scatterSeries.ColorModel.Palette = ChartColorPalette.Natural;
            scatterSeries.EnableDataPointSelection = true;
            chart.Series.Add(scatterSeries);

            if (secondarySeries != null)
            {
                secondarySeries.ForEach(series => chart.Series.Add(series));               
            }
            
            if (showError)
            {
                var errorSeries = new ErrorBarSeries()
                {
                    ItemsSource = data,
                    XBindingPath = xBindingPath,
                    YBindingPath = yBindingPath,
                    Type = ErrorBarType.Custom,
                    Mode = errorBarMode!.Value,
                    HorizontalCapLineStyle = new ErrorBarCapLineStyle() { StrokeColor = Color.Black.MultiplyAlpha(0.4) },
                    VerticalCapLineStyle = new ErrorBarCapLineStyle() { StrokeColor = Color.Black.MultiplyAlpha(0.4) },
                    HorizontalLineStyle = new ErrorBarLineStyle() { StrokeColor = Color.Black.MultiplyAlpha(0.4) },
                    VerticalLineStyle = new ErrorBarLineStyle() { StrokeColor = Color.Black.MultiplyAlpha(0.4) },
                    HorizontalErrorPath = errorXBindingPath,
                    VerticalErrorPath = errorYBindingPath
                };
                chart.Series.Add(errorSeries);
            }
            chart.Legend = new ChartLegend()
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
                    id.SetBinding(Label.TextProperty, "DataPoint." + markerLabel);
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
            return chart;
        }
    }
}
