using System;
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

        public void StartCooldown(SkillCooldown sk)
        {
            var skill = MainSkills.FirstOrDefault(x => x.Skill.IconName == sk.Skill.IconName);
            if (skill != null)
            {
                skill.Start(sk.Cooldown);

                return;
            }
            skill = SecondarySkills.FirstOrDefault(x => x.Skill.IconName == sk.Skill.IconName);
            if (skill != null)
            {
                skill.Start(sk.Cooldown);
                return;
            }

            if (StartSpecialSkill(sk)) return;

            AddOrRefreshSkill(sk);
        }
        protected void AddOrRefreshSkill(SkillCooldown sk)
        {
            sk.SetDispatcher(Dispatcher);

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

        public virtual void ResetCooldown(SkillCooldown sk)
        {
            sk.SetDispatcher(Dispatcher);
            var skill = MainSkills.FirstOrDefault(x => x.Skill.IconName == sk.Skill.IconName);
            if (skill != null)
            {
                skill.Refresh(0);
                return;
            }
            skill = SecondarySkills.FirstOrDefault(x => x.Skill.IconName == sk.Skill.IconName);
            if (skill != null)
            {
                skill.Refresh(0);
                return;
            }
            try
            {
                var otherSkill = OtherSkills.FirstOrDefault(x => x.Skill.Name == sk.Skill.Name);
                if (otherSkill != null)
                {

                    OtherSkills.Remove(otherSkill);
                    otherSkill.Dispose();
                }
            }
            catch { }
        }
        public virtual void RemoveSkill(Skill sk)
        {
            try
            {
                var otherSkill = OtherSkills.FirstOrDefault(x => x.Skill.Name == sk.Name);
                if (otherSkill != null)
                {

                    OtherSkills.Remove(otherSkill);
                    otherSkill.Dispose();
                }
            }
            catch
            {

            }
        }
        protected virtual bool StartSpecialSkill(SkillCooldown sk)
        {
            return false;
        }

        protected abstract void LoadSkills(string filename, Class c);

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

        public ClassManager()
        {
            SecondarySkills = new SynchronizedObservableCollection<FixedSkillCooldown>(WindowManager.ClassWindow.Dispatcher);
            MainSkills = new SynchronizedObservableCollection<FixedSkillCooldown>(WindowManager.ClassWindow.Dispatcher);
            OtherSkills = new SynchronizedObservableCollection<SkillCooldown>(WindowManager.ClassWindow.Dispatcher);
            HP = new StatTracker();
            MP = new StatTracker();
            ST = new StatTracker();
        }

    }
}



