using System.Linq;
using System.Windows.Threading;
using FoglioUtils;
using TCC.Interop.Proxy;
using TeraDataLite;

namespace TCC.ViewModels
{
    public class TooltipInfo : TSPropertyChanged
    {
        private string _name;
        private string _info;
        private int _level;
        private Class _class;
        private bool _showPartyInvite;
        private bool _showGuildInvite;

        public string Name
        {
            get => _name; set
            {
                if (_name == value) return;
                _name = value;
                N(nameof(Name));
                N(nameof(BlockLabelText));
                N(nameof(ShowAddFriend));
                N(nameof(ShowWhisper));
            }
        }
        public string Info
        {
            get => _info; set
            {
                if (_info == value) return; _info = value;
                N(nameof(Info));
            }
        }
        public int Level
        {
            get => _level; set
            {
                if (_level == value) return; _level = value; N(nameof(Level));
            }
        }
        public Class Class
        {
            get => _class;
            set
            {
                if (_class == value) return;
                _class = value;
                N(nameof(Class));
            }
        }
        public bool ShowPartyInvite
        {
            get => _showPartyInvite;
            set
            {
                if (_showPartyInvite == value) return;
                _showPartyInvite = value;
                N();
            }
        }
        public bool ShowGuildInvite
        {
            get => _showGuildInvite;
            set
            {
                if (_showGuildInvite == value) return;
                _showGuildInvite = value;
                N();
            }
        }
        public bool ShowAddFriend => !IsBlocked && Name != Game.Me.Name;
        public bool ShowWhisper => !IsBlocked;
        public string BlockLabelText => IsBlocked ? "Unblock" : "Block";
        public string FriendLabelText => IsFriend ? "Remove friend" : "Add friend";
        public string PowersLabelText => !Game.Group.HasPowers(Name)?
                                         "Grant invite power" : "Revoke invite power";

        public bool ShowGrantPowers => Game.Group.IsRaid 
                                    && Game.Group.AmILeader
                                    && Game.Group.Has(Name)
                                    && Game.Me.Name != Name;
        public bool ShowKick => Game.Group.Has(Name) 
                             && Game.Me.Name != Name;
        public bool ShowDelegateLeader => Game.Group.AmILeader
                                       && Game.Group.Has(Name)
                                       && Game.Me.Name != Name;
        public bool IsBlocked => _name != "" 
                              && Game.BlockList?.Contains(_name) == true;
        public bool IsFriend => !Game.FriendList.FirstOrDefault(x => x.Name == _name).Equals(default(FriendData));
        public bool ShowFpsUtils => ProxyInterface.Instance.IsStubAvailable 
                                 && ProxyInterface.Instance.IsFpsUtilsAvailable;

        public bool ShowMakeGuildMaster => Game.Guild.AmIMaster &&
                                           Game.Guild.Has(Name);

        public bool ShowGuildKick => Game.Guild.AmIMaster &&
                                     Game.Guild.Has(Name);

        public TooltipInfo(string n, string i, int l)
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            Name = n;
            Info = i;
            Level = l;
        }
        public void Refresh()
        {
            N(nameof(ShowPartyInvite));
            N(nameof(ShowGuildInvite));
            N(nameof(ShowAddFriend));
            N(nameof(ShowKick));
            N(nameof(ShowWhisper));
            N(nameof(BlockLabelText));
            N(nameof(FriendLabelText));
            N(nameof(IsBlocked));
            N(nameof(IsFriend));
            N(nameof(PowersLabelText));
            N(nameof(ShowDelegateLeader));
            N(nameof(ShowGrantPowers));
            N(nameof(ShowFpsUtils));
            N(nameof(ShowMakeGuildMaster));
            N(nameof(ShowGuildKick));
        }
        public void SetInfo(uint model)
        {
            var c = model % 100 - 1;
            Class = (Class)c;
        }
    }
}
