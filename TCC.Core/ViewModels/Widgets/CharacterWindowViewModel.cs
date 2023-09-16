using System.ComponentModel;
using TCC.Data.Pc;
using TCC.Settings.WindowSettings;
using TCC.Utilities;
using TCC.Utils;
using TeraDataLite;
using TeraPacketParser.Analysis;
using TeraPacketParser.Messages;

namespace TCC.ViewModels.Widgets;

[TccModule]
public class CharacterWindowViewModel : TccWindowViewModel
{
    public Player Player => Game.Me;
    public bool ShowRe =>
        (Player.Class == Class.Brawler || Player.Class == Class.Gunner ||
         Player.Class == Class.Ninja || Player.Class == Class.Valkyrie) &&
        ((CharacterWindowSettings)Settings!).ShowStamina;
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

            if (cl == CustomLaurel.Game) return TccUtils.CustomFromLaurel(Player.Laurel);
            else return cl;
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

    protected override void InstallHooks()
    {
        PacketAnalyzer.Processor.Hook<S_PLAYER_STAT_UPDATE>(OnPlayerStatUpdate);
        PacketAnalyzer.Processor.Hook<S_CREATURE_LIFE>(OnCreatureLife);
        PacketAnalyzer.Processor.Hook<S_ABNORMALITY_DAMAGE_ABSORB>(OnAbnormalityDamageAbsorb);
        PacketAnalyzer.Processor.Hook<S_CREATURE_CHANGE_HP>(OnCreatureChangeHp);
        PacketAnalyzer.Processor.Hook<S_PLAYER_CHANGE_MP>(OnPlayerChangeMp);
        PacketAnalyzer.Processor.Hook<S_PLAYER_CHANGE_STAMINA>(OnPlayerChangeStamina);
    }

    protected override void RemoveHooks()
    {
        PacketAnalyzer.Processor.Unhook<S_PLAYER_STAT_UPDATE>(OnPlayerStatUpdate);
        PacketAnalyzer.Processor.Unhook<S_CREATURE_LIFE>(OnCreatureLife);
        PacketAnalyzer.Processor.Unhook<S_ABNORMALITY_DAMAGE_ABSORB>(OnAbnormalityDamageAbsorb);
        PacketAnalyzer.Processor.Unhook<S_CREATURE_CHANGE_HP>(OnCreatureChangeHp);
        PacketAnalyzer.Processor.Unhook<S_PLAYER_CHANGE_MP>(OnPlayerChangeMp);
        PacketAnalyzer.Processor.Unhook<S_PLAYER_CHANGE_STAMINA>(OnPlayerChangeStamina);
    }

    void OnPlayerChangeStamina(S_PLAYER_CHANGE_STAMINA m)
    {
        Player.MaxST = m.MaxST + m.BonusST;
        Player.CurrentST = m.CurrentST;
    }
    void OnPlayerChangeMp(S_PLAYER_CHANGE_MP m)
    {
        if (!Game.IsMe(m.Target)) return;
        Player.MaxMP = m.MaxMP;
        Player.CurrentMP = m.CurrentMP;
    }
    void OnCreatureChangeHp(S_CREATURE_CHANGE_HP m)
    {
        if (!Game.IsMe(m.Target)) return;
        Player.MaxHP = m.MaxHP;
        Player.CurrentHP = m.CurrentHP;
    }
    void OnAbnormalityDamageAbsorb(S_ABNORMALITY_DAMAGE_ABSORB p)
    {
        // todo: add chat message too
        if (!Game.IsMe(p.Target) || Player.CurrentShield < 0) return;
        Player.DamageShield(p.Damage);
    }

    void OnCreatureLife(S_CREATURE_LIFE m)
    {
        if (!Game.IsMe(m.Target)) return;
        Player.IsAlive = m.Alive;
    }

    // TODO: move these to Game?
    void OnPlayerStatUpdate(S_PLAYER_STAT_UPDATE m)
    {
        Player.ItemLevel = m.Ilvl;
        Player.Level = m.Level;
        Player.CritFactor = m.TotalCritFactor;
        Player.MaxHP = m.MaxHP;
        Player.MaxMP = m.MaxMP;
        Player.MaxST = m.MaxST + m.BonusST;
        Player.CurrentHP = m.CurrentHP;
        Player.CurrentMP = m.CurrentMP;
        Player.CurrentST = m.CurrentST;
        Player.MagicalResistance = m.TotalMagicalResistance;
    }

    void MePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        N(e.PropertyName);
        if (e.PropertyName != nameof(Data.Pc.Player.Class)) return;
        N(nameof(ShowRe));
        N(nameof(ShowEdge));
        N(nameof(ShowElements));
        N(nameof(Laurel));
    }
}