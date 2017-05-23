using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using TCC.Parsing.Messages;

namespace TCC.ViewModels
{

    public class CooldownBarWindowManager : DependencyObject
    {
        private static CooldownBarWindowManager _instance;
        public static CooldownBarWindowManager Instance => _instance ?? (_instance = new CooldownBarWindowManager());

        private SynchronizedObservableCollection<SkillCooldown> _shortSkills;
        public SynchronizedObservableCollection<SkillCooldown> ShortSkills
        {
            get { return _shortSkills; }
            set
            {
                if (_shortSkills == value) return;
                _shortSkills = value;
            }
        }
        private SynchronizedObservableCollection<SkillCooldown> _longSkills;
        public SynchronizedObservableCollection<SkillCooldown> LongSkills
        {
            get { return _longSkills; }
            set
            {
                if (_longSkills == value) return;
                _longSkills = value;
            }
        }

        public void AddOrRefreshSkill(SkillCooldown sk)
        {
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
                        if(existing == null)
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
        public void RemoveSkill(Skill sk)
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
    }
}
