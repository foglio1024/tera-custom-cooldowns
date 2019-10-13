using Microsoft.Win32;
using Newtonsoft.Json;
using System.IO;
using System.Windows;
using TCC.Data;
using TCC.Windows;
using MessageBoxImage = TCC.Data.MessageBoxImage;

namespace TCC.Settings
{
    public class JsonSettingsReader : SettingsReaderBase
    {
        public JsonSettingsReader()
        {
            FileName = SettingsGlobals.JsonFileName;
        }
        public void LoadSettings(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    var file = File.ReadAllText(path);
                    #region Compatibility
                    file = file.Replace("\"TabName\"", "\"Name\"")
                               .Replace("\"ExcludedAuthors\"", "\"HiddenAuthors\"")
                               .Replace("\"ExcludedChannels\"", "\"HiddenChannels\"")
                               .Replace("\"Channels\"", "\"ShowedChannels\"")
                               .Replace("\"Authors\"", "\"ShowedAuthors\"");
                    #endregion
                    App.Settings = JsonConvert.DeserializeObject<SettingsContainer>(file);
                }
                else
                {
                    var res = TccMessageBox.Show("Settings file not found. Do you want to import an existing one?", MessageBoxType.ConfirmationWithYesNo);
                    if (res == MessageBoxResult.No)
                    {
                        App.Settings = new SettingsContainer();
                        return;
                    }
                    var diag = new OpenFileDialog
                    {
                        Title = $"Import TCC settings file ({FileName})",
                        Filter = $"{FileName} (*.json)|*.json"
                    };
                    if (diag.ShowDialog() == true)
                    {
                        path = diag.FileName;
                        LoadSettings(path);
                    }
                    else App.Settings = new SettingsContainer();
                }
            }
            catch
            {
                var res = TccMessageBox.Show("TCC", "Cannot load settings file. Do you want TCC to delete it and recreate a default file?", MessageBoxButton.YesNo, MessageBoxImage.Error);
                if (res == MessageBoxResult.Yes) File.Delete(path);
                LoadSettings(path);
            }
        }
    }
}
