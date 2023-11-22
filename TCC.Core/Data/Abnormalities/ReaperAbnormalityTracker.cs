using System;
using TCC.Data.Skills;
using TCC.ViewModels.ClassManagers;
using TeraPacketParser.Messages;

namespace TCC.Data.Abnormalities;

public class ReaperAbnormalityTracker : AbnormalityTracker
{
    const int ShadowReapingId = 10151010;
    const int ShadowStepId = 10151000;
    const int DeathSpiralId = 10151131;
    const int AssassinateId = 10151192;
    const int PowerlinkedDeathSpiralId = 29112;
    const int PowerlinkedDoubleShearId = 29020;

    readonly Skill _shadowStep;
    readonly Skill _deathSpiral;

    public ReaperAbnormalityTracker()
    {
        Game.DB!.SkillsDatabase.TryGetSkillByIconName("icon_skills.chainbrandish_tex", Game.Me.Class, out var ds);
        Game.DB.SkillsDatabase.TryGetSkillByIconName("icon_skills.instantleap_tex", Game.Me.Class, out var ss);

        _deathSpiral = ds ?? throw new NullReferenceException("Skill not found!");
        _shadowStep = ss ?? throw new NullReferenceException("Skill not found!");
    }

    public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
    {
        if (!Game.IsMe(p.TargetId)) return;
        CheckShadowReaping(p);
        CheckShadowStep(p);
        CheckDeathSpiral(p);
        CheckAssassinate(p);
        PowerlinkedDeathSpiralBegin(p);
        PowerlinkedDoubleShearBegin(p);
    }
    public override void CheckAbnormality(S_ABNORMALITY_REFRESH p)
    {
        if (!Game.IsMe(p.TargetId)) return;
        CheckShadowReaping(p);
        CheckAssassinate(p);
        PowerlinkedDeathSpiralRefresh(p);
        PowerlinkedDoubleShearRefresh(p);

    }
    public override void CheckAbnormality(S_ABNORMALITY_END p)
    {
        if (!Game.IsMe(p.TargetId)) return;
        CheckShadowReaping(p);
        CheckAssassinate(p);
        PowerlinkedDeathSpiralEnd(p);
        PowerlinkedDoubleShearEnd(p);

    }

    void CheckDeathSpiral(S_ABNORMALITY_BEGIN p)
    {
        if (p.AbnormalityId != DeathSpiralId) return;
        StartPrecooldown(_deathSpiral, p.Duration);
    }

    void CheckShadowStep(S_ABNORMALITY_BEGIN p)
    {
        if (p.AbnormalityId != ShadowStepId) return;
        StartPrecooldown(_shadowStep, p.Duration);
    }

    static void PowerlinkedDoubleShearBegin(S_ABNORMALITY_BEGIN p)
    {
        if (p.AbnormalityId != PowerlinkedDoubleShearId) return;
        if (!IsViewModelAvailable<ReaperLayoutViewModel>(out var vm)) return;
        vm.PowerlinkedDoubleShear.RefreshEffect(p.Duration);
    }
    static void PowerlinkedDoubleShearRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (p.AbnormalityId != PowerlinkedDoubleShearId) return;
        if (!IsViewModelAvailable<ReaperLayoutViewModel>(out var vm)) return;
        vm.PowerlinkedDoubleShear.RefreshEffect(p.Duration);
    }
    static void PowerlinkedDoubleShearEnd(S_ABNORMALITY_END p)
    {
        if (PowerlinkedDoubleShearId != p.AbnormalityId) return;
        if (!IsViewModelAvailable<ReaperLayoutViewModel>(out var vm)) return;

        vm.PowerlinkedDoubleShear.StopEffect();
    }
    
    static void PowerlinkedDeathSpiralBegin(S_ABNORMALITY_BEGIN p)
    {
        if (p.AbnormalityId != PowerlinkedDeathSpiralId) return;
        if (!IsViewModelAvailable<ReaperLayoutViewModel>(out var vm)) return;
        vm.PowerlinkedDeathSpiral.RefreshEffect(p.Duration);
    }
    static void PowerlinkedDeathSpiralRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (p.AbnormalityId != PowerlinkedDeathSpiralId) return;
        if (!IsViewModelAvailable<ReaperLayoutViewModel>(out var vm)) return;
        vm.PowerlinkedDeathSpiral.RefreshEffect(p.Duration);
    }
    static void PowerlinkedDeathSpiralEnd(S_ABNORMALITY_END p)
    {
        if (PowerlinkedDeathSpiralId != p.AbnormalityId) return;
        if (!IsViewModelAvailable<ReaperLayoutViewModel>(out var vm)) return;

        vm.PowerlinkedDeathSpiral.StopEffect();
    }

    static void CheckAssassinate(S_ABNORMALITY_BEGIN p)
    {
        if (AssassinateId != p.AbnormalityId) return;
        if (!IsViewModelAvailable<ReaperLayoutViewModel>(out var vm)) return;

        vm.ShroudedEscape.StartEffect(p.Duration);
    }
    static void CheckAssassinate(S_ABNORMALITY_REFRESH p)
    {
        if (AssassinateId != p.AbnormalityId) return;
        if (!IsViewModelAvailable<ReaperLayoutViewModel>(out var vm)) return;

        vm.ShroudedEscape.RefreshEffect(p.Duration);
    }
    static void CheckAssassinate(S_ABNORMALITY_END p)
    {
        if (AssassinateId != p.AbnormalityId) return;
        if (!IsViewModelAvailable<ReaperLayoutViewModel>(out var vm)) return;

        vm.ShroudedEscape.StopEffect();
    }

    static void CheckShadowReaping(S_ABNORMALITY_BEGIN p)
    {
        if (ShadowReapingId != p.AbnormalityId) return;
        if (!IsViewModelAvailable<ReaperLayoutViewModel>(out var vm)) return;

        vm.ShadowReaping.StartEffect(p.Duration);
    }
    static void CheckShadowReaping(S_ABNORMALITY_REFRESH p)
    {
        if (ShadowReapingId != p.AbnormalityId) return;
        if (!IsViewModelAvailable<ReaperLayoutViewModel>(out var vm)) return;

        vm.ShadowReaping.RefreshEffect(p.Duration);
    }
    static void CheckShadowReaping(S_ABNORMALITY_END p)
    {
        if (ShadowReapingId != p.AbnormalityId) return;
        if (!IsViewModelAvailable<ReaperLayoutViewModel>(out var vm)) return;

        vm.ShadowReaping.StopEffect();
    }

}