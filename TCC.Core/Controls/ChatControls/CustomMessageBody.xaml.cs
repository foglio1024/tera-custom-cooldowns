using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TCC.Data;

namespace TCC.Controls.ChatControls
{
    /// <summary>
    /// Logica di interazione per CustomMessageBody.xaml
    /// </summary>
    public partial class CustomMessageBody : UserControl
    {
        public CustomMessageBody()
        {
            InitializeComponent();
        }
        ChatMessage _context;
        protected override void OnRender(DrawingContext drawingContext)
        {
            _context = (ChatMessage)DataContext;
            //InvalidateVisual();
            //base.OnRender(drawingContext);

            if (_context == null) return;
            var textString = _context.RawMessage;
            var sb = new StringBuilder();
            foreach (var item in _context.Pieces)
            {
                sb.Append(item.Text);
            }
            var tp = new Typeface((FontFamily)App.Current.FindResource("Frutiger"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            var ft = new FormattedText(sb.ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, tp, 15, Brushes.White);
            foreach (var item in _context.Pieces)
            {
                if (item.Text == null) continue;
                ft.SetForegroundBrush(item.Color, sb.ToString().IndexOf(item.Text), item.Text.Length);
            }
            ft.MaxTextWidth = this.ActualWidth;
            Height = ft.Height + 2;
            var p = new Point(0, 0);
            var gt = ft.BuildGeometry(p);

            drawingContext.DrawGeometry(null, new Pen(new SolidColorBrush(Color.FromArgb(0xaa, 0, 0, 0)), 4) {DashCap = PenLineCap.Round, EndLineCap = PenLineCap.Round, LineJoin = PenLineJoin.Round, StartLineCap = PenLineCap.Round }, gt);
            drawingContext.DrawText(ft, p);
        }

    }
}
