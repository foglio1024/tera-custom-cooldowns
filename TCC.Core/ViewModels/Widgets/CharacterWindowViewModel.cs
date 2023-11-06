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
        settings.SorcererShowElementsChanged += () => N(nameof(ShowElements));
        settings.WarriorShowEdgeChanged += () => N(nameof(ShowEdge));
        settings.ShowStaminaChanged += () => N(nameof(ShowRe));
        settings.CustomLaurelChanged += () => N(nameof(Laurel));
    }

    void OnLeaderChanged()
    {
        N(nameof(ShowLeaderIcon));
    }

    void MePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        N(e.PropertyName); // TODO: remove????
        if (e.PropertyName != nameof(Data.Pc.Player.Class)) return;
        N(nameof(ShowRe));
        N(nameof(ShowEdge));
        N(nameof(ShowElements));
        N(nameof(Laurel));
    }
}