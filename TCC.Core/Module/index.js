const { spawn } = require('child_process');
const path = require('path');

module.exports = function TCC(m)
{
    const ngenPath = path.join(__dirname, "ngen.exe");
    const tccPath = path.join(__dirname, "TCC.exe");
    const modsPath = path.join(__dirname, "TCC.Modules.dll");

    m.log("Starting ngen...");

    // Cringe.
    ngen(modsPath).on('exit', () => {
        ngen(tccPath).on('exit', () => {
            run();
        });
    });

    function run(){
        m.log("Starting TCC...");
        const tcc = spawn(tccPath, ["--toolbox"], { stdio: "ignore" });
        tcc.on('exit', () => m.log("TCC exited."));
    }

    function ngen(assembly)
    {
        return spawn(ngenPath, ["install", assembly], {stdio : "ignore"});
    }
}