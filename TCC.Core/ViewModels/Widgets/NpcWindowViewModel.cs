using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using FoglioUtils;
using TCC.Data;
using TCC.Data.Abnormalities;
using TCC.Data.Databases;
using TCC.Data.NPCs;
using TCC.Parsing;
using TCC.Settings;
using TCC.Utilities;
using TeraDataLite;
using TeraPacketParser.Messages;

namespace TCC.ViewModels.Widgets
{
    [TccModule]
    public class NpcWindowViewModel : TccWindowViewModel
    {
        private readonly SynchronizedObservableCollection<NPC> _npcList;

        private ICollectionViewLiveShaping _bams;
        private ICollectionViewLiveShaping _mobs;
        private ICollectionViewLiveShaping _guildTowers;

        private readonly Dictionary<ulong, string> _towerNames = new Dictionary<ulong, string>();
        private readonly Dictionary<ulong, float> _savedHp = new Dictionary<ulong, float>();
        private readonly Dictionary<ulong, float> _cache = new Dictionary<ulong, float>();

        public event Action NpcListChanged;

        public int VisibleBossesCount => _npcList.ToSyncList().Count(x => x.Visible && x.CurrentHP > 0);
        public int VisibleMobsCount => _npcList.ToSyncList().Count(x => x.Visible && x.CurrentHP > 0 && !x.IsBoss);
        public bool IsCompact => VisibleMobsCount > 6;

        public ICollectionViewLiveShaping Bams
        {
            get
            {
                if (_bams != null) return _bams;
                _bams = CollectionViewUtils.InitLiveView(_npcList,
                    npc => npc.IsBoss && !npc.IsTower && npc.Visible,
                    new[] { nameof(NPC.Visible), nameof(NPC.IsBoss) },
                    new[] { new SortDescription(nameof(NPC.CurrentHP), ListSortDirection.Ascending) });
                return _bams;
            }
        }
        public ICollectionViewLiveShaping Mobs
        {
            get
            {
                if (_mobs != null) return _mobs;
                _mobs = CollectionViewUtils.InitLiveView(_npcList,
                    npc => !npc.IsBoss && !npc.IsTower && npc.Visible,
                    new[] { nameof(NPC.Visible), nameof(NPC.IsBoss) },
                    new[] { new SortDescription(nameof(NPC.CurrentHP), ListSortDirection.Ascending) });
                return _mobs;
            }
        }
        public ICollectionViewLiveShaping GuildTowers
        {
            get
            {
                if (_guildTowers != null) return _guildTowers;
                _guildTowers = CollectionViewUtils.InitLiveView(_npcList,
                        npc => npc.IsTower,
                        new string[] { },
                        new[] { new SortDescription(nameof(NPC.CurrentHP), ListSortDirection.Ascending) });
                return _guildTowers;
            }
        }
        public Dictionary<ulong, uint> GuildIds { get; } = new Dictionary<ulong, uint>();

        public NpcWindowViewModel(WindowSettings settings) : base(settings)
        {
            _npcList = new SynchronizedObservableCollection<NPC>(Dispatcher);
            InitFlushTimer();
            NpcListChanged += FlushCache;
            ((NpcWindowSettings)settings).AccurateHpChanged += OnAccurateHpChanged;
            ((NpcWindowSettings)settings).HideAddsChanged += OnHideAddsChanged;
            MonsterDatabase.OverrideChangedEvent += RefreshOverride;
            void InitFlushTimer()
            {
                var flushTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
                flushTimer.Tick += (_, __) => FlushCache();
                flushTimer.Start();
            }
        }

        private void OnHideAddsChanged()
        {
            if (!((NpcWindowSettings)Settings).HideAdds) return;
            _npcList.ToSyncList().Where(x => !x.IsBoss).ToList().ForEach(RemoveAndDisposeNPC);
        }

        public void AddOrUpdateNpc(ulong entityId, float maxHp, float curHp, bool isBoss, HpChangeSource src, uint templateId = 0, uint zoneId = 0, bool visibility = true, int remainingEnrageTime = 0)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!TryFindNPC(entityId, out var boss)) boss = AddNPC(entityId, zoneId, templateId, isBoss, visibility);
                if (boss == null) return;
                SetHP(boss, maxHp, curHp, src);
                SetEnrageTime(entityId, remainingEnrageTime);
                if (boss.Visible == visibility) return;
                boss.Visible = visibility;
                NpcListChanged?.Invoke();
            }));
        }
        public void RemoveNPC(NPC npc, uint delay)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!TryFindNPC(npc.EntityId, out _)) return;
                npc.Buffs.Clear();
                if (delay != 0)
                {
#if false
                    var dt = new DispatcherTimer(DispatcherPriority.Background, Dispatcher) { Interval = TimeSpan.FromMilliseconds(delay) };
                    dt.Tick += (s, ev) =>
                    {
                        dt.Stop();
                        RemoveAndDisposeNPC(npc);
                    };
                    dt.Start();
#else
                    Task.Delay(TimeSpan.FromMilliseconds(delay))
                        .ContinueWith(t => Dispatcher.Invoke(() => RemoveAndDisposeNPC(npc)));
#endif
                }
                else
                {
                    RemoveAndDisposeNPC(npc);
                }
            }));
        }
        public void UpdateAbnormality(Abnormality ab, int stacks, uint duration, ulong target)
        {
            if (!TryFindNPC(target, out var boss)) return;
            boss.AddorRefresh(ab, duration, stacks);
        }
        public void EndAbnormality(ulong target, Abnormality ab)
        {
            if (!TryFindNPC(target, out var boss)) return;
            boss.EndBuff(ab);
        }
        public void Clear()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                foreach (var npc in _npcList.ToSyncList())
                {
                    npc.Dispose();
                }
                _npcList.Clear();
            }));
        }
        public void CopyToClipboard()
        {
            var sb = new StringBuilder();
            foreach (var boss in _npcList.ToSyncList())
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
                WindowManager.ViewModels.NotificationArea.Enqueue("Boss window", "Failed to copy boss HP to clipboard.", NotificationType.Error);
                ChatWindowManager.Instance.AddTccMessage("Failed to copy boss HP.");
            }
        }
        public bool TryFindNPC(ulong entityId, out NPC found)
        {
            found = _npcList.ToSyncList().FirstOrDefault(x => x.EntityId == entityId);
            return found != null;
        }

        private void RefreshOverride(uint zoneId, uint templateId, bool b)
        {
            _npcList.ToSyncList()
                    .Where(n => n.ZoneId == zoneId && n.TemplateId == templateId)
                    .ToList()
                    .ForEach(n => n.IsBoss = b);
        }
        private void AddOrUpdateNpc(S_SPAWN_NPC spawn, Monster npcData)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var visibility = npcData.IsBoss && TccUtils.IsFieldBoss(spawn.HuntingZoneId, spawn.TemplateId);
                if (!TryFindNPC(spawn.EntityId, out var boss))
                    boss = AddNPC(spawn.EntityId, spawn.HuntingZoneId, spawn.TemplateId, npcData.IsBoss, visibility);
                if (boss == null) return;
                SetHP(boss, npcData.MaxHP, npcData.MaxHP, HpChangeSource.CreatureChangeHp);
                SetEnrageTime(spawn.EntityId, spawn.RemainingEnrageTime);
                if (boss.Visible == visibility) return;
                boss.Visible = visibility;
                NpcListChanged?.Invoke();
            }));
        }
        private void SetEnrageStatus(ulong entityId, bool enraged)
        {
            if (!TryFindNPC(entityId, out var boss)) return;
            boss.Enraged = enraged;
        }
        private void SetEnrageTime(ulong entityId, int remainingEnrageTime)
        {
            if (remainingEnrageTime == 0 || !TryFindNPC(entityId, out var boss)) return;
            boss.RemainingEnrageTime = remainingEnrageTime;
        }
        private void OnAccurateHpChanged()
        {
            if (App.Settings.NpcWindowSettings.AccurateHp) PacketAnalyzer.Processor.Hook<S_SHOW_HP>(OnShowHp);
            else PacketAnalyzer.Processor.Unhook<S_SHOW_HP>(OnShowHp);
        }
        private void FlushCache()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (_cache.Count == 0) return;
                try
                {
                    _cache.ToList().ForEach(hpc => SetFromCache(hpc.Key, hpc.Value));
                }
                catch (Exception ex)
                {
                    Log.All($"[{nameof(NpcWindowViewModel)}.{nameof(FlushCache)}] Error while setting HP from cache: {ex.Message}");
                }

                _cache.Clear();

            }), DispatcherPriority.Background);
        }
        private void SetFromCache(ulong hpcEntityId, float hpcCurrentHp)
        {
            if (!TryFindNPC(hpcEntityId, out var npc)) return;
            npc.CurrentHP = hpcCurrentHp;
        }
        private NPC AddNPC(ulong entityId, uint zoneId, uint templateId, bool isBoss, bool visibility)
        {
            if (App.Settings.NpcWindowSettings.HideAdds && !isBoss) return null;
            if (templateId == 0 || zoneId == 0) return null;
            if (zoneId == 1023) return null;

            var boss = new NPC(entityId, zoneId, templateId, isBoss, visibility);

            boss.Visible = boss.IsTower || visibility;
            if (boss.IsTower)
            {
                HandleNewTower(boss, entityId);
            }
            else if (boss.IsPhase1Dragon)
            {
                HandleNewPh1Dragon(boss, entityId);
            }
            else
            {
                AddNormalNPC(boss);
            }

            boss.SetTimerPattern();
            boss.SetEnragePattern();
            NpcListChanged?.Invoke();
            return boss;
        }
        private void AddNormalNPC(NPC boss)
        {
            SetVergos(boss);
            if (_savedHp.TryGetValue(boss.EntityId, out var cached)) boss.CurrentHP = cached;
            _npcList.Add(boss);
        }
        private void HandleNewTower(NPC tower, ulong entityId)
        {
            if (_towerNames.TryGetValue(entityId, out var towerName))
            {
                tower.Name = towerName;
                WindowManager.ViewModels.CivilUnrest.SetGuildName(tower.GuildId, towerName); //TODO: check for enabled?
            }
            tower.IsBoss = true;
            _npcList.Add(tower);
            if (_savedHp.TryGetValue(entityId, out var hp)) tower.CurrentHP = hp;

        }
        private void SetHP(NPC boss, float maxHp, float curHp, HpChangeSource src)
        {
            boss.MaxHP = maxHp;

            if (src == HpChangeSource.BossGage) boss.HasGage = true;
            else if (src == HpChangeSource.Me && boss.HasGage)
            {
                FlushCache();
                return;
            }

            CacheHP(boss.EntityId, curHp);
            if (src == HpChangeSource.Me) FlushCache();
        }
        private void CacheHP(ulong entityId, float curHp)
        {
            _cache[entityId] = curHp;
        }
        private void SetAggroTarget(ulong entityId, ulong user)
        {
            if (!TryFindNPC(entityId, out var boss)) return;
            boss.Target = user;
            boss.CurrentAggroType = AggroCircle.Main;
            if (boss.Visible) WindowManager.ViewModels.Group.SetAggro(entityId);
        }
        private void UnsetAggroTarget(ulong entityId)
        {
            if (!TryFindNPC(entityId, out var boss)) return;
            boss.Target = 0;
        }
        private void ClearGuildTowers()
        {
            _towerNames.Clear();
        }
        private void AddGuildTower(ulong towerId, string guildName, uint guildId)
        {
            GuildIds[towerId] = guildId;

            if (TryFindNPC(towerId, out var t))
            {
                t.Name = guildName;
                t.ExN(nameof(NPC.GuildId));
            }
            _towerNames[towerId] = guildName;
        }
        private void UpdateShield(ulong target, uint damage)
        {
            if (!TryFindNPC(target, out var boss)) return;
            boss.CurrentShield -= damage;
        }
        private void RemoveAndDisposeNPC(NPC b)
        {
            b.Dispose();
            _npcList.Remove(b);
        }

        protected override void InstallHooks()
        {
            PacketAnalyzer.Sniffer.EndConnection += OnDisconnected;

            PacketAnalyzer.Processor.Hook<S_ABNORMALITY_DAMAGE_ABSORB>(OnAbnormalityDamageAbsorb);
            PacketAnalyzer.Processor.Hook<S_CREATURE_CHANGE_HP>(OnCreatureChangeHp);
            PacketAnalyzer.Processor.Hook<S_BOSS_GAGE_INFO>(OnBossGageInfo);
            PacketAnalyzer.Processor.Hook<S_NPC_STATUS>(OnNpcStatus);
            PacketAnalyzer.Processor.Hook<S_USER_EFFECT>(OnUserEffect);
            PacketAnalyzer.Processor.Hook<S_GET_USER_LIST>(OnGetUserList);
            PacketAnalyzer.Processor.Hook<S_LOGIN>(OnLogin);
            PacketAnalyzer.Processor.Hook<S_RETURN_TO_LOBBY>(OnReturnToLobby);
            PacketAnalyzer.Processor.Hook<S_LOAD_TOPO>(OnLoadTopo);
            PacketAnalyzer.Processor.Hook<S_SPAWN_ME>(OnSpawnMe);
            PacketAnalyzer.Processor.Hook<S_SPAWN_NPC>(OnSpawnNpc);
            PacketAnalyzer.Processor.Hook<S_DESPAWN_NPC>(OnDespawnNpc);
            PacketAnalyzer.Processor.Hook<S_GUILD_TOWER_INFO>(OnGuildTowerInfo);
            PacketAnalyzer.Processor.Hook<S_ABNORMALITY_BEGIN>(OnAbnormalityBegin);
            PacketAnalyzer.Processor.Hook<S_ABNORMALITY_REFRESH>(OnAbnormalityRefresh);
            PacketAnalyzer.Processor.Hook<S_ABNORMALITY_END>(OnAbnormalityEnd);

            if (App.Settings.NpcWindowSettings.AccurateHp) PacketAnalyzer.Processor.Hook<S_SHOW_HP>(OnShowHp);
        }
        protected override void RemoveHooks()
        {
            PacketAnalyzer.Processor.Unhook<S_ABNORMALITY_DAMAGE_ABSORB>(OnAbnormalityDamageAbsorb);
            PacketAnalyzer.Processor.Unhook<S_SHOW_HP>(OnShowHp);
            PacketAnalyzer.Processor.Unhook<S_CREATURE_CHANGE_HP>(OnCreatureChangeHp);
            PacketAnalyzer.Processor.Unhook<S_BOSS_GAGE_INFO>(OnBossGageInfo);
            PacketAnalyzer.Processor.Unhook<S_NPC_STATUS>(OnNpcStatus);
            PacketAnalyzer.Processor.Unhook<S_USER_EFFECT>(OnUserEffect);
            PacketAnalyzer.Processor.Unhook<S_GET_USER_LIST>(OnGetUserList);
            PacketAnalyzer.Processor.Unhook<S_LOGIN>(OnLogin);
            PacketAnalyzer.Processor.Unhook<S_RETURN_TO_LOBBY>(OnReturnToLobby);
            PacketAnalyzer.Processor.Unhook<S_LOAD_TOPO>(OnLoadTopo);
            PacketAnalyzer.Processor.Unhook<S_SPAWN_ME>(OnSpawnMe);
            PacketAnalyzer.Processor.Unhook<S_SPAWN_NPC>(OnSpawnNpc);
            PacketAnalyzer.Processor.Unhook<S_DESPAWN_NPC>(OnDespawnNpc);
            PacketAnalyzer.Processor.Unhook<S_GUILD_TOWER_INFO>(OnGuildTowerInfo);
            PacketAnalyzer.Processor.Unhook<S_ABNORMALITY_BEGIN>(OnAbnormalityBegin);
            PacketAnalyzer.Processor.Unhook<S_ABNORMALITY_REFRESH>(OnAbnormalityRefresh);
            PacketAnalyzer.Processor.Unhook<S_ABNORMALITY_END>(OnAbnormalityEnd);
        }
        private void OnDisconnected()
        {
            Clear();
        }

        #region Hooks

        private void OnPlayerLocation(C_PLAYER_LOCATION p)
        {
            SelectDragon(p.X, p.Y);
        }
        private void OnGuildTowerInfo(S_GUILD_TOWER_INFO m)
        {
            AddGuildTower(m.TowerId, m.GuildName, m.GuildId);
        }
        private void OnSpawnNpc(S_SPAWN_NPC p)
        {
            if (p.Villager) return;
            if (!EntityManager.Pass(p.HuntingZoneId, p.TemplateId)) return;
            CheckHarrowholdPhase(p.HuntingZoneId, p.TemplateId);
            if (!Game.DB.MonsterDatabase.TryGetMonster(p.TemplateId, p.HuntingZoneId, out var m)) return;
            if (App.Settings.NpcWindowSettings.HideAdds && !m.IsBoss) return;
            AddOrUpdateNpc(p, m);
            //AddOrUpdateNpc(p.EntityId, m.MaxHP, m.MaxHP, m.IsBoss, HpChangeSource.CreatureChangeHp, p.TemplateId, p.HuntingZoneId, m.IsBoss && TccUtils.IsFieldBoss(p.HuntingZoneId, p.TemplateId), p.RemainingEnrageTime);
        }
        private void OnSpawnMe(S_SPAWN_ME m)
        {
            Clear();
        }
        private void OnLoadTopo(S_LOAD_TOPO m)
        {
            CurrentHHphase = HarrowholdPhase.None;
            ClearGuildTowers();
        }
        private void OnReturnToLobby(S_RETURN_TO_LOBBY m)
        {
            Clear();
        }
        private void OnLogin(S_LOGIN m)
        {
            Clear();
        }
        private void OnGetUserList(S_GET_USER_LIST m)
        {
            Clear();
        }
        private void OnUserEffect(S_USER_EFFECT m)
        {
            SetAggroTarget(m.Source, m.User);
        }
        private void OnNpcStatus(S_NPC_STATUS m)
        {
            if (m.Target == 0) UnsetAggroTarget(m.EntityId);

            SetEnrageTime(m.EntityId, m.RemainingEnrageTime);
            SetEnrageStatus(m.EntityId, m.IsEnraged);

            SetAggroTarget(m.EntityId, m.Target);
        }
        private void OnDespawnNpc(S_DESPAWN_NPC p)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!TryFindNPC(p.Target, out var boss)) return;
                if (p.Type == DespawnType.OutOfView)
                {
                    _savedHp[p.Target] = boss.CurrentHP;
                }
                else
                {
                    _savedHp.Remove(p.Target);
                }
                if (!boss.Visible || boss.IsTower)
                {
                    _npcList.Remove(boss);
                    boss.Dispose();
                }
                else
                {
                    boss.Delete();
                }
                NpcListChanged?.Invoke();

                if (SelectedDragon != null && SelectedDragon.EntityId == p.Target) SelectedDragon = null;
            }));
        }
        private void OnBossGageInfo(S_BOSS_GAGE_INFO m)
        {
            AddOrUpdateNpc(m.EntityId, m.MaxHP, m.CurrentHP, true, HpChangeSource.BossGage, m.TemplateId, m.HuntingZoneId);
            EntityManager.SetEncounter(m.CurrentHP, m.MaxHP);
        }
        private void OnCreatureChangeHp(S_CREATURE_CHANGE_HP m)
        {
            if (Game.IsMe(m.Target)) return;
            AddOrUpdateNpc(m.Target, m.MaxHP, m.CurrentHP, false, Game.IsMe(m.Source) ? HpChangeSource.Me : HpChangeSource.CreatureChangeHp);
            EntityManager.SetEncounter(m.CurrentHP, m.MaxHP);
        }
        private void OnShowHp(S_SHOW_HP m)
        {
            AddOrUpdateNpc(m.GameId, m.MaxHp, m.CurrentHp, false, HpChangeSource.CreatureChangeHp);
        }
        private void OnAbnormalityDamageAbsorb(S_ABNORMALITY_DAMAGE_ABSORB p)
        {
            if (Game.IsMe(p.Target)) return;
            UpdateShield(p.Target, p.Damage);
        }
        private void OnAbnormalityBegin(S_ABNORMALITY_BEGIN p)
        {
            if (!AbnormalityUtils.Exists(p.AbnormalityId, out var ab) || !AbnormalityUtils.Pass(ab)) return;
            if (p.Duration == Int32.MaxValue) ab.Infinity = true;


            UpdateAbnormality(ab, p.Stacks, p.Duration, p.TargetId);
        }
        private void OnAbnormalityRefresh(S_ABNORMALITY_REFRESH p)
        {
            if (!AbnormalityUtils.Exists(p.AbnormalityId, out var ab) || !AbnormalityUtils.Pass(ab)) return;
            if (p.Duration == Int32.MaxValue) ab.Infinity = true;

            UpdateAbnormality(ab, p.Stacks, p.Duration, p.TargetId);
        }
        private void OnAbnormalityEnd(S_ABNORMALITY_END p)
        {
            if (!AbnormalityUtils.Exists(p.AbnormalityId, out var ab) || !AbnormalityUtils.Pass(ab)) return;
            EndAbnormality(p.TargetId, ab);
        }

        #endregion

        #region HH
        public const int Ph1ShieldDuration = 16;

        private NPC _selectedDragon;
        private NPC _vergos;
        private readonly List<NPC> _holdedDragons = new List<NPC>();
        private HarrowholdPhase _currentHHphase;
        private ICollectionView _dragons;

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
        public HarrowholdPhase CurrentHHphase
        {
            get => _currentHHphase;
            set
            {
                if (_currentHHphase == value) return;
                _currentHHphase = value;
                if (value == HarrowholdPhase.Phase1)
                {
                    PacketAnalyzer.Processor.Hook<S_DUNGEON_EVENT_MESSAGE>(OnDungeonEventMessage);
                    PacketAnalyzer.Processor.Hook<C_PLAYER_LOCATION>(OnPlayerLocation);
                }
                else
                {
                    PacketAnalyzer.Processor.Unhook<S_DUNGEON_EVENT_MESSAGE>(OnDungeonEventMessage);
                    PacketAnalyzer.Processor.Unhook<C_PLAYER_LOCATION>(OnPlayerLocation);
                }
                //PacketAnalyzer.Processor.Update(); //TODO?
                N(nameof(CurrentHHphase));
            }
        }
        public ICollectionView Dragons
        {
            get
            {
                _dragons = new CollectionViewSource { Source = _npcList }.View;
                _dragons.Filter = p => ((NPC)p).TemplateId > 1099 && ((NPC)p).TemplateId < 1104;
                return _dragons;
            }
        }

        private void SelectDragon(float x, float y)
        {
            var dragon = CheckCurrentDragon(new Point(x, y));
            foreach (var item in _npcList.ToSyncList().Where(d => d.TemplateId > 1099 && d.TemplateId < 1104))
            {
                if (item.TemplateId == (uint)dragon) { item.IsSelected = true; SelectedDragon = item; }
                else item.IsSelected = false;
            }
        }
        private void AddSortedDragons()
        {
            _npcList.Add(_holdedDragons.FirstOrDefault(x => x.TemplateId == 1102));
            _npcList.Add(_holdedDragons.FirstOrDefault(x => x.TemplateId == 1100));
            _npcList.Add(_holdedDragons.FirstOrDefault(x => x.TemplateId == 1101));
            _npcList.Add(_holdedDragons.FirstOrDefault(x => x.TemplateId == 1103));
            _holdedDragons.Clear();
        }
        private void OnDungeonEventMessage(S_DUNGEON_EVENT_MESSAGE p)
        {
            switch (p.MessageId)
            {
                case 9950045:
                    //shield start
                    foreach (var item in _npcList.Where(x => x.IsPhase1Dragon))
                    {
                        item.StartShield();
                    }
                    break;
                case 9950113:
                    //aquadrax interrupted
                    _npcList.First(x => x.ZoneId == 950 && x.TemplateId == 1103).BreakShield();
                    break;
                case 9950114:
                    //umbradrax interrupted
                    _npcList.First(x => x.ZoneId == 950 && x.TemplateId == 1102).BreakShield();
                    break;
                case 9950115:
                    //ignidrax interrupted
                    _npcList.First(x => x.ZoneId == 950 && x.TemplateId == 1100).BreakShield();
                    break;
                case 9950116:
                    //terradrax interrupted
                    _npcList.First(x => x.ZoneId == 950 && x.TemplateId == 1101).BreakShield();
                    break;
                case 9950044:
                    //shield fail
                    break;
            }

        }
        private void SetVergos(NPC boss)
        {
            Vergos = boss.ZoneId == 950 && (boss.TemplateId == 1000 || boss.TemplateId == 2000 ||
                                            boss.TemplateId == 3000 || boss.TemplateId == 4000)
                ? boss
                : null;
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
        private void CheckHarrowholdPhase(ushort zoneId, uint templateId)
        {
            if (zoneId != 950) return;
            if (templateId >= 1100 && templateId <= 1103)
            {
                CurrentHHphase = HarrowholdPhase.Phase1;
            }
            else switch (templateId)
                {
                    case 1000:
                        CurrentHHphase = HarrowholdPhase.Phase2;
                        break;
                    case 2000:
                        CurrentHHphase = HarrowholdPhase.Balistas;
                        break;
                    case 3000:
                        CurrentHHphase = HarrowholdPhase.Phase3;
                        break;
                    case 4000:
                        CurrentHHphase = HarrowholdPhase.Phase4;
                        break;
                }
        }
        private static Dragon CheckCurrentDragon(Point p)
        {
            var rel = MathUtils.GetRelativePoint(p.X, p.Y, -7672, -84453);

            Dragon d;
            if (rel.Y > .8 * rel.X - 78)
                d = rel.Y > -1.3 * rel.X - 94 ? Dragon.Aquadrax : Dragon.Umbradrax;
            else d = rel.Y > -1.3 * rel.X - 94 ? Dragon.Terradrax : Dragon.Ignidrax;

            return d;
        }

        #endregion

    }
}
