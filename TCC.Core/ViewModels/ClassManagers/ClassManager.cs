using System.Collections.Generic;
using System.Windows.Threading;
using TCC.Data;

namespace TCC.ViewModels
{
    public abstract class ClassManager : TSPropertyChanged
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
            if (CurrentClassManager == null || !SettingsManager.ClassWindowSettings.Enabled) return;
            CurrentClassManager.GetDispatcher().Invoke(() => { CurrentClassManager.HP.Max = v; });

        }
        public static void SetMaxMP(int v)
        {
            if (CurrentClassManager == null || !SettingsManager.ClassWindowSettings.Enabled) return;
            CurrentClassManager.GetDispatcher().Invoke(() => { CurrentClassManager.MP.Max = v; });
        }
        public static void SetMaxST(int v)
        {
            if (CurrentClassManager == null || !SettingsManager.ClassWindowSettings.Enabled) return;
            CurrentClassManager.GetDispatcher().Invoke(() => { CurrentClassManager.ST.Max = v; });
        }

        public static void SetHP(int hp)
        {
            if (CurrentClassManager == null || !SettingsManager.ClassWindowSettings.Enabled) return;
            CurrentClassManager.GetDispatcher().Invoke(() => { CurrentClassManager.HP.Val = hp; });

        }
        public static void SetMP(int mp)
        {
            if (CurrentClassManager == null || !SettingsManager.ClassWindowSettings.Enabled) return;
            CurrentClassManager.GetDispatcher().Invoke(() => { CurrentClassManager.MP.Val = mp; });
        }
        public static void SetST(int currentStamina)
        {
            if (CurrentClassManager == null || !SettingsManager.ClassWindowSettings.Enabled) return;
            CurrentClassManager.GetDispatcher().Invoke(() => { CurrentClassManager.ST.Val = currentStamina; });
        }

        private static List<uint> _debuffs;
        public static void SetStatus(Abnormality ab, bool adding)
        {
            if (CurrentClassManager == null || ab.IsBuff || !SettingsManager.ClassWindowSettings.Enabled) return;

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

            var status = _debuffs.Count == 0 ? false : true;
            CurrentClassManager.GetDispatcher().Invoke(() => { CurrentClassManager.HP.Status = status; });
        }
        public ClassManager()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            HP = new StatTracker();
            MP = new StatTracker();
            ST = new StatTracker();
            _debuffs = new List<uint>();
        }

    }
}



