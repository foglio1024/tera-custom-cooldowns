using System;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using TCC.Windows;

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
            try
            {
                File.WriteAllText(savePath, json);
            }
            catch (IOException ex)
            {
                var res = TccMessageBox.Show("TCC", $"Failed to save settings: {ex.Message}\nTry again?", MessageBoxButton.YesNo);
                if(res == MessageBoxResult.Yes) Save();
            }
        }
    }
}
