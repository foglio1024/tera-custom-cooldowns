const { spawn } = require('child_process');
const path = require('path');

exports.ClientMod = class
{
    constructor(m)
    {
        const tccPath = path.join(__dirname, 'TCC.exe');

        if (!m.manager.isInstalled("tcc-chat-link"))
        {
            m.log("TCC chat link is not installed. Advanced functionality won't be available.");
        }

        m.clientInterface.once('ready', () => 
        {
		    m.log('Starting TCC...');
		    const tcc = spawn(tccPath, ['--toolbox'], { stdio: 'ignore' });
		    tcc.on('exit', () => m.log('TCC exited because it closed or it is already running.'));
	    });
    }
}