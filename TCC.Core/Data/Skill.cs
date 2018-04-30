using System.Text;

namespace TCC.Data
{
    public class Skill
    {
        public string IconName { get; set; }
        public uint Id { get; set; }
        public Class Class { get; set; }
        public string Name { get; set; }
        public string ToolTip { get; set; }
        public string ShortName
        {
            get
            {
                var n = Name.Split(' ');
                var last = n[n.Length - 1];
                if (last.Length < 5)
                {
                    if (!(last.Contains("X") || last.Contains("I") || last.Contains("V"))) return Name;
                    var sb = new StringBuilder();
                    for (var i = 0; i < n.Length-1; i++)
                    {
                        sb.Append(n[i]);
                        sb.Append(" ");
                    }
                    
                    return sb.ToString().Substring(0,sb.Length-1);
                }
                return Name;
            }
        }
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
            IconName = iconName.ToLower();//.Replace("Icon_Skills.", "");

            //CooldownWindow.Instance.Dispatcher.BeginInvoke(new Action(() =>
            //{
            //    iconBitmap = (Bitmap)Properties.Icon_Skills.ResourceManager.GetObject(iconName);
            //}));
        }

    }
}