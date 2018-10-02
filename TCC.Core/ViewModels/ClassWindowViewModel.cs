using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using TCC.Data;

namespace TCC.ViewModels
{
    public class ClassWindowViewModel : TccWindowViewModel
    {
        private static ClassWindowViewModel _instance;
        public static ClassWindowViewModel Instance => _instance ?? (_instance = new ClassWindowViewModel());

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
                    switch (_currentClass)
                    {
                        case Class.Warrior:
                            CurrentManager = new WarriorBarManager();
                            break;
                        case Class.Valkyrie:
                            CurrentManager = new ValkyrieBarManager();
                            break;
                        case Class.Archer:
                            CurrentManager = new ArcherBarManager();
                            break;
                        case Class.Lancer:
                            CurrentManager = new LancerBarManager();
                            break;
                        case Class.Priest:
                            CurrentManager = new PriestBarManager();
                            break;
                        case Class.Mystic:
                            CurrentManager = new MysticBarManager();
                            break;
                        case Class.Slayer:
                            CurrentManager = new SlayerBarManager();
                            break;
                        case Class.Berserker:
                            CurrentManager = new BerserkerBarManager();
                            break;
                        case Class.Sorcerer:
                            CurrentManager = new SorcererBarManager();
                            break;
                        case Class.Reaper:
                            CurrentManager = new ReaperBarManager();
                            break;
                        case Class.Gunner:
                            CurrentManager = new GunnerBarManager();
                            break;
                        case Class.Brawler:
                            CurrentManager = new BrawlerBarManager();
                            break;
                        case Class.Ninja:
                            CurrentManager = new NinjaBarManager();
                            break;
                        default:
                            CurrentManager = new NullClassManager();
                            break;
                    }
                });
                NPC();
            }
        }

        private ClassManager _currentManager = new NullClassManager();
        public ClassManager CurrentManager
        {
            get => _currentManager;
            set
            {
                if (_currentManager == value) return;
                _currentManager = value;
                Instance.CurrentManager = _currentManager;
                NPC();
                CurrentManager.LoadSpecialSkills();
            }
        }

        public static bool ClassWindowExists()
        {
            var result = false;
            WindowManager.ClassWindow.Dispatcher.Invoke(() =>
            {
                if (((ClassWindowViewModel)WindowManager.ClassWindow.DataContext).CurrentManager != null) result = true;
                else result = false;
            });
            return result;
        }

        //public void StartCooldown(SkillCooldown skillCooldown)
        //{
        //    CurrentManager.Dispatcher.Invoke(() =>
        //    {
        //        CurrentManager.StartCooldown(skillCooldown);
        //    });
        //}
        //public void ChangeSkillCooldown(Skill skill, uint cd)
        //{
        //    CurrentManager.Dispatcher.Invoke(() =>
        //    {
        //        CurrentManager.ChangeSkillCooldown(skill, cd);
        //    });
        //}
        //public void ResetCooldown(SkillCooldown skillCooldown)
        //{
        //    CurrentManager.Dispatcher.Invoke(() =>
        //    {
        //        CurrentManager.ResetCooldown(skillCooldown);
        //    });

        //}
        //public void RemoveSkill(Skill skill)
        //{
        //    CurrentManager.Dispatcher.Invoke(() =>
        //    {
        //        CurrentManager.RemoveSkill(skill);
        //    });
        //}
        //internal void ClearSkills()
        //{
        //    if (CurrentManager == null) return;
        //    CurrentManager.Dispatcher.Invoke(() =>
        //    {
        //        CurrentManager.OtherSkills.Clear();
        //    });
        //}
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
            switch (ClassWindowViewModel.Instance.CurrentClass)
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



