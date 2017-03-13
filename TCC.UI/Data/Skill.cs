using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TCC
{
    public class Skill
    {
        Bitmap iconBitmap;

        public uint Id { get; set; }
        public Class Class { get; set; }
        public string Name { get; set; }
        public string ToolTip { get; set; }
        public ImageBrush IconBrush { get
            {
                return new ImageBrush(SkillsDatabase.BitmapToImageSource(iconBitmap));
            }
        }

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
            CooldownWindow.Instance.Dispatcher.Invoke(() =>
            {
                iconBitmap = (Bitmap)Image.FromFile(Environment.CurrentDirectory + @"/resources/icons/" + iconName + ".png");
            });
        }

    }
}