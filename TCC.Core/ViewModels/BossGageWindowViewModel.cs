using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using TCC.Data;
using TCC.Parsing;

namespace TCC.ViewModels
{
    public class BossGageWindowViewModel : TSPropertyChanged
    {
        public const int PH1SHIELD_DURATION = 16;
        private static BossGageWindowViewModel _instance;
        private HarrowholdPhase _currentHHphase;
        private ICollectionView _bams;
        private ICollectionView _mobs;
        private ICollectionView _dragons;
        private Boss selectedDragon;
        private Boss _vergos;
        private SynchronizedObservableCollection<Boss> _currentNPCs;
        private double scale = SettingsManager.BossWindowSettings.Scale;

        private List<Boss> _holdedDragons = new List<Boss>();
        private Dictionary<ulong, string> GuildTowers = new Dictionary<ulong, string>();

        private void AddSortedDragons()
        {
            _currentNPCs.Add(_holdedDragons.First(x => x.TemplateId == 1102));
            _currentNPCs.Add(_holdedDragons.First(x => x.TemplateId == 1100));
            _currentNPCs.Add(_holdedDragons.First(x => x.TemplateId == 1101));
            _currentNPCs.Add(_holdedDragons.First(x => x.TemplateId == 1103));
            _holdedDragons.Clear();
        }

        public bool IsTeraOnTop
        {
            get => WindowManager.IsTccVisible;
        }
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
        public int VisibleBossesCount
        {
            get => CurrentNPCs.Where(x => x.Visible == Visibility.Visible && x.IsBoss).Count();
        }
        public HarrowholdPhase CurrentHHphase
        {
            get => _currentHHphase;
            set
            {
                if (_currentHHphase == value) return;
                _currentHHphase = value;
                MessageFactory.Update();
                NotifyPropertyChanged(nameof(CurrentHHphase));
            }
        }
        public ICollectionView Bams
        {
            get
            {
                _bams = new CollectionViewSource { Source = _currentNPCs }.View;
                _bams.Filter = p => ((Boss)p).IsBoss == true;
                return _bams;
            }
        }
        public ICollectionView Mobs
        {
            get
            {
                _mobs = new CollectionViewSource { Source = _currentNPCs }.View;
                _mobs.Filter = p => ((Boss)p).IsBoss == false;
                return _mobs;
            }
        }
        public ICollectionView Dragons
        {
            get
            {
                _dragons = new CollectionViewSource { Source = _currentNPCs }.View;
                _dragons.Filter = p => ((Boss)p).TemplateId > 1099 && ((Boss)p).TemplateId < 1104;
                return _dragons;
            }
        }
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
        public SynchronizedObservableCollection<Boss> CurrentNPCs
        {
            get
            {
                return _currentNPCs;
            }
            set
            {
                if (_currentNPCs == value) return;
                _currentNPCs = value;
            }
        }
        public static BossGageWindowViewModel Instance => _instance ?? (_instance = new BossGageWindowViewModel());

        public BossGageWindowViewModel()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            _currentNPCs = new SynchronizedObservableCollection<Boss>(_dispatcher);
            WindowManager.TccVisibilityChanged += (s, ev) =>
            {
                NotifyPropertyChanged("IsTeraOnTop");
                if (IsTeraOnTop)
                {
                    WindowManager.BossWindow.RefreshTopmost();
                }
            };

        }


        public void AddOrUpdateBoss(ulong entityId, float maxHp, float curHp, bool isBoss, uint templateId = 0, uint zoneId = 0, Visibility v = Visibility.Visible)
        {
            var boss = _currentNPCs.FirstOrDefault(x => x.EntityId == entityId);
            if (boss == null)
            {
                if (SettingsManager.ShowOnlyBosses && !isBoss) return;

                if (templateId == 0 || zoneId == 0) return;

                v = Utils.IsGuildTower(zoneId, templateId) ? Visibility.Visible : v;
                boss = new Boss(entityId, zoneId, templateId, isBoss, v);
                if (Utils.IsGuildTower(zoneId, templateId))
                {
                    if (GuildTowers.TryGetValue(entityId, out string towerName))
                    {
                        boss.Name = towerName;
                    }
                    boss.IsBoss = true;
                }
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
                    _currentNPCs.Add(boss);
                }

            }
            boss.MaxHP = maxHp;
            boss.CurrentHP = curHp;
            boss.Visible = v;
        }
        public void RemoveBoss(ulong id)
        {
            var boss = _currentNPCs.FirstOrDefault(x => x.EntityId == id);
            if (boss == null) return;
            _currentNPCs.Remove(boss);
            boss.Dispose();
            if (SelectedDragon != null && SelectedDragon.EntityId == id) SelectedDragon = null;
        }
        public void CopyToClipboard()
        {
            var sb = new StringBuilder();
            foreach (var boss in _currentNPCs)
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
            _currentNPCs.Clear();           
        }
        public void EndNpcAbnormality(ulong target, Abnormality ab)
        {
            var boss = _currentNPCs.FirstOrDefault(x => x.EntityId == target);
            if (boss != null)
            {
                boss.EndBuff(ab);
            }
        }
        public void AddOrRefreshNpcAbnormality(Abnormality ab, int stacks, uint duration, ulong target, double size, double margin)
        {
            var boss = _currentNPCs.FirstOrDefault(x => x.EntityId == target);
            if (boss != null)
            {
                boss.AddorRefresh(ab, duration, stacks, size, margin);
            }
        }
        public void SetBossEnrage(ulong entityId, bool enraged)
        {
            var boss = _currentNPCs.FirstOrDefault(x => x.EntityId == entityId);
            if (boss == null)
            {
                return;
            }
            boss.Enraged = enraged;
        }
        public void UnsetBossTarget(ulong entityId)
        {
            var boss = _currentNPCs.FirstOrDefault(x => x.EntityId == entityId);
            if (boss == null)
            {
                return;
            }
            boss.Target = 0;
        }
        public void SetBossAggro(ulong entityId, AggroCircle circle, ulong user)
        {
            var boss = _currentNPCs.FirstOrDefault(x => x.EntityId == entityId);
            if (boss == null)
            {
                return;
            }
            boss.Target = user;
            boss.CurrentAggroType = AggroCircle.Main;
        }
        public void SelectDragon(Dragon dragon)
        {
            foreach (var item in _currentNPCs.Where(x => x.TemplateId > 1099 && x.TemplateId < 1104))
            {
                if (item.TemplateId == (uint)dragon) { item.IsSelected = true; SelectedDragon = item; }
                else item.IsSelected = false;
            }
        }
        public void ClearGuildTowers()
        {
            GuildTowers.Clear();
        }
        public void AddGuildTower(ulong towerId, string guildName)
        {
            var t = CurrentNPCs.FirstOrDefault(x => x.EntityId == towerId);
            if (t != null)
            {
                t.Name = guildName;
            }
            if (GuildTowers.ContainsKey(towerId)) return;
            GuildTowers.Add(towerId, guildName);
        }
    }
}
