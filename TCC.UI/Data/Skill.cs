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
        public string IconName { get; set; }
        public uint Id { get; set; }
        public Class Class { get; set; }
        public string Name { get; set; }
        public string ToolTip { get; set; }
        //public ImageBrush IconBrush { get
        //    {

        //        return new ImageBrush(Utils.BitmapToImageSource((Bitmap)Properties.Icon_Skills.ResourceManager.GetObject(iconName)));

        //    }
        //}


        public Skill(uint id, Class c, string name, string toolTip)
        {
            Id = id;
            Class = c;
            Name = name;
            ToolTip = toolTip;           
        }

        public void SetSkillIcon(string iconName)
        {
            //if (!iconName.Contains("Icon_Skills.")) return;
            this.IconName = iconName;//.Replace("Icon_Skills.", "");

            //CooldownWindow.Instance.Dispatcher.BeginInvoke(new Action(() =>
            //{
            //    iconBitmap = (Bitmap)Properties.Icon_Skills.ResourceManager.GetObject(iconName);
            //}));
        }

    }
}