using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Collections.Generic;
using Xamarin.Forms;

namespace MobileTracking
{
    public class Canvas : SKCanvasView
    {
        public Canvas()
        {
        }

        public double positionX { get; set; }

        public double positionY { get; set; }

        public List<Marker> Markers { get; set; } = new List<Marker>();

        public void SetCoordinates(double x, double y)
        {
            this.positionX = x*100 + 250;
            this.positionY = y*100 + 50;
            this.InvalidateSurface();
        }

        public void SetMarkers(List<Marker> markers)
        {
            this.Markers = markers;
            this.InvalidateSurface();
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            canvas.Clear();
            SKPaint circlePaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = Color.Red.ToSKColor(),
                StrokeWidth = 25
            };

            SKPaint paint = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                Color = Color.Gray.ToSKColor(),
                StrokeWidth = 25
            };
            
            canvas.DrawRect(0, 0, 500, 500, paint);
            canvas.DrawCircle(float.Parse(positionX.ToString()), float.Parse(positionY.ToString()), 10F, circlePaint);

            SKPaint markerPaint = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                Color = Color.Yellow.ToSKColor(),
                StrokeWidth = 25
            };
            SKPaint markerLetterPaint = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                Color = Color.White.ToSKColor(),
                StrokeWidth = 25
            };

            Markers.ForEach(marker =>
            {
                canvas.DrawCircle(float.Parse(marker.X.ToString()), float.Parse(marker.Y.ToString()), 10F, markerPaint);
                canvas.DrawText(marker.Name, float.Parse(marker.X.ToString()), float.Parse(marker.Y.ToString()), markerLetterPaint);
            });
        }
    }
}
