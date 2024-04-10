using System.Collections.Generic;
using System.Windows.Input;
using Nostrum.WPF;
using Nostrum.WPF.ThreadSafe;
using TCC.Interop.Moongourd;

namespace TCC.UI.Controls.Chat;

public class MoongourdPopupViewModel : ThreadSafeObservableObject
{
    private readonly IMoongourdManager _manager;

    private string _playerName = "";

    public string PlayerName
    {
        get => _playerName;
        set => RaiseAndSetIfChanged(value, ref _playerName);
    }

    private string _emptyText = "No data.";

    public string EmptyText
    {
        get => _emptyText;
        set => RaiseAndSetIfChanged(value, ref _emptyText);
    }

    public ThreadSafeObservableCollection<EncounterViewModel> Encounters { get; }

    public MoongourdPopupViewModel()
    {
        Encounters = new ThreadSafeObservableCollection<EncounterViewModel>(_dispatcher);

        _manager = new KabedonManager();
        _manager.Started += OnSearchStarted;
        _manager.Finished += OnSearchFinished;
        _manager.Failed += OnSearchFailed;
    }

    private void OnSearchStarted()
    {
        _dispatcher.Invoke(() => EmptyText = "Loading...");
    }

    private void OnSearchFinished(List<IMoongourdEncounter> list)
    {
        _dispatcher.Invoke(() =>
        {
            EmptyText = "No entries.";
            Encounters.Clear();
            list.ForEach(e => Encounters.Add(new EncounterViewModel(e.AreaId, e.BossId, e.LogId) { PlayerDeaths = e.PlayerDeaths, PlayerDps = e.PlayerDps }));
        });
    }

    private void OnSearchFailed(string error)
    {
        _dispatcher.Invoke(() =>
        {
            EmptyText = $"Failed to retrieve data.\n{error}";
        });
    }

    public void RequestInfo(string name, string region)
    {
        _dispatcher.Invoke(() =>
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

public class EncounterViewModel
{
    public string BossName { get; }
    public string DungeonName { get; }
    public int PlayerDps { get; init; }
    public int PlayerDeaths { get; init; }

    public ICommand Browse { get; }

    public EncounterViewModel(int areaId, int bossId, long logId)
    {
        DungeonName = Game.DB!.RegionsDatabase.GetZoneName((uint)areaId);
        BossName = Game.DB.MonsterDatabase.GetMonsterName((uint)bossId, (uint)areaId);

        Browse = new RelayCommand(_ =>
        {
            var reg = App.Settings.LastLanguage.ToLower();
            reg = reg.StartsWith("eu") ? "eu" : reg;
            reg = reg == "na" ? "" : reg + "/";
            Utils.Utilities.OpenUrl("https://" + $"moongourd.com/{reg}upload/{areaId}/{bossId}/1/{logId}");
        });
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