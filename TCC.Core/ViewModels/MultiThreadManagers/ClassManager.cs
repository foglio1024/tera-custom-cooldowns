using System.Linq;
using System.Windows;
using TCC.Data;

namespace TCC.ViewModels
{
    public abstract class ClassManager : DependencyObject
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

        public abstract void StartCooldown(SkillCooldown sk);
        public abstract void ResetCooldown(SkillCooldown sk);
        public abstract void RemoveSkill(Skill sk);

        protected abstract void LoadSkills(string filename, Class c);

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
        protected void AddOrRefreshSkill(SkillCooldown sk)
        {
            sk.SetDispatcher(this.Dispatcher);

            try
            {
                var existing = OtherSkills.FirstOrDefault(x => x.Skill.Name == sk.Skill.Name);
                if (existing == null)
                {
                    OtherSkills.Add(sk);
                    return;
                }
                existing.Refresh(sk.Cooldown);
            }
            catch
            {

            }
        }

    }
}



