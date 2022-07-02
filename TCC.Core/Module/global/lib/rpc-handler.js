const { table } = require("console");
const fs = require("fs");
const path = require("path");
const { Globals } = require("./globals");
const { Helpers } = require("./helpers");

class RpcHandler {
    constructor(mod) {
        this.mod = mod;
    }

    debug(msg) {
        //if (!this.mod.settings.debug) return;
        //this.mod.command.message(`<font color="#fff1b5">${msg}</font>`);
    }

    setNetworkMod(networkMod) {
        this.currNetworkMod = networkMod.mod;
    }

    async handle(request) {
        return (await this[request.method](request.params));
    }

    dumpSysMsg(params) {
        this.debug(`Dumping sysmsg`);

        let ret = true;
        let sysmsg = "";
        let names = Object.keys(this.mod.clientInterface.info.sysmsg);
        names.forEach(name => {
            sysmsg += `${name} ${this.mod.clientInterface.info.sysmsg[name]}${(names.indexOf(name) != names.length - 1 ? "\n" : "")}`;
        });
        fs.writeFile(params.path, sysmsg, function (err) {
            if (err) {
                this.mod.log(err);
                ret = false;
            }
        });
        return ret;
    }

    pingStub(params) {
        this.debug(`Ping received`);
        return true;
    }
    getIsModAvailable(params) {
        this.debug(`Checking mod available: ${params.modName}`);
        return this.mod.manager.isLoaded(params.modName);
    }
    resetInstance(params) {
        this.currNetworkMod.send("C_RESET_ALL_DUNGEON", 1, {});
        this.debug("Sent C_RESET_ALL_DUNGEON");
    }
    requestPartyInfo(params) {
        if (this.currNetworkMod.majorPatchVersion >= 108) {

            this.currNetworkMod.send("C_REQUEST_PARTY_INFO", 3, {
                playerId: params.playerId,
                serverId: params.serverId
            });
        }
        else{
            
            this.currNetworkMod.send("C_REQUEST_PARTY_INFO", 2, {
                playerId: params.playerId
            });
        }
        this.debug("Sent C_REQUEST_PARTY_INFO");
    }
    applyToGroup(params) {
        if (this.currNetworkMod.majorPatchVersion >= 108) {

            this.currNetworkMod.send("C_APPLY_PARTY", 2, {
                playerId: params.playerId,
                serverId: params.serverId,
            });
        }
        else {
            this.currNetworkMod.send("C_APPLY_PARTY", 1, {
                playerId: params.playerId
            });

        }


        this.debug(`Sent C_APPLY_PARTY { playerId : ${params.listingId}}`);
        return true;
    }
    friendUser(params) {
        this.currNetworkMod.send("C_ADD_FRIEND", 1, {
            name: params.userName,
            message: params.message
        });
        this.debug(`Sent C_ADD_FRIEND`);
    }
    unfriendUser(params) {
        if (this.currNetworkMod.majorPatchVersion >= 103) {
            this.currNetworkMod.send("C_DELETE_FRIEND", 2, {
                playerId: params.playerId
            });
        }
        else {
            this.currNetworkMod.send("C_DELETE_FRIEND", 1, {
                name: params.userName
            });
        }
        this.debug(`Sent C_DELETE_FRIEND`);
    }
    blockUser(params) {
        this.currNetworkMod.send("C_BLOCK_USER", 2, {
            serverId: params.serverId,
            name: params.userName
        });
        this.debug(`Sent C_BLOCK_USER`);
    }
    unblockUser(params) {
        this.currNetworkMod.send("C_REMOVE_BLOCKED_USER", 1, {
            name: params.userName
        });
        this.debug(`Sent C_REMOVE_BLOCKED_USER`);
    }
    setInvitePower(params) {
        this.currNetworkMod.send("C_CHANGE_PARTY_MEMBER_AUTHORITY", 1, {
            serverId: params.serverId,
            playerId: params.playerId,
            canInvite: params.canInvite
        });
        this.debug(`Sent C_CHANGE_PARTY_MEMBER_AUTHORITY`);
    }
    delegateLeader(params) {
        this.currNetworkMod.send("C_CHANGE_PARTY_MANAGER", 2, {
            serverId: params.serverId,
            playerId: params.playerId
        });
        this.debug(`Sent C_CHANGE_PARTY_MANAGER`);
    }
    kickUser(params) {
        this.currNetworkMod.send("C_BAN_PARTY_MEMBER", 1, {
            serverId: params.serverId,
            playerId: params.playerId
        });
        this.debug(`Sent C_BAN_PARTY_MEMBER`);
    }
    inspectUser(params) {
        if (this.currNetworkMod.majorPatchVersion >= 108) {

            this.currNetworkMod.send("C_REQUEST_USER_PAPERDOLL_INFO", 4, {
                name: params.userName,
                serverId: params.serverId,
                zoom: false
            });
        }
        else {

            this.currNetworkMod.send("C_REQUEST_USER_PAPERDOLL_INFO", 3, {
                name: params.userName,
                zoom: false
            });
        }
        this.debug(`Sent C_REQUEST_USER_PAPERDOLL_INFO`);
    }
    inspectUserWithGameId(params) {
        this.currNetworkMod.send("C_REQUEST_USER_PAPERDOLL_INFO_WITH_GAMEID", 3, {
            gameId: params.gameId
        });
        this.debug(`Sent C_REQUEST_USER_PAPERDOLL_INFO_WITH_GAMEID`);
    }
    groupInviteUser(params) {
        var dataArray = new Buffer.alloc(1, Number(params.isRaid));
        this.currNetworkMod.send("C_REQUEST_CONTRACT", 1, {
            type: 4,
            name: params.userName,
            data: dataArray
        });
        this.debug(`Sent C_REQUEST_CONTRACT`);
    }
    guildInviteUser(params) {
        this.currNetworkMod.send("C_INVITE_USER_TO_GUILD", 1, {
            name: params.userName
        });
        this.debug(`Sent C_INVITE_USER_TO_GUILD`);
    }
    acceptBrokerOffer(params) {
        const data = Buffer.alloc(30);
        data.writeUInt32LE(params.playerId, 0);
        data.writeUInt32LE(params.listingId, 4);
        this.currNetworkMod.send("C_REQUEST_CONTRACT", 1, {
            type: 35,
            data
        });
        this.debug(`Sent C_REQUEST_CONTRACT`);
    }
    declineBrokerOffer(params) {
        this.currNetworkMod.send("C_TRADE_BROKER_REJECT_SUGGEST", 1, {
            playerId: params.playerId,
            listing: params.listingId
        });
        this.debug(`Sent C_TRADE_BROKER_REJECT_SUGGEST`);
    }
    declineUserGroupApply(params) {
        this.currNetworkMod.send("C_PARTY_APPLICATION_DENIED", 2, {
            playerId: params.playerId,
            serverId: params.serverId

        });
        this.debug(`Sent C_PARTY_APPLICATION_DENIED`);
    }
    publicizeListing(params) {
        this.currNetworkMod.send("C_REQUEST_PARTY_MATCH_LINK", 1, {});
        this.debug(`Sent C_REQUEST_PARTY_MATCH_LINK`);
    }
    removeListing(params) {
        this.currNetworkMod.send("C_UNREGISTER_PARTY_INFO", 1, {
            unk1: 20,
            minLevel: 1,
            maxLevel: 65,
            unk3: 3
        });
        this.debug(`Sent C_UNREGISTER_PARTY_INFO`);
    }
    requestListings(params) {
        this.currNetworkMod.send("C_PARTY_MATCH_WINDOW_CLOSED", 1, {});
        this.debug(`Sent C_PARTY_MATCH_WINDOW_CLOSED`);

        let min = params.minLevel;
        let max = params.maxLevel;
        if (min > max)
            min = max;
        if (min < 1)
            min = 1;
        this.currNetworkMod.send("C_REQUEST_PARTY_MATCH_INFO", 1, {
            minlvl: min,
            maxlvl: max,
            unk2: 3
        });
        this.debug(`Sent C_REQUEST_PARTY_MATCH_INFO`);

    }
    requestListingsPage(params) {
        this.currNetworkMod.send("C_REQUEST_PARTY_MATCH_INFO_PAGE", 1, {
            page: params.page,
            unk1: 3
        });
        this.debug(`Sent C_REQUEST_PARTY_MATCH_INFO_PAGE`);
    }
    askInteractive(params) {
        this.currNetworkMod.send("C_ASK_INTERACTIVE", 2, {
            unk: 1,
            serverId: params.serverId,
            name: params.userName
        });
        this.debug(`Sent C_ASK_INTERACTIVE { serverId: ${params.serverId}, name: ${params.userName} }`);
    }
    requestExTooltip(params) {
        this.currNetworkMod.send("C_SHOW_ITEM_TOOLTIP_EX", this.currNetworkMod.majorPatchVersion >= 106 ? 6 : 5, {
            type: 17,
            dbid: params.itemUid,
            playerId: -1,
            owner: params.ownerName
        });
        this.debug(`Sent C_SHOW_ITEM_TOOLTIP_EX`);

    }
    requestNonDbItemInfo(params) {
        this.currNetworkMod.send("C_REQUEST_NONDB_ITEM_INFO", 2, {
            item: params.itemId
        });
        this.debug(`Sent C_REQUEST_NONDB_ITEM_INFO`);
    }
    registerListing(params) {
        this.currNetworkMod.send("C_REGISTER_PARTY_INFO", 1, {
            isRaid: params.isRaid,
            message: params.message
        });
        this.debug(`Sent C_REGISTER_PARTY_INFO`);
    }
    disbandGroup(params) {
        this.currNetworkMod.send("C_DISMISS_PARTY", 1, {});
        this.debug(`Sent C_DISMISS_PARTY`);
    }
    leaveGroup(params) {
        this.currNetworkMod.send("C_LEAVE_PARTY", 1, {});
        this.debug(`Sent C_LEAVE_PARTY`);
    }
    requestListingCandidates(params) {
        this.currNetworkMod.send("C_REQUEST_CANDIDATE_LIST", 1, {});
        this.debug(`Sent C_REQUEST_CANDIDATE_LIST`);
    }
    forceSystemMessage(params) {
        this.currNetworkMod.send("S_SYSTEM_MESSAGE", 1, {
            message: params.message
        });
        this.debug(`Sent S_SYSTEM_MESSAGE`);
    }
    invokeCommand(params) {
        this.mod.command.exec(params.command);
        this.debug(`Invoking command: ${params.command}`);
    }
    returnToLobby(params) {
        this.currNetworkMod.send("C_RETURN_TO_LOBBY", 1, {});
        this.debug(`Sent C_RETURN_TO_LOBBY`);
    }
    chatLinkAction(params) {
        this.currNetworkMod.send("S_CHAT", 3, {
            channel: 18,
            name: "tccChatLink",
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
        if (name == "useLfg") {
            msg = `TCC LFG window ${(value ? "enabled" : "disabled")}. Ingame LFG ${(value ? "will" : "won't")} be blocked.`;
        }
        else if (name == "EnablePlayerMenu") {
            msg = `TCC player menu ${(value ? "enabled" : "disabled")}. Ingame player menu ${(value ? "will" : "won't")} be blocked.`;
        }
        else if (name == "ShowIngameChat") {
            this.currNetworkMod.notifyShowIngameChatChanged();
        }
        else if (name == "TccChatEnabled") {
            // do nothing
        }
        this.mod.log(msg);
    }
    async querySingleValue(params) {
        let result = (await this.mod.queryData(params.path, params.arguments, false, false, [params.attribute]));
        let res = "";
        if (result !== undefined) {
            res = result.attributes[params.attribute];
        }
        return res;
    }

    // DB ------------------------------------
    getDataCenterHash(params) {

    }

    async dumpDatabase(params) {
        await this.dumpStrSheet(params.basePath, "acc_benefits", "StrSheet_AccountBenefit");
        await this.dumpStrSheet(params.basePath, "achievements", "StrSheet_Achievement");
        await this.dumpStrSheet(params.basePath, "social", "StrSheet_Region");
    }

    // StrSheet_AccountBenefit, StrSheet_Achievement, StrSheet_Region
    async dumpStrSheet(basePath, subpath, tableName) {
        let ret = true;
        let res = (await this.mod.queryData(`/${tableName}/String/`, [], true, false, ['id', 'string']));
        let tsv = "";
        res.forEach(result => {
            let id = result.attributes.id;
            let str = result.attributes.string;
            if (str === "" || id == 0) return;
            let line = `${id}\t${str.replace("\n", "&#xA")}\n`
            tsv += line;
        });

        fs.writeFile(path.join(basePath, subpath + mod.clientInterface.info.language.toUpperCase() + ".tsv"), tsv, function (err, data) {
            if (err) {
                ret = false;
            }
        });
        return ret;
    }

    async dumpAchievementGrades(params) {
        let ret = true;
        let res = (await this.mod.queryData('/StrSheet_AchievementGradeInfo/String/', [], true, false, ['id', 'string']));
        let tsv = "";
        res.forEach(result => {
            let id = result.attributes.id;
            let str = result.attributes.string;
            if (str === "" || id == 0 || (id < 100 || id > 106)) return;
            let line = `${id}\t${str}\n`
            tsv += line;
        });

        fs.writeFile(params.path, tsv, function (err, data) {
            if (err) {
                ret = false;
            }
        });
        return ret;

    }

    async dumpDungeons(params) {
        let ret = true;
        let names = (await this.mod.queryData('/StrSheet_Dungeon/String/', [], true, false, ['id', 'string']));
        let constraints = (await this.mod.queryData('/DungeonConstraint/ConstraintList/Constraint/', [], true, false, ['continentId', 'requiredActPoint']));
        let dungeons = [];
        names.forEach(n => {
            let constr = constraints.find(c => c.attributes.continentId == n.attributes.id);
            if (constr === undefined) return;
            dungeons.push({
                id: n.attributes.id,
                name: n.attributes.string,
                requiredActPoint: constr.attributes.requiredActPoint
            });
        });

        dungeons.sort(function (a, b) {
            return a.id - b.id;
        });
        let tsv = "";
        dungeons.forEach(d => {
            tsv += `${d.id}\t${d.name}\t${d.requiredActPoint}\n`;
        });

        fs.writeFile(params.path, tsv, function (err, data) {
            if (err) {
                ret = false;
            }
        });
        return ret;
    }

    async dumpGuildQuests(params) {
        let tsv = "";
        let strSheet_guildQuest = (await this.mod.queryData('/StrSheet_GuildQuest/String/', [], true, false, ['id', 'string']));
        strSheet_guildQuest.forEach(x => {
            if (x.attributes.id % 2 !== 1) return;
            tsv += `${x.attributes.id}\t${x.attributes.string.replace("\n", "&#xA")}\n`;
        });

        fs.writeFile(params.path, tsv, function (err, data) {
            if (err) {
                ret = false;
            }
        });
    }

    async dumpItems(params) {
        let ret = true;
        const strings = (await this.mod.queryData(`/StrSheet_Item/String/`, [], true, false, ['id', 'string']));
        const items = (await this.mod.queryData(`/ItemData/Item/`, [], true, false, ['id', 'coolTime', 'icon', 'rareGrade']));

        let tsv = "";
        items.forEach(result => {
            const id = result.attributes.id;
            const strEl = strings.find(x => x.attributes.id === id);
            if (strEl === undefined) return;
            const str = strEl.attributes.string;
            if (str === "" || id == 0) return;
            const line = `${id}\t${result.attributes.rareGrade}\t${str}\t${result.attributes.coolTime}\t${result.attributes.icon.toLowerCase()}\n`;
            tsv += line;
        });

        fs.writeFile(params.path, tsv, function (err, data) {
            if (err) {
                ret = false;
            }
        });
    }

    async dumpSocial(params) {
        let ret = true;
        let res = (await this.mod.queryData(`/StrSheet_Social/String/`, [], true, false, ['id', 'string']));
        let tsv = "";
        res.forEach(result => {
            let id = result.attributes.id;
            let str = result.attributes.string;
            if (str === "" || id == 0 || !str.startsWith("{")) return;
            let line = `${id}\t${str.replace("\n", "&#xA")}\n`
            tsv += line;
        });

        fs.writeFile(params.path, tsv, function (err, data) {
            if (err) {
                ret = false;
            }
        });
        return ret;
    }



    async dumpSkills(params) {
        //RawExtract
        const skills = [];
        const strSheet_UserSkill = (await this.mod.queryData(`/StrSheet_UserSkill/String/`, [], true, false, ['id', 'name', 'race', 'gender', 'class']));

        strSheet_UserSkill.forEach(x => {
            if (x.attributes.id == 0
                || x.attributes.race == ""
                || x.attributes.gender == ""
                || x.attributes.class == ""
                || x.attributes.name == "") return;

            skills.push({
                id: x.attributes.id,
                name: x.attributes.name,
                race: x.attributes.race,
                gender: x.attributes.gender,
                pClass: Helpers.convClass(x.attributes.class)
            });
        });

        const skillIconData = (await this.mod.queryData(`/SkillIconData/Icon/`, [], true, false, ['skillId', 'race', 'gender', 'class', 'iconName']));

        skills.forEach(s => {
            const iconData = skillIconData.find(x => x.attributes.skillId == s.id
                && x.attributes.race == s.race
                && x.attributes.gender == s.gender
                && Helpers.convClass(x.attributes.class) == s.pClass);
            if (iconData === undefined) return;
            s.iconName = iconData.attributes.iconName.toLowerCase();
        });

        let tsv = "";

        skills.forEach(skill => {
            tsv += `${skill.id}\t${skill.race}\t${skill.gender}\t${skill.pClass}\t${skill.name}\t${skill.chained}\t${skill.detail}\t${skill.iconName}\n`;
        });

        fs.writeFile("skills.tsv", tsv, function (err, data) { });
    }
    async dumpHotDot(params) { }
    async dumpMonsters(params) { }

    async dumpWorldMap(params) { }
    //----------------------------------------

    initialize(params) {
        Globals.useLfg = params.useLfg;
        Globals.EnablePlayerMenu = params.EnablePlayerMenu;
        Globals.EnableProxy = params.EnableProxy;
        Globals.ShowIngameChat = params.ShowIngameChat;
        Globals.TccChatEnabled = params.TccChatEnabled;
        if (Globals.useLfg) this.mod.log("TCC LFG window enabled. Ingame LFG listings will be blocked.");
        if (Globals.EnablePlayerMenu) this.mod.log("TCC player menu enabled. Ingame player menu will be blocked.");
        if (!Globals.TccChatEnabled) this.mod.log("TCC chat disabled. Advanced Chat2.gpk functionalities won't be available.");
        return true;
    }
}
exports.RpcHandler = RpcHandler;
