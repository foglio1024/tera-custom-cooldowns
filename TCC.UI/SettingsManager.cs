using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TCC
{
    public class WindowSettings
    {
        public double X;
        public double Y;
        public Visibility Visibility;
        public bool ClickThru;
    }
    public static class SettingsManager
    {
        static Rectangle _screen =  System.Windows.Forms.Screen.PrimaryScreen.Bounds;

        public static WindowSettings CooldownWindowSettings = new WindowSettings()
        {
            X = _screen.Width / 3,
            Y = _screen.Height /1.5,
            Visibility = Visibility.Visible,
            ClickThru = false
        };
        public static WindowSettings BossGaugeWindowSettings = new WindowSettings()
        {
            X = _screen.Width / 2 - 200,
            Y = 20,
            Visibility = Visibility.Visible,
            ClickThru = false
        };
        public static WindowSettings BuffBarWindowSettings = new WindowSettings()
        {
            X = _screen.Width -1000,
            Y = _screen.Height/1.5,
            Visibility = Visibility.Visible,
            ClickThru = false
        };
        public static WindowSettings CharacterWindowSettings = new WindowSettings()
        {
            X = _screen.Width / 2 - 200,
            Y = _screen.Height - 120,
            Visibility = Visibility.Visible,
            ClickThru = false
        };
    }
}
