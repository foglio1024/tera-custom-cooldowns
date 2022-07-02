class Globals
{
    static useLfg = false;
    static EnablePlayerMenu = false;
    static EnableProxy = true;
    static ShowIngameChat = false;
    static TccChatEnabled = true;

    static setCommand(command){
        this.command = command;
    }
    static setLogging(mod){
        this.mod = mod;
        this.debug = function(msg){
            if (!this.mod.settings.debug) return;
            this.command?.message(`<font color="#fff1b5">${msg}</font>`);
            this.mod.log(`${msg}`);
        }
    }
}

exports.Globals = Globals;
