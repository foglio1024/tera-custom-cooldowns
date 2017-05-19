using System;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        private Class currentClass = Class.None;
        public Class CurrentClass
        {
            get { return currentClass; }
            set
            {
                if (currentClass == value) return;
                currentClass = value;
                RaisePropertyChanged("CurrentClass");
                switch (currentClass)
                {
                    case Class.Warrior:
                        CurrentManager = WarriorBarManager.Instance;
                        break;
                    case Class.Glaiver:
                        CurrentManager = ValkyrieBarManager.Instance;
                        break;
                    default:
                        CurrentManager = null;
                        break;
                }
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
                RaisePropertyChanged("CurrentManager");
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

        public void StartCooldown(SkillCooldown skillCooldown)
        {
            CurrentManager.Dispatcher.Invoke(() =>
            {
                switch (SessionManager.CurrentPlayer.Class)
                {
                    case Class.Warrior:
                        (CurrentManager as WarriorBarManager).StartCooldown(skillCooldown);
                        break;
                    case Class.Glaiver:
                        (CurrentManager as ValkyrieBarManager).StartCooldown(skillCooldown);
                        break;
                    default:
                        break;
                }
            });
        }

        public void ResetCooldown(SkillCooldown skillCooldown)
        {
            CurrentManager.Dispatcher.Invoke(() =>
            {
                switch (SessionManager.CurrentPlayer.Class)
                {
                    case Class.Warrior:
                        (CurrentManager as WarriorBarManager).ResetCooldown(skillCooldown);
                        break;
                    case Class.Glaiver:
                        (CurrentManager as ValkyrieBarManager).ResetCooldown(skillCooldown);
                        break;
                    default:
                        break;
                }
            });

        }
        public void RemoveSkill(Skill skill)
        {
            CurrentManager.Dispatcher.Invoke(() =>
            {
                switch (SessionManager.CurrentPlayer.Class)
                {
                    case Class.Warrior:
                        (CurrentManager as WarriorBarManager).RemoveSkill(skill);
                        break;
                    case Class.Glaiver:
                        (CurrentManager as ValkyrieBarManager).RemoveSkill(skill);
                        break;
                    default:
                        break;
                }
            });
        }

        internal void ClearSkills()
        {
            if (CurrentManager == null) return;
            CurrentManager.Dispatcher.Invoke(() =>
            {
                CurrentManager.OtherSkills.Clear();
            });

        }
    }

    public class ClassManager : DependencyObject
    {

        public static ClassManager CurrentClassManager;


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
        public IntTracker ST { get; set; }

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
        public static void SetST(int currentStamina)
        {
            if (CurrentClassManager == null) return;
            WindowManager.ClassWindow.Dispatcher.Invoke(() => { CurrentClassManager.ST.Val = currentStamina; });
        }

        public ClassManager()
        {
            SecondarySkills = new SynchronizedObservableCollection<FixedSkillCooldown>(WindowManager.ClassWindow.Dispatcher);
            MainSkills = new SynchronizedObservableCollection<FixedSkillCooldown>(WindowManager.ClassWindow.Dispatcher);
            OtherSkills = new SynchronizedObservableCollection<SkillCooldown>(WindowManager.ClassWindow.Dispatcher);
            HP = new IntTracker();
            MP = new IntTracker();
            ST = new IntTracker();
        }

    }
}



