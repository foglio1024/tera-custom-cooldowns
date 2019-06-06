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
            File.WriteAllText(Path.Combine(App.BasePath, FileName), json);
        }
    }
}
