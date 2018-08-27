using System.Linq;
using System.Windows.Threading;
using TCC.Data;

namespace TCC.ViewModels
{
    public class TooltipInfo : TSPropertyChanged
    {
        private string _name;
        public string Name
        {
            get => _name; set
            {
                if (_name == value) return;
                _name = value;
                NPC(nameof(Name));
                NPC(nameof(BlockLabelText));
                NPC(nameof(ShowAddFriend));
                NPC(nameof(ShowWhisper));
            }
        }
        private string _info;
        public string Info
        {
            get => _info; set
            {
                if (_info == value) return; _info = value;
                NPC(nameof(Info));
            }
        }
        private int _level;
        public int Level
        {
            get => _level; set
            {
                if (_level == value) return; _level = value; NPC(nameof(Level));
            }
        }
        private Class _charClass;
        public Class Class
        {
            get => _charClass;
            set
            {
                if (_charClass == value) return;
                _charClass = value;
                NPC(nameof(Class));
            }
        }
        private bool _showPartyInvite;
        public bool ShowPartyInvite
        {
            get => _showPartyInvite;
            set
            {
                if (_showPartyInvite == value) return;
                _showPartyInvite = value;
                NPC(nameof(ShowPartyInvite));
            }
        }
        private bool _showGuildInvite;
        public bool ShowGuildInvite
        {
            get => _showGuildInvite;
            set
            {
                if (_showGuildInvite == value) return;
                _showGuildInvite = value;
                NPC(nameof(ShowGuildInvite));
            }
        }

        public bool ShowAddFriend => !IsBlocked;
        public bool ShowWhisper => !IsBlocked;
        public string BlockLabelText => IsBlocked ? "Unblock" : "Block";
        public string FriendLabelText => IsFriend ? "Remove friend" : "Add friend";
        public string PowersLabelText => !GroupWindowViewModel.Instance.HasPowers(Name) ? "Grant invite power" : "Revoke invite power";

        public bool ShowGrantPowers => GroupWindowViewModel.Instance.AmILeader && GroupWindowViewModel.Instance.Raid && GroupWindowViewModel.Instance.Exists(Name) && Name != SessionManager.CurrentPlayer.Name;
        public bool ShowKick => GroupWindowViewModel.Instance.Exists(Name) && Name != SessionManager.CurrentPlayer.Name;
        public bool ShowDelegateLeader => GroupWindowViewModel.Instance.AmILeader && GroupWindowViewModel.Instance.Exists(Name) && Name != SessionManager.CurrentPlayer.Name;
        public bool IsBlocked => ChatWindowManager.Instance.BlockedUsers.Contains(_name);
        public bool IsFriend
        {
            get
            {
                var f = ChatWindowManager.Instance.Friends.FirstOrDefault(x => x.Name == _name);
                return f != null;
            }
        }
        public bool ShowFpsUtils => Proxy.IsConnected && Proxy.IsFpsUtilsAvailable;
        public TooltipInfo(string n, string i, int l)
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            Name = n;
            Info = i;
            Level = l;
        }
        public void Refresh()
        {
            NPC(nameof(ShowPartyInvite));
            NPC(nameof(ShowGuildInvite));
            NPC(nameof(ShowAddFriend));
            NPC(nameof(ShowKick));
            NPC(nameof(ShowWhisper));
            NPC(nameof(BlockLabelText));
            NPC(nameof(FriendLabelText));
            NPC(nameof(IsBlocked));
            NPC(nameof(IsFriend));
            NPC(nameof(PowersLabelText));
            NPC(nameof(ShowDelegateLeader));
            NPC(nameof(ShowGrantPowers));
            NPC(nameof(ShowFpsUtils));
        }
        public void SetInfo(uint model)
        {
            var c = (model % 100) - 1;
            Class = (Class)c;
        }
    }
}
