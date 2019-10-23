using System;
using System.Globalization;
using System.Windows.Data;
using TeraDataLite;

namespace TCC.Converters
{
    public class ClassImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) value = Class.Common;
            var c = (Class)value;
            string className;
            switch (c)
            {
                case Class.Warrior:
                    className = "warrior";
                    break;
                case Class.Lancer:
                    className = "lancer";
                    break;
                case Class.Slayer:
                    className = "slayer";
                    break;
                case Class.Berserker:
                    className = "berserker";
                    break;
                case Class.Sorcerer:
                    className = "sorcerer";
                    break;
                case Class.Archer:
                    className = "archer";
                    break;
                case Class.Priest:
                    className = "priest";
                    break;
                case Class.Mystic:
                    className = "mystic";
                    break;
                case Class.Reaper:
                    className = "reaper";
                    break;
                case Class.Gunner:
                    className = "gunner";
                    break;
                case Class.Brawler:
                    className = "brawler";
                    break;
                case Class.Ninja:
                    className = "ninja";
                    break;
                case Class.Valkyrie:
                    className = "glaiver";
                    break;
                case Class.None: return null;
                default:
                    className = "common";
                    break;

            }
            return "/resources/images/Icon_Classes/" + className + ".png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
