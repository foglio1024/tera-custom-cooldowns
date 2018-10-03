//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.IO.Packaging;
//using System.Windows.Media.Imaging;

//namespace TCC.TeraCommon.Game.Services
//{
//    public class IconsDatabase
//    {
//        private readonly Dictionary<string, Bitmap> _bitmaps = new Dictionary<string, Bitmap>();
//        public readonly Package Icons;
//        private readonly Dictionary<string, BitmapImage> _images = new Dictionary<string, BitmapImage>();

//        public IconsDatabase(string resourceDirectory, Package icons)
//        {
//            Icons = icons;
////            IconsDirectory = Path.Combine(resourceDirectory, "icons/");
//            //_icons = Package.Open(resourceDirectory + "icons.zip", FileMode.Open, FileAccess.Read);
//        }

//        public BitmapImage GetImage(string iconName)
//        {
//            BitmapImage image;
//            if (_images.TryGetValue(iconName, out image) || string.IsNullOrEmpty(iconName))
//            {
//                return image;
//            }
//            image = new BitmapImage();
//            var ur = new Uri("/" + iconName + ".png", UriKind.Relative);
//            if (Icons.PartExists(ur))
//            {
//                image.BeginInit();
//                image.CacheOption = BitmapCacheOption.OnLoad;
//                image.StreamSource = Icons.GetPart(ur).GetStream();
//                image.EndInit();
//            }
//            //var filename = IconsDirectory + iconName + ".png";
//            //image = new BitmapImage(new Uri(filename));
//            _images.Add(iconName, image);
//            return image;
//        }

//        public Bitmap GetBitmap(string iconName)
//        {
//            Bitmap image;
//            if (_bitmaps.TryGetValue(iconName, out image) || string.IsNullOrEmpty(iconName))
//            {
//                return image;
//            }
//            var ur = new Uri("/" + iconName + ".png", UriKind.Relative);
//            image = Icons.PartExists(ur) ? new Bitmap(Icons.GetPart(ur).GetStream()) : new Bitmap(1, 1);
//            _bitmaps.Add(iconName, image);
//            return image;
//        }
//    }
//}