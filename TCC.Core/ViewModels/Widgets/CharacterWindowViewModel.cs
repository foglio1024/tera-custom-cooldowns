using JetBrains.Annotations;
using System.ComponentModel;
using TCC.Data;
using TCC.Data.Abnormalities;
using TCC.Data.Pc;
using TCC.Settings.WindowSettings;
using TCC.Utilities;
using TCC.Utils;
using TeraDataLite;
using TeraPacketParser.Analysis;
using TeraPacketParser.Messages;

namespace TCC.ViewModels.Widgets;

[TccModule]
[UsedImplicitly]
public class CharacterWindowViewModel : TccWindowViewModel // hooks in Game
{
    public Player Player => Game.Me;
    public bool ShowRe => Player.Class is Class.Brawler
                                       or Class.Gunner
                                       or Class.Ninja
                                       or Class.Valkyrie
                                    && ((CharacterWindowSettings)Settings!).ShowStamina;
    public bool ShowElements => Player.Class == Class.Sorcerer &&
                                ((CharacterWindowSettings)Settings!).SorcererShowElements;

    public bool ShowEdge => Player.Class == Class.Warrior &&
                            ((CharacterWindowSettings)Settings!).WarriorShowEdge;

    public bool ShowLeaderIcon => Game.Group.AmILeader;

    public Counter EdgeCounter { get; set; } = new(10, true);

    public StanceTracker<WarriorStance> WarriorStanceTracker { get; } = new();

    private FusionElements _sorcererFusionBoost;
    public FusionElements SorcererFusionBoost
    {
        get => _sorcererFusionBoost;
        set => RaiseAndSetIfChanged(value, ref _sorcererFusionBoost);
    }

    private FusionElements _sorcererFusionElements;
    public FusionElements SorcererFusionElements
    {
        get => _sorcererFusionElements;
        set => RaiseAndSetIfChanged(value, ref _sorcererFusionElements);
    }

    public CustomLaurel Laurel
    {
        get
        {
            var cl = ((CharacterWindowSettings)Settings!).CustomLaurel;

            return cl == CustomLaurel.Game ? TccUtils.CustomFromLaurel(Player.Laurel) : cl;
        }
    }


    public CharacterWindowViewModel(CharacterWindowSettings settings) : base(settings)
    {
        Game.Me.PropertyChanged += MePropertyChanged;
        Game.Group.LeaderChanged += OnLeaderChanged;
        settings.SorcererShowElementsChanged += () => InvokePropertyChanged(nameof(ShowElements));
        settings.WarriorShowEdgeChanged += () => InvokePropertyChanged(nameof(ShowEdge));
        settings.ShowStaminaChanged += () => InvokePropertyChanged(nameof(ShowRe));
        settings.CustomLaurelChanged += () => InvokePropertyChanged(nameof(Laurel));
    }

    protected override void InstallHooks()
    {
        PacketAnalyzer.Processor.Hook<S_LOGIN>(OnLogin);
        PacketAnalyzer.Processor.Hook<S_GET_USER_LIST>(OnGetUserList);
    }

    protected override void RemoveHooks()
    {
        PacketAnalyzer.Processor.Unhook<S_LOGIN>(OnLogin);
        PacketAnalyzer.Processor.Unhook<S_GET_USER_LIST>(OnGetUserList);
    }

    private void OnWarriorStanceChanged(WarriorStance stance)
    {
        WarriorStanceTracker.CurrentStance = stance;
    }
    private void OnSorcererBoostChanged(FusionElements element)
    {
        SorcererFusionBoost = element;
    }

    private void OnLogin(S_LOGIN m)
    {
        switch (m.CharacterClass)
        {
            case Class.Warrior:
                PacketAnalyzer.Processor.Hook<S_PLAYER_STAT_UPDATE>(OnPlayerStatUpdate);
                WarriorAbnormalityTracker.StanceChanged += OnWarriorStanceChanged;
                break;
            case Class.Sorcerer:
                PacketAnalyzer.Processor.Hook<S_PLAYER_STAT_UPDATE>(OnPlayerStatUpdate);
                SorcererAbnormalityTracker.BoostChanged += OnSorcererBoostChanged;
                break;

        }
    }

    private void OnGetUserList(S_GET_USER_LIST m)
    {
        PacketAnalyzer.Processor.Unhook<S_PLAYER_STAT_UPDATE>(OnPlayerStatUpdate);
        WarriorAbnormalityTracker.StanceChanged -= OnWarriorStanceChanged;
        SorcererAbnormalityTracker.BoostChanged -= OnSorcererBoostChanged;
    }

    private void OnPlayerStatUpdate(S_PLAYER_STAT_UPDATE m)
    {
        switch (Game.Me.Class)
        {
            case Class.Warrior:
                EdgeCounter.Val = m.Edge;
                break;

            case Class.Sorcerer:
                SorcererFusionElements = TccUtils.BoolsToElements(m.Fire, m.Ice, m.Arcane);
                break;
        }
    }

    private void OnLeaderChanged()
    {
        InvokePropertyChanged(nameof(ShowLeaderIcon));
    }

    private void MePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        InvokePropertyChanged(e.PropertyName); // TODO: remove????
        if (e.PropertyName != nameof(Data.Pc.Player.Class)) return;

        InvokePropertyChanged(nameof(ShowRe));
        InvokePropertyChanged(nameof(ShowEdge));
        InvokePropertyChanged(nameof(ShowElements));
        InvokePropertyChanged(nameof(Laurel));
    }
}