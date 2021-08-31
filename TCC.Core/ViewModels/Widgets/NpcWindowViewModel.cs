using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using Nostrum;
using Nostrum.WPF.Extensions;
using Nostrum.WPF.Factories;
using Nostrum.WPF.ThreadSafe;
using TCC.Data;
using TCC.Data.Abnormalities;
using TCC.Data.Databases;
using TCC.Data.Npc;
using TCC.Settings.WindowSettings;
using TCC.UI;
using TCC.Utilities;
using TCC.Utils;
using TeraDataLite;
using TeraPacketParser.Analysis;
using TeraPacketParser.Messages;

namespace TCC.ViewModels.Widgets
{

    // TODO: move HH stuff to own class
    [TccModule]
    public class NpcWindowViewModel : TccWindowViewModel
    {
        private readonly ThreadSafeObservableCollection<NPC> _npcList;

        private readonly Dictionary<ulong, string> _towerNames = new();
        private readonly Dictionary<ulong, double> _savedHp = new();
        private readonly Dictionary<ulong, double> _cache = new();

        public event Action? NpcListChanged;

        public int VisibleBossesCount => _npcList.ToSyncList().Count(x => x.Visible && x.CurrentHP > 0);
        public int VisibleMobsCount => _npcList.ToSyncList().Count(x => x.Visible/* && x.CurrentHP > 0 */&& !x.IsBoss);
        public bool IsCompact => VisibleMobsCount > 6;

        public ICollectionViewLiveShaping Bams { get; }
        public ICollectionViewLiveShaping Mobs { get; }
        public ICollectionViewLiveShaping GuildTowers { get; }

        public Dictionary<ulong, uint> GuildIds { get; } = new();

        public NpcWindowViewModel(NpcWindowSettings settings) : base(settings)
        {
            _npcList = new ThreadSafeObservableCollection<NPC>(_dispatcher);
            Bams = CollectionViewFactory.CreateLiveCollectionView(_npcList,
                npc => npc != null && npc.IsBoss && !npc.IsTower && npc.Visible,
                new[] { nameof(NPC.Visible), nameof(NPC.IsBoss) },
                new[] { new SortDescription(nameof(NPC.CurrentHP), ListSortDirection.Ascending) });
            Mobs = CollectionViewFactory.CreateLiveCollectionView(_npcList,
                npc => npc != null && !npc.IsBoss && !npc.IsTower && npc.Visible,
                new[] { nameof(NPC.Visible), nameof(NPC.IsBoss) },
                new[] { new SortDescription(nameof(NPC.CurrentHP), ListSortDirection.Ascending) });
            GuildTowers = CollectionViewFactory.CreateLiveCollectionView(_npcList,
                npc => npc != null && npc.IsTower,
                sortFilters: new[] { new SortDescription(nameof(NPC.CurrentHP), ListSortDirection.Ascending) });

            PendingAbnormalities = new List<PendingAbnormality>();
            InitFlushTimer();
            NpcListChanged += FlushCache;
            settings.AccurateHpChanged += OnAccurateHpChanged;
            settings.HideAddsChanged += OnHideAddsChanged;
            MonsterDatabase.OverrideChangedEvent += RefreshOverride;
            MonsterDatabase.BlacklistChangedEvent += RefreshBlacklist;

            void InitFlushTimer()
            {
                var flushTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
                flushTimer.Tick += (_, _) => FlushCache();
                flushTimer.Start();
            }
        }

        private void RefreshBlacklist(uint zoneId, uint templateId, bool b)
        {
            if (!b) return;
            _npcList.ToSyncList()
                .Where(n => n.ZoneId == zoneId && n.TemplateId == templateId)
                .ToList()
                .ForEach(RemoveAndDisposeNPC);
        }

        private void OnHideAddsChanged()
        {
            if (!((NpcWindowSettings)Settings!).HideAdds) return;
            _npcList.ToSyncList().Where(x => !x.IsBoss).ToList().ForEach(RemoveAndDisposeNPC);
        }

        public void AddOrUpdateNpc(ulong entityId, float maxHp, float curHp, bool isBoss, HpChangeSource src, uint templateId = 0, uint zoneId = 0, bool visibility = true, int remainingEnrageTime = 0)
        {
            _dispatcher?.InvokeAsync(() =>
            {
                var boss = GetOrAddNpc(entityId, zoneId, templateId, isBoss, visibility);
                SetHP(boss, maxHp, curHp, src);
                SetEnrageTime(entityId, remainingEnrageTime);
                if (boss.Visible == visibility) return;
                boss.Visible = visibility;
                _dispatcher?.Invoke(() => CheckPendingAbnormalities(boss));
                NpcListChanged?.Invoke();
            });
        }

        private void CheckPendingAbnormalities(NPC npc)
        {
            if (PendingAbnormalities.Count == 0) return;
            var npcAbs = PendingAbnormalities.Where(x => x.Target == npc.EntityId).ToList();
            npcAbs.ForEach(ab =>
            {
                if (npc.EntityId != ab.Target) return;

                var delay = (uint)(DateTime.Now - ab.ArrivalTime).TotalMilliseconds;
                if (delay >= ab.Duration) return;
                npc.AddorRefresh(ab.Abnormality, ab.Duration - delay, ab.Stacks);
                Log.CW($"Applied pending abnormal {ab.Abnormality.Name} to {ab.Target} ({npc.Name})");
            });
            npcAbs.ForEach(aa => PendingAbnormalities.Remove(aa));
        }

        public void RemoveNPC(NPC npc, uint delay)
        {
            _dispatcher?.InvokeAsync(() =>
            {
                if (!TryFindNPC(npc.EntityId, out _)) return;
                npc.Buffs.Clear();
                if (delay != 0)
                {
                    Task.Delay(TimeSpan.FromMilliseconds(delay))
                        .ContinueWith(_ => _dispatcher?.Invoke(() => RemoveAndDisposeNPC(npc)));
                }
                else
                {
                    RemoveAndDisposeNPC(npc);
                }
            });
        }
        public void UpdateAbnormality(Abnormality ab, int stacks, uint duration, ulong target)
        {
            if (!TryFindNPC(target, out var boss))
            {
                if (Game.IsMe(target) || Game.NearbyPlayers.ContainsKey(target)) return;
                Log.CW($"Added pending abnormal {ab.Name} for {target}");
                _dispatcher?.Invoke(() =>
                {
                    PendingAbnormalities.Add(new PendingAbnormality
                    {
                        Target = target,
                        Abnormality = ab,
                        Duration = duration,
                        Stacks = stacks,
                        ArrivalTime = DateTime.Now
                    });
                });
                return;
            }
            boss!.AddorRefresh(ab, duration, stacks);
        }

        public List<PendingAbnormality> PendingAbnormalities { get; }

        public struct PendingAbnormality
        {
            public ulong Target { get; set; }
            public Abnormality Abnormality { get; set; }
            public uint Duration { get; set; }
            public int Stacks { get; set; }
            public DateTime ArrivalTime { get; set; }
        }

        public void EndAbnormality(ulong target, Abnormality ab)
        {
            if (!TryFindNPC(target, out var boss)) return;
            boss!.EndBuff(ab);
        }
        public void Clear()
        {
            _dispatcher?.InvokeAsync(() =>
            {
                foreach (var npc in _npcList.ToSyncList())
                {
                    npc?.Dispose();
                }
                _npcList.Clear();
            });
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
                Log.N("Boss window", "Failed to copy boss HP to clipboard.", NotificationType.Error);
                ChatManager.Instance.AddTccMessage("Failed to copy boss HP.");
            }
        }
        public bool TryFindNPC(ulong entityId, out NPC found)
        {
            found = new NPC(0, 0, 0, false, false);
            var f = _npcList.ToSyncList().FirstOrDefault(x => x?.EntityId == entityId);
            if (f != null)
            {
                found = f;
                return true;
            }
            return false;
        }

        private void RefreshOverride(uint zoneId, uint templateId, bool b)
        {
            _npcList.ToSyncList()
                    .Where(n => n.ZoneId == zoneId && n.TemplateId == templateId)
                    .ToList()
                    .ForEach(n => n.IsBoss = b);
        }

        private NPC GetOrAddNpc(ulong eid, uint zone, uint template, bool isBoss, bool visible)
        {
            if (TryFindNPC(eid, out var npc))
            {
                return npc;
            }

            AddNPC(eid, zone, template, isBoss, visible);
            TryFindNPC(eid, out npc);
            return npc;
        }
        private void AddOrUpdateNpc(S_SPAWN_NPC spawn, Monster npcData)
        {
            _dispatcher?.InvokeAsync(() =>
            {
                var visibility = npcData.IsBoss && TccUtils.IsFieldBoss(npcData.ZoneId, npcData.TemplateId);
                var boss = GetOrAddNpc(spawn.EntityId, npcData.ZoneId, npcData.TemplateId, npcData.IsBoss, visibility);
                SetHP(boss, spawn.MaxHP != 0 ? spawn.MaxHP : npcData.MaxHP, spawn.MaxHP != 0 ? spawn.MaxHP : npcData.MaxHP, HpChangeSource.CreatureChangeHp);
                SetEnrageTime(spawn.EntityId, spawn.RemainingEnrageTime);
                if (boss.Visible == visibility) return;
                boss.Visible = visibility;
                NpcListChanged?.Invoke();
            });
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
            _dispatcher?.InvokeAsync(() =>
            {
                if (_cache.Count == 0) return;
                try
                {
                    _cache.ToList().ForEach(hpc => SetFromCache(hpc.Key, hpc.Value));
                }
                catch (Exception ex)
                {
                    Log.CW($"[{nameof(NpcWindowViewModel)}.{nameof(FlushCache)}] Error while setting HP from cache: {ex.Message}");
                }

                _cache.Clear();

            }, DispatcherPriority.Background);
        }
        private void SetFromCache(ulong hpcEntityId, double hpcCurrentHp)
        {
            if (!TryFindNPC(hpcEntityId, out var npc)) return;
            npc.CurrentHP = hpcCurrentHp;
        }
        private void AddNPC(ulong entityId, uint zoneId, uint templateId, bool isBoss, bool visibility)
        {
            if (App.Settings.NpcWindowSettings.HideAdds && !isBoss) return;
            if (templateId == 0 || zoneId == 0) return;
            if (zoneId == 1023) return;

            var boss = new NPC(entityId, zoneId, templateId, isBoss, visibility);

            boss.Visible = boss.IsTower || visibility;
            if (boss.IsTower)
            {
                HandleNewTower(boss, entityId);
            }
            else if (boss.IsPhase1Dragon)
            {
                HandleNewPh1Dragon(boss);
            }
            else
            {
                AddNormalNPC(boss);
            }

            boss.SetTimerPattern();
            boss.SetEnragePattern();
            NpcListChanged?.Invoke();
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
                WindowManager.ViewModels.CivilUnrestVM.SetGuildName(tower.GuildId, towerName); //TODO: check for enabled?
            }
            tower.IsBoss = true;
            _npcList.Add(tower);
            if (_savedHp.TryGetValue(entityId, out var hp)) tower.CurrentHP = hp;

        }
        private void SetHP(NPC boss, double maxHp, double curHp, HpChangeSource src)
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
        private void CacheHP(ulong entityId, double curHp)
        {
            _cache[entityId] = curHp;
        }
        private void SetAggroTarget(ulong entityId, ulong user)
        {
            if (!TryFindNPC(entityId, out var boss)) return;
            boss.Target = user;
            boss.CurrentAggroType = AggroCircle.Main;
            if (boss.Visible) WindowManager.ViewModels.GroupVM.SetAggro(entityId);
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
            CheckHarrowholdPhase(p.HuntingZoneId, p.TemplateId);
            if (!Game.DB!.MonsterDatabase.TryGetMonster(p.TemplateId, p.HuntingZoneId, out var m)) return;
            if (App.Settings.NpcWindowSettings.HideAdds && !m.IsBoss) return;
            AddOrUpdateNpc(p, m);
        }
        private void OnSpawnMe(S_SPAWN_ME m)
        {
            Clear();
        }
        private void OnLoadTopo(S_LOAD_TOPO m)
        {
            CurrentHHphase = HarrowholdPhase.None;
            ClearGuildTowers();
            _dispatcher?.Invoke(() => PendingAbnormalities.Clear());
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
            _dispatcher?.InvokeAsync(() =>
            {
                if (TryFindNPC(p.Target, out var boss))
                {
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

                    if (VisibleBossesCount == 0)
                    {
                        Game.Encounter = false;
                    }

                }

            });
        }
        private void OnBossGageInfo(S_BOSS_GAGE_INFO m)
        {
            AddOrUpdateNpc(m.EntityId, m.MaxHP, m.CurrentHP, true, HpChangeSource.BossGage, m.TemplateId, m.HuntingZoneId);
        }
        private void OnCreatureChangeHp(S_CREATURE_CHANGE_HP m)
        {
            if (Game.IsMe(m.Target)) return;
            AddOrUpdateNpc(m.Target, m.MaxHP, m.CurrentHP, false, Game.IsMe(m.Source) ? HpChangeSource.Me : HpChangeSource.CreatureChangeHp);
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
            if (!Game.DB!.AbnormalityDatabase.GetAbnormality(p.AbnormalityId, out var ab) || !ab.CanShow) return;
            if (p.Duration == int.MaxValue) ab.Infinity = true;


            UpdateAbnormality(ab, p.Stacks, p.Duration, p.TargetId);
        }
        private void OnAbnormalityRefresh(S_ABNORMALITY_REFRESH p)
        {
            if (!Game.DB!.AbnormalityDatabase.GetAbnormality(p.AbnormalityId, out var ab) || !ab.CanShow) return;
            if (p.Duration == Int32.MaxValue) ab.Infinity = true;

            UpdateAbnormality(ab, p.Stacks, p.Duration, p.TargetId);
        }
        private void OnAbnormalityEnd(S_ABNORMALITY_END p)
        {
            if (!Game.DB!.AbnormalityDatabase.GetAbnormality(p.AbnormalityId, out var ab) || !ab.CanShow) return;
            EndAbnormality(p.TargetId, ab);
        }

        #endregion

        #region HH
        public const int Ph1ShieldDuration = 16;

        private NPC? _selectedDragon;
        private NPC? _vergos;
        private readonly List<NPC> _holdedDragons = new();
        private HarrowholdPhase _currentHHphase;
        private ICollectionView? _dragons;

        public NPC? SelectedDragon
        {
            get => _selectedDragon;
            set
            {
                if (_selectedDragon == value) return;
                _selectedDragon = value;
                N(nameof(SelectedDragon));
            }
        }
        public NPC? Vergos
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
        public ICollectionView? Dragons
        {
            get
            {
                _dragons = new CollectionViewSource { Source = _npcList }.View;
                _dragons.Filter = p => p!=null && ((NPC)p).TemplateId > 1099 && ((NPC)p).TemplateId < 1104;
                return _dragons;
            }
        }

        private void SelectDragon(float x, float y)
        {
            var dragon = CheckCurrentDragon(new Point(x, y));
            foreach (var item in _npcList.ToSyncList().Where(d => d!= null&& d.TemplateId > 1099 && d.TemplateId < 1104))
            {
                if (item.TemplateId == (uint)dragon) { item.IsSelected = true; SelectedDragon = item; }
                else item.IsSelected = false;
            }
        }
        private void AddSortedDragons()
        {
            var umbradrax = _holdedDragons.FirstOrDefault(x => x.TemplateId == 1102);
            var ignidrax = _holdedDragons.FirstOrDefault(x => x.TemplateId == 1100);
            var terradrax = _holdedDragons.FirstOrDefault(x => x.TemplateId == 1101);
            var aquadrax = _holdedDragons.FirstOrDefault(x => x.TemplateId == 1103);
            System.Diagnostics.Debug.Assert(umbradrax != null && ignidrax != null && terradrax != null && aquadrax != null);
            _npcList.Add(umbradrax);
            _npcList.Add(ignidrax);
            _npcList.Add(terradrax);
            _npcList.Add(aquadrax);
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
        private void HandleNewPh1Dragon(NPC boss)
        {
            var d = _holdedDragons.FirstOrDefault(x => x.TemplateId == boss.TemplateId && x.ZoneId == boss.ZoneId);
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
            else
                CurrentHHphase = templateId switch
                {
                    1000 => HarrowholdPhase.Phase2,
                    2000 => HarrowholdPhase.Balistas,
                    3000 => HarrowholdPhase.Phase3,
                    4000 => HarrowholdPhase.Phase4,
                    _ => CurrentHHphase
                };
        }
        private static Dragon CheckCurrentDragon(Point p)
        {
            var rel = p.RelativeTo(new Point(-7672, -84453));

            Dragon d;
            if (rel.Y > .8 * rel.X - 78)
                d = rel.Y > -1.3 * rel.X - 94 ? Dragon.Aquadrax : Dragon.Umbradrax;
            else d = rel.Y > -1.3 * rel.X - 94 ? Dragon.Terradrax : Dragon.Ignidrax;

            return d;
        }

        #endregion

    }
}
