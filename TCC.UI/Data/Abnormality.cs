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
        Bitmap iconBitmap;

        public uint Id { get; set; }
        public string Name { get; set; }
        public string ToolTip { get; set; }
        public ImageBrush IconBrush
        {
            get
            {
                return new ImageBrush(SkillsDatabase.BitmapToImageSource(iconBitmap));
            }
        }

        public Abnormality(uint id, string name, string toolTip)
        {
            Id = id;
            Name = name;
            ToolTip = toolTip;
        }

        public void SetIcon(string iconName)
        {
            if (iconName.Contains("Icon_Skills."))
            {
                iconName = iconName.Replace("Icon_Skills.", "");
                CooldownWindow.Instance.Dispatcher.BeginInvoke(new Action(() =>
                {
                    iconBitmap = (Bitmap)Properties.Icon_Skills.ResourceManager.GetObject(iconName);
                }));

            }
            else if (iconName.Contains("Icon_Status."))
            {
                iconName = iconName.Replace("Icon_Status.", "");
                CooldownWindow.Instance.Dispatcher.BeginInvoke(new Action(() =>
                {
                    iconBitmap = (Bitmap)Properties.Icon_Status.ResourceManager.GetObject(iconName);
                }));
            }
            else if (iconName.Contains("Icon_Crest."))
            {
                iconName = iconName.Replace("Icon_Crest.", "");
                CooldownWindow.Instance.Dispatcher.BeginInvoke(new Action(() =>
                {
                    iconBitmap = (Bitmap)Properties.Icon_Crest.ResourceManager.GetObject(iconName);
                }));
            }
            else
            {
                iconBitmap = (Bitmap)Properties.Icon_Classes.common;
            }
        }

    }
}
