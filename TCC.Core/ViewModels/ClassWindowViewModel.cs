using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using TeraDataLite;

namespace TCC.ViewModels
{
    public class ClassWindowViewModel : TccWindowViewModel
    {
        //private static ClassWindowViewModel _instance;
        //public static ClassWindowViewModel Instance => _instance ?? (_instance = new ClassWindowViewModel());

        public ClassWindowViewModel()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
        }
        //public bool IsTeraOnTop => WindowManager.IsTccVisible;
        private Class _currentClass = Class.None;
        public Class CurrentClass
        {
            get => _currentClass;
            set
            {
                if (_currentClass == value) return;
                _currentClass = value;
                Dispatcher.Invoke(() =>
                {
                    CurrentManager.Dispose();
                    switch (_currentClass)
                    {
                        case Class.Warrior:
                            CurrentManager = new WarriorLayoutVM();
                            break;
                        case Class.Valkyrie:
                            CurrentManager = new ValkyrieLayoutVM();
                            break;
                        case Class.Archer:
                            CurrentManager = new ArcherLayoutVM();
                            break;
                        case Class.Lancer:
                            CurrentManager = new LancerLayoutVM();
                            break;
                        case Class.Priest:
                            CurrentManager = new PriestLayoutVM();
                            break;
                        case Class.Mystic:
                            CurrentManager = new MysticLayoutVM();
                            break;
                        case Class.Slayer:
                            CurrentManager = new SlayerLayoutVM();
                            break;
                        case Class.Berserker:
                            CurrentManager = new BerserkerLayoutVM();
                            break;
                        case Class.Sorcerer:
                            CurrentManager = new SorcererLayoutVM();
                            break;
                        case Class.Reaper:
                            CurrentManager = new ReaperLayoutVM();
                            break;
                        case Class.Gunner:
                            CurrentManager = new GunnerLayoutVM();
                            break;
                        case Class.Brawler:
                            CurrentManager = new BrawlerLayoutVM();
                            break;
                        case Class.Ninja:
                            CurrentManager = new NinjaLayoutVM();
                            break;
                        default:
                            CurrentManager = new NullClassManager();
                            break;
                    }
                });
                N();
            }
        }

        private BaseClassLayoutVM _currentManager = new NullClassManager();
        public BaseClassLayoutVM CurrentManager
        {
            get => _currentManager;
            set
            {
                if (_currentManager == value) return;
                _currentManager = value;
                CurrentManager = _currentManager;
                N();
                CurrentManager.LoadSpecialSkills();
            }
        }
    }

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
            switch (WindowManager.ClassWindow.VM.CurrentClass)
                {
                    case Class.Warrior:
                        return Warrior;
                    case Class.Lancer:
                        return Lancer;
                    case Class.Slayer:
                        return Slayer;
                    case Class.Berserker:
                        return Berserker;
                    case Class.Sorcerer:
                        return Sorcerer;
                    case Class.Archer:
                        return Archer;
                    case Class.Priest:
                        return Priest;
                    case Class.Mystic:
                        return Mystic;
                    case Class.Reaper:
                        return Reaper;
                    case Class.Gunner:
                        return Gunner;
                    case Class.Brawler:
                        return Brawler;
                    case Class.Ninja:
                        return Ninja;
                    case Class.Valkyrie:
                        return Valkyrie;
                    default:
                        return None;
                }
        }
    }
}



