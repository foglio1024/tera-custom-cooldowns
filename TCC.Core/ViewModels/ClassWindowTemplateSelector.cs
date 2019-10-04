using System.Windows;
using System.Windows.Controls;
using TeraDataLite;

namespace TCC.ViewModels
{
    public class ClassWindowTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Warrior { get; set; }
        public DataTemplate Archer { get; set; }
        public DataTemplate Ninja { get; set; }
        public DataTemplate Mystic { get; set; }
        public DataTemplate Priest { get; set; }
        public DataTemplate Lancer { get; set; }
        public DataTemplate Brawler { get; set; }
        public DataTemplate Sorcerer { get; set; }
        public DataTemplate Slayer { get; set; }
        public DataTemplate Berserker { get; set; }
        public DataTemplate Gunner { get; set; }
        public DataTemplate Valkyrie { get; set; }
        public DataTemplate Reaper { get; set; }

        public DataTemplate None { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            switch (WindowManager.ViewModels.ClassVM.CurrentClass)
            {
                case Class.Warrior: return Warrior;
                case Class.Lancer: return Lancer;
                case Class.Slayer: return Slayer;
                case Class.Berserker: return Berserker;
                case Class.Sorcerer: return Sorcerer;
                case Class.Archer: return Archer;
                case Class.Priest: return Priest;
                case Class.Mystic: return Mystic;
                case Class.Reaper: return Reaper;
                case Class.Gunner: return Gunner;
                case Class.Brawler: return Brawler;
                case Class.Ninja: return Ninja;
                case Class.Valkyrie: return Valkyrie;
                default: return None;
            }
        }
    }
}