using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TCC.Data;

namespace TCC.ViewModels
{
    public class ClassWindowViewModel : BaseINPC
    {
        public ClassWindowViewModel()
        {
            WindowManager.TccVisibilityChanged += (s, ev) =>
            {
                RaisePropertyChanged("IsTeraOnTop");
                if (IsTeraOnTop)
                {
                    WindowManager.ClassWindow.Dispatcher.Invoke(() =>
                    {
                        WindowManager.ClassWindow.Topmost = false;
                        WindowManager.ClassWindow.Topmost = true;
                    });
                }
            };
        }
        public bool IsTeraOnTop
        {
            get => WindowManager.IsTccVisible;
        }
        private double scale = SettingsManager.ClassWindowSettings.Scale;
        public double Scale
        {
            get
            {
                return scale;
            }
            set
            {
                if (scale == value) return;
                scale = value;
                RaisePropertyChanged("Scale");
            }
        }
        private Class _barTemplate = Class.None;
        public Class BarTemplate
        {
            get { return _barTemplate; }
            set
            {
                if (_barTemplate == value) return;
                _barTemplate = value;
                switch (_barTemplate)
                {
                    case Class.Warrior:
                        CurrentBar = WarriorBarManager.Instance;
                        break;
                    default:
                        CurrentBar = null;
                        break;
                }
                RaisePropertyChanged("BarTemplate");
            }
        }

        private ClassManager currentBar;
        public ClassManager CurrentBar
        {
            get { return currentBar; }
            set
            {
                if (currentBar == value) return;
                currentBar = value;
                RaisePropertyChanged("CurrentBar");
            }
        }

        public static bool ClassWindowExists()
        {
            var result = false;
            WindowManager.ClassWindow.Dispatcher.Invoke(() =>
            {
                if (((ClassWindowViewModel)WindowManager.ClassWindow.DataContext).CurrentBar != null) result = true;
                else result = false;
            });
            return result;
        }


        public void StartCooldown(SkillCooldown skillCooldown)
        {
            switch (SessionManager.CurrentPlayer.Class)
            {
                case Class.Warrior:
                    (CurrentBar as WarriorBarManager).StartCooldown(skillCooldown);
                    break;
                default:
                    break;
            }
        }
        public void ResetCooldown(SkillCooldown skillCooldown)
        {
            switch (SessionManager.CurrentPlayer.Class)
            {
                case Class.Warrior:
                    (CurrentBar as WarriorBarManager).ResetCooldown(skillCooldown);
                    break;
                default:
                    break;
            }

        }
        public void RemoveSkill(Skill skill)
        {
            switch (SessionManager.CurrentPlayer.Class)
            {
                case Class.Warrior:
                    (CurrentBar as WarriorBarManager).RemoveSkill(skill);
                    break;
                default:
                    break;
            }
        }

        internal void ClearSkills()
        {
            if (CurrentBar == null) return;
            CurrentBar.Dispatcher.Invoke(() =>
            {
                CurrentBar.OtherSkills.Clear();
            });
            
        }
    }

    public class ClassManager : DependencyObject
    {

        public static ClassManager CurrentClassManager;

        public static void SetHP(int hp)
        {
            if (CurrentClassManager == null) return;
            WindowManager.ClassWindow.Dispatcher.Invoke(() => { CurrentClassManager.HP.Val = hp; });

        }
        public static void SetMP(int mp)
        {
            if (CurrentClassManager == null) return;
            WindowManager.ClassWindow.Dispatcher.Invoke(() => { CurrentClassManager.MP.Val = mp; });
        }

        private SynchronizedObservableCollection<FixedSkillCooldown> mainSkills;
        public SynchronizedObservableCollection<FixedSkillCooldown> MainSkills
        {
            get
            {
                return mainSkills; ;
            }
            set
            {
                if (mainSkills == value) return;
                mainSkills = value;
            }
        }

        private SynchronizedObservableCollection<FixedSkillCooldown> secondarySkills;
        public SynchronizedObservableCollection<FixedSkillCooldown> SecondarySkills
        {
            get
            {
                return secondarySkills; ;
            }
            set
            {
                if (secondarySkills == value) return;
                secondarySkills = value;
            }
        }

        private SynchronizedObservableCollection<SkillCooldown> otherSkills;
        public SynchronizedObservableCollection<SkillCooldown> OtherSkills
        {
            get
            {
                return otherSkills;
            }
            set
            {
                if (otherSkills == value) return;
                otherSkills = value;
            }
        }

        public IntTracker HP { get; set; }
        public IntTracker MP { get; set; }

        public ClassManager()
        {
            SecondarySkills = new SynchronizedObservableCollection<Data.FixedSkillCooldown>(WindowManager.ClassWindow.Dispatcher);
            MainSkills = new SynchronizedObservableCollection<Data.FixedSkillCooldown>(WindowManager.ClassWindow.Dispatcher);
            OtherSkills = new SynchronizedObservableCollection<SkillCooldown>(WindowManager.ClassWindow.Dispatcher);
            HP = new IntTracker();
            MP = new IntTracker();
        }

    }
}



