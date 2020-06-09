using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace Medical.CoreLayer.Gui
{

    public class ShadeAdorner : Adorner
    {
        private static readonly Point startPoint = new Point(0, 0);

        public string OverlayedText
        {
            get { return (string)GetValue(OverlayedTextProperty); }
            set { SetValue(OverlayedTextProperty, value); }
        }

        public static readonly DependencyProperty OverlayedTextProperty =
            DependencyProperty.Register("OverlayedText", typeof(string), typeof(ShadeAdorner), new UIPropertyMetadata(""));
        public Brush ForeGround
        {
            get { return (Brush)GetValue(ForeGroundProperty); }
            set { SetValue(ForeGroundProperty, value); }
        }

        public static readonly DependencyProperty ForeGroundProperty =
            DependencyProperty.Register("ForeGround", typeof(Brush), typeof(ShadeAdorner),
            new UIPropertyMetadata(Brushes.Black));
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(double), typeof(ShadeAdorner), new UIPropertyMetadata(10.0));
        public Typeface Typeface
        {
            get { return (Typeface)GetValue(TypefaceProperty); }
            set { SetValue(TypefaceProperty, value); }
        }

        public static readonly DependencyProperty TypefaceProperty =
            DependencyProperty.Register("Typeface", typeof(Typeface), typeof(ShadeAdorner),
            new UIPropertyMetadata(new Typeface("Verdana")));

        public ShadeAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
        }
 
        protected override void OnRender(DrawingContext drawingContext)
        {
            var sr = new SolidColorBrush
            {
                Color = Colors.Gray, 
                Opacity = 0.5
            };
            drawingContext.DrawRectangle(sr,null, new Rect(new Point(0, 0), DesiredSize));
            var text = new FormattedText(OverlayedText, Thread.CurrentThread.CurrentUICulture,FlowDirection.LeftToRight, Typeface, FontSize, ForeGround)
            {
                TextAlignment = TextAlignment.Center
            };
            drawingContext.DrawText(text, new Point(DesiredSize.Width / 2, DesiredSize.Height / 2 - text.Height / 2));
            base.OnRender(drawingContext);
        }
    }
}
