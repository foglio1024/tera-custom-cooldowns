using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
