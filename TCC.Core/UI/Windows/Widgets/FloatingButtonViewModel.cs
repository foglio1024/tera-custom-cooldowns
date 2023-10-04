using System;
using System.Windows.Input;
using Nostrum;
using Nostrum.WPF;
using TCC.Settings.WindowSettings;
using TCC.ViewModels;
using TeraPacketParser.Analysis;
using TeraPacketParser.Messages;

namespace TCC.UI.Windows.Widgets;

public class FloatingButtonViewModel : TccWindowViewModel
{
    public event Action? NotificationsAdded;
    public event Action? NotificationsCleared;
    bool _pendingNotifications;
    int _pendingNotificationsAmount;
    int _currPP;
    int _maxPP;

    public bool PendingNotifications
    {
        get => _pendingNotifications;
        set
        {
            if (_pendingNotifications == value) return;
            _pendingNotifications = value;
            N();
        }
    }
    public int PendingNotificationsAmount
    {
        get => _pendingNotificationsAmount;
        set
        {
            if (_pendingNotificationsAmount == value) return;
            _pendingNotificationsAmount = value;
            N();
        }
    }
    public ICommand OpenSettingsCommand { get; }
    public ICommand OpenLfgCommand { get; }
    public ICommand OpenDashboardCommand { get; }

    public int CurrPP
    {
        get => _currPP;
        set
        {
            if (_currPP == value) return;
            _currPP = value;
            N();
        }
    }
    public int MaxPP
    {
        get => _maxPP;
        set
        {
            if (_maxPP == value) return;
            _maxPP = value;
            N();
        }
    }

    public double CoinsFactor => Game.Me.CoinsFactor;
    public double CurrCoins => Game.Me.Coins;
    public double MaxCoins => Game.Me.MaxCoins;

    public double PPFactor => MathUtils.FactorCalc(_currPP, _maxPP);

    public FloatingButtonViewModel(FloatingButtonWindowSettings settings) : base(settings)
    {
        OpenSettingsCommand = new RelayCommand(_ => WindowManager.SettingsWindow.ShowWindow());
        OpenLfgCommand = new RelayCommand(_ => WindowManager.LfgListWindow.ShowWindow());
        OpenDashboardCommand = new RelayCommand(_ =>
        {
            WindowManager.DashboardWindow.ShowWindow();
            PendingNotifications = false;
            PendingNotificationsAmount = 0;
            NotificationsCleared?.Invoke();
        });

        Game.Me.CoinsUpdated += OnCoinsUpdated;
    }

    void OnCoinsUpdated()
    {
        N(nameof(CoinsFactor));
        N(nameof(CurrCoins));
        N(nameof(MaxCoins));
    }

    protected override void InstallHooks()
    {
        PacketAnalyzer.Processor.Hook<S_FATIGABILITY_POINT>(OnFatigabilityPoint);
    }

    void OnFatigabilityPoint(S_FATIGABILITY_POINT p)
    {
        CurrPP = p.CurrFatigability;
        MaxPP = p.MaxFatigability;
        N(nameof(PPFactor));
    }

    public void WarnCloseEvents(int closeEventsCount)
    {
        PendingNotificationsAmount = closeEventsCount;
        PendingNotifications = true;
        NotificationsAdded?.Invoke();
    }
}