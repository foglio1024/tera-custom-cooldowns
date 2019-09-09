const { spawn } = require('child_process');
const path = require('path');

module.exports = function TCC(m)
{
    // if (!global.TeraProxy.IsAdmin)
    // {
    //     m.error("TCC requires Toolbox to be executed as admin.");
    //     return;
    // }
    const tcc = spawn(path.join(__dirname, 'TCC.exe'), ["--toolbox"], { stdio: "ignore" });
    tcc.on('exit', (code, signal) => { "TCC exited." });
}