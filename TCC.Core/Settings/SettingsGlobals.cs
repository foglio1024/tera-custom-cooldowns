using System.IO;

namespace TCC.Settings
{
    public static class SettingsGlobals
    {
        public const string SettingsFileName = "tcc-settings.json";

        public static readonly string CharacterXmlPath = Path.Combine(App.ResourcesPath, "config/characters.xml");
        public static readonly string CharacterJsonPath = Path.Combine(App.ResourcesPath, "config/characters.json");
    }
}
