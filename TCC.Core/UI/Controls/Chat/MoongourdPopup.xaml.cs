using Nostrum;
using System.Collections.Generic;
using System.Windows.Threading;
using TCC.Interop.Moongourd;

namespace TCC.UI.Controls.Chat
{
    public class MoongourdPopupViewModel : TSPropertyChanged
    {
        private readonly IMoongourdManager _manager;

        private string _playerName = "";

        public string PlayerName
        {
            get => _playerName;
            set
            {
                if (_playerName == value) return;
                _playerName = value;
                N();
            }
        }

        private string _emptyText = "No data.";

        public string EmptyText
        {
            get => _emptyText;
            set
            {
                if (_emptyText == value) return;
                _emptyText = value;
                N();
            }
        }

        public TSObservableCollection<MoongourdEncounter> Encounters { get; }

        public MoongourdPopupViewModel()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;

            Encounters = new TSObservableCollection<MoongourdEncounter>(Dispatcher);

            _manager = new KabedonManager();
            _manager.Started += OnSearchStarted;
            _manager.Finished += OnSearchFinished;
            _manager.Failed += OnSearchFailed;
        }

        private void OnSearchStarted()
        {
            Dispatcher?.Invoke(() => EmptyText = "Loading...");
        }

        private void OnSearchFinished(List<MoongourdEncounter> list)
        {
            Dispatcher.Invoke(() =>
            {
                EmptyText = "No entries.";
                Encounters.Clear();
                list.ForEach(Encounters.Add);
            });
        }

        private void OnSearchFailed(string error)
        {
            Dispatcher?.Invoke(() =>
            {
                EmptyText = $"Failed to retrieve data.\n{error}";
            });
        }

        public void RequestInfo(string name, string region)
        {
            Dispatcher?.Invoke(() =>
            {
                PlayerName = name;
                Encounters.Clear();
            });

            if (region.StartsWith("EU"))
            {
                region = "EU";
            }
            _manager.GetEncounters(name, region, Game.Server.Name);
        }
    }

    public partial class MoongourdPopup
    {
        public MoongourdPopup()
        {
            MouseLeave += (_, _) => WindowManager.ViewModels.PlayerMenuVM.Close();
            InitializeComponent();
        }
    }
}