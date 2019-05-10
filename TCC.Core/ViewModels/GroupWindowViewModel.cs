using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using TCC.Annotations;
using TCC.Data;
using TCC.Data.Abnormalities;
using TCC.Data.Pc;
using TCC.Interop.Proxy;
using TCC.Parsing;
using TCC.Parsing.Messages;
using TCC.Settings;

namespace TCC.ViewModels
{
    public class GroupWindowViewModel : TccWindowViewModel
    {
        //private static GroupWindowViewModel _instance;
        private bool _raid;
        private bool _firstCheck = true;
        private readonly object _lock = new object();
        private bool _leaderOverride;
        private ulong _aggroHolder;
        public event Action SettingsUpdated;

        //public static GroupWindowViewModel Instance => _instance ?? (_instance = new GroupWindowViewModel());
        public SynchronizedObservableCollection<User> Members { get; }
        public GroupWindowLayout GroupWindowLayout => SettingsHolder.GroupWindowLayout;

        public ICollectionViewLiveShaping All { get; }
        public ICollectionViewLiveShaping Dps { [UsedImplicitly] get; }
        public ICollectionViewLiveShaping Tanks { [UsedImplicitly] get; }
        public ICollectionViewLiveShaping Healers { [UsedImplicitly] get; }
        public bool Raid
        {
            get => _raid;
            set
            {
                if (_raid == value) return;
                _raid = value;
                N();
            }
        }
        public int Size => Members.Count;
        public int ReadyCount => Members.Count(x => x.Ready == ReadyStatus.Ready);
        public int AliveCount => Members.Count(x => x.Alive);
        public bool Formed => Size > 0;
        public bool ShowDetails => Formed && SettingsHolder.ShowGroupWindowDetails;
        public bool ShowLeaveButton => Formed && /*ProxyOld.IsConnected */ ProxyInterface.Instance.IsStubAvailable;
        public bool ShowLeaderButtons => Formed && /*ProxyOld.IsConnected */ ProxyInterface.Instance.IsStubAvailable && AmILeader;
        public bool Rolling { get; set; }

        public GroupWindowViewModel()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            Members = new SynchronizedObservableCollection<User>(Dispatcher);
            Members.CollectionChanged += Members_CollectionChanged;

            Dps = Utils.InitLiveView(o => ((User)o).Role == Role.Dps && ((User)o).Visible, Members, new[] { nameof(User.Role), nameof(User.Visible) }, new[] { new SortDescription(nameof(User.UserClass), ListSortDirection.Ascending) });
            Tanks = Utils.InitLiveView(o => ((User)o).Role == Role.Tank && ((User)o).Visible, Members, new[] { nameof(User.Role), nameof(User.Visible) }, new[] { new SortDescription(nameof(User.UserClass), ListSortDirection.Ascending) });
            Healers = Utils.InitLiveView(o => ((User)o).Role == Role.Healer && ((User)o).Visible, Members, new[] { nameof(User.Role), nameof(User.Visible) }, new[] { new SortDescription(nameof(User.UserClass), ListSortDirection.Ascending) });
            All = Utils.InitLiveView(o => ((User)o).Visible, Members, new [] {nameof(User.Visible) }, new[]
            {
                new SortDescription(nameof(User.Role), ListSortDirection.Descending),
                new SortDescription(nameof(User.UserClass), ListSortDirection.Ascending)
            });

            ((ICollectionView)Dps).CollectionChanged += GcPls;
            ((ICollectionView)Tanks).CollectionChanged += GcPls;
            ((ICollectionView)Healers).CollectionChanged += GcPls;
            ((ICollectionView)All).CollectionChanged += GcPls;
        }
        private void GcPls(object sender, EventArgs ev) { }

        private void Members_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //Task.Delay(0).ContinueWith(t =>
            //{
            //});
            N(nameof(Size));
            N(nameof(Formed));
            N(nameof(AmILeader));
            N(nameof(ShowDetails));
            N(nameof(ShowLeaveButton));
            N(nameof(AliveCount));
            N(nameof(ReadyCount));
            N(nameof(ShowLeaderButtons));
        }
        public void NotifySettingUpdated()
        {
            SettingsUpdated?.Invoke();

            N(nameof(ShowDetails));
        }
        public bool Exists(ulong id)
        {
            return Members.ToSyncList().Any(x => x.EntityId == id);
        }
        public bool Exists(string name)
        {
            return Members.ToSyncList().Any(x => x.Name == name);
        }
        public bool Exists(uint pId, uint sId)
        {
            return Members.ToSyncList().Any(x => x.PlayerId == pId && x.ServerId == sId);
        }

        public bool TryGetUser(string name, out User u)
        {
            u = Exists(name) ? Members.ToSyncList().FirstOrDefault(x => x.Name == name) : null;
            return Exists(name);
        }
        public bool TryGetUser(ulong id, out User u)
        {
            u = Exists(id) ? Members.ToSyncList().FirstOrDefault(x => x.EntityId == id) : null;
            return Exists(id);
        }
        public bool TryGetUser(uint pId, uint sId, out User u)
        {
            u = Exists(pId, sId) ? Members.ToSyncList().FirstOrDefault(x => x.PlayerId == pId && x.ServerId == sId) : null;
            return Exists(pId, sId);
        }

        public bool IsLeader(string name)
        {
            return Members.FirstOrDefault(x => x.Name == name)?.IsLeader ?? false;
        }
        public bool HasPowers(string name)
        {
            return Members.ToSyncList().FirstOrDefault(x => x.Name == name)?.CanInvite ?? false;
        }
        public bool AmILeader => IsLeader(SessionManager.CurrentPlayer.Name) || _leaderOverride;

        public void SetAggro(ulong target)
        {
            if (target == 0)
            {
                foreach (var item in Members.ToSyncList())
                {
                    item.HasAggro = false;
                }
                return;
            }

            if (_aggroHolder == target) return;
            _aggroHolder = target;
            foreach (var item in Members.ToSyncList())
            {
                item.HasAggro = item.EntityId == target;
            }
        }
        public void SetAggroCircle(S_USER_EFFECT p)
        {
            if (WindowManager.BossWindow.VM.CurrentHHphase != HarrowholdPhase.None) return;

            if (p.Circle != AggroCircle.Main) return;
            if (p.Action == AggroAction.Add)
            {
                SetAggro(p.User);
            }
        }
        public void BeginOrRefreshAbnormality(Abnormality ab, int stacks, uint duration, uint playerId, uint serverId)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (ab.Infinity) duration = uint.MaxValue;
                var u = Members.ToSyncList().FirstOrDefault(x => x.ServerId == serverId && x.PlayerId == playerId);
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
                    if (WindowManager.BossWindow.VM.CurrentHHphase >= HarrowholdPhase.Phase2)
                    {
                        if (ab.Id != 950023 && SettingsHolder.ShowOnlyAggroStacks) return;
                    }
                    // -------------------------------------------- //
                    u.AddOrRefreshDebuff(ab, duration, stacks);
                }
            }));
        }
        public void EndAbnormality(Abnormality ab, uint playerId, uint serverId)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {

                var u = Members.ToSyncList().FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
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
            }));
        }
        public void ClearAbnormality(uint playerId, uint serverId)
        {
            Dispatcher.Invoke(() =>
            {
                Members.ToSyncList().FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId)?.ClearAbnormalities();
            });
        }
        public void AddOrUpdateMember(User p)
        {
            if (SettingsHolder.IgnoreMeInGroupWindow && p.IsPlayer)
            {
                _leaderOverride = p.IsLeader;
                p.Visible = false;
                //return;
            }
            lock (_lock) //TODO: really needed?
            {
                var user = Members.ToSyncList().FirstOrDefault(x => x.PlayerId == p.PlayerId && x.ServerId == p.ServerId);
                if (user == null)
                {
                    Members.Add(p);
                    SendAddMessage(p.Name);
                    return;
                }

                if (user.Online != p.Online) SendOnlineMessage(user.Name, p.Online);
                user.Online = p.Online;
                user.EntityId = p.EntityId;
                user.IsLeader = p.IsLeader;
                user.Order = p.Order;
                user.Awakened = p.Awakened;
                user.Visible = p.Visible;
            }
        }

        private void SendOnlineMessage(string name, bool newVal)
        {
            var opcode = newVal ? "TCC_PARTY_MEMBER_LOGON" : "TCC_PARTY_MEMBER_LOGOUT";
            var msg = "@0\vUserName\v" + name;
            SessionManager.DB.SystemMessagesDatabase.Messages.TryGetValue(opcode, out var m);
            SystemMessagesProcessor.AnalyzeMessage(msg, m, opcode);
        }

        private void SendAddMessage(string name)
        {
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
            SessionManager.DB.SystemMessagesDatabase.Messages.TryGetValue(opcode, out var m);
            SystemMessagesProcessor.AnalyzeMessage(msg, m, opcode);
        }
        private void SendDeathMessage(string name)
        {
            string msg;
            string opcode;
            if (Raid)
            {
                msg = "@0\vPartyPlayerName\v" + name;
                opcode = "SMT_BATTLE_PARTY_DIE";
            }
            else
            {
                opcode = "SMT_BATTLE_PARTY_DIE";
                msg = "@0\vPartyPlayerName\v" + name + "\vparty\vparty";
            }
            SessionManager.DB.SystemMessagesDatabase.Messages.TryGetValue(opcode, out var m);
            SystemMessagesProcessor.AnalyzeMessage(msg, m, opcode);
            if (/*ProxyOld.IsConnected */ ProxyInterface.Instance.IsStubAvailable) ProxyInterface.Instance.Stub.ForceSystemMessage(msg, opcode); //ProxyOld.ForceSystemMessage(msg, opcode);
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
            SessionManager.DB.SystemMessagesDatabase.Messages.TryGetValue(opcode, out var m);
            SystemMessagesProcessor.AnalyzeMessage(msg, m, opcode);

        }
        public void RemoveMember(uint playerId, uint serverId, bool kick = false)
        {
            var u = Members.ToSyncList().FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            if (u == null) return;
            u.ClearAbnormalities();
            Members.Remove(u);
            if (!kick) SendLeaveMessage(u.Name);
        }
        public void ClearAll()
        {
            if (!SettingsHolder.GroupWindowSettings.Enabled || !Dispatcher.Thread.IsAlive) return;
            Members.ToSyncList().ForEach(x => x.ClearAbnormalities());
            Members.Clear();
            Raid = false;
            _leaderOverride = false;
        }
        public void LogoutMember(uint playerId, uint serverId)
        {
            var u = Members.ToSyncList().FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
            if (u == null) return;
            u.Online = false;
        }
        public void ToggleMe(bool visible)
        {
            var me = Members.ToSyncList().FirstOrDefault(x => x.IsPlayer);
            if (me == null) return;
            me.Visible = visible;
            //me.ClearAbnormalities();
            //Members.Remove(me);
        }
        internal void ClearAllAbnormalities()
        {
            foreach (var x in Members.ToSyncList())
            {
                foreach (var b in x.Buffs.ToSyncList())
                {
                    b.Dispose();
                }
                x.Buffs.Clear();
                foreach (var b in x.Debuffs.ToSyncList())
                {
                    b.Dispose();
                }
                x.Debuffs.Clear();
            }
        }
        public void SetNewLeader(ulong entityId, string name)
        {
            foreach (var m in Members.ToSyncList())
            {
                m.IsLeader = m.Name == name;
            }
            _leaderOverride = name == SessionManager.CurrentPlayer.Name;
            N(nameof(AmILeader));
            N(nameof(ShowLeaderButtons));
        }
        public void StartRoll()
        {
            Rolling = true;
            //Members.ToList().ForEach(u => u.IsRolling = true);
            foreach (var m in Members.ToSyncList())
            {
                m.IsRolling = true;
            }
        }
        public void SetRoll(ulong entityId, int rollResult)
        {
            if (rollResult == int.MaxValue) rollResult = -1;
            Members.ToSyncList().ForEach(member =>
            {
                if (member.EntityId == entityId)
                {
                    member.RollResult = rollResult;
                }
                member.IsWinning = member.EntityId == GetWinningUser() && member.RollResult != -1;
            });
            //var u = Members.ToSyncList().FirstOrDefault(x => x.EntityId == entityId);
            //if (u == null) return;
            //u.RollResult = rollResult;
            //u.IsWinning = u.EntityId == GetWinningUser();
        }
        public void EndRoll()
        {
            Rolling = false;

            foreach (var m in Members.ToSyncList())
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
        private ulong GetWinningUser()
        {
            return Members.ToSyncList().OrderByDescending(u => u.RollResult).First().EntityId;
            //Members.ToList().ForEach(user => user.IsWinning = user.EntityId == Members.OrderByDescending(u => u.RollResult).First().EntityId);
        }
        public void SetReadyStatus(ReadyPartyMember p)
        {

            if (_firstCheck)
            {
                foreach (var u in Members.ToSyncList())
                {
                    u.Ready = ReadyStatus.Undefined;
                }
            }
            var user = Members.ToSyncList().FirstOrDefault(u => u.PlayerId == p.PlayerId && u.ServerId == p.ServerId);
            if (user != null) user.Ready = p.Status;
            _firstCheck = false;
            N(nameof(ReadyCount));
        }
        public void EndReadyCheck()
        {
            Task.Delay(4000).ContinueWith(t =>
            {
                //Members.ToList().ForEach(x => x.Ready = ReadyStatus.None);
                foreach (var u in Members.ToSyncList())
                {
                    u.Ready = ReadyStatus.None;
                }
            });
            _firstCheck = true;
        }
        public void UpdateMemberHp(uint playerId, uint serverId, int curHp, int maxHp)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var u = Members.ToSyncList().FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
                if (u == null) return;
                u.CurrentHp = curHp;
                u.MaxHp = maxHp;
            }));
        }
        public void UpdateMemberMp(uint playerId, uint serverId, int curMp, int maxMp)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var u = Members.ToSyncList().FirstOrDefault(x => x.PlayerId == playerId && x.ServerId == serverId);
                if (u == null) return;
                u.CurrentMp = curMp;
                u.MaxMp = maxMp;
            }));
        }
        public void SetRaid(bool raid)
        {
            Dispatcher.BeginInvoke(new Action(() => Raid = raid));
        }
        public void UpdateMember(S_PARTY_MEMBER_STAT_UPDATE p)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var u = Members.ToSyncList().FirstOrDefault(x => x.PlayerId == p.PlayerId && x.ServerId == p.ServerId);
                if (u == null) return;
                u.CurrentHp = p.CurrentHP;
                u.CurrentMp = p.CurrentMP;
                u.MaxHp = p.MaxHP;
                u.MaxMp = p.MaxMP;
                u.Level = (uint)p.Level;
                if (u.Alive && !p.Alive) SendDeathMessage(u.Name);
                u.Alive = p.Alive;
                N(nameof(AliveCount));
                if (!p.Alive) u.HasAggro = false;
            }));
        }
        public void NotifyThresholdChanged()
        {
            N(nameof(Size));
        }
        public void UpdateMemberGear(S_SPAWN_USER p)
        {
            var u = Members.ToSyncList().FirstOrDefault(x => x.PlayerId == p.PlayerId && x.ServerId == p.ServerId);
            if (u == null) return;
            u.Weapon = p.Weapon;
            u.Armor = p.Armor;
            u.Gloves = p.Gloves;
            u.Boots = p.Boots;
        }
        public void UpdateMyGear()
        {
            var u = Members.ToSyncList().FirstOrDefault(x => x.IsPlayer);
            if (u == null) return;
            var currCharGear = WindowManager.Dashboard.VM.CurrentCharacter.Gear;
            u.Weapon = currCharGear.FirstOrDefault(x => x.Piece == GearPiece.Weapon);
            u.Armor = currCharGear.FirstOrDefault(x => x.Piece == GearPiece.Armor);
            u.Gloves = currCharGear.FirstOrDefault(x => x.Piece == GearPiece.Hands);
            u.Boots = currCharGear.FirstOrDefault(x => x.Piece == GearPiece.Feet);

        }
        public void UpdateMemberLocation(S_PARTY_MEMBER_INTERVAL_POS_UPDATE p)
        {
            var u = Members.ToSyncList().FirstOrDefault(x => x.PlayerId == p.PlayerId && x.ServerId == p.ServerId);
            if (u == null) return;
            var ch = p.Channel > 1000 ? "" : " ch." + p.Channel;
            u.Location = SessionManager.DB.TryGetGuardOrDungeonNameFromContinentId(p.ContinentId, out var l) ? l + ch : "Unknown";
        }

    }

    public class DragBehavior
    {
        public readonly TranslateTransform Transform = new TranslateTransform();
        private Point _elementStartPosition2;
        private Point _mouseStartPosition2;
        private static DragBehavior _instance = new DragBehavior();
        public static DragBehavior Instance
        {
            get => _instance;
            set => _instance = value;
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
            var isDrag = (bool)e.NewValue;

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
            var diff = mousePos - _mouseStartPosition2;
            if (!((UIElement)sender).IsMouseCaptured) return;
            Transform.X = _elementStartPosition2.X + diff.X;
            Transform.Y = _elementStartPosition2.Y + diff.Y;
        }
    }
}
