const fs = require('fs');
const { Globals } = require("./globals")

class RpcHandler
{
    constructor(mod)
    {
        this.mod = mod;
    }

    debug(msg)
    {
        if (!this.mod.settings.debug) return;
        this.mod.command.message(`<font color="#fff1b5">${msg}</font>`);
    }

    handle(request)
    {
        return this[request.method](request.params);
    }

    dumpSysMsg(params)
    {
        this.debug(`Dumping sysmsg`);

        let ret = true;
        let sysmsg = "";
        let names = Object.keys(this.mod.clientInterface.info.sysmsg);
        names.forEach(name =>
        {
            sysmsg += `${name} ${this.mod.clientInterface.info.sysmsg[name]}${(names.indexOf(name) != names.length - 1 ? '\n' : '')}`;
        });
        fs.writeFile(params.path, sysmsg, function (err)
        {
            if (err)
            {
                this.mod.log(err);
                ret = false;
            }
        });
        return ret;
    }

    pingStub(params)
    {
        this.debug(`Ping received`);
        return true;
    }
    getIsModAvailable(params)
    {
        this.debug(`Checking mod available: ${params.modName}`);
        return this.mod.manager.isLoaded(params.modName);
    }
    resetInstance(params)
    {
        this.mod.send('C_RESET_ALL_DUNGEON', 1, {});
        this.debug('Sent C_RESET_ALL_DUNGEON');
    }
    requestPartyInfo(params)
    {
        this.mod.send('C_REQUEST_PARTY_INFO', 2, {
            playerId: params.listingId
        });
        this.debug('Sent C_REQUEST_PARTY_INFO');
    }
    applyToGroup(params)
    {
        this.mod.send('C_APPLY_PARTY', 1, {
            playerId: params.listingId
        });
        this.debug(`Sent C_APPLY_PARTY { playerId : ${params.listingId}}`);
        return true;
    }
    friendUser(params)
    {
        this.mod.send('C_ADD_FRIEND', 1, {
            name: params.userName,
            message: params.message
        });
        this.debug(`Sent C_ADD_FRIEND`);
    }
    unfriendUser(params)
    {
        this.mod.send('C_DELETE_FRIEND', 1, {
            name: params.userName
        });
        this.debug(`Sent C_DELETE_FRIEND`);
    }
    blockUser(params)
    {
        this.mod.send('C_BLOCK_USER', 1, {
            name: params.userName
        });
        this.debug(`Sent C_BLOCK_USER`);
    }
    unblockUser(params)
    {
        this.mod.send('C_REMOVE_BLOCKED_USER', 1, {
            name: params.userName
        });
        this.debug(`Sent C_REMOVE_BLOCKED_USER`);
    }
    setInvitePower(params)
    {
        this.mod.send('C_CHANGE_PARTY_MEMBER_AUTHORITY', 1, {
            serverId: params.serverId,
            playerId: params.playerId,
            canInvite: params.canInvite
        });
        this.debug(`Sent C_CHANGE_PARTY_MEMBER_AUTHORITY`);
    }
    delegateLeader(params)
    {
        this.mod.send('C_CHANGE_PARTY_MANAGER', 2, {
            serverId: params.serverId,
            playerId: params.playerId
        });
        this.debug(`Sent C_CHANGE_PARTY_MANAGER`);
    }
    kickUser(params)
    {
        this.mod.send('C_BAN_PARTY_MEMBER', 1, {
            serverId: params.serverId,
            playerId: params.playerId
        });
        this.debug(`Sent C_BAN_PARTY_MEMBER`);
    }
    inspectUser(params)
    {
        this.mod.send('C_REQUEST_USER_PAPERDOLL_INFO', 3, {
            name: params.userName
        });
        this.debug(`Sent C_REQUEST_USER_PAPERDOLL_INFO`);
    }
    inspectUserWithGameId(params)
    {
        this.mod.send('C_REQUEST_USER_PAPERDOLL_INFO_WITH_GAMEID', 3, {
            gameId: params.gameId
        });
        this.debug(`Sent C_REQUEST_USER_PAPERDOLL_INFO`);
    }
    groupInviteUser(params)
    {
        var dataArray = new Buffer.alloc(1, Number(params.isRaid));
        this.mod.send('C_REQUEST_CONTRACT', 1, {
            type: 4,
            name: params.userName,
            data: dataArray
        });
        this.debug(`Sent C_REQUEST_CONTRACT`);
    }
    guildInviteUser(params)
    {
        this.mod.send('C_INVITE_USER_TO_GUILD', 1, {
            name: params.userName
        });
        this.debug(`Sent C_INVITE_USER_TO_GUILD`);
    }
    acceptBrokerOffer(params)
    {
        const data = Buffer.alloc(30);
        data.writeUInt32LE(params.playerId, 0);
        data.writeUInt32LE(params.listingId, 4);
        this.mod.send('C_REQUEST_CONTRACT', 1, {
            type: 35,
            data
        });
        this.debug(`Sent C_REQUEST_CONTRACT`);
    }
    declineBrokerOffer(params)
    {
        this.mod.send('C_TRADE_BROKER_REJECT_SUGGEST', 1, {
            playerId: params.playerId,
            listing: params.listingId
        });
        this.debug(`Sent C_TRADE_BROKER_REJECT_SUGGEST`);
    }
    declineUserGroupApply(params)
    {
        this.mod.send('C_PARTY_APPLICATION_DENIED', 1, {
            pid: params.playerId
        });
        this.debug(`Sent C_PARTY_APPLICATION_DENIED`);
    }
    publicizeListing(params)
    {
        this.mod.send('C_REQUEST_PARTY_MATCH_LINK', 1, {});
        this.debug(`Sent C_REQUEST_PARTY_MATCH_LINK`);
    }
    removeListing(params)
    {
        this.mod.send('C_UNREGISTER_PARTY_INFO', 1, {
            unk1: 20,
            minLevel: 1,
            maxLevel: 65,
            unk3: 3
        });
        this.debug(`Sent C_UNREGISTER_PARTY_INFO`);
    }
    requestListings(params)
    {
        this.mod.send("C_PARTY_MATCH_WINDOW_CLOSED", 1, {});
        this.debug(`Sent C_PARTY_MATCH_WINDOW_CLOSED`);

        let min = params.minLevel;
        let max = params.maxLevel;
        if (min > max)
            min = max;
        if (min < 1)
            min = 1;
        this.mod.send("C_REQUEST_PARTY_MATCH_INFO", 1, {
            minlvl: min,
            maxlvl: max,
            unk2: 3
        });
        this.debug(`Sent C_REQUEST_PARTY_MATCH_INFO`);

    }
    requestListingsPage(params)
    {
        this.mod.send('C_REQUEST_PARTY_MATCH_INFO_PAGE', 1, {
            page: params.page,
            unk1: 3
        });
        this.debug(`Sent C_REQUEST_PARTY_MATCH_INFO_PAGE`);
    }
    askInteractive(params)
    {
        this.mod.send('C_ASK_INTERACTIVE', 2, {
            unk: 1,
            serverId: params.serverId,
            name: params.userName
        });
        this.debug(`Sent C_ASK_INTERACTIVE { serverId: ${params.serverId}, name: ${params.userName} }`);
    }
    requestExTooltip(params)
    {
        this.mod.send('C_SHOW_ITEM_TOOLTIP_EX', 3, {
            type: 17,
            id: params.itemUid,
            playerId: -1,
            owner: params.ownerName
        });
        this.debug(`Sent C_SHOW_ITEM_TOOLTIP_EX`);

    }
    requestNonDbItemInfo(params)
    {
        this.mod.send('C_REQUEST_NONDB_ITEM_INFO', 2, {
            item: params.itemId
        });
        this.debug(`Sent C_REQUEST_NONDB_ITEM_INFO`);
    }
    registerListing(params)
    {
        this.mod.send('C_REGISTER_PARTY_INFO', 1, {
            isRaid: params.isRaid,
            message: params.message
        });
        this.debug(`Sent C_REGISTER_PARTY_INFO`);
    }
    disbandGroup(params)
    {
        this.mod.send('C_DISMISS_PARTY', 1, {});
        this.debug(`Sent C_DISMISS_PARTY`);
    }
    leaveGroup(params)
    {
        this.mod.send('C_LEAVE_PARTY', 1, {});
        this.debug(`Sent C_LEAVE_PARTY`);
    }
    requestListingCandidates(params)
    {
        this.mod.send('C_REQUEST_CANDIDATE_LIST', 1, {});
        this.debug(`Sent C_REQUEST_CANDIDATE_LIST`);
    }
    forceSystemMessage(params)
    {
        this.mod.send("S_SYSTEM_MESSAGE", 1, {
            message: params.message
        });
        this.debug(`Sent S_SYSTEM_MESSAGE`);
    }
    invokeCommand(params)
    {
        this.mod.command.exec(params.command);
        this.debug(`Invoking command: ${params.command}`);
    }
    returnToLobby(params)
    {
        this.mod.send("C_RETURN_TO_LOBBY", 1, {});
        this.debug(`Sent C_RETURN_TO_LOBBY`);
    }
    chatLinkAction(params)
    {
        this.mod.send('S_CHAT', 3, {
            channel: 18,
            name: 'tccChatLink',
            message: params.linkData
        });
        this.debug(`Calling chatLinkAction: ${params.linkData}`);
    }
    updateSetting(params) // bool only, send type if needed for other setting
    {
        let value = params.value == "True"; // JS PLS
        let name = params.name;
        Globals[name] = value; 
        let msg = `${name} set to ${value}`;
        if (name == "useLfg")
        {
            msg = `TCC LFG window ${(value ? 'enabled' : 'disabled')}. Ingame LFG ${(value ? 'will' : "won't")} be blocked.`;
        }
        else if (name == "EnablePlayerMenu")
        {
            msg = `TCC player menu ${(value ? "enabled" : "disabled")}. Ingame player menu ${(value ? "will" : "won't")} be blocked.`;
        }
        else if (name == "ShowIngameChat")
        {
            this.mod.networkMod.notifyShowIngameChatChanged();
        }
        else if (name == "TccChatEnabled"){
            // do nothing
        }
        this.mod.log(msg);

    }
    initialize(params)
    {
        Globals.useLfg = params.useLfg;
        Globals.EnablePlayerMenu = params.EnablePlayerMenu;
        // this.mod.networkMod.EnableProxy = params.EnableProxy;
        Globals.EnableProxy = params.EnableProxy;
        Globals.ShowIngameChat = params.ShowIngameChat;
        Globals.TccChatEnabled = params.TccChatEnabled;
        if (Globals.useLfg) this.mod.log("TCC LFG window enabled. Ingame LFG listings will be blocked.");
        if (Globals.EnablePlayerMenu) this.mod.log("TCC player menu enabled. Ingame player menu will be blocked.");
        if (Globals.TccChatEnabled) this.mod.log("TCC chat enabled. Advanced Chat2.gpk functionalities won't be available.");
        return true;
    }
}
exports.RpcHandler = RpcHandler;
