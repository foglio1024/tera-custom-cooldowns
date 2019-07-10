using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using FoglioUtils;
using TCC.Data;
using TCC.Data.Abnormalities;
using TCC.Data.NPCs;
using TCC.Parsing;
using TCC.Settings;
using TeraDataLite;
using TeraPacketParser.Messages;

namespace TCC.ViewModels.Widgets
{

    [TccModule]
    public class NpcWindowViewModel : TccWindowViewModel
    {
        public const int Ph1ShieldDuration = 16;
        //private static BossGageWindowViewModel _instance;
        private HarrowholdPhase _currentHHphase;
        private ICollectionViewLiveShaping _bams;
        private ICollectionViewLiveShaping _mobs;
        private ICollectionView _dragons;
        private ICollectionViewLiveShaping _guildTowers;

        private NPC _selectedDragon;
        private NPC _vergos;

        private readonly List<NPC> _holdedDragons = new List<NPC>();
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

        public int VisibleBossesCount => NpcList.ToSyncList().Count(x => x.Visible && x.CurrentHP > 0);
        public int VisibleMobsCount => NpcList.ToSyncList().Count(x => x.Visible && x.CurrentHP > 0 && !x.IsBoss);

        private void HandlePhase1Shields(S_DUNGEON_EVENT_MESSAGE p)
        {
            switch (p.MessageId)
            {
                case 9950045:
                    //shield start
                    foreach (var item in NpcList.Where(x => x.IsPhase1Dragon))
                    {
                        item.StartShield();
                    }
                    break;
                case 9950113:
                    //aquadrax interrupted
                    NpcList.First(x => x.ZoneId == 950 && x.TemplateId == 1103).BreakShield();
                    break;
                case 9950114:
                    //umbradrax interrupted
                    NpcList.First(x => x.ZoneId == 950 && x.TemplateId == 1102).BreakShield();
                    break;
                case 9950115:
                    //ignidrax interrupted
                    NpcList.First(x => x.ZoneId == 950 && x.TemplateId == 1100).BreakShield();
                    break;
                case 9950116:
                    //terradrax interrupted
                    NpcList.First(x => x.ZoneId == 950 && x.TemplateId == 1101).BreakShield();
                    break;
                case 9950044:
                    //shield fail
                    break;
            }

        }
        private void HandlePlayerLocation(C_PLAYER_LOCATION p)
        {
            SelectDragon(p.X, p.Y);
        }
        public HarrowholdPhase CurrentHHphase
        {
            get => _currentHHphase;
            set
            {
                if (_currentHHphase == value) return;
                _currentHHphase = value;
                if (value == HarrowholdPhase.Phase1)
                {
                    PacketAnalyzer.NewProcessor.Hook<S_DUNGEON_EVENT_MESSAGE>(HandlePhase1Shields);
                    PacketAnalyzer.NewProcessor.Hook<C_PLAYER_LOCATION>(HandlePlayerLocation);
                }
                else
                {
                    PacketAnalyzer.NewProcessor.Unhook<S_DUNGEON_EVENT_MESSAGE>(HandlePhase1Shields);
                    PacketAnalyzer.NewProcessor.Unhook<C_PLAYER_LOCATION>(HandlePlayerLocation);
                }
                //PacketAnalyzer.Processor.Update();
                N(nameof(CurrentHHphase));
            }
        }

        public void SetBossEnrageTime(ulong entityId, int remainingEnrageTime)
        {
            var target = NpcList.ToSyncList().FirstOrDefault(x => x.EntityId == entityId);
            if (target != null) target.RemainingEnrageTime = remainingEnrageTime;
        }

        public ICollectionViewLiveShaping Bams
        {
            get
            {
                _bams = CollectionViewUtils.InitLiveView(
                    p => ((NPC)p).IsBoss && !((NPC)p).IsTower && ((NPC)p).Visible,
                    NpcList, 
                    new[] { nameof(NPC.Visible) },
                    new[] { new SortDescription(nameof(NPC.CurrentHP), ListSortDirection.Ascending) });
                return _bams;
            }
        }
        public ICollectionViewLiveShaping Mobs
        {
            get
            {
                _mobs = CollectionViewUtils.InitLiveView(p => !((NPC)p).IsBoss && !((NPC)p).IsTower && ((NPC)p).Visible, NpcList, new[] { nameof(NPC.Visible) },
                    new[] { new SortDescription(nameof(NPC.CurrentHP), ListSortDirection.Ascending) });
                return _mobs;
            }
        }
        public ICollectionViewLiveShaping GuildTowers
        {
            get
            {
                _guildTowers = CollectionViewUtils.InitLiveView(p => ((NPC)p).IsTower, NpcList, new string[] { },
                    new[] { new SortDescription(nameof(NPC.CurrentHP), ListSortDirection.Ascending) });
                return _guildTowers;
            }
        }
        public ICollectionView Dragons
        {
            get
            {
                _dragons = new CollectionViewSource { Source = NpcList }.View;
                _dragons.Filter = p => ((NPC)p).TemplateId > 1099 && ((NPC)p).TemplateId < 1104;
                return _dragons;
            }
        }
        public NPC SelectedDragon
        {
            get => _selectedDragon;
            set
            {
                if (_selectedDragon == value) return;
                _selectedDragon = value;
                N(nameof(SelectedDragon));
            }
        }
        public NPC Vergos
        {
            get => _vergos;
            set
            {
                if (_vergos == value) return;
                _vergos = value;
                N(nameof(Vergos));
            }
        }
        public SynchronizedObservableCollection<NPC> NpcList { get; set; }

        //public static BossGageWindowViewModel Instance => _instance ?? (_instance = new BossGageWindowViewModel());
        public Dictionary<ulong, uint> GuildIds { get; private set; }

        public NpcWindowViewModel(WindowSettings settings) : base(settings)
        {
            NpcList = new SynchronizedObservableCollection<NPC>(Dispatcher);
            _cache = new Dictionary<ulong, float>();
            var flushTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            flushTimer.Tick += FlushCache;
            flushTimer.Start();
            GuildIds = new Dictionary<ulong, uint>();
            NpcListChanged += OnNpcCollectionChanged;
        }

        private void OnNpcCollectionChanged()
        {
            //if (!_flushTimer.IsEnabled)
            //{
            //    Log.All($"+ Starting flush timer");
            //    _flushTimer.Start();
            //}
            //else if (NpcList.Count == 0)
            //{
            //    Log.All($"- Stopping flush timer");
            //    _flushTimer.Stop();
            //    FlushCache(null, null);
            //}
            FlushCache(null, null);
        }

        private void FlushCache(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (_cache.Count == 0)
                {
                    //Log.CW($"FlushCache() - nothing to flush");
                    return;
                }
                try
                {
                    foreach (var hpc in _cache.ToList())
                    {
                        SetHpFromCache(hpc.Key, hpc.Value);
                        //Log.CW($"FlushCache() - flushing HP for {hpc.Key}");
                    }
                }
                catch (Exception ex)
                {
                    Log.All($"[BossGageWindowViewModel.FlushCache()] Error while setting HP from cache: {ex.Message}");

                    // ignored
                }

                _cache.Clear();

            }), DispatcherPriority.Background);
        }

        private void SetHpFromCache(ulong hpcEntityId, float hpcCurrentHp)
        {
            var npc = NpcList.FirstOrDefault(x => x.EntityId == hpcEntityId);
            if (npc != null) npc.CurrentHP = hpcCurrentHp;
            else
            {
                Log.All($"[BossGageWindowViewModel.SetHpFromCache()] NPC {hpcEntityId} not found while setting HP from cache");

            }
        }

        private bool IsCaching => true;//VisibleBossesCount > 3;
        public bool IsCompact => VisibleMobsCount > 6;

        private NPC AddNpc(ulong entityId, uint zoneId, uint templateId, bool isBoss, bool visibility)
        {
            if (App.Settings.NpcWindowSettings.ShowOnlyBosses && !isBoss) return null;
            if (templateId == 0 || zoneId == 0) return null;
            if (zoneId == 1023) return null;

            var boss = new NPC(entityId, zoneId, templateId, isBoss, visibility);
            boss.Visible = boss.IsTower || visibility;
            if (boss.IsTower) HandleNewTower(boss, entityId);
            else if (boss.IsPhase1Dragon) HandleNewPh1Dragon(boss, entityId);
            else AddNormalNpc(boss);

            SetTimerPattern(boss);
            SetEnragePattern(boss);
            NpcListChanged?.Invoke();
            return boss;
        }

        private void SetVergos(NPC boss)
        {
            Vergos = boss.ZoneId == 950 && (boss.TemplateId == 1000 || boss.TemplateId == 2000 ||
                                            boss.TemplateId == 3000 || boss.TemplateId == 4000)
                ? boss
                : null;
        }
        private void AddNormalNpc(NPC boss)
        {
            SetVergos(boss);
            if (_savedHp.TryGetValue(boss.EntityId, out var cached)) boss.CurrentHP = cached;
            NpcList.Add(boss);
        }

        private void HandleNewPh1Dragon(NPC boss, ulong entityId)
        {
            var d = _holdedDragons.FirstOrDefault(x => x.EntityId == entityId);
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

        private void HandleNewTower(NPC tower, ulong entityId)
        {
            if (_towerNames.TryGetValue(entityId, out var towerName))
            {
                tower.Name = towerName;
                WindowManager.ViewModels.CivilUnrest.SetGuildName(tower.GuildId, towerName); //TODO: check for enabled?
            }
            tower.IsBoss = true;
            NpcList.Add(tower);
            if (_savedHp.TryGetValue(entityId, out var hp)) tower.CurrentHP = hp;

        }

        public void AddOrUpdateNpc(ulong entityId, float maxHp, float curHp, bool isBoss, HpChangeSource src, uint templateId = 0, uint zoneId = 0, bool visibility = true)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var boss = NpcList.ToSyncList().FirstOrDefault(x => x.EntityId == entityId) ?? AddNpc(entityId, zoneId, templateId, isBoss, visibility);
                if (boss == null) return;
                SetHp(boss, maxHp, curHp, src);
                if (boss.Visible != visibility)
                {
                    boss.Visible = visibility;
                    NpcListChanged?.Invoke();
                }
            }));
        }

        private void SetHp(NPC boss, float maxHp, float curHp, HpChangeSource src)
        {
            boss.MaxHP = maxHp;

            if (src == HpChangeSource.BossGage) boss.HasGage = true;
            else if (src == HpChangeSource.Me && boss.HasGage)
            {
                FlushCache(null, null);
                return;
            }

            if (!IsCaching) boss.CurrentHP = curHp;
            else
            {
                //Log.CW($"[BossGageWindowViewModel L273] Adding HP ({curHp}) to cache for NPC with eid {boss.EntityId}");
                AddToCache(boss.EntityId, curHp);
                if (src == HpChangeSource.Me) FlushCache(null, null);
            }

        }

        private readonly Dictionary<ulong, float> _cache;
        private void AddToCache(ulong entityId, float curHp)
        {
            //if (!_cache.ContainsKey(entityId)) _cache.Add(entityId, curHp);
            /*else */
            //Log.CW($"AddToCache({entityId}, {curHp})");
            _cache[entityId] = curHp;
        }

        private static void SetTimerPattern(NPC n)
        {
            if (App.Settings.EthicalMode) return;

            // vergos ph4
            if (n.TemplateId == 4000 && n.ZoneId == 950) n.TimerPattern = new HpTriggeredTimerPattern(10 * 60, 1f);
            // nightmare kylos
            if (n.TemplateId == 3000 && n.ZoneId == 982) n.TimerPattern = new HpTriggeredTimerPattern(9 * 60, .8f);
            // nightmare antaroth
            if (n.TemplateId == 3000 && n.ZoneId == 920) n.TimerPattern = new HpTriggeredTimerPattern(5 * 60, .5f);
            // bahaar
            if (n.TemplateId == 2000 && n.ZoneId == 444) n.TimerPattern = new HpTriggeredTimerPattern(5 * 60, .3f);
            // dreadspire
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

        private static void SetEnragePattern(NPC n)
        {
            if (App.Settings.EthicalMode)
            {
                n.EnragePattern = new EnragePattern(0, 0);
                return;
            }
            if (n.IsPhase1Dragon) n.EnragePattern = new EnragePattern(14, 50);
            if (n.ZoneId == 950 && !n.IsPhase1Dragon) n.EnragePattern = new EnragePattern(0, 0);
            if (n.ZoneId == 450)
            {
                if (n.TemplateId == 1003) n.EnragePattern = new EnragePattern((long)n.MaxHP, 600000000, 72);
            }

            //ghilli
            if (n.TemplateId == 81301 && n.ZoneId == 713) n.EnragePattern = new EnragePattern(100 - 65, int.MaxValue) { StaysEnraged = true };
            if (n.TemplateId == 81312 && n.ZoneId == 713) n.EnragePattern = new EnragePattern(0, 0);
            if (n.TemplateId == 81398 && n.ZoneId == 713) n.EnragePattern = new EnragePattern(100 - 25, int.MaxValue) { StaysEnraged = true };
            if (n.TemplateId == 81399 && n.ZoneId == 713) n.EnragePattern = new EnragePattern(100 - 25, int.MaxValue) { StaysEnraged = true };


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
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var boss = NpcList.ToSyncList().FirstOrDefault(x => x.EntityId == id);
                if (boss == null) return;
                //Log.CW($"RemoveBoss({boss.Name}, {type}) - HP:{boss.CurrentHP}");
                if (type == DespawnType.OutOfView)
                {
                    _savedHp[id] = boss.CurrentHP;
                }
                else
                {
                    _savedHp.Remove(id);
                }
                if (!boss.Visible || boss.IsTower)
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
            }));
        }
        public void CopyToClipboard()
        {
            var sb = new StringBuilder();
            foreach (var boss in NpcList.ToSyncList())
            {
                if (!boss.Visible) continue;
                sb.Append(boss.Name);
                sb.Append(": ");
                sb.Append($"{boss.HPFactor:##0%}");
                sb.Append("\\");
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
            Dispatcher.BeginInvoke(new Action(() =>
            {
                foreach (var npc in NpcList.ToSyncList())
                {
                    npc.Dispose();
                }
                NpcList.Clear();
            }));
        }
        public void EndNpcAbnormality(ulong target, Abnormality ab)
        {
            var boss = NpcList.ToSyncList().FirstOrDefault(x => x.EntityId == target);
            boss?.EndBuff(ab);
        }
        public void AddOrRefreshNpcAbnormality(Abnormality ab, int stacks, uint duration, ulong target)
        {
            var boss = NpcList.ToSyncList().FirstOrDefault(x => x.EntityId == target);
            boss?.AddorRefresh(ab, duration, stacks);
        }
        public void SetBossEnrage(ulong entityId, bool enraged)
        {
            var boss = NpcList.ToSyncList().FirstOrDefault(x => x.EntityId == entityId);
            if (boss == null) return;
            boss.Enraged = enraged;
        }
        public void UnsetBossTarget(ulong entityId)
        {
            var boss = NpcList.ToSyncList().FirstOrDefault(x => x.EntityId == entityId);
            if (boss == null)
            {
                return;
            }
            boss.Target = 0;
        }
        public void SetBossAggro(ulong entityId, ulong user)
        {
            var boss = NpcList.ToSyncList().FirstOrDefault(x => x.EntityId == entityId);
            if (boss == null)
            {
                return;
            }
            boss.Target = user;
            boss.CurrentAggroType = AggroCircle.Main;
            if (boss.Visible) WindowManager.ViewModels.Group.SetAggro(entityId);
        }
        public void SelectDragon(float x, float y)
        {
            var dragon = EntityManager.CheckCurrentDragon(new Point(x, y));
            foreach (var item in NpcList.ToSyncList().Where(d => d.TemplateId > 1099 && d.TemplateId < 1104))
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
            GuildIds[towerId] = guildId;
            var t = NpcList.ToSyncList().FirstOrDefault(x => x.EntityId == towerId);
            if (t != null)
            {
                t.Name = guildName;
                t.ExN(nameof(NPC.GuildId));
            }
            _towerNames[towerId] = guildName;
        }

        public void UpdateShield(ulong target, uint damage)
        {
            var boss = NpcList.ToSyncList().FirstOrDefault(x => x.EntityId == target);
            if (boss == null) return;
            boss.CurrentShield -= damage;
        }

        public void RemoveMe(NPC npc, uint delay)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (NpcList.ToSyncList().All(x => x != npc)) return;
                //Log.CW($"RemoveMe({npc.Name}, {delay}) - HP:{npc.CurrentHP}");
                npc.Buffs.Clear();
                if (delay != 0)
                {

                    var dt = new DispatcherTimer(DispatcherPriority.Background, Dispatcher)
                    {
                        Interval = TimeSpan.FromMilliseconds(delay)
                    };
                    dt.Tick += (s, ev) =>
                    {
                        dt.Stop();
                        RemoveAndDisposeNPC(npc);
                    };
                    dt.Start();
                }
                else
                {
                    RemoveAndDisposeNPC(npc);
                }
            }));
        }

        private void RemoveAndDisposeNPC(NPC b)
        {
            b.Dispose();
            NpcList.Remove(b);
        }


        protected override void InstallHooks()
        {
            PacketAnalyzer.NewProcessor.Hook<S_ABNORMALITY_DAMAGE_ABSORB>(OnAbnormalityDamageAbsorb);
            PacketAnalyzer.NewProcessor.Hook<S_SHOW_HP>(OnShowHp);
            PacketAnalyzer.NewProcessor.Hook<S_CREATURE_CHANGE_HP>(OnCreatureChangeHp);
            PacketAnalyzer.NewProcessor.Hook<S_BOSS_GAGE_INFO>(OnBossGageInfo);
            PacketAnalyzer.NewProcessor.Hook<S_NPC_STATUS>(OnNpcStatus);
            PacketAnalyzer.NewProcessor.Hook<S_USER_EFFECT>(OnUserEffect);
            PacketAnalyzer.NewProcessor.Hook<S_GET_USER_LIST>(OnGetUserList);
            PacketAnalyzer.NewProcessor.Hook<S_LOGIN>(OnLogin);
            PacketAnalyzer.NewProcessor.Hook<S_RETURN_TO_LOBBY>(OnReturnToLobby);
            PacketAnalyzer.NewProcessor.Hook<S_LOAD_TOPO>(OnLoadTopo);
            PacketAnalyzer.NewProcessor.Hook<S_SPAWN_ME>(OnSpawnMe);
            PacketAnalyzer.NewProcessor.Hook<S_SPAWN_NPC>(OnSpawnNpc);
            PacketAnalyzer.NewProcessor.Hook<S_GUILD_TOWER_INFO>(OnGuildTowerInfo);
        }

        protected override void RemoveHooks()
        {
            PacketAnalyzer.NewProcessor.Unhook<S_ABNORMALITY_DAMAGE_ABSORB>(OnAbnormalityDamageAbsorb);
            PacketAnalyzer.NewProcessor.Unhook<S_SHOW_HP>(OnShowHp);
            PacketAnalyzer.NewProcessor.Unhook<S_CREATURE_CHANGE_HP>(OnCreatureChangeHp);
            PacketAnalyzer.NewProcessor.Unhook<S_BOSS_GAGE_INFO>(OnBossGageInfo);
            PacketAnalyzer.NewProcessor.Unhook<S_NPC_STATUS>(OnNpcStatus);
            PacketAnalyzer.NewProcessor.Unhook<S_USER_EFFECT>(OnUserEffect);
            PacketAnalyzer.NewProcessor.Unhook<S_GET_USER_LIST>(OnGetUserList);
            PacketAnalyzer.NewProcessor.Unhook<S_LOGIN>(OnLogin);
            PacketAnalyzer.NewProcessor.Unhook<S_RETURN_TO_LOBBY>(OnReturnToLobby);
            PacketAnalyzer.NewProcessor.Unhook<S_LOAD_TOPO>(OnLoadTopo);
            PacketAnalyzer.NewProcessor.Unhook<S_SPAWN_ME>(OnSpawnMe);
            PacketAnalyzer.NewProcessor.Unhook<S_SPAWN_NPC>(OnSpawnNpc);
            PacketAnalyzer.NewProcessor.Unhook<S_GUILD_TOWER_INFO>(OnGuildTowerInfo);
        }

        private void OnGuildTowerInfo(S_GUILD_TOWER_INFO m)
        {
            AddGuildTower(m.TowerId, m.GuildName, m.GuildId);
        }
        private void OnSpawnNpc(S_SPAWN_NPC m)
        {
            EntityManager.CheckHarrowholdMode(m.HuntingZoneId, m.TemplateId);
            EntityManager.SpawnNPC(m.HuntingZoneId, m.TemplateId, m.EntityId, false, m.Villager, m.RemainingEnrageTime);
        }
        private void OnSpawnMe(S_SPAWN_ME m)
        {
            EntityManager.ClearNPC();
        }
        private void OnLoadTopo(S_LOAD_TOPO m)
        {
            CurrentHHphase = HarrowholdPhase.None;
            ClearGuildTowers();
        }
        private void OnReturnToLobby(S_RETURN_TO_LOBBY m)
        {
            EntityManager.ClearNPC();
        }
        private void OnLogin(S_LOGIN m)
        {
            EntityManager.ClearNPC();
        }
        private void OnGetUserList(S_GET_USER_LIST m)
        {
            EntityManager.ClearNPC();
        }
        private void OnUserEffect(S_USER_EFFECT m)
        {
            SetBossAggro(m.Source, m.User);
        }
        private void OnNpcStatus(S_NPC_STATUS m)
        {
            EntityManager.SetNPCStatus(m.EntityId, m.IsEnraged, m.RemainingEnrageTime);
            if (m.Target == 0) UnsetBossTarget(m.EntityId);
            SetBossAggro(m.EntityId, m.Target);
        }
        private void OnBossGageInfo(S_BOSS_GAGE_INFO m)
        {
            EntityManager.UpdateNPC(m.EntityId, m.CurrentHP, m.MaxHP, (ushort) m.HuntingZoneId, (uint) m.TemplateId);
        }
        private void OnCreatureChangeHp(S_CREATURE_CHANGE_HP m)
        {
            if (Session.IsMe(m.Target)) return;
            EntityManager.UpdateNPC(m.Target, m.CurrentHP, m.MaxHP, m.Source);
        }
        private void OnShowHp(S_SHOW_HP m)
        {
            AddOrUpdateNpc(m.GameId, m.MaxHp, m.CurrentHp, false, HpChangeSource.CreatureChangeHp);
        }
        private void OnAbnormalityDamageAbsorb(S_ABNORMALITY_DAMAGE_ABSORB p)
        {
            if (Session.IsMe(p.Target)) return;
            UpdateShield(p.Target, p.Damage);
        }

        /*
                public void UpdateBySkillResult(ulong target, ulong damage)
                {
                    var boss = NpcList.ToSyncList().FirstOrDefault(x => x.EntityId == target);
                    if (boss != null && !boss.HasGage)
                    {
                        if (boss.CurrentHP - damage < 0) boss.CurrentHP = 0;
                        else boss.CurrentHP -= damage;
                    }
                }
        */
    }


}
