using System.Windows.Threading;

namespace TCC.ViewModels
{
    public class ClassWindowViewModel : TccWindowViewModel
    {
        private static ClassWindowViewModel _instance;
        public static ClassWindowViewModel Instance => _instance ?? (_instance = new ClassWindowViewModel());

        public ClassWindowViewModel()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            _scale = SettingsManager.ClassWindowSettings.Scale;
            WindowManager.TccVisibilityChanged += (s, ev) =>
            {
                NotifyPropertyChanged("IsTeraOnTop");
                if (IsTeraOnTop)
                {
                    WindowManager.ClassWindow.RefreshTopmost();
                }
            };

            _dispatcher = Dispatcher.CurrentDispatcher;
        }
        public bool IsTeraOnTop
        {
            get => WindowManager.IsTccVisible;
        }
        private Class currentClass = Class.None;
        public Class CurrentClass
        {
            get { return currentClass; }
            set
            {
                if (currentClass == value) return;
                currentClass = value;
                WindowManager.ClassWindow.Dispatcher.Invoke(() =>
                {
                    switch (currentClass)
                    {
                        case Class.Warrior:
                            CurrentManager = WarriorBarManager.Instance;
                            break;
                        case Class.Glaiver:
                            CurrentManager = ValkyrieBarManager.Instance;
                            break;
                        case Class.Archer:
                            CurrentManager = ArcherBarManager.Instance;
                            break;
                        case Class.Lancer:
                            CurrentManager = LancerBarManager.Instance;
                            break;
                        case Class.Priest:
                            CurrentManager = PriestBarManager.Instance;
                            break;
                        case Class.Elementalist:
                            CurrentManager = MysticBarManager.Instance;
                            break;
                        case Class.Slayer:
                            CurrentManager = SlayerBarManager.Instance;
                            break;
                        case Class.Berserker:
                            CurrentManager = BerserkerBarManager.Instance;
                            break;
                        case Class.Sorcerer:
                            CurrentManager = SorcererBarManager.Instance;
                            break;
                        case Class.Soulless:
                            CurrentManager = ReaperBarManager.Instance;
                            break;
                        case Class.Engineer:
                            CurrentManager = GunnerBarManager.Instance;
                            break;
                        case Class.Fighter:
                            CurrentManager = BrawlerBarManager.Instance;
                            break;
                        case Class.Assassin:
                            CurrentManager = NinjaBarManager.Instance;
                            break;
                        default:
                            CurrentManager = null;
                            break;
                    }
                });
                NotifyPropertyChanged("CurrentClass");
            }
        }

        private ClassManager currentManager;
        public ClassManager CurrentManager
        {
            get { return currentManager; }
            set
            {
                if (currentManager == value) return;
                currentManager = value;
                ClassManager.CurrentClassManager = currentManager;
                NotifyPropertyChanged("CurrentManager");
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
}



