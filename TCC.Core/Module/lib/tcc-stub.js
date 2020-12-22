const { TccInterface } = require("./tcc-interface");
const { Globals } = require("./globals");

// const BadGui = require('../badGui');
class TccStub
{
    constructor(mod)
    {
        this.mod = mod;

        this.tcc = new TccInterface(mod);
        this.installHooks();

        // soon (TM)
        //this.gui = new BadGui(mod);
        //this.mod.command.add('tcc-toggle-gpk', (guiName, bMode) => {
        //    this.gui.parse([{
        //        gpk: `OnGameEventShowUI,${guiName},${bMode}`
        //    }],"HideCharWindow");
        //});

        //memes
        this.mod.hook("S_SYSTEM_MESSAGE", 1, ev =>
        {
            if (mod.game.me.inDungeon === true || mod.game.me.inBattleground === true) return;
            if (ev.message.indexOf("Foglio") === -1 &&
                ev.message.indexOf("Folyemi") === -1) return;

            const sm = mod.parseSystemMessage(ev.message);
            if (sm.id === "SMT_FRIEND_SEND_HELLO"
                || sm.id === "SMT_FRIEND_RECEIVE_HELLO")
            {
                this.memeA();
            }
        });

        this.mod.command.add("tcc", (arg) =>
        {
            if (arg !== "debug") return;
            mod.settings.debug = !mod.settings.debug;
            mod.command.message(`<font color="#cccccc">Debug mode </font><font color="#${(mod.settings.debug ? "42F5AD" : "F05164")}">${(mod.settings.debug ? "en" : "dis")}abled</font>`);
        });

        this.mod.command.add(":tcc-chatmode", (arg) =>
        {
            this.debug("Setting ChatMode to " + arg);
            this.tcc.call("setChatMode", { 'chatMode': arg == "true" });
        });

        this.mod.command.add(":tcc-uimode", (arg) =>
        {
            this.debug("Setting UiMode to " + arg);
            this.tcc.call("setUiMode", { 'uiMode': arg == "true" });
        });

        this.mod.command.add(":tcc-proxyOn:", (arg) => { });
        this.mod.command.add(":tcc-chatOn:", (arg) => { });
        this.mod.command.add(":tcc-chatOff:", (arg) => { });

    }

    notifyShowIngameChatChanged()
    {
        if(!Globals.EnableProxy || !Globals.TccChatEnabled) return;
        this.mod.send("S_CHAT",
            3,
            {
                channel: 18,
                name: "tccChatLink",
                message: Globals.ShowIngameChat ? ":tcc-chatOn:" : ":tcc-chatOff:"
            });
    }

    installHooks()
    {
        // block ingame player menu
        this.mod.hook("S_ANSWER_INTERACTIVE", "raw", () => { return !Globals.EnablePlayerMenu; });
        // block ingame lfg list
        this.mod.hook("S_SHOW_PARTY_MATCH_INFO", "raw", () => { return !Globals.useLfg; });
        // block ingame lfg details
        this.mod.hook("S_PARTY_MEMBER_INFO", "raw", () => { return !Globals.useLfg; });
        this.mod.hook("S_SHOW_CANDIDATE_LIST", "raw", () => { return !Globals.useLfg; });
        // block tcc messages from gpk file
        this.mod.hook("S_CHAT", 3, (p) => { return p.authorName != "tccChatLink"; });
        // hook Command messages to display them in tcc {order: 999, filter:{fake:true}}
        this.mod.hook("S_PRIVATE_CHAT", 1, { order: 999, filter: { fake: true } }, p =>
        {
            var author = "";
            var text = p.message.toString();
            if (p.author == undefined)
            {
                var authorEnd = p.message.toString().indexOf("]");
                if (authorEnd != -1)
                {
                    author = text.substring(1, authorEnd);
                    text = text.substring(authorEnd + 2);
                }
            }
            else author = p.author;

            // handle chatMode from Chat2
            const chatModeParameter = ":tcc-chatMode:";
            let chatModeIdx = text.indexOf(chatModeParameter);
            if (chatModeIdx != -1)
            {
                // Unknown command ":tcc-chatMode:false" 
                let chatMode = text.indexOf(":true") != -1; // WTB <GPK Interface> /w me
                this.tcc.call("setChatMode", { 'chatMode': chatMode });
                return true;
            }

            // handle uiMode from Chat2
            const uiModeParameter = ":tcc-uiMode:";
            let uiModeIdx = text.indexOf(uiModeParameter);
            if (uiModeIdx != -1)
            {
                let uiMode = text.indexOf(":true") != -1; // WTB <GPK Interface> /w me
                this.tcc.call("setUiMode", { 'uiMode': uiMode });
                return true;
            }

            // handle normal proxy output (mostly /7 or /8)
            this.tcc.call("handleChatMessage", {
                'author': author,
                'channel': p.channel,
                'message': text
            });
            return true;
        });
        // register private proxy channels (like /7 and /8)
        this.mod.hook("S_JOIN_PRIVATE_CHANNEL", "raw", { order: 999, filter: { fake: true } }, (code, data, fromServer) =>
        {
            this.tcc.call("handleRawPacket", {
                'direction': fromServer ? 2 : 1,
                'content': data.toString("hex")
            });
            return true;
        });
        // notify to Chat2 that proxy is active
        this.mod.hook("C_LOAD_TOPO_FIN", "raw", () =>
        {
            if (!Globals.EnableProxy || !Globals.TccChatEnabled) {
                this.debug("Globals.EnableProxy is false, returning");
                return true;
            }
            this.mod.setTimeout(() =>
            {
                this.debug("Sending tcc-proxyOn to Chat2.gpk");
                this.mod.send("S_CHAT", 3, {
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

    isChatLinkAvailable()
    {
        return true;
    }

    memeA()
    {
        this.mod.send("S_USER_EFFECT", 1, {
            target: this.mod.game.me.gameId,
            source: 0,
            circle: 2,
            operation: 1
        });
        this.mod.setTimeout(() =>
        {
            this.mod.send("S_USER_EFFECT", 1, {
                target: this.mod.game.me.gameId,
                source: 0,
                circle: 2,
                operation: 2
            });
        }, 10000);
    }

    debug(msg)
    {
        if (!this.mod.settings.debug) return;
        this.mod.command.message(`<font color="#fff1b5">${msg}</font>`);
        this.mod.log(`${msg}`);
    }

    destructor()
    {
        this.tcc.stopServer();
    }
}

exports.TccStub = TccStub;