//#define SCYTHES

using System.ComponentModel;
using TCC.Data;
using TCC.Data.Skills;
using TeraDataLite;

namespace TCC.ViewModels.ClassManagers;

public class WarriorLayoutViewModel : BaseClassLayoutViewModel
{

    public SkillWithEffect DeadlyGamble { get; set; }
    public SkillWithEffect AdrenalineRush { get; set; }
    public SkillWithEffect Swift { get; set; }
    public StatTracker TraverseCut { get; set; }
    public Cooldown Infuriate { get; }

#if SCYTHES
    public Cooldown Scythe { get; }
    public Cooldown AerialScythe { get; }
#endif

    public StanceTracker<WarriorStance> Stance => Game.Me.WarriorStance; //for binding
    public Counter EdgeCounter => Game.Me.StacksCounter; //for binding

    bool _warningStance;
    public bool WarningStance
    {
        get => _warningStance;
        set
        {
            if (_warningStance == value) return;
            _warningStance = value;
            N();
        }
    }

    public bool AtkSpeedProc => !(Swift.Effect.IsAvailable && AdrenalineRush.Effect.IsAvailable);

    public WarriorLayoutViewModel()
    {
        TraverseCut = new StatTracker { Max = 13, Val = 0 };
        Game.Me.Death += OnDeath;
        Game.CombatChanged += CheckStanceWarning;
        Stance.PropertyChanged += OnStanceOnPropertyChanged; // StanceTracker has only one prop

        Game.DB!.SkillsDatabase.TryGetSkill(200200, Class.Warrior, out var dg);
        DeadlyGamble = new SkillWithEffect(_dispatcher, dg);

        Game.DB.SkillsDatabase.TryGetSkill(170250, Class.Lancer, out var ar);
        AdrenalineRush = new SkillWithEffect(_dispatcher, ar, false);

        Game.DB.SkillsDatabase.TryGetSkill(350100, Class.Warrior, out var infu);
        Infuriate = new Cooldown(infu, true) { CanFlash = true };
#if SCYTHES
        Game.DB.SkillsDatabase.TryGetSkill(380130, Class.Warrior, out var scythe);
        Scythe = new Cooldown(scythe, true);

        Game.DB.SkillsDatabase.TryGetSkill(410100, Class.Warrior, out var ascythe);
        AerialScythe = new Cooldown(ascythe, true);
#endif
        var ab = Game.DB.AbnormalityDatabase.Abnormalities[21010];
        Swift = new SkillWithEffect(_dispatcher, new Skill(ab), false);

    }

    void OnDeath()
    {
        DeadlyGamble.StopEffect();
    }

    public bool ShowEdge => App.Settings.ClassWindowSettings.WarriorShowEdge;
    public bool ShowInfuriate => App.Settings.ClassWindowSettings.WarriorShowInfuriate;
    public bool ShowTraverseCut => App.Settings.ClassWindowSettings.WarriorShowTraverseCut;
    public WarriorEdgeMode WarriorEdgeMode => App.Settings.ClassWindowSettings.WarriorEdgeMode;


    public override void Dispose()
    {
        Game.Me.Death -= OnDeath;
        Game.CombatChanged -= CheckStanceWarning;
        Stance.PropertyChanged -= OnStanceOnPropertyChanged;
        DeadlyGamble.Dispose();
        Infuriate.Dispose();
    }

    void OnStanceOnPropertyChanged(object? _, PropertyChangedEventArgs __)
    {
        CheckStanceWarning();
    }

    public override bool StartSpecialSkill(Cooldown sk)
    {
        if (sk.Skill.IconName == DeadlyGamble.Cooldown.Skill.IconName)
        {
            DeadlyGamble.StartCooldown(sk.Duration);
            return true;
        }

        if (sk.Skill.IconName == Infuriate.Skill.IconName)
        {
            Infuriate.Start(sk.Duration);
            return true;
        }
#if SCYTHES
        if (sk.Skill.IconName == Scythe.Skill.IconName)
        {
            Scythe.Start(sk.Duration);
            return true;
        }

        if (sk.Skill.IconName == AerialScythe.Skill.IconName)
        {
            AerialScythe.Start(sk.Duration);
            return true;
        }
#endif
        return false;
    }

    void CheckStanceWarning()
    {
        WarningStance = Game.Me.WarriorStance.CurrentStance == WarriorStance.None && Game.Combat;
    }

    public void SetSwift(uint duration)
    {
        if (duration == 0)
        {
            Swift.StopEffect();
        }
        else
        {
            Swift.StartEffect(duration);
        }
        N(nameof(AtkSpeedProc));
    }

    public void SetArush(uint duration)
    {
        if (duration == 0)
        {
            AdrenalineRush.StopEffect();
        }
        else
        {
            AdrenalineRush.StartEffect(duration);
        }
        N(nameof(AtkSpeedProc));
    }
}