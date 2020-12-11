using SkiaSharp;
using SkiaSharp.Views.Forms;
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

        public void SetCoordinates(double x, double y)
        {
            this.positionX = x*100 + 250;
            this.positionY = y*100 + 50;
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
        }
    }
}
