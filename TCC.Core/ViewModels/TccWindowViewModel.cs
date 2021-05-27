using System.Windows.Threading;
using Nostrum;
using TCC.Settings.WindowSettings;
using TeraPacketParser.Analysis;

namespace TCC.ViewModels
{
    public class TccWindowViewModel : TSPropertyChanged
    {
        public WindowSettingsBase? Settings { get; }
        
        /// <summary>
        /// Called from <see cref="OnEnabledChanged"/> or when <see cref="PacketAnalyzer.ProcessorReady"/> is raised.
        /// </summary>
        protected virtual void InstallHooks() { }
        /// <summary>
        /// Called from <see cref="OnEnabledChanged"/>.
        /// </summary>
        protected virtual void RemoveHooks() { }
        /// <summary>
        /// Called when <see cref="WindowSettingsBase.Enabled"/> changes.
        /// </summary>
        /// <param name="enabled"></param>
        protected virtual void OnEnabledChanged(bool enabled)
        {
            if (enabled) InstallHooks();
            else RemoveHooks();
        }

        protected TccWindowViewModel(WindowSettingsBase? settings)
        {
            Dispatcher = Dispatcher.CurrentDispatcher;

            if (settings != null)
            {
                Settings = settings;
                settings.EnabledChanged += OnEnabledChanged;
                if (!settings.Enabled) return;
            }

            PacketAnalyzer.ProcessorReady += InstallHooks;
        }
    }
}
