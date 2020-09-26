const { spawn } = require('child_process');
const Path = require('path');
const FS = require('fs');

class TccLauncher
{
    constructor(m)
    {
        const tccPath = Path.join(__dirname, '../TCC.exe');
        m.clientInterface.once('ready', () => 
        {
            m.log('Starting TCC...');
            const tcc = spawn(tccPath, ['--toolbox'], { stdio: 'ignore' });
            tcc.on('exit', () => m.log('TCC exited because it closed or it is already running.'));
        });

        this.install = function (installer)
        {
            const tccSettingsPath = Path.join(__dirname, '../tcc-settings.json');
            let noSettings = false;
            if (!FS.existsSync(tccSettingsPath))
                noSettings = true;;

            if (noSettings)
            {
                // no settings, remove everything by default
                m.log("No settings found, installing all removers by default.")
                installer.gpk('gpk/S1UI_CharacterWindow.gpk');
                installer.gpk('gpk/S1UI_TargetInfo.gpk');
                installer.gpk('gpk/S1UI_GageBoss.gpk');
                installer.gpk('gpk/S1UI_Abnormality.gpk');
                installer.gpk("gpk/S1UI_PartyWindow.gpk");
                installer.gpk("gpk/S1UI_PartyWindowRaidInfo.gpk");
                // installer.gpk('gpk/S1UI_ProgressBar.gpk');
                installer.gpk("gpk/S1UI_PartyBoard.gpk");
                installer.gpk("gpk/S1UI_PartyBoardMemberInfo.gpk");
                installer.gpk("gpk/S1UI_Chat2.gpk");
                return;
            }

            m.log("Reading TCC settings.")
            let settings = JSON.parse(FS.readFileSync(tccSettingsPath));

            if (settings.CharacterWindowSettings.Enabled === true)
            {
                installer.gpk('gpk/S1UI_CharacterWindow.gpk');
                m.log("Installed S1UI_CharacterWindow remover.");
            }

            if (settings.NpcWindowSettings.Enabled === true)
            {
                installer.gpk('gpk/S1UI_TargetInfo.gpk');
                installer.gpk('gpk/S1UI_GageBoss.gpk');
                m.log("Installed S1UI_TargetInfo and S1UI_GageBoss remover.");
            }

            if (settings.BuffWindowSettings.Enabled === true)
            {
                installer.gpk('gpk/S1UI_Abnormality.gpk');
                m.log("Installed S1UI_Abnormality remover.");
            }

            if (settings.GroupWindowSettings.Enabled === true)
            {
                installer.gpk("gpk/S1UI_PartyWindow.gpk");
                installer.gpk("gpk/S1UI_PartyWindowRaidInfo.gpk");
                m.log("Installed S1UI_PartyWindow and S1UI_PartyWindowRaidInfo remover.");
            }

            // if (settings.FlightGaugeWindowSettings.Enabled === true)
            // {
            //     installer.gpk('gpk/S1UI_ProgressBar.gpk');
            //     m.log("Installed S1UI_ProgressBar remover.");
            // }

            if (settings.LfgWindowSettings.Enabled === true)
            {
                installer.gpk("gpk/S1UI_PartyBoard.gpk");
                installer.gpk("gpk/S1UI_PartyBoardMemberInfo.gpk");
                m.log("Installed S1UI_PartyBoard and S1UI_PartyBoardMemberInfo remover.");
            }

            if (settings.ChatEnabled === true)
            {
                installer.gpk("gpk/S1UI_Chat2.gpk");
                m.log("Installed S1UI_Chat2.");
            }
        };
    }
}

exports.TccLauncher = TccLauncher;