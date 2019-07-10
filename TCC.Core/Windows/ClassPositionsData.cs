using System.Windows;
using TCC.Data;

namespace TCC.Windows
{
    public class ClassPositionsData
    {
        public Point Position { get; set; }
        public ButtonsPosition Buttons { get; set; }

        public ClassPositionsData()
        {
                
        }
        public ClassPositionsData(ClassPositionsData origin)
        {
            Position = new Point(origin.Position.X, origin.Position.Y);
            Buttons = origin.Buttons;
        }

        public ClassPositionsData(double x, double y, ButtonsPosition buttons)
        {
            Position = new Point(x, y);
            Buttons = buttons;
        }

        public void ApplyCorrection(Size sc)
        {
            Position = new Point(sc.Width * Position.X, sc.Height * Position.Y);
        }
    }
}