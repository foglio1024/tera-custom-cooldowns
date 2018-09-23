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
    public class BossGageWindowViewModel : TccWindowViewModel
    {
        public const int Ph1ShieldDuration = 16;
        private static BossGageWindowViewModel _instance;
        private HarrowholdPhase _currentHHphase;
        private ICollectionViewLiveShaping _bams;
        private ICollectionView _mobs;
        private ICollectionView _dragons;
        private ICollectionView _guildTowers;

        private Npc _selectedDragon;
        private Npc _vergos;

        private readonly DispatcherTimer _flushTimer;

        private readonly List<Npc> _holdedDragons = new List<Npc>();
        private readonly Dictionary<ulong, string> _towerNames = new Dictionary<ulong, string>();
        private readonly Dictionary<ulong, float> _savedHp = new Dictionary<ulong, float>();

        public event Action NpcListChanged;

        private void AddSortedDragons()
        {
            NpcList.Add(_holdedDragons.FirstOrDefault(x => x.TemplateId == 1102));
            NpcList.Add(_holdedDragons.FirstOrDefault(x => x.TemplateId == 1100));
            NpcList.Add(_holdedDragons.FirstOrDefault(x => x.TemplateId == 1101));
            NpcList.Add(_holdedDragons.FirstOrDefault(x => x.TemplateId == 1103));
            _holdedDragons.Clear();
        }

        //public bool IsTeraOnTop => WindowManager.IsTccVisible;

        public int VisibleBossesCount => NpcList.ToSyncArray().Count(x => x.Visible == Visibility.Visible && x.CurrentHP > 0);
        public int VisibleMobsCount => NpcList.ToSyncArray().Count(x => x.Visible == Visibility.Visible && x.CurrentHP > 0 && !x.IsBoss);

        public HarrowholdPhase CurrentHHphase
        {
            get => _currentHHphase;
            set
            {
                if (_currentHHphase == value) return;
                _currentHHphase = value;
                MessageFactory.Update();
                NPC(nameof(CurrentHHphase));
            }
        }
        public ICollectionViewLiveShaping Bams
        {
            get
            {
                _bams = Utils.InitLiveView(p => ((Npc)p).IsBoss && !((Npc)p).IsTower, NpcList, new string[] { },
                    new[] { new SortDescription(nameof(Npc.Visible), ListSortDirection.Ascending),
                        new SortDescription(nameof(Npc.CurrentHP), ListSortDirection.Ascending) });
                //_bams = new CollectionViewSource { Source = _npcList }.View;
                //_bams.Filter = p => ((Npc)p).IsBoss && !((Npc)p).IsTower;
                return _bams;
            }
        }
        public ICollectionView Mobs
        {
            get
            {
                _mobs = new CollectionViewSource { Source = NpcList }.View;
                _mobs.Filter = p => !((Npc)p).IsBoss && !((Npc)p).IsTower;
                return _mobs;
            }
        }
        public ICollectionView GuildTowers
        {
            get
            {
                _guildTowers = new CollectionViewSource { Source = NpcList }.View;
                _guildTowers.Filter = p => ((Npc)p).IsTower;
                return _guildTowers;
            }
        }
        public ICollectionView Dragons
        {
            get
            {
                _dragons = new CollectionViewSource { Source = NpcList }.View;
                _dragons.Filter = p => ((Npc)p).TemplateId > 1099 && ((Npc)p).TemplateId < 1104;
                return _dragons;
            }
        }
        public Npc SelectedDragon
        {
            get => _selectedDragon;
            set
            {
                if (_selectedDragon == value) return;
                _selectedDragon = value;
                NPC(nameof(SelectedDragon));
            }
        }
        public Npc Vergos
        {
            get => _vergos;
            set
            {
                if (_vergos == value) return;
                _vergos = value;
                NPC(nameof(Vergos));
            }
        }
        public SynchronizedObservableCollection<Npc> NpcList { get; set; }

        public static BossGageWindowViewModel Instance => _instance ?? (_instance = new BossGageWindowViewModel());
        public Dictionary<ulong, uint> GuildIds { get; private set; }

        public BossGageWindowViewModel()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            NpcList = new SynchronizedObservableCollection<Npc>(_dispatcher);
            _cache = new Dictionary<ulong, float>();
            _flushTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(250) };
            _flushTimer.Tick += FlushCache;
            //_flushTimer.Start();
            GuildIds = new Dictionary<ulong, uint>();
            NpcListChanged += OnNpcCollectionChanged;
            //WindowManager.TccVisibilityChanged += (s, ev) =>
            //{
            //NPC(nameof(IsTeraOnTop));
            //if (IsTeraOnTop)
            //{
            //WindowManager.BossWindow.RefreshTopmost();
            //}
            //};

        }

        private void OnNpcCollectionChanged()
        {
            var caching = IsCaching;
            if (caching && !_flushTimer.IsEnabled)
            {
                _flushTimer.Start();
            }
            else if (!caching && _flushTimer.IsEnabled)
            {
                _flushTimer.Stop();
                FlushCache(null, null);
            }
        }

        private void FlushCache(object sender, EventArgs e)
        {
            _dispatcher.Invoke(() =>
            {
                if (_cache.Count == 0) return;
                try
                {
                    foreach (var hpc in _cache.ToList())
                    {
                        SetHpFromCache(hpc.Key, hpc.Value);
                    }
                }
                catch (Exception exception)
                {
                }
                _cache.Clear();
            });
        }

        private void SetHpFromCache(ulong hpcEntityId, float hpcCurrentHp)
        {
            var npc = NpcList.FirstOrDefault(x => x.EntityId == hpcEntityId);
            if (npc != null) npc.CurrentHP = hpcCurrentHp;
        }

        private bool IsCaching => VisibleBossesCount > 1;
        public bool IsCompact => VisibleMobsCount > 6;

        private Npc AddNpc(ulong entityId, uint zoneId, uint templateId, bool isBoss, Visibility visibility)
        {
            if (Settings.ShowOnlyBosses && !isBoss) return null;
            if (templateId == 0 || zoneId == 0) return null;
            if (zoneId == 1023) return null;

            var boss = new Npc(entityId, zoneId, templateId, isBoss, visibility);
            boss.Visible = boss.IsTower ? Visibility.Visible : visibility;
            if (boss.IsTower) HandleNewTower(boss, entityId);
            else if (boss.IsPhase1Dragon) HandleNewPh1Dragon(boss, entityId);
            else AddNormalNpc(boss);

            SetTimerPattern(boss);
            SetEnragePattern(boss);
            NpcListChanged?.Invoke();
            return boss;
        }

        private void SetVergos(Npc boss)
        {
            Vergos = boss.ZoneId == 950 && (boss.TemplateId == 1000 || boss.TemplateId == 2000 ||
                                            boss.TemplateId == 3000 || boss.TemplateId == 4000)
                ? boss
                : null;
        }
        private void AddNormalNpc(Npc boss)
        {
            SetVergos(boss);
            if (_savedHp.ContainsKey(boss.EntityId)) boss.CurrentHP = _savedHp[boss.EntityId];
            NpcList.Add(boss);
        }

        private void HandleNewPh1Dragon(Npc boss, ulong entityId)
        {
            Npc d = null;
            d = _holdedDragons.FirstOrDefault(x => x.EntityId == entityId);
            if (d != null) return;
            _holdedDragons.Add(boss);
            if (_holdedDragons.Count != 4) return;
            try
            {
                AddSortedDragons();
            }
            catch
            {
                //TODO: send error?
            }
        }

        private void HandleNewTower(Npc boss, ulong entityId)
        {
            if (_towerNames.TryGetValue(entityId, out var towerName))
            {
                boss.Name = towerName;
                WindowManager.CivilUnrestWindow.VM.SetGuildName(boss.GuildId, towerName); //TODO: check for enabled?
            }
            boss.IsBoss = true;
            NpcList.Add(boss);
            if (_savedHp.ContainsKey(entityId)) boss.CurrentHP = _savedHp[entityId];

        }

        public void AddOrUpdateBoss(ulong entityId, float maxHp, float curHp, bool isBoss, HpChangeSource src, uint templateId = 0, uint zoneId = 0, Visibility visibility = Visibility.Visible)
        {
            Npc boss = null;
            boss = NpcList.ToSyncArray().FirstOrDefault(x => x.EntityId == entityId) ?? AddNpc(entityId, zoneId, templateId, isBoss, visibility);
            if (boss == null) return;
            SetHp(boss, maxHp, curHp, src);
            if (boss.Visible != visibility)
            {
                boss.Visible = visibility;
                NpcListChanged?.Invoke();
            }
        }

        private void SetHp(Npc boss, float maxHp, float curHp, HpChangeSource src)
        {
            boss.MaxHP = maxHp;

            if (src == HpChangeSource.BossGage) boss.HasGage = true;
            else if (src == HpChangeSource.CreatureChangeHp && boss.HasGage) return;

            if (!IsCaching) boss.CurrentHP = curHp;
            else AddToCache(boss.EntityId, curHp);

        }

        private readonly Dictionary<ulong, float> _cache;
        private void AddToCache(ulong entityId, float curHp)
        {
            if (!_cache.ContainsKey(entityId)) _cache.Add(entityId, curHp);
            else _cache[entityId] = curHp;
        }

        private static void SetTimerPattern(Npc n)
        {
            if (n.TemplateId == 4000 && n.ZoneId == 950) n.TimerPattern = new HpTriggeredTimerPattern(10 * 60, 1f);
            if (n.TemplateId == 3000 && n.ZoneId == 920) n.TimerPattern = new HpTriggeredTimerPattern(5 * 60, .5f);

            if (n.TemplateId == 1000 && n.ZoneId == 434) n.TimerPattern = new HpTriggeredTimerPattern(10 * 60, 1f);
            if (n.TemplateId == 2000 && n.ZoneId == 434) n.TimerPattern = new HpTriggeredTimerPattern(10 * 60, 1f);
            if (n.TemplateId == 3000 && n.ZoneId == 434) n.TimerPattern = new HpTriggeredTimerPattern(10 * 60, 1f);
            if (n.TemplateId == 4000 && n.ZoneId == 434) n.TimerPattern = new HpTriggeredTimerPattern(10 * 60, 1f);
            if (n.TemplateId == 5000 && n.ZoneId == 434) n.TimerPattern = new HpTriggeredTimerPattern(10 * 60, 1f);
            if (n.TemplateId == 6000 && n.ZoneId == 434) n.TimerPattern = new HpTriggeredTimerPattern(10 * 60, 1f);
            if (n.TemplateId == 7000 && n.ZoneId == 434) n.TimerPattern = new HpTriggeredTimerPattern(10 * 60, 1f);
            if (n.TemplateId == 8000 && n.ZoneId == 434) n.TimerPattern = new HpTriggeredTimerPattern(10 * 60, 1f);
            if (n.TemplateId == 9000 && n.ZoneId == 434) n.TimerPattern = new HpTriggeredTimerPattern(10 * 60, 1f);
            if (n.TemplateId == 10000 && n.ZoneId == 434) n.TimerPattern = new HpTriggeredTimerPattern(10 * 60, 1f);

            n.TimerPattern?.SetTarget(n);
        }

        private static void SetEnragePattern(Npc n)
        {
            if (n.IsPhase1Dragon) n.EnragePattern = new EnragePattern(14, 50);
            if (n.ZoneId == 950 && !n.IsPhase1Dragon) n.EnragePattern = new EnragePattern(0, 0);
            if (n.ZoneId == 450)
            {
                if (n.TemplateId == 1003) n.EnragePattern = new EnragePattern((long)n.MaxHP, 600000000, 112);
            }

            if (n.ZoneId == 620 && n.TemplateId == 1000) n.EnragePattern = new EnragePattern((long)n.MaxHP, 420000000, 36);
            if (n.ZoneId == 622 && n.TemplateId == 1000) n.EnragePattern = new EnragePattern((long)n.MaxHP, 480000000, 36);
            if (n.ZoneId == 628)
            {
                if (n.TemplateId == 1000) n.EnragePattern = new EnragePattern(0, 0);
                if (n.TemplateId == 3000) n.EnragePattern = new EnragePattern(10, 36);
                if (n.TemplateId == 3001) n.EnragePattern = new EnragePattern(10, 36);
            }
        }


        public void RemoveBoss(ulong id, DespawnType type)
        {
            Npc boss = null;

            boss = NpcList.ToSyncArray().FirstOrDefault(x => x.EntityId == id);
            if (boss == null) return;
            if (type == DespawnType.OutOfView)
            {
                if (!_savedHp.ContainsKey(id)) _savedHp.Add(id, boss.CurrentHP);
            }
            else
            {
                if (_savedHp.ContainsKey(id)) _savedHp.Remove(id);
            }
            if (boss.Visible != Visibility.Visible || boss.IsTower)
            {
                NpcList.Remove(boss);
                boss.Dispose();
            }
            else
            {
                boss.Delete();
            }
            NpcListChanged?.Invoke();

            //_currentNPCs.Remove(boss);
            //boss.Dispose();
            if (SelectedDragon != null && SelectedDragon.EntityId == id) SelectedDragon = null;
        }
        public void CopyToClipboard()
        {
            var sb = new StringBuilder();
            foreach (var boss in NpcList.ToSyncArray())
            {
                if (boss.Visible == Visibility.Visible)
                {
                    sb.Append(boss.Name);
                    sb.Append(": ");
                    sb.Append(string.Format("{0:##0%}", boss.CurrentFactor));
                    sb.Append("\\");
                }
            }
            try
            {
                Clipboard.SetText(sb.ToString());
            }
            catch
            {
                WindowManager.FloatingButton.NotifyExtended("Boss window", "Failed to copy boss HP to clipboard.", NotificationType.Error);
                ChatWindowManager.Instance.AddTccMessage("Failed to copy boss HP.");
            }
        }
        public void ClearBosses()
        {
            foreach (var npc in NpcList.ToSyncArray())
            {
                npc.Dispose();
            }
            NpcList.Clear();
        }
        public void EndNpcAbnormality(ulong target, Abnormality ab)
        {
            Npc boss = null;
            boss = NpcList.ToSyncArray().FirstOrDefault(x => x.EntityId == target);
            boss?.EndBuff(ab);
        }
        public void AddOrRefreshNpcAbnormality(Abnormality ab, int stacks, uint duration, ulong target)
        {
            Npc boss = null;
            boss = NpcList.ToSyncArray().FirstOrDefault(x => x.EntityId == target);
            boss?.AddorRefresh(ab, duration, stacks);
        }
        public void SetBossEnrage(ulong entityId, bool enraged)
        {
            Npc boss = null;
            boss = NpcList.ToSyncArray().FirstOrDefault(x => x.EntityId == entityId);
            if (boss == null) return;
            boss.Enraged = enraged;
        }
        public void UnsetBossTarget(ulong entityId)
        {
            Npc boss = null;
            boss = NpcList.ToSyncArray().FirstOrDefault(x => x.EntityId == entityId);
            if (boss == null)
            {
                return;
            }
            boss.Target = 0;
        }
        public void SetBossAggro(ulong entityId, AggroCircle circle, ulong user)
        {
            Npc boss = null;
            boss = NpcList.ToSyncArray().FirstOrDefault(x => x.EntityId == entityId);
            if (boss == null)
            {
                return;
            }
            boss.Target = user;
            boss.CurrentAggroType = AggroCircle.Main;
        }
        public void SelectDragon(Dragon dragon)
        {
            foreach (var item in NpcList.ToSyncArray().Where(x => x.TemplateId > 1099 && x.TemplateId < 1104))
            {
                if (item.TemplateId == (uint)dragon) { item.IsSelected = true; SelectedDragon = item; }
                else item.IsSelected = false;
            }
        }
        public void ClearGuildTowers()
        {
            _towerNames.Clear();
        }
        public void AddGuildTower(ulong towerId, string guildName, uint guildId)
        {
            if (!GuildIds.ContainsKey(towerId)) GuildIds.Add(towerId, guildId);
            Npc t = null;
            t = NpcList.ToSyncArray().FirstOrDefault(x => x.EntityId == towerId);
            if (t != null)
            {
                t.Name = guildName;
                t.ExNPC(nameof(Npc.GuildId));
            }
            if (_towerNames.ContainsKey(towerId)) return;
            _towerNames.Add(towerId, guildName);
        }

        public void UpdateShield(ulong target, uint damage)
        {
            Npc boss = null;
            boss = NpcList.ToSyncArray().FirstOrDefault(x => x.EntityId == target);
            if (boss != null)
            {
                boss.CurrentShield -= damage;
            }
        }

        public void RemoveMe(Npc npc)
        {
            _dispatcher.Invoke(() =>
            {
                var b = NpcList.ToSyncArray().FirstOrDefault(x => x == npc);
                if (b == null) return;
                b.Buffs.Clear();
                var dt = new DispatcherTimer(DispatcherPriority.Background, _dispatcher)
                {
                    Interval = TimeSpan.FromMilliseconds(5500)
                };
                dt.Tick += (s, ev) =>
                {
                    dt.Stop();
                    NpcList.Remove(b);
                    b.Dispose();
                };
                dt.Start();
            });
        }

        public void UpdateBySkillResult(ulong target, ulong damage)
        {
            var boss = NpcList.ToSyncArray().FirstOrDefault(x => x.EntityId == target);
            if (boss != null && !boss.HasGage)
            {
                if (boss.CurrentHP - damage < 0) boss.CurrentHP = 0;
                else boss.CurrentHP -= damage;
            }
        }
    }


}
