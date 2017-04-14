using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TCC.Data
{
    public class Abnormality
    {
        //Bitmap iconBitmap;
        public string IconName { get; set; }
        public uint Id { get; set; }
        public string Name { get; set; }
        public string ToolTip { get; set; }
        //public bool IsBuff { get; set; }
        public bool IsShow { get; set; }
        public bool Infinity { get; set; }
        public AbnormalityType Type { get; set; }
        //public ImageBrush IconBrush
        //{
        //    get
        //    {
        //        Bitmap iconBitmap = (Bitmap)Properties.Icon_Classes.common;

        //        if (iconName.Contains("Icon_Skills."))
        //        {
        //            var img = iconName.Replace("Icon_Skills.", "");
        //            CooldownWindow.Instance.Dispatcher.Invoke(() =>
        //            {
        //                iconBitmap = (Bitmap)Properties.Icon_Skills.ResourceManager.GetObject(img);
        //            });
        //        }
        //        else if (iconName.Contains("Icon_Status."))
        //        {
        //            var img = iconName.Replace("Icon_Status.", "");
        //            CooldownWindow.Instance.Dispatcher.Invoke(() =>
        //            {
        //                iconBitmap = (Bitmap)Properties.Icon_Status.ResourceManager.GetObject(img);
        //            });
        //        }
        //        else if (iconName.Contains("Icon_Crest."))
        //        {
        //            var img = iconName.Replace("Icon_Crest.", "");
        //            CooldownWindow.Instance.Dispatcher.Invoke(() =>
        //            {
        //                iconBitmap = (Bitmap)Properties.Icon_Crest.ResourceManager.GetObject(img);
        //            });
        //        }

        //        return new ImageBrush(Utils.BitmapToImageSource(iconBitmap));
        //    }
        //}

        public Abnormality(uint id, bool isShow, bool infinity, AbnormalityType prop)
        {
            Id = id;
            //IsBuff = isBuff;
            IsShow = isShow;
            Infinity = infinity;
            Type = prop;
        }

        public void SetIcon(string iconName)
        {
            this.IconName = iconName;
            //if (iconName.Contains("Icon_Skills."))
            //{
            //    this.iconName = iconName.Replace("Icon_Skills.", "");
            //    //CooldownWindow.Instance.Dispatcher.BeginInvoke(new Action(() =>
            //    //{
            //    //    iconBitmap = (Bitmap)Properties.Icon_Skills.ResourceManager.GetObject(iconName);
            //    //}));

            //}
            //else if (iconName.Contains("Icon_Status."))
            //{
            //    this.iconName = iconName.Replace("Icon_Status.", "");
            //    //CooldownWindow.Instance.Dispatcher.BeginInvoke(new Action(() =>
            //    //{
            //    //    iconBitmap = (Bitmap)Properties.Icon_Status.ResourceManager.GetObject(iconName);
            //    //}));
            //}
            //else if (iconName.Contains("Icon_Crest."))
            //{
            //    this.iconName = iconName.Replace("Icon_Crest.", "");
            //    //CooldownWindow.Instance.Dispatcher.BeginInvoke(new Action(() =>
            //    //{
            //    //    iconBitmap = (Bitmap)Properties.Icon_Crest.ResourceManager.GetObject(iconName);
            //    //}));
            //}
            //else
            //{

            //    //iconBitmap = (Bitmap)Properties.Icon_Classes.common;
            //}
        }

        public void SetInfo(string name, string toolTip)
        {
            Name = name;
            ToolTip = toolTip;
        }

    }
}
