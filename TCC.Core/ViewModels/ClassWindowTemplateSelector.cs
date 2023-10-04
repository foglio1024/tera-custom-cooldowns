using System.Windows;
using System.Windows.Controls;
using TCC.UI;
using TeraDataLite;

namespace TCC.ViewModels;

public class ClassWindowTemplateSelector : DataTemplateSelector
{
    public DataTemplate? Warrior { get; set; }
    public DataTemplate? Archer { get; set; }
    public DataTemplate? Ninja { get; set; }
    public DataTemplate? Mystic { get; set; }
    public DataTemplate? Priest { get; set; }
    public DataTemplate? Lancer { get; set; }
    public DataTemplate? Brawler { get; set; }
    public DataTemplate? Sorcerer { get; set; }
    public DataTemplate? Slayer { get; set; }
    public DataTemplate? Berserker { get; set; }
    public DataTemplate? Gunner { get; set; }
    public DataTemplate? Valkyrie { get; set; }
    public DataTemplate? Reaper { get; set; }

    public DataTemplate? None { get; set; }

    public override DataTemplate? SelectTemplate(object? item, DependencyObject container)
    {
        if (App.Loading) return None;
        return WindowManager.ViewModels.ClassVM.CurrentClass switch
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
            _ => None
        };
    }
}