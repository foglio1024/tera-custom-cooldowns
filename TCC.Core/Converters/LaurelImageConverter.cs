using System;
using System.Globalization;
using System.Windows.Data;
using TCC.Data;

namespace TCC.Converters
{
    public class LaurelImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var l = (Laurel)value;
            bool kr = false, big = false;
            if (parameter != null)
            {
                kr = parameter.ToString().Contains("kr");
                big = parameter.ToString().Contains("big");
            }
            //System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(50, 50);
            var laurel = "";
            switch (l)
            {
                case Laurel.None:
                    laurel = "blank";
                    break;
                case Laurel.Bronze:
                    laurel = "bronze";
                    break;
                case Laurel.Silver:
                    laurel = "silver";
                    break;
                case Laurel.Gold:
                    laurel = "gold";
                    break;
                case Laurel.Diamond:
                    laurel = "diamond";
                    break;
                case Laurel.Champion:
                    laurel = "champion";
                    break;
            }

            if (kr) laurel += "_kr";
            if (big) laurel += "_big";
            // return new ImageBrush(CharacterWindow.Bitmap2BitmapImage(bitmap));
            return "/resources/images/Icon_Laurels/" + laurel + ".png";
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
