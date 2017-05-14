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
}
