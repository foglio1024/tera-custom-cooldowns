const { spawn } = require('child_process');
module.exports = function TCC(m)
{
    const tcc = spawn('TCC.exe', ["--toolbox"]);
    tcc.on('exit', (code, signal) => { "TCC exited." });
}