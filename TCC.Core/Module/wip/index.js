const { spawn } = require('child_process');
const path = require('path');
const { TccStub } = require('./lib/tcc-stub');
const { TccChatLink } = require('./lib/tcc-chat-link');
const { TccInterface } = require("./lib/tcc-interface");

class TCC
{
    // debug(msg)
    // {
    //     if (!this.mod.settings.debug) return;
    //     this.mod.command.message(`<font color="#fff1b5">${msg}</font>`);
    // }

    constructor(m)
    {
        const tccPath = path.join(__dirname, 'TCC.exe');
        const tccProcess = spawn(tccPath, ['--toolbox'], { stdio: 'ignore' });
        tccProcess.on('exit', () => m.log('TCC exited because it closed or it is already running.'));


        this.mod = m;
        this.mod.debug = function (msg)
        {
            if (!m.settings.debug) return;
            m.command.message(`<font color="#fff1b5">${msg}</font>`);
        }

        this.tcc = new TccInterface(m);
    }


    destructor()
    {
        this.debug('Stopping rpc server');
        this.tcc.server.stop();
    }
}


module.exports = { GlobalMod: TCC, ClientMod: TccChatLink, NetworkMod: TccStub };