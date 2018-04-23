using System.Linq;
using System.Windows.Threading;

namespace TCC.ViewModels
{
    public class TooltipInfo : TSPropertyChanged
    {
        private string name;
        public string Name
        {
            get => name; set
            {
                if (name == value) return;
                name = value;
                NPC(nameof(Name));
                NPC(nameof(BlockLabelText));
                NPC(nameof(ShowAddFriend));
                NPC(nameof(ShowWhisper));
            }
        }
        private string info;
        public string Info
        {
            get => info; set
            {
                if (info == value) return; info = value;
                NPC(nameof(Info));
            }
        }
        private int level;
        public int Level
        {
            get => level; set
            {
                if (level == value) return; level = value; NPC(nameof(Level));
            }
        }
        private Class charClass;
        public Class Class
        {
            get => charClass;
            set
            {
                if (charClass == value) return;
                charClass = value;
                NPC(nameof(Class));
            }
        }
        private bool showPartyInvite;
        public bool ShowPartyInvite
        {
            get { return showPartyInvite; }
            set
            {
                if (showPartyInvite == value) return;
                showPartyInvite = value;
                NPC(nameof(ShowPartyInvite));
            }
        }
        private bool showGuildInvite;
        public bool ShowGuildInvite
        {
            get { return showGuildInvite; }
            set
            {
                if (showGuildInvite == value) return;
                showGuildInvite = value;
                NPC(nameof(ShowGuildInvite));
            }
        }

        public bool ShowAddFriend
        {
            get { return !IsBlocked; }
        }
        public bool ShowWhisper
        {
            get { return !IsBlocked; }
        }
        public string BlockLabelText
        {
            get
            {
                if (IsBlocked) return "Unblock";
                else return "Block";
            }
        }
        public string FriendLabelText
        {
            get
            {
                if (IsFriend) return "Remove friend";
                else return "Add friend";
            }
        }
        public string PowersLabelText
        {
            get
            {
                if (!GroupWindowViewModel.Instance.HasPowers(Name)) return "Grant invite power";
                else return "Revoke invite power";
            }
        }

        public bool ShowGrantPowers
        {
            get => GroupWindowViewModel.Instance.AmILeader() && GroupWindowViewModel.Instance.Raid && GroupWindowViewModel.Instance.Exists(Name) && Name != SessionManager.CurrentPlayer.Name;
        }
        public bool ShowKick => GroupWindowViewModel.Instance.AmILeader() && GroupWindowViewModel.Instance.Exists(Name) && Name != SessionManager.CurrentPlayer.Name;
        public bool ShowDelegateLeader
        {
            get => GroupWindowViewModel.Instance.AmILeader() && GroupWindowViewModel.Instance.Exists(Name) && Name != SessionManager.CurrentPlayer.Name;
        }
        public bool IsBlocked
        {
            get => ChatWindowManager.Instance.BlockedUsers.Contains(name);
        }
        public bool IsFriend
        {
            get
            {
                var f = ChatWindowManager.Instance.Friends.FirstOrDefault(x => x.Name == name);
                return f == null ? false : true;
            }
        }

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
        }

        public void SetInfo(uint model)
        {
            var c = (model % 100) - 1;
            Class = (Class)c;
        }
    }
}
