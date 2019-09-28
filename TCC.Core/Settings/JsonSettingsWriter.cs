using System.IO;
using Newtonsoft.Json;

namespace TCC.Settings
{
    public class JsonSettingsWriter : SettingsWriterBase
    {
        public JsonSettingsWriter()
        {
            FileName = SettingsGlobals.JsonFileName;
        }
        public override void Save()
        {
            var json = JsonConvert.SerializeObject(App.Settings, Formatting.Indented);
            var savePath = SettingsContainer.SettingsOverride == ""
                ? Path.Combine(App.BasePath, FileName)
                : SettingsContainer.SettingsOverride;
            File.WriteAllText(savePath, json);
        }
    }
}
