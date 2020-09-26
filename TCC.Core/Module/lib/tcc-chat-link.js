exports.TccChatLink = class
{
    constructor(mod)
    {
        this.isInstalled = false;
        this.mod = mod;    
        this.tcc = mod.globalMod.tcc;
    }

    install(installer)
    {
        if(!this.globalMod.installChat2) return;
        installer.gpk("../gpk/S1UI_Chat2.gpk");
        this.isInstalled = true;
    }
};