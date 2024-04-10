using System;
using System.Collections.Generic;
using TCC.Data.Skills;
using TCC.ViewModels.ClassManagers;
using TeraPacketParser.Messages;

namespace TCC.Data.Abnormalities;

public class GunnerAbnormalityTracker : AbnormalityTracker
{
    private const uint DashingReloadId = 10152354;
    private static readonly List<uint> LaserTargetingIDs = [10152340];
    private readonly Skill _dashingReload;
    private readonly Skill _rollingReload;

    public GunnerAbnormalityTracker()
    {
        Game.DB!.SkillsDatabase.TryGetSkillByIconName("icon_skills.airdash_tex", Game.Me.Class, out var dr);
        Game.DB.SkillsDatabase.TryGetSkillByIconName("icon_skills.ambushrolling_tex", Game.Me.Class, out var rr);

        _dashingReload = dr ?? throw new NullReferenceException("Skill not found!");
        _rollingReload = rr ?? throw new NullReferenceException("Skill not found!");
    }

    public override void OnAbnormalityBegin(S_ABNORMALITY_BEGIN p)
    {
        if (!Game.IsMe(p.TargetId)) return;

        CheckDashingReloadBegin(p);
        CheckLaserTargetingBegin(p);
    }

    public override void OnAbnormalityRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (!Game.IsMe(p.TargetId)) return;

        CheckLaserTargetingRefresh(p);
    }

    public override void OnAbnormalityEnd(S_ABNORMALITY_END p)
    {
        if (!Game.IsMe(p.TargetId)) return;

        CheckLaserTargetingEnd(p);
    }

    private void CheckDashingReloadBegin(S_ABNORMALITY_BEGIN p)
    {
        if (p.AbnormalityId != DashingReloadId) return;

        //TODO: choose icon based on gunner's status?
        StartPrecooldown(_dashingReload, p.Duration);
        StartPrecooldown(_rollingReload, p.Duration);
    }

    private static void CheckLaserTargetingBegin(S_ABNORMALITY_BEGIN p)
    {
        if (!LaserTargetingIDs.Contains(p.AbnormalityId)) return;
        if (!TryGetClassViewModel<GunnerLayoutViewModel>(out var vm)) return;

        vm.ModularSystem.StartEffect(p.Duration);
    }

    private static void CheckLaserTargetingRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (!LaserTargetingIDs.Contains(p.AbnormalityId)) return;
        if (!TryGetClassViewModel<GunnerLayoutViewModel>(out var vm)) return;

        vm.ModularSystem.RefreshEffect(p.Duration);
    }

    private static void CheckLaserTargetingEnd(S_ABNORMALITY_END p)
    {
        if (!LaserTargetingIDs.Contains(p.AbnormalityId)) return;
        if (!TryGetClassViewModel<GunnerLayoutViewModel>(out var vm)) return;

        vm.ModularSystem.StopEffect();
    }
}