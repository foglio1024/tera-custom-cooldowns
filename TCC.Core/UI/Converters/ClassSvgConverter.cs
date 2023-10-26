using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TeraDataLite;

namespace TCC.UI.Converters;

public class ClassSvgConverter : IValueConverter
{
    public StreamGeometry? Warrior { get; set; }
    public StreamGeometry? Lancer { get; set; }
    public StreamGeometry? Slayer { get; set; }
    public StreamGeometry? Berserker { get; set; }
    public StreamGeometry? Sorcerer { get; set; }
    public StreamGeometry? Archer { get; set; }
    public StreamGeometry? Priest { get; set; }
    public StreamGeometry? Mystic { get; set; }
    public StreamGeometry? Reaper { get; set; }
    public StreamGeometry? Gunner { get; set; }
    public StreamGeometry? Brawler { get; set; }
    public StreamGeometry? Ninja { get; set; }
    public StreamGeometry? Valkyrie { get; set; }
    public StreamGeometry? Common { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var c = value switch
        {
            Class cl => cl,
            string s => Enum.Parse<Class>(s),
            _ => Class.Common
        };

        return c switch
        {
            Class.Warrior => Warrior,
            Class.Lancer => Lancer,
            Class.Slayer => Slayer,
            Class.Berserker => Berserker,
            Class.Sorcerer => Sorcerer,
            Class.Archer => Archer,
            Class.Priest => Priest,
            Class.Mystic => Mystic,
            Class.Reaper => Reaper,
            Class.Gunner => Gunner,
            Class.Brawler => Brawler,
            Class.Ninja => Ninja,
            Class.Valkyrie => Valkyrie,
            _ => Common
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}