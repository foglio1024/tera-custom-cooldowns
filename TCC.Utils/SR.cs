using System.Text;

namespace TCC.Utils;

/// <summary>
/// String Resources file.
/// </summary>
public static class SR
{
    public const string AnotherInstanceRunning        = "Another instance of TCC is already running. Shutting down.";
    public const string CannotDetectCurrentRegion     = "Current region could not be detected, so TCC will load EU-EN database. To force a specific language, use Region Override setting in Misc Settings.";
    public const string RenderThreadError             = "An error in render thread occured. This is usually caused by outdated video card drivers. TCC will now close.";
    public const string FatalError                    = "An error occured and TCC will now close. Please report this issue to the developer attaching crash.log from TCC folder.";
    public const string FatalErrorAskUpload           = "An error occured and TCC will now close. Do you want TCC to automatically upload a crash report?\n Please report this issue to the developer attaching crash.log from TCC folder.";
    public const string OutOfMemoryError              = "TCC doesn't have enough available RAM to keep running and will now close.";
    public const string SettingsNotFoundImport        = "Settings file not found. Do you want to import an existing one?";
    public const string SettingsNotFoundDefault       = "Cannot load settings file. Do you want TCC to delete it and recreate a default file?";
    public const string IconDownloadFailed            = "Failed to download icons, try again?";
    public const string IconExtractFailed             = "Error while extracting some icons (details in error.log). Try again?";
    public const string UpdateCheckFailed             = "Error while checking updates. Try again?";
    public const string UpdateDownloadFailed          = "Error while downloading update. Try again? If the error perists download TCC manually.";
    public const string ServersFileDownloadFailed     = "Failed to download servers file. Try again?";
    public const string CannotReadCharacters          = "There was an error while reading characters file (more info in error.log). \nManually correct the error and press Ok to try again, else press Cancel to delete current data.";
    public const string RestartToApplySetting         = "TCC needs to be restarted to apply this setting. Restart now?";
    public const string BetaUnstableWarning           = "Warning: beta build could be unstable. Proceed?";
    public const string BetaAvailable                 = "A beta version of TCC is available. Open System settings to download it or to disable this notification.";
    public const string UpdatingDatabase              = "Some database files are out of date, updating... Contact the developer if you see this message at every login.";
    public const string BgMatchingComplete            = "Battleground matching completed";
    public const string DungMatchingComplete          = "Dungeon matching completed";
    public const string GreetMemeContent              = "Nice TCC (° -°)";
    public const string GlobalSellAngery              = "Stop selling and buying stuff in global.\nYou nob.";
    public const string CannotRetrieveGbamInfo        = "Failed to retrieve guild BAM info.";
    public const string CannotUploadGbamInfo          = "Failed to upload guild BAM info.";
    public const string ReadyToConnect                = "Ready to connect.";
    public const string Disconnected                  = "Disconnected.";
    public const string ForcingGameDrivenClickThruOff = "TERA client architecture is 64bit. This means that gpk mods will not work at the moment. Widgets having clickthru mode set to Game-driven have been set to be always clickthru. Adjust your setting according to the desired behavior.";


    public static string ErrorWhileLoadingModule(string? filename)
    {
        return $"An error occured while loading '{filename}'. TCC will now close. You can find more info about this error in TERA Dps discord #known-issues channel.";
    }
    public static string CannotLoadDbForLang(string? lang)
    {
        return $"Unable to load database for language '{lang}'. \nThis could be caused by a wrong Language override value or corrupted TCC download.\n\n Do you want to open settings and change it?\n\n Choosing 'No' will load EU-EN database,\nchoosing 'Cancel' will close TCC.";
    }
    public static string CannotDetectClientVersion(bool stubAvailable)
    {
        var sb = new StringBuilder("Failed to detect client version.");

        sb.Append(stubAvailable
            ? "\nSince you're already using TERA Toolbox, please consider installing TCC as a module (more info in the wiki)."
            : "\nPlease consider installing TCC as a TERA Toolbox module (more info in the wiki).");

        sb.Append("\nTCC will now close.");

        return sb.ToString();
    }
    public static string UnknownClientVersion(uint v)
    {
        return $"Unknown client version: {v}";
    }
    public static string InvalidOpcodeFile(string error)
    {
        return $"TCC encountered errors while reading opcodes file. This is probably caused by a manually mapped file containing wrong values. Error: {error}. TCC will now close.";
    }
    public static string InvalidSysMsgFile(int releaseVersion, uint factoryVersion)
    {
        return $"sysmsg.{releaseVersion}.map or sysmsg.{factoryVersion}.map not found.\nWait for update or use tcc-stub to automatically retreive sysmsg files from game client.\nTCC will now close.";
    }
    public static string UnableToOpenUrl(string url)
    {
        return $"Unable to open URL {url}.";
    }
    public static string CannotSaveSettings(string error)
    {
        return $"Failed to save settings: {error}\nTry again?";
    }
    public static string DbDownloadFailed(string path)
    {
        return $"Failed to download database file {path}. Try again?";
    }
    public static string CannotReadEventsFile(string region)
    {
        return $"There was an error while reading events-{region}.xml. Manually correct the error and and press Ok to try again, else press Cancel to build a default config file.";
    }

    public static string ConnectedToServer(string srv)
    {
        return $"Connected to {srv}";
    }
}