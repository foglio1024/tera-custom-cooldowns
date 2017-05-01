using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace TCC
{
    public static class Utils
    {
        static MemoryStream ms;

        public static BitmapImage BitmapToImageSource(System.Drawing.Bitmap bitmap)
        {
            //using (MemoryStream memory = new MemoryStream())
            //{
            ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            ms.Position = 0;
            BitmapImage bitmapimage = new BitmapImage();
            bitmapimage.BeginInit();
            bitmapimage.StreamSource = ms;
            bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapimage.EndInit();
            ms.Flush();
            ms.Close();
            ms.Dispose();
            ms = null;
            return bitmapimage;
            //}
        }

        public static Point GetRelativePoint(double x, double y, double cx, double cy)
        {
            return new Point(x - cx, y - cy);
        }
    }

    public class DependencyPropertyWatcher<T> : DependencyObject, IDisposable
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(object),
                typeof(DependencyPropertyWatcher<T>),
                new PropertyMetadata(null, OnPropertyChanged));

        public event EventHandler PropertyChanged;

        public DependencyPropertyWatcher(DependencyObject target, string propertyPath)
        {
            this.Target = target;
            BindingOperations.SetBinding(
                this,
                ValueProperty,
                new Binding() { Source = target, Path = new PropertyPath(propertyPath), Mode = BindingMode.OneWay });
        }

        public DependencyObject Target { get; private set; }

        public T Value
        {
            get { return (T)this.GetValue(ValueProperty); }
        }

        public static void OnPropertyChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            DependencyPropertyWatcher<T> source = (DependencyPropertyWatcher<T>)sender;

            if (source.PropertyChanged != null)
            {
                source.PropertyChanged(source, EventArgs.Empty);
            }
        }

        public void Dispose()
        {
            this.ClearValue(ValueProperty);
        }
    }
}
