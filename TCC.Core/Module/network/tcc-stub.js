const { Globals } = require("../global/lib/globals");

class TccStub {
    constructor(mod) {
        this.mod = mod;

        Globals.setCommand(mod.command);

        this.globalMod().setNetworkMod(this);

        this.installHooks();

        //memes
        this.mod.hook("S_SYSTEM_MESSAGE", 1, ev => {
            if (mod.game.me.inDungeon === true || mod.game.me.inBattleground === true) return;
            if (ev.message.indexOf("Foglio") === -1) return;

            const sm = mod.parseSystemMessage(ev.message);
            if (sm.id === "SMT_FRIEND_SEND_HELLO"
                || sm.id === "SMT_FRIEND_RECEIVE_HELLO") {
                this.memeA();
            }
        });

        this.mod.command.add("tcc", (cmd, a1, a2, a3, a4) => {
            switch (cmd) {
                case "debug":
                    {
                        mod.settings.debug = !mod.settings.debug;
                        mod.command.message(`<font color="#cccccc">Debug mode </font><font color="#${(mod.settings.debug ? "42F5AD" : "F05164")}">${(mod.settings.debug ? "en" : "dis")}abled</font>`);
                        break;
                    }
                case "notify":
                    {
                        this.globalMod().call("enqueueNotification",
                            {
                                'title': a1,
                                'message': a2,
                                'notificationType': a3,
                                'secDuration': a4
                            });
                        mod.command.message(`<font color="#cccccc">Sending notification: </font><font color="#42F5AD">${a1} ${a2} ${a3} ${a4}</font>`);

                        break;
                    }
                default:
                    break;
            }


        });

        this.mod.command.add(":tcc-chatmode", (arg) => {
            Globals.debug("Setting ChatMode to " + arg);
            this.globalMod().call("setChatMode", { 'chatMode': arg == "true" });
        });

        this.mod.command.add(":tcc-uimode", (arg) => {
            Globals.debug("Setting UiMode to " + arg);
            this.globalMod().call("setUiMode", { 'uiMode': arg == "true" });
        });

        this.mod.command.add(":tcc-proxyOn:", (arg) => { });
        this.mod.command.add(":tcc-chatOn:", (arg) => { });
        this.mod.command.add(":tcc-chatOff:", (arg) => { });

    }

    globalMod() {
        return this.mod.globalMod;
    }

    notifyShowIngameChatChanged() {
        if (!Globals.TccChatEnabled) return;
        this.mod.send("S_CHAT", this.mod.majorPatchVersion >= 108 ? 4 : 3,
            {
                channel: 18,
                name: "tccChatLink",
                message: Globals.ShowIngameChat ? ":tcc-chatOn:" : ":tcc-chatOff:"
            });
    }

    installHooks() {
        // block ingame player menu
        this.mod.hook("S_ANSWER_INTERACTIVE", "raw", () => { return !Globals.EnablePlayerMenu; });
        // block ingame lfg list
        this.mod.hook("S_SHOW_PARTY_MATCH_INFO", "raw", () => { return !Globals.useLfg; });
        // block ingame lfg details
        this.mod.hook("S_PARTY_MEMBER_INFO", "raw", () => { return !Globals.useLfg; });
        this.mod.hook("S_SHOW_CANDIDATE_LIST", "raw", () => { return !Globals.useLfg; });
        // block tcc messages from gpk file
        this.mod.hook("S_CHAT", this.mod.majorPatchVersion >= 108 ? 4 : 3, { order: 999, filter: { fake: true } }, (p) => {
            if (p.message.indexOf("(Translated)") !== -1) {

                this.globalMod().call("handleTranslatedMessage", {
                    'author': p.name,
                    'channel': p.channel,
                    'message': p.message,
                    'gm': p.gm
                });
                return false;
            }
            if (p.name !== "tccChatLink") return false;
        });
        this.mod.hook("S_WHISPER", this.mod.majorPatchVersion >= 108 ? 4 : 3, { order: 999, filter: { fake: true } }, (p) => {
            if (p.message.indexOf("(Translated)") !== -1) {

                this.globalMod().call("handleTranslatedMessage", {
                    'author': p.name,
                    'channel': 301, // ReceivedWhisper (custom)
                    'message': p.message,
                    'gm': p.gm
                });
            }
        });

        // hook Command messages to display them in tcc {order: 999, filter:{fake:true}}
        this.mod.hook("S_PRIVATE_CHAT", 1, { order: 999, filter: { fake: true } }, p => {
            var author = "";
            var text = p.message.toString();
            if (p.author == undefined) {
                var authorEnd = p.message.toString().indexOf("]");
                if (authorEnd != -1) {
                    author = text.substring(1, authorEnd);
                    text = text.substring(authorEnd + 2);
                }
            }
            else author = p.author;

            // handle chatMode from Chat2
            const chatModeParameter = ":tcc-chatMode:";
            let chatModeIdx = text.indexOf(chatModeParameter);
            if (chatModeIdx != -1) {
                // Unknown command ":tcc-chatMode:false" 
                let chatMode = text.indexOf(":true") != -1; // WTB <GPK Interface> /w me
                this.globalMod().call("setChatMode", { 'chatMode': chatMode });
                return true;
            }

            // handle uiMode from Chat2
            const uiModeParameter = ":tcc-uiMode:";
            let uiModeIdx = text.indexOf(uiModeParameter);
            if (uiModeIdx != -1) {
                let uiMode = text.indexOf(":true") != -1; // WTB <GPK Interface> /w me
                this.globalMod().call("setUiMode", { 'uiMode': uiMode });
                return true;
            }

            // handle normal proxy output (mostly /7 or /8)
            this.globalMod().call("handleChatMessage", {
                'author': author,
                'channel': p.channel,
                'message': text
            });
            return true;
        });
        // register private proxy channels (like /7 and /8)
        this.mod.hook("S_JOIN_PRIVATE_CHANNEL", "raw", { order: 999, filter: { fake: true } }, (code, data, fromServer) => {
            this.globalMod().call("handleRawPacket", {
                'direction': fromServer ? 2 : 1,
                'content': data.toString("hex")
            });
            return true;
        });
        // notify to Chat2 that proxy is active
        this.mod.hook("C_LOAD_TOPO_FIN", "raw", () => {
            if (!Globals.TccChatEnabled) {
                Globals.debug("Globals.TccChatEnabled is false, returning");
                return true;
            }
            this.mod.setTimeout(() => {
                Globals.debug("Sending tcc-proxyOn to Chat2.gpk");
                this.mod.send("S_CHAT", this.mod.majorPatchVersion >= 108 ? 4 : 3, {
                    channel: 18,
                    name: "tccChatLink",
                    message: ":tcc-proxyOn:"
                });
                this.notifyShowIngameChatChanged();
            }, 2000);
            return true;
        });
        this.mod.hook("S_LOGIN",
            "raw",
            () => {
                // doing it upon connection causes the client to not enter the server
                this.notifyShowIngameChatChanged();
            });
    }

    isChatLinkAvailable() {
        return true;
    }

    memeA() {
        this.mod.send("S_USER_EFFECT", 1, {
            target: this.mod.game.me.gameId,
            source: 0,
            circle: 2,
            operation: 1
        });
        this.mod.setTimeout(() => {
            this.mod.send("S_USER_EFFECT", 1, {
                target: this.mod.game.me.gameId,
                source: 0,
                circle: 2,
                operation: 2
            });
        }, 10000);
    }

    destructor() {
        //this.globalMod().stopServer();
    }
}

exports.TccStub = TccStub;