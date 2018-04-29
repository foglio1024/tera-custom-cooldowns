using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using TCC.Data;
using TCC.Data.Databases;
using TCC.Parsing;
using TCC.Parsing.Messages;

namespace TCC.ViewModels
{
    public class GroupWindowViewModel : TccWindowViewModel
    {
        private static GroupWindowViewModel _instance;
        private bool _raid;
        private ulong _aggroHolder;
        private bool _firstCheck = true;
        private readonly object _lock = new object();
        public event Action SettingsUpdated;

        public static GroupWindowViewModel Instance => _instance ?? (_instance = new GroupWindowViewModel());
        public bool IsTeraOnTop => WindowManager.IsTccVisible; //TODO: is this needed? need to check for all VM
        public SynchronizedObservableCollection<User> Members { get; }
        public ICollectionViewLiveShaping Dps { get; }
        public ICollectionViewLiveShaping Tanks { get; }
        public ICollectionViewLiveShaping Healers { get; }
        public bool Raid
        {
            get => _raid;
            set
            {
                if (_raid == value) return;
                _raid = value;
                NPC(nameof(Raid));
            }
        }
        public int Size => Members.Count;
        public int ReadyCount => Members.ToSyncArray().Count(x => x.Ready == ReadyStatus.Ready);
        public int AliveCount => Members.ToSyncArray().Count(x => x.Alive);
        public bool Formed => Size > 0;
        public bool Rolling { get; set; }

        public GroupWindowViewModel()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            _scale = SettingsManager.GroupWindowSettings.Scale;

            WindowManager.TccVisibilityChanged += (s, ev) =>
            {
                NPC("IsTeraOnTop");
                if (IsTeraOnTop)
                {
                    WindowManager.GroupWindow.RefreshTopmost();
                }
            };

            Members = new SynchronizedObservableCollection<User>(_dispatcher);
            Members.CollectionChanged += Members_CollectionChanged;

            Dps = Utils.InitLiveView(o => ((User)o).Role == Role.Dps, Members, new string[] { nameof(User.Role) }, new string[] { });
            Tanks = Utils.InitLiveView(o => ((User)o).Role == Role.Tank, Members, new string[] { nameof(User.Role) }, new string[] { });
            Healers = Utils.InitLiveView(o => ((User)o).Role == Role.Healer, Members, new string[] { nameof(User.Role) }, new string[] { });

        }

        private void Members_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Task.Delay(100).ContinueWith(t => NPC(nameof(Size)));
            NPC(nameof(Formed));
            NPC(nameof(AliveCount));
            NPC(nameof(ReadyCount));

        }
        public void NotifySettingUpdated()
        {
            SettingsUpdated?.Invoke();
        }
        public bool Exists(ulong id)
        {
            return Members.ToSyncArray().Any(x => x.EntityId == id);
        }
        public bool Exists(string name)
        {
            return Members.ToSyncArray().Any(x => x.Name == name);
        }
        public bool Exists(uint pId, uint sId)
        {
            return Members.ToSyncArray().Any(x => x.PlayerId == pId && x.ServerId == sId);
        }

        public bool TryGetUser(string name, out User u)
        {
            u = Exists(name) ? Members.ToSyncArray().FirstOrDefault(x => x.Name == name) : null;
            return Exists(name);
        }
        public bool TryGetUser(ulong id, out User u)
        {
            u = Exists(id) ? Members.ToSyncArray().FirstOrDefault(x => x.EntityId == id) : null;
            return Exists(id);
        }
        public bool TryGetUser(uint pId, uint sId, out User u)
        {
            u = Exists(pId, sId) ? Members.ToSyncArray().FirstOrDefault(x => x.PlayerId == pId && x.ServerId == sId) : null;
            return Exists(pId, sId);
        }

        public bool IsLeader(string name)
        {
            return Members.ToSyncArray().FirstOrDefault(x => x.Name == name)?.IsLeader ?? false;
        }
        public bool HasPowers(string name)
        {
            return Members.ToSyncArray().FirstOrDefault(x => x.Name == name)?.CanInvite ?? false;
        }
        public bool AmILeader => IsLeader(SessionManager.CurrentPlayer.Name);

        public void SetAggro(ulong target)
        {
            if (target == 0)
            {
                //Members.ToList().ForEach(user => user.HasAggro = false);
                foreach (var item in Members.ToSyncArray())
                {
                    item.HasAggro = false;
                }
                return;
            }
            if (_aggroHolder == target) return;
            //Members.ToList().ForEach(user => user.HasAggro = user.EntityId == target);
            foreach (var item in Members.ToSyncArray())
            {
                item.HasAggro = item.EntityId == target;
            }
        }
        public void SetAggroCircle(S_USER_EFFECT p)
        {
            if (BossGageWindowViewModel.Instance.CurrentHHphase != HarrowholdPhase.None) return;

            if (p.Circle != AggroCircle.Main) return;
            if (p.Action == AggroAction.Add)
            {
                SetAggro(p.User);
            }
        }
        public void BeginOrRefreshAbnormality(Abnormality ab, int stacks, uint duration, uint playerId, uint serverId)
        {
            if (ab.Infinity) duration = uint.MaxValue;
            var u = Members.ToSyncArray().FirstOrDefault(x => x.ServerId == serverId && x.PlayerId == playerId);
            if (u == null) return;

            if (ab.Type == AbnormalityType.Buff)
            {
                u.AddOrRefreshBuff(ab, duration, stacks);
                if (u.UserClass == Class.Warrior && ab.Id >= 100200 && ab.Id <= 100203)
                {
                    u.Role = Role.Tank; //def stance turned on: switch warrior to tank 
                }
            }
            else
            {
                // -- show only aggro stacks if we are in HH -- //
                if (BossGageWindowViewModel.Instance.CurrentHHphase >= HarrowholdPhase.Phase2)
                {
                    if (ab.Id != 950023 && SettingsManager.ShowOnlyAggroStacks) return;
                }
                // -------------------------------------------- //
                u.AddOrRefreshDebuff(ab, duration, stacks);
            }
        }
        public void EndAbnormality(Abnormality ab, uint playerId, uint serverId)
        {
            User u = null;
            u = Members.ToSyncArray().FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            if (u == null) return;

            if (ab.Type == AbnormalityType.Buff)
            {
                u.RemoveBuff(ab);
                if (u.UserClass == Class.Warrior && ab.Id >= 100200 && ab.Id <= 100203)
                {
                    u.Role = Role.Dps; //def stance ended: make warrior dps again
                }
            }
            else
            {
                u.RemoveDebuff(ab);
            }



        }
        public void ClearAbnormality(uint playerId, uint serverId)
        {
            Members.ToSyncArray().FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId)?.ClearAbnormalities();
        }
        public void AddOrUpdateMember(User p)
        {
            if (SettingsManager.IgnoreMeInGroupWindow && p.IsPlayer) return;
            lock (_lock)
            {
                var user = Members.ToSyncArray().FirstOrDefault(x => x.PlayerId == p.PlayerId && x.ServerId == p.ServerId);
                if (user == null)
                {
                    Members.Add(p);
                    SendAddMessage(p.Name);
                    return;
                }
                user.Online = p.Online;
                user.EntityId = p.EntityId;
                user.IsLeader = p.IsLeader;
            }
        }
        private void SendAddMessage(string name)
        {
            if (App.Debug) return;
            string msg;
            string opcode;
            if (Raid)
            {
                msg = "@0\vPartyPlayerName\v" + name;
                opcode = "SMT_JOIN_UNIONPARTY_PARTYPLAYER";
            }
            else
            {
                opcode = "SMT_JOIN_PARTY_PARTYPLAYER";
                msg = "@0\vPartyPlayerName\v" + name + "\vparty\vparty";
            }
            SystemMessages.Messages.TryGetValue(opcode, out var m);
            SystemMessagesProcessor.AnalyzeMessage(msg, m, opcode);
        }
        private void SendLeaveMessage(string name)
        {
            string msg;
            string opcode;
            if (Raid)
            {
                msg = "@0\vPartyPlayerName\v" + name;
                opcode = "SMT_LEAVE_UNIONPARTY_PARTYPLAYER";
            }
            else
            {
                opcode = "SMT_LEAVE_PARTY_PARTYPLAYER";
                msg = "@0\vPartyPlayerName\v" + name + "\vparty\vparty";
            }
            SystemMessages.Messages.TryGetValue(opcode, out var m);
            SystemMessagesProcessor.AnalyzeMessage(msg, m, opcode);

        }
        public void RemoveMember(uint playerId, uint serverId, bool kick = false)
        {
            var u = Members.ToSyncArray().FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            if (u == null) return;
            Members.Remove(u);
            if (!kick) SendLeaveMessage(u.Name);
        }
        public void ClearAll()
        {
            if (!SettingsManager.GroupWindowSettings.Enabled || !_dispatcher.Thread.IsAlive) return;
            Members.Clear();
            Raid = false;
        }
        public void LogoutMember(uint playerId, uint serverId)
        {
            var u = Members.ToSyncArray().FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            if (u == null) return;
            u.Online = false;
        }
        public void RemoveMe()
        {
            var me = Members.ToSyncArray().FirstOrDefault(x => x.IsPlayer);
            if (me != null) Members.Remove(me);
        }
        public void ClearAllBuffs()
        {
            foreach (var x in Members.ToSyncArray())
            {
                foreach (var b in x.Buffs.ToSyncArray())
                {
                    b.Dispose();
                }
                x.Buffs.Clear();
            }
        }
        internal void ClearAllAbnormalities()
        {
            foreach (var x in Members.ToSyncArray())
            {
                foreach (var b in x.Buffs.ToSyncArray())
                {
                    b.Dispose();
                }
                x.Buffs.Clear();
                foreach (var b in x.Debuffs.ToSyncArray())
                {
                    b.Dispose();
                }
                x.Debuffs.Clear();
            }
        }
        public void SetNewLeader(ulong entityId, string name)
        {
            foreach (var m in Members.ToSyncArray())
            {
                m.IsLeader = m.Name == name;
            }
        }
        public void StartRoll()
        {
            Rolling = true;
            //Members.ToList().ForEach(u => u.IsRolling = true);
            foreach (var m in Members.ToSyncArray())
            {
                m.IsRolling = true;
            }
        }
        public void SetRoll(ulong entityId, int rollResult)
        {
            if (rollResult == int.MaxValue) rollResult = -1;
            var u = Members.ToSyncArray().FirstOrDefault(x => x.EntityId == entityId);
            if (u != null) u.RollResult = rollResult;
        }
        public void EndRoll()
        {
            Rolling = false;

            foreach (var m in Members.ToSyncArray())
            {
                m.IsRolling = false;
                m.IsWinning = false;
                m.RollResult = 0;
            }
            //Members.ToList().ForEach(u =>
            //{
            //u.IsRolling = false;
            //u.IsWinning = false;
            //u.RollResult = 0;
            //});
        }
        private void FindHighestRoll()
        {
            foreach (var user in Members.ToSyncArray())
            {
                user.EntityId = Members.ToSyncArray().OrderByDescending(u => u.RollResult).First().EntityId;
            }
            //Members.ToList().ForEach(user => user.IsWinning = user.EntityId == Members.OrderByDescending(u => u.RollResult).First().EntityId);
        }
        public void SetReadyStatus(ReadyPartyMember p)
        {
            if (_firstCheck)
            {
                //Members.ToList().ForEach(u => u.Ready = ReadyStatus.Undefined);
                foreach (var u in Members.ToSyncArray())
                {
                    u.Ready = ReadyStatus.Undefined;
                }
            }
            var user = Members.ToSyncArray().FirstOrDefault(u => u.PlayerId == p.PlayerId && u.ServerId == p.ServerId);
            if (user != null) user.Ready = p.Status;
            _firstCheck = false;
            NPC(nameof(ReadyCount));
        }
        public void EndReadyCheck()
        {
            Task.Delay(4000).ContinueWith(t =>
            {
                //Members.ToList().ForEach(x => x.Ready = ReadyStatus.None);
                foreach (var u in Members.ToSyncArray())
                {
                    u.Ready = ReadyStatus.None;
                }
            });
            _firstCheck = true;
        }
        public void UpdateMemberHp(uint playerId, uint serverId, int curHp, int maxHp)
        {
            User u = null;
            u = Members.ToSyncArray().FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            if (u == null) return;
            u.CurrentHp = curHp;
            u.MaxHp = maxHp;
        }
        public void UpdateMemberMp(uint playerId, uint serverId, int curMp, int maxMp)
        {
            User u = null;
            u = Members.ToSyncArray().FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            if (u == null) return;
            u.CurrentMp = curMp;
            u.MaxMp = maxMp;
        }
        public void SetRaid(bool raid)
        {
            _dispatcher.Invoke(() => Raid = raid);
        }
        public void UpdateMember(S_PARTY_MEMBER_STAT_UPDATE p)
        {
            var u = Members.ToSyncArray().FirstOrDefault(x => x.PlayerId == p.PlayerId && x.ServerId == p.ServerId);
            if (u != null)
            {
                u.CurrentHp = p.CurrentHP;
                u.CurrentMp = p.CurrentMP;
                u.MaxHp = p.MaxHP;
                u.MaxMp = p.MaxMP;
                u.Level = (uint)p.Level;
                u.Alive = p.Alive;
                NPC(nameof(AliveCount));
                if (!p.Alive) u.HasAggro = false;
            }
        }
        public void NotifyThresholdChanged()
        {
            NPC(nameof(Size));
        }
        public void UpdateMemberGear(S_SPAWN_USER sSpawnUser)
        {
            var u = Members.ToSyncArray().FirstOrDefault(x => x.PlayerId == sSpawnUser.PlayerId && x.ServerId == sSpawnUser.ServerId);
            if (u == null) return;
            u.Weapon = sSpawnUser.Weapon;
            u.Armor = sSpawnUser.Armor;
            u.Gloves = sSpawnUser.Gloves;
            u.Boots = sSpawnUser.Boots;
        }
        public void UpdateMyGear()
        {
            var u = Members.ToSyncArray().FirstOrDefault(x => x.IsPlayer);
            if (u == null) return;

            u.Weapon = InfoWindowViewModel.Instance.CurrentCharacter.Gear.FirstOrDefault(x => x.Piece == GearPiece.Weapon);
            u.Armor = InfoWindowViewModel.Instance.CurrentCharacter.Gear.FirstOrDefault(x => x.Piece == GearPiece.Armor);
            u.Gloves = InfoWindowViewModel.Instance.CurrentCharacter.Gear.FirstOrDefault(x => x.Piece == GearPiece.Hands);
            u.Boots = InfoWindowViewModel.Instance.CurrentCharacter.Gear.FirstOrDefault(x => x.Piece == GearPiece.Feet);

        }
        public void UpdateMemberLocation(S_PARTY_MEMBER_INTERVAL_POS_UPDATE p)
        {
            User u = null;
            u = Members.ToSyncArray().FirstOrDefault(x => x.PlayerId == p.PlayerId && x.ServerId == p.ServerId);
            if (u == null) return;
            var ch = p.Channel > 1000 ? "" : " ch." + p.Channel;
            u.Location = MapDatabase.TryGetGuardOrDungeonNameFromContinentId(p.ContinentId, out var l) ? l + ch : "Unknown";
        }

    }

    public class DragBehavior
    {
        public readonly TranslateTransform Transform = new TranslateTransform();
        private System.Windows.Point _elementStartPosition2;
        private System.Windows.Point _mouseStartPosition2;
        private static DragBehavior _instance = new DragBehavior();
        public static DragBehavior Instance
        {
            get { return _instance; }
            set { _instance = value; }
        }

        public static bool GetDrag(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsDragProperty);
        }

        public static void SetDrag(DependencyObject obj, bool value)
        {
            obj.SetValue(IsDragProperty, value);
        }

        public static readonly DependencyProperty IsDragProperty =
          DependencyProperty.RegisterAttached("Drag",
          typeof(bool), typeof(DragBehavior),
          new PropertyMetadata(false, OnDragChanged));

        private static void OnDragChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // ignoring error checking
            var element = (UIElement)sender;
            var isDrag = (bool)(e.NewValue);

            Instance = new DragBehavior();
            ((UIElement)sender).RenderTransform = Instance.Transform;

            if (isDrag)
            {
                element.MouseLeftButtonDown += Instance.ElementOnMouseLeftButtonDown;
                element.MouseLeftButtonUp += Instance.ElementOnMouseLeftButtonUp;
                element.MouseMove += Instance.ElementOnMouseMove;
            }
            else
            {
                element.MouseLeftButtonDown -= Instance.ElementOnMouseLeftButtonDown;
                element.MouseLeftButtonUp -= Instance.ElementOnMouseLeftButtonUp;
                element.MouseMove -= Instance.ElementOnMouseMove;
            }
        }

        private void ElementOnMouseLeftButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var parent = Application.Current.MainWindow;
            _mouseStartPosition2 = mouseButtonEventArgs.GetPosition(parent);
            ((UIElement)sender).CaptureMouse();
        }

        private void ElementOnMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            ((UIElement)sender).ReleaseMouseCapture();
            _elementStartPosition2.X = Transform.X;
            _elementStartPosition2.Y = Transform.Y;
        }

        private void ElementOnMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            var parent = Application.Current.MainWindow;
            var mousePos = mouseEventArgs.GetPosition(parent);
            var diff = (mousePos - _mouseStartPosition2);
            if (!((UIElement)sender).IsMouseCaptured) return;
            Transform.X = _elementStartPosition2.X + diff.X;
            Transform.Y = _elementStartPosition2.Y + diff.Y;
        }
    }
}
