using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using TCC.Data;
using TCC.Data.Abnormalities;
using TCC.Data.NPCs;
using TCC.Parsing;

namespace TCC.ViewModels
{
    public class BossGageWindowViewModel : TccWindowViewModel
    {
        public const int Ph1ShieldDuration = 16;
        private static BossGageWindowViewModel _instance;
        private HarrowholdPhase _currentHHphase;
        private ICollectionViewLiveShaping _bams;
        private ICollectionViewLiveShaping _mobs;
        private ICollectionView _dragons;
        private ICollectionView _guildTowers;

        private NPC _selectedDragon;
        private NPC _vergos;

        private readonly DispatcherTimer _flushTimer;

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

        public HarrowholdPhase CurrentHHphase
        {
            get => _currentHHphase;
            set
            {
                if (_currentHHphase == value) return;
                _currentHHphase = value;
                MessageFactory.Update();
                N(nameof(CurrentHHphase));
            }
        }

        public void SetBossEnrageTime(ulong entityId, int remainingEnrageTime)
        {
            var target = NpcList.FirstOrDefault(x => x.EntityId == entityId);
            if (target != null) target.RemainingEnrageTime = remainingEnrageTime;
        }

        public ICollectionViewLiveShaping Bams
        {
            get
            {
                _bams = Utils.InitLiveView(p => ((NPC)p).IsBoss && !((NPC)p).IsTower && ((NPC)p).Visible, NpcList, new string[] { nameof(NPC.Visible) },
                    new[] { new SortDescription(nameof(NPC.CurrentHP), ListSortDirection.Ascending) });
                return _bams;
            }
        }
        public ICollectionViewLiveShaping Mobs
        {
            get
            {
                _mobs = Utils.InitLiveView(p => !((NPC)p).IsBoss && !((NPC)p).IsTower && ((NPC)p).Visible, NpcList, new string[] { nameof(NPC.Visible) },
                    new[] { new SortDescription(nameof(NPC.CurrentHP), ListSortDirection.Ascending) });
                return _mobs;
            }
        }
        public ICollectionView GuildTowers
        {
            get
            {
                _guildTowers = new CollectionViewSource { Source = NpcList }.View;
                _guildTowers.Filter = p => ((NPC)p).IsTower;
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

        public static BossGageWindowViewModel Instance => _instance ?? (_instance = new BossGageWindowViewModel());
        public Dictionary<ulong, uint> GuildIds { get; private set; }

        public BossGageWindowViewModel()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            NpcList = new SynchronizedObservableCollection<NPC>(Dispatcher);
            _cache = new Dictionary<ulong, float>();
            _flushTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            _flushTimer.Tick += FlushCache;
            _flushTimer.Start();
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
                    //Log.CW($"[BossGageWindowViewModel L161] HP cache is empty, returning");
                    return;
                }
                try
                {
                    foreach (var hpc in _cache.ToList())
                    {
                        SetHpFromCache(hpc.Key, hpc.Value);
                    }
                }
                catch (Exception ex)
                {
                    Log.CW($"[BossGageWindowViewModel L173] Error while setting HP from cache: {ex.Message}");

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
                Log.CW($"[BossGageWindowViewModel L189] NPC {hpcEntityId} not found while setting HP from cache");

            }
        }

        private bool IsCaching => true;//VisibleBossesCount > 3;
        public bool IsCompact => VisibleMobsCount > 6;

        private NPC AddNpc(ulong entityId, uint zoneId, uint templateId, bool isBoss, bool visibility)
        {
            if (Settings.SettingsHolder.ShowOnlyBosses && !isBoss) return null;
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

        private void HandleNewTower(NPC boss, ulong entityId)
        {
            if (_towerNames.TryGetValue(entityId, out var towerName))
            {
                boss.Name = towerName;
                WindowManager.CivilUnrestWindow.VM.SetGuildName(boss.GuildId, towerName); //TODO: check for enabled?
            }
            boss.IsBoss = true;
            NpcList.Add(boss);
            if (_savedHp.TryGetValue(entityId, out var hp)) boss.CurrentHP = hp;

        }

        public void AddOrUpdateBoss(ulong entityId, float maxHp, float curHp, bool isBoss, HpChangeSource src, uint templateId = 0, uint zoneId = 0, bool visibility = true)
        {
            var boss = NpcList.ToSyncList().FirstOrDefault(x => x.EntityId == entityId) ?? AddNpc(entityId, zoneId, templateId, isBoss, visibility);
            if (boss == null) return;
            SetHp(boss, maxHp, curHp, src);
            if (boss.Visible != visibility)
            {
                boss.Visible = visibility;
                NpcListChanged?.Invoke();
            }
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
            _cache[entityId] = curHp;
        }

        private static void SetTimerPattern(NPC n)
        {
            if (Settings.SettingsHolder.EthicalMode) return;

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
            if (Settings.SettingsHolder.EthicalMode)
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
            var boss = NpcList.ToSyncList().FirstOrDefault(x => x.EntityId == id);
            if (boss == null) return;
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
            foreach (var npc in NpcList.ToSyncList())
            {
                npc.Dispose();
            }
            NpcList.Clear();
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
            if (boss.Visible) GroupWindowViewModel.Instance.SetAggro(entityId);
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
            if (boss != null)
            {
                boss.CurrentShield -= damage;
            }
        }

        public void RemoveMe(NPC npc, uint delay)
        {
            Dispatcher.Invoke(() =>
            {
                if (NpcList.ToSyncList().All(x => x != npc)) return;
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
            });
        }

        private void RemoveAndDisposeNPC(NPC b)
        {
            b.Dispose();
            NpcList.Remove(b);
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
