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
using TCC.Data;
using TCC.Messages;

namespace TCC.ViewModels
{

    public class BossGageWindowManager : DependencyObject
    {
        private static BossGageWindowManager _instance;
        public static BossGageWindowManager Instance => _instance ?? (_instance = new BossGageWindowManager());

        private SynchronizedObservableCollection<Boss> _bosses;
        public SynchronizedObservableCollection<Boss> CurrentNPCs
        {
            get
            {
                if (SessionManager.HarrowholdMode)
                {
                    return null;
                }
                else
                {
                    return _bosses;
                }
            }
            set
            {
                if (_bosses == value) return;
                _bosses = value;
            }
        }

        public void AddOrUpdateBoss(ulong entityId, float maxHp, float curHp)
        {
            var boss = _bosses.FirstOrDefault(x => x.EntityId == entityId);
            if (boss == null)
            {
                if (!EntitiesManager.TryGetBossById(entityId, out Boss b)) return;
                boss = b;
                _bosses.Add(b);
            }
            boss.MaxHP = maxHp;
            boss.CurrentHP = curHp;
        }

        public void RemoveBoss(ulong id)
        {
            var boss = _bosses.FirstOrDefault(x => x.EntityId == id);
            if (boss == null) return;
            _bosses.Remove(boss);
        }

        public void ClearBosses()
        {
            this.Dispatcher.Invoke(() =>
            {
                _bosses.Clear();
            });
        }

        public BossGageWindowManager()
        {

        }
    }

    public class BuffBarWindowManager : DependencyObject
    {
        private static BuffBarWindowManager _instance;
        public static BuffBarWindowManager Instance => _instance ?? (_instance = new BuffBarWindowManager());

        private Player _player;
        public Player Player
        {
            get { return _player; }
            set
            {
                if (_player == value) return;
                _player = value;
            }
        }
    }

    public class CharacterWindowManager :DependencyObject
    {
        private static CharacterWindowManager _instance;
        public static CharacterWindowManager Instance => _instance ?? (_instance = new CharacterWindowManager());

        private Player _player;
        public Player Player
        {
            get { return _player; }
            set
            {
                if (_player == value) return;
                _player = value;
            }
        }
    }

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
            if (sk.Cooldown < SkillManager.LongSkillTreshold)
            {
                var existing = _shortSkills.FirstOrDefault(x => x.Skill.Name == sk.Skill.Name);
                if (existing == null)
                {
                    _shortSkills.Add(sk);
                    return;
                }
                existing.Cooldown = sk.Cooldown;
                existing.Refresh();
            }
            else
            {
                var existing = _longSkills.FirstOrDefault(x => x.Skill.Name == sk.Skill.Name);
                if (existing == null)
                {
                    _longSkills.Add(sk);
                    return;
                }
                existing.Cooldown = sk.Cooldown;
                existing.Refresh();
            }
        }
        public void RemoveSkill(Skill sk)
        {
            var shortSkill = _shortSkills.FirstOrDefault(x => x.Skill.Name == sk.Name);
            if (shortSkill != null)
            {
                
                _shortSkills.Remove(shortSkill);
                shortSkill.Dispose();
                return;
            }
            var longSkill = _longSkills.FirstOrDefault(x => x.Skill.Name == sk.Name);
            if (longSkill != null)
            {
                _longSkills.Remove(longSkill);
                longSkill.Dispose();
            }
        }
    }
}
