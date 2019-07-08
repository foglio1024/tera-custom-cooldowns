const { spawn } = require('child_process');
const path = require('path');

module.exports = function TCC(m)
{
    const tcc = spawn(path.join(__dirname, 'TCC.exe'), ["--toolbox"], {stdio : "ignore"});
    tcc.on('exit', (code, signal) => { "TCC exited." });
}