using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using TCC.Data;

namespace TCC.ViewModels
{
    public class BossGageWindowViewModel : TSPropertyChanged
    {
        private static BossGageWindowViewModel _instance;
        public static BossGageWindowViewModel Instance => _instance ?? (_instance = new BossGageWindowViewModel());
        public bool HarrowholdMode
        {
            get => SessionManager.HarrowholdMode;
        }
        private HarrowholdPhase _currentHHphase;
        public HarrowholdPhase CurrentHHphase
        {
            get => _currentHHphase;
            set
            {
                if (_currentHHphase == value) return;
                _currentHHphase = value;

                NotifyPropertyChanged(nameof(CurrentHHphase));
            }
        }
        private void SessionManager_HhModeChanged(bool val)
        {
            NotifyPropertyChanged("CurrentNPCs");
            NotifyPropertyChanged("HarrowholdMode");
        }
        public bool IsTeraOnTop
        {
            get => WindowManager.IsTccVisible;
        }
        private ICollectionView _bams;
        public ICollectionView Bams
        {
            get
            {
                _bams = new CollectionViewSource { Source = _bosses }.View;
                _bams.Filter = p => ((Boss)p).IsBoss == true;
                return _bams;
            }
        }
        private ICollectionView _mobs;
        public ICollectionView Mobs
        {
            get
            {
                _mobs = new CollectionViewSource { Source = _bosses }.View;
                _mobs.Filter = p => ((Boss)p).IsBoss == false;
                return _mobs;
            }
        }
        private ICollectionView _dragons;
        public ICollectionView Dragons
        {
            get
            {
                _dragons = new CollectionViewSource { Source = _bosses }.View;
                _dragons.Filter = p => ((Boss)p).TemplateId > 1099 && ((Boss)p).TemplateId < 1104;
                return _dragons;
            }
        }
        private double scale = SettingsManager.BossGaugeWindowSettings.Scale;
        public double Scale
        {
            get { return scale; }
            set
            {
                if (scale == value) return;
                scale = value;
                NotifyPropertyChanged("Scale");
            }
        }

        private SynchronizedObservableCollection<Boss> _bosses;
        public SynchronizedObservableCollection<Boss> CurrentNPCs
        {
            get
            {
                return _bosses;
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
        public void AddOrUpdateBoss(ulong entityId, float maxHp, float curHp, bool isBoss, uint templateId = 0, uint zoneId = 0, Visibility v = Visibility.Visible)
        {
            var boss = _bosses.FirstOrDefault(x => x.EntityId == entityId);
            if (boss == null)
            {
                if (SettingsManager.ShowOnlyBosses && !isBoss) return;

                if (templateId == 0 || zoneId == 0) return;

                boss = new Boss(entityId, zoneId, templateId, isBoss, v);
                if (Utils.IsPhase1Dragon(zoneId, templateId))
                {
                    var d = _holdedDragons.FirstOrDefault(x => x.EntityId == entityId);
                    if (d == null)
                    {
                        _holdedDragons.Add(boss);
                        if (_holdedDragons.Count == 4)
                        {
                            AddSortedDragons();
                        }
                    }
                }
                else
                {
                    if (boss.ZoneId == 950 && (boss.TemplateId == 1000 || boss.TemplateId == 2000 || boss.TemplateId == 3000 || boss.TemplateId == 4000)) Vergos = boss;
                    _bosses.Add(boss);
                }

            }
            boss.MaxHP = maxHp;
            boss.CurrentHP = curHp;
            boss.Visible = v;
        }

        public BossGageWindowViewModel()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            _bosses = new SynchronizedObservableCollection<Boss>(_dispatcher);
            SessionManager.HhModeChanged += SessionManager_HhModeChanged;
            WindowManager.TccVisibilityChanged += (s, ev) =>
            {
                NotifyPropertyChanged("IsTeraOnTop");
                if (IsTeraOnTop)
                {
                    WindowManager.BossGauge.RefreshTopmost();
                }
            };

        }
        private void AddSortedDragons()
        {
            _bosses.Add(_holdedDragons.First(x => x.TemplateId == 1102));
            _bosses.Add(_holdedDragons.First(x => x.TemplateId == 1100));
            _bosses.Add(_holdedDragons.First(x => x.TemplateId == 1101));
            _bosses.Add(_holdedDragons.First(x => x.TemplateId == 1103));
            _holdedDragons.Clear();
        }
        private Boss selectedDragon;
        public Boss SelectedDragon
        {
            get => selectedDragon;
            set
            {
                if (selectedDragon == value) return;
                selectedDragon = value;
                NotifyPropertyChanged(nameof(SelectedDragon));
            }
        }

        private Boss _vergos;
        public Boss Vergos
        {
            get => _vergos;
            set
            {
                if (_vergos == value) return;
                _vergos = value;
                NotifyPropertyChanged(nameof(Vergos));
            }
        }
        private List<Boss> _holdedDragons = new List<Boss>();
        public void RemoveBoss(ulong id)
        {
            var boss = _bosses.FirstOrDefault(x => x.EntityId == id);
            if (boss == null) return;
            _bosses.Remove(boss);
            boss.Dispose();
            if (SelectedDragon != null && SelectedDragon.EntityId == id) SelectedDragon = null;
        }
        public void CopyToClipboard()
        {
            var sb = new StringBuilder();
            foreach (var boss in _bosses)
            {
                if (boss.Visible == Visibility.Visible)
                {
                    sb.Append(boss.Name);
                    sb.Append(": ");
                    sb.Append(String.Format("{0:##0%}", boss.CurrentFactor));
                    sb.Append("\\");
                }
            }
            Clipboard.SetText(sb.ToString());
        }
        public void ClearBosses()
        {
            _dispatcher.Invoke(() =>
            {
                _bosses.Clear();
            });
        }
        public void EndNpcAbnormality(ulong target, Abnormality ab)
        {
            var boss = _bosses.FirstOrDefault(x => x.EntityId == target);
            if (boss != null)
            {
                boss.EndBuff(ab);
            }
        }
        public void AddOrRefreshNpcAbnormality(Abnormality ab, int stacks, uint duration, ulong target, double size, double margin)
        {
            var boss = _bosses.FirstOrDefault(x => x.EntityId == target);
            if (boss != null)
            {
                boss.AddorRefresh(ab, duration, stacks, size, margin);
            }
        }
        public void SetBossEnrage(ulong entityId, bool enraged)
        {
            var boss = _bosses.FirstOrDefault(x => x.EntityId == entityId);
            if (boss == null)
            {
                return;
            }
            boss.Enraged = enraged;
        }
        public void UnsetBossTarget(ulong entityId)
        {
            var boss = _bosses.FirstOrDefault(x => x.EntityId == entityId);
            if (boss == null)
            {
                return;
            }
            boss.Target = 0;
        }
        public void SetBossAggro(ulong entityId, AggroCircle circle, ulong user)
        {
            var boss = _bosses.FirstOrDefault(x => x.EntityId == entityId);
            if (boss == null)
            {
                return;
            }
            boss.Target = user;
            boss.CurrentAggroType = AggroCircle.Main;
        }
        public void SelectDragon(Dragon dragon)
        {
            foreach (var item in _bosses.Where(x => x.TemplateId > 1099 && x.TemplateId < 1104))
            {
                if (item.TemplateId == (uint)dragon) { item.IsSelected = true; SelectedDragon = item; }
                else item.IsSelected = false;
            }
        }
    }
}
