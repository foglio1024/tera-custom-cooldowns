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
                            ((CharacterWindowSettings) Settings!).WarriorShowEdge;

    public bool ShowLeaderIcon => Game.Group.AmILeader;

    public Counter EdgeCounter { get; set; } = new(10, true);

    public StanceTracker<WarriorStance> WarriorStanceTracker { get; } = new();

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

    void OnLogin(S_LOGIN m)
    {
        if (m.CharacterClass is Class.Warrior)
        {
            PacketAnalyzer.Processor.Hook<S_PLAYER_STAT_UPDATE>(OnPlayerStatUpdate);
            WarriorAbnormalityTracker.StanceChanged += OnStanceChanged;
        }
    }

    private void OnStanceChanged(WarriorStance stance)
    {
        WarriorStanceTracker.CurrentStance = stance;
    }

    void OnGetUserList(S_GET_USER_LIST m)
    {
        PacketAnalyzer.Processor.Unhook<S_PLAYER_STAT_UPDATE>(OnPlayerStatUpdate);
        WarriorAbnormalityTracker.StanceChanged -= OnStanceChanged;
    }


    void OnPlayerStatUpdate(S_PLAYER_STAT_UPDATE m)
    {
        EdgeCounter.Val = m.Edge;
    }

    void OnLeaderChanged()
    {
        InvokePropertyChanged(nameof(ShowLeaderIcon));
    }

    void MePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        InvokePropertyChanged(e.PropertyName); // TODO: remove????
        if (e.PropertyName != nameof(Data.Pc.Player.Class)) return;

        InvokePropertyChanged(nameof(ShowRe));
        InvokePropertyChanged(nameof(ShowEdge));
        InvokePropertyChanged(nameof(ShowElements));
        InvokePropertyChanged(nameof(Laurel));
    }
}