using System.ComponentModel;
using JetBrains.Annotations;
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