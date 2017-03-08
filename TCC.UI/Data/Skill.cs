using System;
using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TCC
{
    public class Skill
    {
        public uint Id { get; set; }
        public Class Class { get; set; }
        public string Name { get; set; }
        public string ToolTip { get; set; }
        //public BitmapImage Icon { get { return icon; } set { if (icon != value) icon = value; } }
        public ImageBrush IconBrush { get
            {
                return new ImageBrush(BitmapToImageSource(iconBitmap));
            }
        }
        Bitmap iconBitmap;
        public Skill(uint id, Class c, string name, string toolTip)
        {
            Id = id;
            Class = c;
            Name = name;
            ToolTip = toolTip;           
        }

        public void SetSkillIcon(string iconName)
        {
            if (!iconName.Contains("Icon_Skills.")) return;
            iconName = iconName.Replace("Icon_Skills.", "");
            CooldownsBarWindow.Instance.Dispatcher.Invoke(() =>
            {
                iconBitmap = (Bitmap)Image.FromFile(Environment.CurrentDirectory + @"/resources/icons/" + iconName + ".png");
                //icon = BitmapToImageSource(iconBitmap);
                //IconBrush = new ImageBrush(icon);
            });
            }

        BitmapImage BitmapToImageSource(System.Drawing.Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                memory.Close();
                return bitmapimage;
            }
        }

    }
}