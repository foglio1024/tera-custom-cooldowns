using System;
using System.Linq;
using System.Windows;
using TCC.Data;

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
        public int VisibleBossesCount
        {
            get => CurrentNPCs.Where(x => x.Visible == Visibility.Visible).Count();
        }
        public void AddOrUpdateBoss(ulong entityId, float maxHp, float curHp, uint templateId = 0, uint zoneId = 0, Visibility v = Visibility.Visible)
        {

            var boss = _bosses.FirstOrDefault(x => x.EntityId == entityId);
            if (boss == null)
            {
                //if (!EntitiesManager.TryGetBossById(entityId, out Boss b)) return;
                boss = new Boss(entityId, zoneId, templateId, v);
                _bosses.Add(boss);
            }
            boss.MaxHP = maxHp;
            boss.CurrentHP = curHp;
            boss.Visible = v;         
        }

        public void RemoveBoss(ulong id)
        {
            var boss = _bosses.FirstOrDefault(x => x.EntityId == id);
            if (boss == null) return;
            _bosses.Remove(boss);
            boss.Dispose();
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

        internal void EndNpcAbnormality(ulong target, Abnormality ab)
        {
            var boss = _bosses.FirstOrDefault(x => x.EntityId == target);
            if(boss != null)
            {
                boss.EndBuff(ab);
            }
        }

        internal void AddOrRefreshNpcAbnormality(Abnormality ab, int stacks, uint duration, ulong target, double size, double margin)
        {
            var boss = _bosses.FirstOrDefault(x => x.EntityId == target);
            if (boss != null)
            {
                boss.AddorRefresh(ab, duration, stacks, size, margin);
            }
        }

        internal void SetBossEnrage(ulong entityId, bool enraged)
        {
            var boss = _bosses.FirstOrDefault(x => x.EntityId == entityId);
            if (boss == null)
            {
                return;
            }
            boss.Enraged = enraged;
        }

        internal void UnsetBossTarget(ulong entityId)
        {
            var boss = _bosses.FirstOrDefault(x => x.EntityId == entityId);
            if (boss == null)
            {
                return;
            }
            boss.Target = 0;
        }

        internal void SetBossAggro(ulong entityId, AggroCircle circle, ulong user)
        {
            var boss = _bosses.FirstOrDefault(x => x.EntityId == entityId);
            if (boss == null)
            {
                return;
            }
            boss.Target = user;
            boss.CurrentAggroType = AggroCircle.Main;
        }
    }
}
