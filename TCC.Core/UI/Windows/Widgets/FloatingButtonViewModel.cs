using System;
using System.Windows.Input;
using JetBrains.Annotations;
using Nostrum;
using Nostrum.WPF;
using TCC.Settings.WindowSettings;
using TCC.ViewModels;
using TeraPacketParser.Analysis;
using TeraPacketParser.Messages;

namespace TCC.UI.Windows.Widgets;

[UsedImplicitly]
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
        set => RaiseAndSetIfChanged(value, ref _pendingNotifications);
    }
    public int PendingNotificationsAmount
    {
        get => _pendingNotificationsAmount;
        set => RaiseAndSetIfChanged(value, ref _pendingNotificationsAmount);
    }
    public ICommand OpenSettingsCommand { get; }
    public ICommand OpenLfgCommand { get; }
    public ICommand OpenDashboardCommand { get; }

    public int CurrPP
    {
        get => _currPP;
        set => RaiseAndSetIfChanged(value, ref _currPP);
    }
    public int MaxPP
    {
        get => _maxPP;
        set => RaiseAndSetIfChanged(value, ref _maxPP);
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
        InvokePropertyChanged(nameof(CoinsFactor));
        InvokePropertyChanged(nameof(CurrCoins));
        InvokePropertyChanged(nameof(MaxCoins));
    }

    protected override void InstallHooks()
    {
        PacketAnalyzer.Processor.Hook<S_FATIGABILITY_POINT>(OnFatigabilityPoint);
    }

    void OnFatigabilityPoint(S_FATIGABILITY_POINT p)
    {
        CurrPP = p.CurrFatigability;
        MaxPP = p.MaxFatigability;
        InvokePropertyChanged(nameof(PPFactor));
    }

    public void WarnCloseEvents(int closeEventsCount)
    {
        PendingNotificationsAmount = closeEventsCount;
        PendingNotifications = true;
        NotificationsAdded?.Invoke();
    }
}