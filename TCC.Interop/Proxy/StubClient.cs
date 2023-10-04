using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeraPacketParser.Sniffing;

namespace TCC.Interop.Proxy;

/// <summary>
/// Sends commands to tcc-stub using a <see cref="ToolboxHttpClient"/>.
/// </summary>
public class StubClient
{
    ToolboxHttpClient TccStub { get; }

    public StubClient()
    {
        TccStub = new ToolboxHttpClient("http://127.0.0.52:9550");
    }

    public async Task<bool> PingStub()
    {
        var resp = await TccStub.CallAsync("pingStub");
        return resp?.Result != null && resp.Result.Value<bool>();
    }

    public async Task<bool> GetIsModAvailable(string modName)
    {
        var resp = await TccStub.CallAsync("getIsModAvailable", new JObject
        {
            { "modName", modName }
        });
        return resp?.Result != null && resp.Result.Value<bool>();
    }

    public async void RequestPartyInfo(uint playerId, uint serverId)
    {
        await TccStub.CallAsync("requestPartyInfo", new JObject
        {
            { "playerId", playerId },
            { "serverId", serverId}
        });
    }

    public async Task<bool> ApplyToGroup(uint playerId, uint serverId)
    {
        var resp = await TccStub.CallAsync("applyToGroup",
            new JObject
            {
                { "playerId", playerId },
                { "serverId", serverId }
            });
        return resp?.Result != null && resp.Result.Value<bool>();
    }

    public async void FriendUser(string userName, string message)
    {
        await TccStub.CallAsync("friendUser", new JObject
        {
            { "userName", userName },
            { "message", message }
        });
    }

    public async void UnfriendUser(string userName)
    {
        await TccStub.CallAsync("unfriendUser", new JObject
        {
            { "userName", userName },
        });
    }

    public async void UnfriendUser(uint playerId)
    {
        await TccStub.CallAsync("unfriendUser", new JObject
        {
            { "playerId", playerId},
        });
    }

    public async void BlockUser(string userName, uint serverId)
    {
        await TccStub.CallAsync("blockUser", new JObject
        {
            { "userName", userName },
            { "serverId", serverId }
        });
    }

    public async void UnblockUser(string userName)
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

    public async void InspectUser(string userName, uint serverId)
    {
        await TccStub.CallAsync("inspectUser", new JObject
        {
            { "userName", userName },
            { "serverId", serverId }
        });
    }

    public async void InspectUser(ulong userGameId)
    {
        await TccStub.CallAsync("inspectUserWithGameId", new JObject
        {
            { "gameId", userGameId}
        });
    }

    public async void GroupInviteUser(string userName, bool raid)
    {
        await TccStub.CallAsync("groupInviteUser", new JObject
        {
            { "userName", userName },
            { "isRaid", raid ? 1 : 0 }
        });
    }

    public async void GuildInviteUser(string userName)
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

    public async void DeclineUserGroupApply(uint playerId, uint serverId)
    {
        await TccStub.CallAsync("declineUserGroupApply", new JObject
        {
            { "playerId", playerId },
            { "serverId", serverId },
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

    public async void RequestListings(int minLevel, int maxLevel)
    {
        if (minLevel < 1) minLevel = 60;
        if (maxLevel < 1) maxLevel = 70;

        if (minLevel > 70) minLevel = 60;
        if (maxLevel > 70) maxLevel = 70;

        if (minLevel > maxLevel) minLevel = maxLevel;
        if (maxLevel < minLevel) maxLevel = minLevel;

        await TccStub.CallAsync("requestListings", new JObject
        {
            { "minLevel", minLevel },
            { "maxLevel", maxLevel }
        });
    }

    public async void AskInteractive(uint serverId, string userName)
    {
        await TccStub.CallAsync("askInteractive", new JObject
        {
            { "serverId", serverId },
            { "userName", userName }
        });
    }

    public async void RequestExTooltip(long itemUid, string ownerName)
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

    public async void RegisterListing(string message, bool isRaid)
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

    public async void ForceSystemMessage(string message, int opc)
    {
        var badOpc = message.Split('\v')[0];
        if (badOpc == "@0") message = message.Replace(badOpc, "@" + opc);

        await TccStub.CallAsync("forceSystemMessage", new JObject
        {
            { "message", message }
        });
    }

    public async void InvokeCommand(string command)
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

    public async void ChatLinkAction(string data)
    {
        await TccStub.CallAsync("chatLinkAction", new JObject
        {
            { "linkData", $":tcc-chatLinkAction:{data}:tcc-chatLinkAction:" }
        });
    }

    public async void ResetInstance()
    {
        await TccStub.CallAsync("resetInstance");
    }

    public async Task<string> QuerySingleValue(string path, IEnumerable<string> arguments, string attribute)
    {
        var resp = await TccStub.CallAsync("querySingleValue", new JObject
        {
            { "path", path },
            { "arguments", new JArray(arguments) },
            { "attribute", attribute }
        });

        return resp?.Result?.Value<string>() ?? string.Empty;
    }

    // bool only, send type if needed for other settings
    public async void UpdateSetting(string settingName, object value)
    {
        await TccStub.CallAsync("updateSetting", new JObject
        {
            { "name", settingName },
            { "value", value.ToString() },
        });
    }

    public async void Initialize(bool useLfg, bool enablePlayerMenu, bool enableProxy, bool showIngameChat, bool tccChatEnabled)
    {
        await TccStub.CallAsync("initialize", new JObject
        {
            { "useLfg", useLfg},
            { "EnablePlayerMenu", enablePlayerMenu},
            { "EnableProxy", enableProxy},
            { "ShowIngameChat", showIngameChat},
            { "TccChatEnabled", tccChatEnabled}
        });
    }
}