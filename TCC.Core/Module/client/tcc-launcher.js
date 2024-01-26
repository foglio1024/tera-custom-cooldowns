const { spawn } = require("child_process");
const Path = require("path");
const FS = require("fs");
//const { TccInterface } = require("./lib/tcc-interface");

class TccLauncher
{
    constructor(m)
    {
        let arch = m.clientInterface.info.arch === 'x64' ? 'x64' : 'x86';

        const tccPath = Path.join(__dirname, "../TCC.exe");
        m.clientInterface.once("ready", () => 
        {
            m.log("Starting TCC...");
            const tcc = spawn(tccPath, ["--toolbox"], { stdio: "ignore" });
            tcc.on("exit", () => m.log("TCC exited because it closed or it is already running."));
        });

        this.tryInstallRemover = function(removerGpkName, installer, removerNiceName)
        {
            try
            {
                installer.gpk(`client/gpk/${arch}/${removerGpkName}`);
                m.log(`Installed ${removerNiceName} remover (${removerGpkName})`);
            } 
            catch (error)
            {
                m.error(`${removerGpkName} has been already installed by another mod. ${removerNiceName} remover won't be installed.`);
                m.error("Error while installing " + removerGpkName + ": ");
                m.error(error);
            }
        }

        this.install = function (installer)
        {
            const tccSettingsPath = Path.join(__dirname, "../tcc-settings.json");
            let noSettings = false;
            if (!FS.existsSync(tccSettingsPath))
                noSettings = true;;

            if (noSettings)
            {
                // no settings, remove everything by default
                m.log("No settings found, installing all removers by default.");
                this.tryInstallRemover("S1UI_CharacterWindow.gpk", installer, "Character window");
                this.tryInstallRemover("S1UI_TargetInfo.gpk", installer, "Mob HP bar");
                this.tryInstallRemover("S1UI_GageBoss.gpk", installer, "Boss HP bar");
                this.tryInstallRemover("S1UI_Abnormality.gpk", installer, "Buff bar");
                this.tryInstallRemover("S1UI_PartyWindow.gpk", installer, "Party windows");
                this.tryInstallRemover("S1UI_PartyWindowRaidInfo.gpk", installer, "Raid windows");
                this.tryInstallRemover("S1UI_PartyBoard.gpk", installer, "LFG window");
                this.tryInstallRemover("S1UI_PartyBoardMemberInfo.gpk", installer, "LFG details info");
                this.tryInstallRemover("S1UI_Chat2.gpk", installer, "Chat");
                this.tryInstallRemover("S1UI_DistributionWindow.gpk", installer, "Dice roll");
                this.tryInstallRemover("S1UI_ProgressBar.gpk", installer, "Flight gauge");
                return;
            }

            let settings = JSON.parse(FS.readFileSync(tccSettingsPath));

            if (settings.CharacterWindowSettings.Enabled === true)
            {
                this.tryInstallRemover("S1UI_CharacterWindow.gpk", installer, "Character window");
            }

            let hideNpcWin = true;
            if (settings.NpcWindowSettings.HideIngameUI !== undefined)
                hideNpcWin = settings.NpcWindowSettings.HideIngameUI;

            if (settings.NpcWindowSettings.Enabled === true && hideNpcWin === true)
            {
                this.tryInstallRemover("S1UI_TargetInfo.gpk", installer, "Mob HP bar");
                this.tryInstallRemover("S1UI_GageBoss.gpk", installer, "Boss HP bar");
            }

            let hideBuffWin = true;
            if (settings.BuffWindowSettings.HideIngameUI !== undefined)
                hideBuffWin = settings.BuffWindowSettings.HideIngameUI;

            if (settings.BuffWindowSettings.Enabled === true && hideBuffWin === true)
            {
                this.tryInstallRemover("S1UI_Abnormality.gpk", installer, "Buff bar");
            }

            let hideGroupWin = true;
            if (settings.GroupWindowSettings.HideIngameUI !== undefined)
                hideGroupWin = settings.GroupWindowSettings.HideIngameUI;

            if (settings.GroupWindowSettings.Enabled === true && hideGroupWin === true)
            {
                this.tryInstallRemover("S1UI_PartyWindow.gpk", installer, "Party windows");
                this.tryInstallRemover("S1UI_PartyWindowRaidInfo.gpk", installer, "Raid windows");
            }

            if (settings.FlightGaugeWindowSettings.Enabled === true)
            {
                this.tryInstallRemover("S1UI_ProgressBar.gpk", installer, "Flight gauge");
            }
            
            if (settings.LfgWindowSettings.Enabled === true)
            {
                this.tryInstallRemover("S1UI_PartyBoard.gpk", installer, "LFG window");
                this.tryInstallRemover("S1UI_PartyBoardMemberInfo.gpk", installer, "LFG details info");
            }

            if (settings.ChatEnabled === true)
            {
                this.tryInstallRemover("S1UI_Chat2.gpk", installer, "Chat");
            }

            if (settings.LootDistributionWindowSettings.Enabled === true) {
                this.tryInstallRemover("S1UI_DistributionWindow.gpk", installer, "Dice roll");
            }

        };
    }
}

exports.TccLauncher = TccLauncher;