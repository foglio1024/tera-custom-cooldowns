﻿using System.Text;
using TCC.Data.Abnormalities;
using TeraDataLite;

namespace TCC.Data.Skills;

public class Skill
{
    string _iconName = "";
    public string IconName
    {
        get => _iconName;
        set => _iconName = value.ToLower();
    }

    public uint Id { get; }
    public Class Class { get; }
    public string Name { get; }
    public string ToolTip { get; }
    public string ShortName
    {
        get
        {
            var n = Name.Split(' ');
            var last = n[^1];
            if (last.Length >= 5) return Name;
            if (!(last.Contains('X') || last.Contains('I') || last.Contains('V'))) return Name;
            var sb = new StringBuilder();
            for (var i = 0; i < n.Length - 1; i++)
            {
                sb.Append(n[i]);
                sb.Append(' ');
            }

            return sb.Length == 0 ? "" : sb.ToString()[..(sb.Length - 1)];
        }
    }

    public string Detail { get; set; } = "";
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
    public Skill(Abnormality ab, Class c = Class.Common)
    {
        Id = ab.Id;
        Class = c;
        Name = ab.Name;
        ToolTip = ab.ToolTip;
        IconName = ab.IconName;
    }
    //public void SetSkillIcon(string iconName)
    //{
    //    //if (!iconName.Contains("Icon_Skills.")) return;
    //    IconName = iconName.ToLower();//.Replace("Icon_Skills.", "");

    //    //CooldownWindow.Instance.Dispatcher.InvokeAsync(new Action(() =>
    //    //{
    //    //    iconBitmap = (Bitmap)Properties.Icon_Skills.ResourceManager.GetObject(iconName);
    //    //}));
    //}

}