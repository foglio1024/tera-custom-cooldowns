using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TCC.Annotations;
using TCC.Parsing;
using TCC.Settings;

namespace TCC.Interop.Proxy
{
    public class TccStubInterface
    {
        private Client TccStub { get; }

        public TccStubInterface()
        {
            TccStub = new Client();
        }

        public async Task<bool> PingStub()
        {
            var resp = await TccStub.CallAsync("pingStub");
            return resp != null && resp.Result.Value<bool>();
        }
        public async Task<bool> GetIsModAvailable([NotNull] string modName)
        {
            var resp = await TccStub.CallAsync("getIsModAvailable", new JObject
            {
                { "modName", modName }
            });
            return resp != null && resp.Result.Value<bool>();
        }
        public async void RequestPartyInfo(uint id)
        {
            await TccStub.CallAsync("requestPartyInfo", new JObject
            {
                { "listingId", id }
            });
        }
        public async void ApplyToGroup(uint id)
        {
            await TccStub.CallAsync("applyToGroup",
            new JObject
            {
                { "listingId", id }
            });
        }
        public async void FriendUser([NotNull] string userName, [NotNull] string message)
        {
            await TccStub.CallAsync("friendUser", new JObject
            {
                { "userName", userName },
                { "message", message }
            });
        }
        public async void UnfriendUser([NotNull] string userName)
        {
            await TccStub.CallAsync("unfriendUser", new JObject
            {
                { "userName", userName },
            });
        }
        public async void BlockUser([NotNull] string userName)
        {
            await TccStub.CallAsync("blockUser", new JObject
            {
                { "userName", userName },
            });
        }
        public async void UnblockUser([NotNull] string userName)
        {
            await TccStub.CallAsync("unblockUser", new JObject
            {
                { "userName", userName },
            });
        }
        public async void SetInvitePower(uint serverId, uint playerId, bool canInvite)
        {
            await TccStub.CallAsync("setInvitePower", new JObject
            {
                { "serverId", serverId},
                { "playerId", playerId },
                { "canInvite", canInvite},
            });
        }
        public async void DelegateLeader(uint serverId, uint playerId)
        {
            await TccStub.CallAsync("delegateLeader", new JObject
            {
                { "serverId", serverId },
                { "playerId", playerId }
            });
        }
        public async void KickUser(uint serverId, uint playerId)
        {
            await TccStub.CallAsync("kickUser", new JObject
            {
                { "serverId", serverId },
                { "playerId", playerId }
            });
        }
        public async void InspectUser([NotNull] string userName)
        {
            await TccStub.CallAsync("inspectUser", new JObject
            {
                { "userName", userName }
            });
        }
        public async void GroupInviteUser([NotNull] string userName)
        {
            await TccStub.CallAsync("groupInviteUser", new JObject
            {
                { "userName", userName },
                { "isRaid", WindowManager.GroupWindow.VM.Raid ? 1 : 0 }
            });
        }
        public async void GuildInviteUser([NotNull] string userName)
        {
            await TccStub.CallAsync("guildInviteUser", new JObject
            {
                { "userName", userName },
            });
        }
        public async void AcceptBrokerOffer(uint playerId, uint listingId)
        {
            await TccStub.CallAsync("acceptBrokerOffer", new JObject
            {
                { "playerId", playerId },
                { "listingId", listingId }
            });
        }
        public async void DeclineBrokerOffer(uint playerId, uint listingId)
        {
            await TccStub.CallAsync("declineBrokerOffer", new JObject
            {
                { "playerId", playerId },
                { "listingId", listingId }
            });
        }
        public async void DeclineUserGroupApply(uint playerId)
        {
            await TccStub.CallAsync("declineUserGroupApply", new JObject
            {
                { "playerId", playerId },
            });
        }
        public async void PublicizeListing()
        {
            await TccStub.CallAsync("publicizeListing");
        }
        public async void RemoveListing()
        {
            await TccStub.CallAsync("removeListing");
        }
        public async void RequestListings()
        {
            await TccStub.CallAsync("requestListings", new JObject
            {
                { "minLevel", 60 },
                { "maxLevel", 70 }
            });
        }
        public async void AskInteractive(uint serverId, [NotNull] string userName)
        {
            await TccStub.CallAsync("askInteractive", new JObject
            {
                { "serverId", serverId },
                { "userName", userName }
            });
        }
        public async void RequestExTooltip(long itemUid, [NotNull] string ownerName)
        {
            await TccStub.CallAsync("requestExTooltip", new JObject
            {
                { "itemUid", itemUid},
                { "ownerName", ownerName}
            });
        }
        public async void RequestNonDbItemInfo(uint itemId)
        {
            await TccStub.CallAsync("requestNonDbItemInfo", new JObject
            {
                { "itemId", itemId}
            });
        }
        public async void RequestListingsPage(int page)
        {
            await TccStub.CallAsync("requestListingsPage", new JObject
            {
                { "page", page }
            });
        }
        public async void RegisterListing([NotNull] string message, bool isRaid)
        {
            await TccStub.CallAsync("registerListing", new JObject
            {
                { "message", message },
                { "isRaid", isRaid }
            });
        }
        public async void DisbandGroup()
        {
            await TccStub.CallAsync("disbandGroup");
        }
        public async void LeaveGroup()
        {
            await TccStub.CallAsync("leaveGroup");
        }
        public async void RequestListingCandidates()
        {
            await TccStub.CallAsync("requestListingCandidates");
        }
        public async void ForceSystemMessage([NotNull] string message, [NotNull] string opcode)
        {
            var opc = PacketAnalyzer.Factory.SystemMessageNamer.GetCode(opcode);
            var badOpc = message.Split('\v')[0];
            if (badOpc == "@0") message = message.Replace(badOpc, "@" + opc);

            await TccStub.CallAsync("forceSystemMessage", new JObject
            {
                { "message", message }
            });
        }
        public async void InvokeCommand([NotNull] string command)
        {
            await TccStub.CallAsync("invokeCommand", new JObject
            {
                { "command", command }
            });
        }
        public async void ReturnToLobby()
        {
            await TccStub.CallAsync("returnToLobby");
        }
        public async void ChatLinkAction([NotNull] string data)
        {
            await TccStub.CallAsync("chatLinkAction", new JObject
            {
                { "linkData", $":tcc:{data.Replace("#####", ":tcc:")}:tcc:" }
            });
        }
        public async void Initialize()
        {
            await TccStub.CallAsync("initialize", new JObject
            {
                { "useLfg", SettingsHolder.LfgEnabled }
            });
        }
        public async void ResetInstance()
        {
            await TccStub.CallAsync("resetInstance");
        }

        public async Task<bool> DumpSysMsg([NotNull] string path)
        {
            var resp = await TccStub.CallAsync("dumpSysMsg", new JObject
            {
                {"path", path}
            });
            return resp?.Result != null && resp.Result.Value<bool>();
        }
    }
}