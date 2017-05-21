using System;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
                    case Class.Archer:
                        CurrentManager = ArcherBarManager.Instance;
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
                //switch (SessionManager.CurrentPlayer.Class)
                //{
                //    case Class.Warrior:
                //        (CurrentManager as WarriorBarManager).StartCooldown(skillCooldown);
                //        break;
                //    case Class.Glaiver:
                //        (CurrentManager as ValkyrieBarManager).StartCooldown(skillCooldown);
                //        break;
                //    default:
                //        break;
                //}
                CurrentManager.StartCooldown(skillCooldown);
            });
        }

        public void ResetCooldown(SkillCooldown skillCooldown)
        {
            CurrentManager.Dispatcher.Invoke(() =>
            {
                //switch (SessionManager.CurrentPlayer.Class)
                //{
                //    case Class.Warrior:
                //        (CurrentManager as WarriorBarManager).ResetCooldown(skillCooldown);
                //        break;
                //    case Class.Glaiver:
                //        (CurrentManager as ValkyrieBarManager).ResetCooldown(skillCooldown);
                //        break;
                //    default:
                //        break;
                //}
                CurrentManager.ResetCooldown(skillCooldown);
            });

        }
        public void RemoveSkill(Skill skill)
        {
            CurrentManager.Dispatcher.Invoke(() =>
            {
                CurrentManager.RemoveSkill(skill);
                //switch (SessionManager.CurrentPlayer.Class)
                //{
                //    case Class.Warrior:
                //        (CurrentManager as WarriorBarManager).RemoveSkill(skill);
                //        break;
                //    case Class.Glaiver:
                //        (CurrentManager as ValkyrieBarManager).RemoveSkill(skill);
                //        break;
                //    default:
                //        break;
                //}
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
}



