using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TCC.Data;

namespace TCC.ViewModels
{
    public abstract class ClassManager : DependencyObject
    {

        public static ClassManager CurrentClassManager;

        public virtual bool StartSpecialSkill(SkillCooldown sk)
        {
            return false;
        }

        protected abstract void LoadSpecialSkills();



        public StatTracker HP { get; set; }
        public StatTracker MP { get; set; }
        public StatTracker ST { get; set; }

        public static void SetMaxHP(int v)
        {
            if (CurrentClassManager == null) return;
            CurrentClassManager.Dispatcher.Invoke(() => { CurrentClassManager.HP.Max = v; });

        }
        public static void SetMaxMP(int v)
        {
            if (CurrentClassManager == null) return;
            CurrentClassManager.Dispatcher.Invoke(() => { CurrentClassManager.MP.Max = v; });
        }
        public static void SetMaxST(int v)
        {
            if (CurrentClassManager == null) return;
            CurrentClassManager.Dispatcher.Invoke(() => { CurrentClassManager.ST.Max = v; });
        }

        public static void SetHP(int hp)
        {
            if (CurrentClassManager == null) return;
            CurrentClassManager.Dispatcher.Invoke(() => { CurrentClassManager.HP.Val = hp; });

        }
        public static void SetMP(int mp)
        {
            if (CurrentClassManager == null) return;
            CurrentClassManager.Dispatcher.Invoke(() => { CurrentClassManager.MP.Val = mp; });
        }
        public static void SetST(int currentStamina)
        {
            if (CurrentClassManager == null) return;
            CurrentClassManager.Dispatcher.Invoke(() => { CurrentClassManager.ST.Val = currentStamina; });
        }

        private static List<uint> _debuffs;
        public static void SetStatus(Abnormality ab, bool adding)
        {
            if (CurrentClassManager == null || ab.IsBuff) return;

            if (adding)
            {
                if (!_debuffs.Contains(ab.Id))
                {
                    _debuffs.Add(ab.Id);
                }
            }
            else
            {
                _debuffs.Remove(ab.Id);
            }

            bool status = _debuffs.Count == 0 ? false : true;
            CurrentClassManager.Dispatcher.Invoke(() => { CurrentClassManager.HP.Status = status; });
        }
        public ClassManager()
        {
            HP = new StatTracker();
            MP = new StatTracker();
            ST = new StatTracker();
            _debuffs = new List<uint>();
        }

    }
}



