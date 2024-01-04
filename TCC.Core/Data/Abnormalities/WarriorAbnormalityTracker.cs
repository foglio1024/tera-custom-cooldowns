using System;
using System.Linq;
using TCC.Data.Skills;
using TCC.ViewModels.ClassManagers;
using TeraPacketParser.Messages;

namespace TCC.Data.Abnormalities;

public class WarriorAbnormalityTracker : AbnormalityTracker
{
    //private static readonly uint[] GambleIDs = { 100800, 100801, 100802, 100803 };
    const uint GambleID = 100801;
    static readonly uint[] AstanceIDs = [100100, 100101, 100102, 100103];
    static readonly uint[] DstanceIDs = [100200, 100201, 100202, 100203];
    static readonly uint[] TraverseCutIDs = [101300/*, 101301*/];
    static readonly uint[] BladeWaltzIDs = [104100];
    static readonly uint[] SwiftGlyphs = [21010, 21070];

    readonly Skill _bladeWaltz;

    public WarriorAbnormalityTracker()
    {
        Game.DB!.SkillsDatabase.TryGetSkillByIconName("icon_skills.doublesworddance_tex", Game.Me.Class, out var bw);
        _bladeWaltz = bw ?? throw new NullReferenceException("Skill not found!");
    }

    public override void OnAbnormalityBegin(S_ABNORMALITY_BEGIN p)
    {
        if (!Game.IsMe(p.TargetId)) return;

        CheckAssaultStanceBegin(p);
        CheckDefensiveStanceBegin(p);
        CheckDeadlyGambleBegin(p);
        CheckTraverseCutBegin(p);
        CheckSwiftGlyphsBegin(p);
        CheckArushBegin(p);
        CheckBladeWaltzBegin(p);
    }

    public override void OnAbnormalityRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (!Game.IsMe(p.TargetId)) return;

        CheckAssaultStanceRefresh(p);
        CheckDefensiveStanceRefresh(p);
        CheckDeadlyGambleRefresh(p);
        CheckTraverseCutRefresh(p);
        CheckSwiftGlyphsRefresh(p);
        CheckArushRefresh(p);
        //CheckTempestAura(p);
    }

    public override void OnAbnormalityEnd(S_ABNORMALITY_END p)
    {
        if (!Game.IsMe(p.TargetId)) return;

        CheckTraverseCutEnd(p);
        CheckSwiftGlyphsEnd(p);
        CheckArushEnd(p);
        CheckDefensiveStanceEnd(p);
        CheckAssaultStanceEnd(p);
        CheckDeadlyGambleEnd(p);
        //CheckTempestAura(p);
    }

    static void CheckAssaultStanceBegin(S_ABNORMALITY_BEGIN p)
    {
        if (!AstanceIDs.Contains(p.AbnormalityId)) return;

        Game.Me.WarriorStance.CurrentStance = WarriorStance.Assault;
    }

    static void CheckAssaultStanceRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (!AstanceIDs.Contains(p.AbnormalityId)) return;

        Game.Me.WarriorStance.CurrentStance = WarriorStance.Assault;
    }

    static void CheckAssaultStanceEnd(S_ABNORMALITY_END p)
    {
        if (!AstanceIDs.Contains(p.AbnormalityId)) return;

        Game.Me.WarriorStance.CurrentStance = WarriorStance.None;
    }

    static void CheckDefensiveStanceBegin(S_ABNORMALITY_BEGIN p)
    {
        if (!DstanceIDs.Contains(p.AbnormalityId)) return;

        Game.Me.WarriorStance.CurrentStance = WarriorStance.Defensive;
    }

    static void CheckDefensiveStanceRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (!DstanceIDs.Contains(p.AbnormalityId)) return;

        Game.Me.WarriorStance.CurrentStance = WarriorStance.Defensive;
    }

    static void CheckDefensiveStanceEnd(S_ABNORMALITY_END p)
    {
        if (!DstanceIDs.Contains(p.AbnormalityId)) return;

        Game.Me.WarriorStance.CurrentStance = WarriorStance.None;
    }

    static void CheckDeadlyGambleBegin(S_ABNORMALITY_BEGIN p)
    {
        if (p.AbnormalityId != GambleID) return;
        //if (!CheckByIconName(p.AbnormalityId, DeadlyGambleIconName)) return; //temporary
        if (!TryGetClassViewModel<WarriorLayoutViewModel>(out var vm)) return;

        vm.DeadlyGamble.StartEffect(p.Duration);
    }

    static void CheckDeadlyGambleRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (p.AbnormalityId != GambleID) return;
        if (!TryGetClassViewModel<WarriorLayoutViewModel>(out var vm)) return;
        //if (!GambleIDs.Contains(p.AbnormalityId)) return;
        //if (!CheckByIconName(p.AbnormalityId, DeadlyGambleIconName)) return; //temporary

        vm.DeadlyGamble.RefreshEffect(p.Duration);
    }

    static void CheckDeadlyGambleEnd(S_ABNORMALITY_END p)
    {
        if (p.AbnormalityId != GambleID) return;
        if (!TryGetClassViewModel<WarriorLayoutViewModel>(out var vm)) return;
        //if (!GambleIDs.Contains(p.AbnormalityId)) return;
        //if (!CheckByIconName(p.AbnormalityId, DeadlyGambleIconName)) return; //temporary

        vm.DeadlyGamble.StopEffect();
    }

    void CheckBladeWaltzBegin(S_ABNORMALITY_BEGIN p)
    {
        if (!BladeWaltzIDs.Contains(p.AbnormalityId)) return;

        StartPrecooldown(_bladeWaltz, p.Duration);
    }

    static void CheckTraverseCutBegin(S_ABNORMALITY_BEGIN p)
    {
        if (!TraverseCutIDs.Contains(p.AbnormalityId)) return;
        if (!TryGetClassViewModel<WarriorLayoutViewModel>(out var vm)) return;

        vm.TraverseCut.Val = p.Stacks;
        vm.TraverseCut.InvokeToZero(p.Duration);
    }

    static void CheckTraverseCutRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (!TraverseCutIDs.Contains(p.AbnormalityId)) return;
        if (!TryGetClassViewModel<WarriorLayoutViewModel>(out var vm)) return;

        vm.TraverseCut.Val = p.Stacks;
        vm.TraverseCut.InvokeToZero(p.Duration);
    }

    static void CheckTraverseCutEnd(S_ABNORMALITY_END p)
    {
        if (!TraverseCutIDs.Contains(p.AbnormalityId)) return;
        if (!TryGetClassViewModel<WarriorLayoutViewModel>(out var vm)) return;

        vm.TraverseCut.Val = 0;
    }

    static void CheckSwiftGlyphsBegin(S_ABNORMALITY_BEGIN p)
    {
        if (!SwiftGlyphs.Contains(p.AbnormalityId)) return;
        if (!TryGetClassViewModel<WarriorLayoutViewModel>(out var vm)) return;

        vm.SetSwift(p.Duration);
    }

    static void CheckSwiftGlyphsRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (!SwiftGlyphs.Contains(p.AbnormalityId)) return;
        if (!TryGetClassViewModel<WarriorLayoutViewModel>(out var vm)) return;

        vm.SetSwift(p.Duration);
    }

    static void CheckSwiftGlyphsEnd(S_ABNORMALITY_END p)
    {
        if (!SwiftGlyphs.Contains(p.AbnormalityId)) return;
        if (!TryGetClassViewModel<WarriorLayoutViewModel>(out var vm)) return;

        vm.SetSwift(0);
    }

    static void CheckArushBegin(S_ABNORMALITY_BEGIN p)
    {
        if (!CheckByIconName(p.AbnormalityId, LancerAbnormalityTracker.AdrenalineRushIconName)) return; //temporary
        if (!TryGetClassViewModel<WarriorLayoutViewModel>(out var vm)) return;

        vm.SetArush(p.Duration);
    }

    static void CheckArushRefresh(S_ABNORMALITY_REFRESH p)
    {
        if (!CheckByIconName(p.AbnormalityId, LancerAbnormalityTracker.AdrenalineRushIconName)) return; //temporary
        if (!TryGetClassViewModel<WarriorLayoutViewModel>(out var vm)) return;

        vm.SetArush(p.Duration);
    }

    static void CheckArushEnd(S_ABNORMALITY_END p)
    {
        if (!CheckByIconName(p.AbnormalityId, LancerAbnormalityTracker.AdrenalineRushIconName)) return; //temporary
        if (!TryGetClassViewModel<WarriorLayoutViewModel>(out var vm)) return;

        vm.SetArush(0);
    }
}

/*
        private static readonly uint[] TempestAuraIDs = { 103000, 103102, 103120, 103131 };
        private static readonly uint[] ShadowTempestIDs = { 103104, 103130 };

        private static void CheckTempestAura(S_ABNORMALITY_BEGIN p)
        {
            if (!TempestAuraIDs.Contains(p.AbnormalityId)) return;
            Utils.CurrentClassVM<WarriorLayoutVM>().TempestAura.Val = p.Stacks;
        }
        private static void CheckTempestAura(S_ABNORMALITY_REFRESH p)
        {
            if (!TempestAuraIDs.Contains(p.AbnormalityId)) return;
            Utils.CurrentClassVM<WarriorLayoutVM>().TempestAura.Val = p.Stacks;
        }
        private static void CheckTempestAura(S_ABNORMALITY_END p)
        {
            if (!TempestAuraIDs.Contains(p.AbnormalityId)) return;
            Utils.CurrentClassVM<WarriorLayoutVM>().TempestAura.Val = 0;
        }
        private static void CheckShadowTempest(S_ABNORMALITY_BEGIN p)
        {
            if (!ShadowTempestIDs.Contains(p.AbnormalityId)) return;
            Utils.CurrentClassVM<WarriorLayoutVM>().TempestAura.ToZero(p.Duration);
        }
*/