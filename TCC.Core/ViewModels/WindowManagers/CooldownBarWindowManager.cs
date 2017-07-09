using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using TCC.Data;
using TCC.Parsing.Messages;

namespace TCC.ViewModels
{

    public class CooldownWindowManager : DependencyObject
    {
        private static CooldownWindowManager _instance;
        public static CooldownWindowManager Instance => _instance ?? (_instance = new CooldownWindowManager());

        private SynchronizedObservableCollection<SkillCooldown> _shortSkills;
        private SynchronizedObservableCollection<SkillCooldown> _longSkills;
        private SynchronizedObservableCollection<FixedSkillCooldown> mainSkills;
        private SynchronizedObservableCollection<FixedSkillCooldown> secondarySkills;
        private SynchronizedObservableCollection<SkillCooldown> otherSkills;

        public SynchronizedObservableCollection<SkillCooldown> ShortSkills
        {
            get { return _shortSkills; }
            set
            {
                if (_shortSkills == value) return;
                _shortSkills = value;
            }
        }
        public SynchronizedObservableCollection<SkillCooldown> LongSkills
        {
            get { return _longSkills; }
            set
            {
                if (_longSkills == value) return;
                _longSkills = value;
            }
        }
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

        private static ClassManager _classManager => ClassManager.CurrentClassManager;

        private void NormalCd_AddOrRefreshSkill(SkillCooldown sk)
        {

            if (_classManager.StartSpecialSkill(sk) && SettingsManager.ClassWindowSettings.Visibility == Visibility.Visible) return;

            try
            {
                if (sk.Cooldown < SkillManager.LongSkillTreshold)
                {
                    var existing = _shortSkills.FirstOrDefault(x => x.Skill.Name == sk.Skill.Name);
                    if (existing == null)
                    {
                        _shortSkills.Add(sk);
                        return;
                    }
                    existing.Refresh(sk.Cooldown);
                }
                else
                {
                    var existing = _longSkills.FirstOrDefault(x => x.Skill.Name == sk.Skill.Name);
                    if (existing == null)
                    {
                        existing = _shortSkills.FirstOrDefault(x => x.Skill.Name == sk.Skill.Name);
                        if (existing == null)
                        {
                            _longSkills.Add(sk);
                        }
                        else
                        {
                            existing.Refresh(sk.Cooldown);
                        }
                        return;
                    }
                    existing.Refresh(sk.Cooldown);
                }
            }
            catch
            {

            }
        }
        private void NormalCd_RefreshSkill(Skill skill, uint cd)
        {
            SkillCooldown sk;
            try
            {
                if (cd < SkillManager.LongSkillTreshold)
                {
                    var existing = _shortSkills.FirstOrDefault(x => x.Skill.Name == skill.Name);
                    if (existing == null)
                    {
                        sk = new SkillCooldown(skill, cd, CooldownType.Skill, Dispatcher);
                        _shortSkills.Add(sk);
                        return;
                    }
                    existing.Refresh(cd);
                }
                else
                {
                    var existing = _longSkills.FirstOrDefault(x => x.Skill.Name == skill.Name);
                    if (existing == null)
                    {
                        existing = _shortSkills.FirstOrDefault(x => x.Skill.Name == skill.Name);
                        if (existing == null)
                        {
                            sk = new SkillCooldown(skill, cd, CooldownType.Skill, Dispatcher);
                            _longSkills.Add(sk);
                        }
                        else
                        {
                            existing.Refresh(cd);
                        }
                        return;
                    }
                    existing.Refresh(cd);
                }
            }
            catch
            {

            }
        }
        private void NormalCd_RemoveSkill(Skill sk)
        {
            try
            {

                var longSkill = _longSkills.FirstOrDefault(x => x.Skill.Name == sk.Name);
                if (longSkill != null)
                {
                    _longSkills.Remove(longSkill);
                    longSkill.Dispose();
                }
                var shortSkill = _shortSkills.FirstOrDefault(x => x.Skill.Name == sk.Name);
                if (shortSkill != null)
                {

                    _shortSkills.Remove(shortSkill);
                    shortSkill.Dispose();
                }
            }
            catch
            {

            }
        }

        private void FixedCd_AddOrRefreshSkill(SkillCooldown sk)
        {
            if (_classManager.StartSpecialSkill(sk) && SettingsManager.ClassWindowSettings.Visibility == Visibility.Visible) return;

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


            AddOrRefreshOtherSkill(sk);
        }
        private void FixedCd_RefreshSkill(Skill sk, uint cd)
        {
            var skill = MainSkills.FirstOrDefault(x => x.Skill.IconName == sk.IconName);
            if (skill != null)
            {
                skill.Refresh(cd);
                return;
            }
            skill = SecondarySkills.FirstOrDefault(x => x.Skill.IconName == sk.IconName);
            if (skill != null)
            {
                skill.Refresh(cd);
                return;
            }
            try
            {
                var otherSkill = OtherSkills.FirstOrDefault(x => x.Skill.Name == sk.Name);
                if (otherSkill != null)
                {

                    //OtherSkills.Remove(otherSkill);
                    otherSkill.Refresh(cd);
                }
            }
            catch { }
        }
        private void FixedCd_RemoveSkill(Skill sk)
        {
            //sk.SetDispatcher(Dispatcher);
            var skill = MainSkills.FirstOrDefault(x => x.Skill.IconName == sk.IconName);
            if (skill != null)
            {
                skill.Refresh(0);
                return;
            }
            skill = SecondarySkills.FirstOrDefault(x => x.Skill.IconName == sk.IconName);
            if (skill != null)
            {
                skill.Refresh(0);
                return;
            }
            try
            {
                var otherSkill = OtherSkills.FirstOrDefault(x => x.Skill.Name == sk.Name);
                if (otherSkill != null)
                {

                    OtherSkills.Remove(otherSkill);
                    otherSkill.Dispose();
                }
            }
            catch { }
        }

        private void AddOrRefreshOtherSkill(SkillCooldown sk)
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
                Debug.WriteLine("Error while refreshing skill");
            }
        }

        public void AddOrRefreshSkill(SkillCooldown sk)
        {
            if (SettingsManager.ClassWindowOn)
            {
                FixedCd_AddOrRefreshSkill(sk);
            }
            else
            {
                NormalCd_AddOrRefreshSkill(sk);
            }
        }
        public void RefreshSkill(Skill skill, uint cd)
        {
            if (SettingsManager.ClassWindowOn)
            {
                FixedCd_RefreshSkill(skill, cd);
            }
            else
            {
                NormalCd_RefreshSkill(skill, cd);
            }
        }
        public void RemoveSkill(Skill sk)
        {
            if (SettingsManager.ClassWindowOn)
            {
                FixedCd_RemoveSkill(sk);
            }
            else
            {
                NormalCd_RemoveSkill(sk);
            }
        }

        public void ClearSkills()
        {
            ShortSkills.Clear();
            LongSkills.Clear();
            MainSkills.Clear();
            SecondarySkills.Clear();
            OtherSkills.Clear();
        }

        public void LoadSkills(string filename, Class c)
        {
            if (!File.Exists("resources/config/" + filename))
            {
                SkillUtils.BuildDefaultSkillConfig(filename, c);
            }
            var sp = new SkillConfigParser(filename, c);
            foreach (var sk in sp.Main)
            {
                MainSkills.Add(sk);
            }
            foreach (var sk in sp.Secondary)
            {
                SecondarySkills.Add(sk);
            }
        }

        public CooldownWindowManager()
        {
            ShortSkills = new SynchronizedObservableCollection<SkillCooldown>(Dispatcher);
            LongSkills = new SynchronizedObservableCollection<SkillCooldown>(Dispatcher);
            SecondarySkills = new SynchronizedObservableCollection<FixedSkillCooldown>(Dispatcher);
            MainSkills = new SynchronizedObservableCollection<FixedSkillCooldown>(Dispatcher);
            OtherSkills = new SynchronizedObservableCollection<SkillCooldown>(Dispatcher);

        }
    }
}
