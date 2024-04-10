using System;
using TCC.Data.Skills;
using TCC.ViewModels.ClassManagers;
using TeraPacketParser.Messages;

namespace TCC.Data.Abnormalities;

public class ReaperAbnormalityTracker : AbnormalityTracker
{
    private const int ShadowReapingId = 10151010;
    private const int ShadowStepId = 10151000;
    private const int DeathSpiralId = 10151131;
    private const int AssassinateId = 10151192;
    private const int PowerlinkedDeathSpiralId = 29112;
    private const int PowerlinkedDoubleShearId = 29020;

    private readonly Skill _shadowStep;
    private readonly Skill _deathSpiral;

    public ReaperAbnormalityTracker()
    {
        Game.DB!.SkillsDatabase.TryGetSkillByIconName("icon_skills.chainbrandish_tex", Game.Me.Class, out var ds);
        Game.DB.SkillsDatabase.TryGetSkillByIconName("icon_skills.instantleap_tex", Game.Me.Class, out var ss);

        _deathSpiral = ds ?? throw new NullReferenceException("Skill not found!");
        _shadowStep = ss ?? throw new NullReferenceException("Skill not found!");
    }

    public override void OnAbnormalityBegin(S_ABNORMALITY_BEGIN p)
    {
        if (!Game.IsMe(p.TargetId)) return;

        CheckShadowReapingBegin(p);
        CheckShadowStepBegin(p);
        CheckDeathSpiralBegin(p);
        CheckAssassinateBegin(p);
        PowerlinkedDeathSpiralBegin(p);
        PowerlinkedDoubleShearBegin(p);
    }

    public override void OnAbnormalityRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (!Game.IsMe(p.TargetId)) return;

        CheckShadowReapingRefresh(p);
        CheckAssassinateRefresh(p);
        PowerlinkedDeathSpiralRefresh(p);
        PowerlinkedDoubleShearRefresh(p);
    }

    public override void OnAbnormalityEnd(S_ABNORMALITY_END p)
    {
        if (!Game.IsMe(p.TargetId)) return;

        CheckShadowReapingEnd(p);
        CheckAssassinateEnd(p);
        PowerlinkedDeathSpiralEnd(p);
        PowerlinkedDoubleShearEnd(p);
    }

    private void CheckDeathSpiralBegin(S_ABNORMALITY_BEGIN p)
    {
        if (p.AbnormalityId != DeathSpiralId) return;

        StartPrecooldown(_deathSpiral, p.Duration);
    }

    private void CheckShadowStepBegin(S_ABNORMALITY_BEGIN p)
    {
        if (p.AbnormalityId != ShadowStepId) return;

        StartPrecooldown(_shadowStep, p.Duration);
    }

    private static void PowerlinkedDoubleShearBegin(S_ABNORMALITY_BEGIN p)
    {
        if (p.AbnormalityId != PowerlinkedDoubleShearId) return;
        if (!TryGetClassViewModel<ReaperLayoutViewModel>(out var vm)) return;

        vm.PowerlinkedDoubleShear.RefreshEffect(p.Duration);
    }

    private static void PowerlinkedDoubleShearRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (p.AbnormalityId != PowerlinkedDoubleShearId) return;
        if (!TryGetClassViewModel<ReaperLayoutViewModel>(out var vm)) return;

        vm.PowerlinkedDoubleShear.RefreshEffect(p.Duration);
    }

    private static void PowerlinkedDoubleShearEnd(S_ABNORMALITY_END p)
    {
        if (PowerlinkedDoubleShearId != p.AbnormalityId) return;
        if (!TryGetClassViewModel<ReaperLayoutViewModel>(out var vm)) return;

        vm.PowerlinkedDoubleShear.StopEffect();
    }

    private static void PowerlinkedDeathSpiralBegin(S_ABNORMALITY_BEGIN p)
    {
        if (p.AbnormalityId != PowerlinkedDeathSpiralId) return;
        if (!TryGetClassViewModel<ReaperLayoutViewModel>(out var vm)) return;

        vm.PowerlinkedDeathSpiral.RefreshEffect(p.Duration);
    }

    private static void PowerlinkedDeathSpiralRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (p.AbnormalityId != PowerlinkedDeathSpiralId) return;
        if (!TryGetClassViewModel<ReaperLayoutViewModel>(out var vm)) return;

        vm.PowerlinkedDeathSpiral.RefreshEffect(p.Duration);
    }

    private static void PowerlinkedDeathSpiralEnd(S_ABNORMALITY_END p)
    {
        if (PowerlinkedDeathSpiralId != p.AbnormalityId) return;
        if (!TryGetClassViewModel<ReaperLayoutViewModel>(out var vm)) return;

        vm.PowerlinkedDeathSpiral.StopEffect();
    }

    private static void CheckAssassinateBegin(S_ABNORMALITY_BEGIN p)
    {
        if (AssassinateId != p.AbnormalityId) return;
        if (!TryGetClassViewModel<ReaperLayoutViewModel>(out var vm)) return;

        vm.ShroudedEscape.StartEffect(p.Duration);
    }

    private static void CheckAssassinateRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (AssassinateId != p.AbnormalityId) return;
        if (!TryGetClassViewModel<ReaperLayoutViewModel>(out var vm)) return;

        vm.ShroudedEscape.RefreshEffect(p.Duration);
    }

    private static void CheckAssassinateEnd(S_ABNORMALITY_END p)
    {
        if (AssassinateId != p.AbnormalityId) return;
        if (!TryGetClassViewModel<ReaperLayoutViewModel>(out var vm)) return;

        vm.ShroudedEscape.StopEffect();
    }

    private static void CheckShadowReapingBegin(S_ABNORMALITY_BEGIN p)
    {
        if (ShadowReapingId != p.AbnormalityId) return;
        if (!TryGetClassViewModel<ReaperLayoutViewModel>(out var vm)) return;

        vm.ShadowReaping.StartEffect(p.Duration);
    }

    private static void CheckShadowReapingRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (ShadowReapingId != p.AbnormalityId) return;
        if (!TryGetClassViewModel<ReaperLayoutViewModel>(out var vm)) return;

        vm.ShadowReaping.RefreshEffect(p.Duration);
    }

    private static void CheckShadowReapingEnd(S_ABNORMALITY_END p)
    {
        if (ShadowReapingId != p.AbnormalityId) return;
        if (!TryGetClassViewModel<ReaperLayoutViewModel>(out var vm)) return;

        vm.ShadowReaping.StopEffect();
    }
}