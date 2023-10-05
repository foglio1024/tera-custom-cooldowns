using System;
using System.Linq;
using TCC.Data.Skills;
using TCC.ViewModels.ClassManagers;
using TeraPacketParser.Messages;

namespace TCC.Data.Abnormalities;

public class WarriorAbnormalityTracker : AbnormalityTracker
{
    //private static readonly uint[] GambleIDs = { 100800, 100801, 100802, 100803 };
    static readonly uint[] AstanceIDs = { 100100, 100101, 100102, 100103 };
    static readonly uint[] DstanceIDs = { 100200, 100201, 100202, 100203 };
    static readonly uint[] TraverseCutIDs = { 101300/*, 101301*/ };
    static readonly uint[] BladeWaltzIDs = { 104100 };
    static readonly uint[] SwiftGlyphs = { 21010, 21070 };
    public const string DeadlyGambleIconName = "icon_skills.deadlywill_tex";

    readonly Skill _bladeWaltz;

    public WarriorAbnormalityTracker()
    {
        Game.DB!.SkillsDatabase.TryGetSkillByIconName("icon_skills.doublesworddance_tex", Game.Me.Class, out var bw);

        _bladeWaltz = bw ?? throw new NullReferenceException("Skill not found!");

    }

    public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
    {
        if (!Game.IsMe(p.TargetId)) return;
        CheckAssaultStance(p);
        CheckDefensiveStance(p);
        CheckDeadlyGamble(p);
        CheckTraverseCut(p);
        CheckSwiftGlyphs(p);
        CheckArush(p);
        CheckBladeWaltz(p);
    }
    public override void CheckAbnormality(S_ABNORMALITY_REFRESH p)
    {
        if (!Game.IsMe(p.TargetId)) return;
        CheckAssaultStance(p);
        CheckDefensiveStance(p);
        CheckDeadlyGamble(p);
        CheckTraverseCut(p);
        CheckSwiftGlyphs(p);
        CheckArush(p);
        //CheckTempestAura(p);
    }
    public override void CheckAbnormality(S_ABNORMALITY_END p)
    {
        if (!Game.IsMe(p.TargetId)) return;
        CheckTraverseCut(p);
        CheckSwiftGlyphs(p);
        CheckArush(p);
        CheckDefensiveStance(p);
        CheckAssaultStance(p);
        CheckDeadlyGamble(p);
        //CheckTempestAura(p);
    }

    static void CheckAssaultStance(S_ABNORMALITY_BEGIN p)
    {
        if (!AstanceIDs.Contains(p.AbnormalityId)) return;
        Game.Me.WarriorStance.CurrentStance = WarriorStance.Assault;
    }

    static void CheckAssaultStance(S_ABNORMALITY_REFRESH p)
    {
        if (!AstanceIDs.Contains(p.AbnormalityId)) return;
        Game.Me.WarriorStance.CurrentStance = WarriorStance.Assault;
    }

    static void CheckAssaultStance(S_ABNORMALITY_END p)
    {
        if (!AstanceIDs.Contains(p.AbnormalityId)) return;
        Game.Me.WarriorStance.CurrentStance = WarriorStance.None;
    }

    static void CheckDefensiveStance(S_ABNORMALITY_BEGIN p)
    {
        if (!DstanceIDs.Contains(p.AbnormalityId)) return;
        Game.Me.WarriorStance.CurrentStance = WarriorStance.Defensive;
    }

    static void CheckDefensiveStance(S_ABNORMALITY_REFRESH p)
    {
        if (!DstanceIDs.Contains(p.AbnormalityId)) return;
        Game.Me.WarriorStance.CurrentStance = WarriorStance.Defensive;
    }

    static void CheckDefensiveStance(S_ABNORMALITY_END p)
    {
        if (!DstanceIDs.Contains(p.AbnormalityId)) return;
        Game.Me.WarriorStance.CurrentStance = WarriorStance.None;
    }

    static void CheckDeadlyGamble(S_ABNORMALITY_BEGIN p)
    {
        if (p.AbnormalityId != 100801) return;
        //if (!CheckByIconName(p.AbnormalityId, DeadlyGambleIconName)) return; //temporary
        if (!IsViewModelAvailable<WarriorLayoutViewModel>(out var vm)) return;

        vm.DeadlyGamble.StartEffect(p.Duration);
    }

    static void CheckDeadlyGamble(S_ABNORMALITY_REFRESH p)
    {
        if (p.AbnormalityId != 100801) return;
        if (!IsViewModelAvailable<WarriorLayoutViewModel>(out var vm)) return;
        //if (!GambleIDs.Contains(p.AbnormalityId)) return;
        //if (!CheckByIconName(p.AbnormalityId, DeadlyGambleIconName)) return; //temporary
        vm.DeadlyGamble.RefreshEffect(p.Duration);
    }

    static void CheckDeadlyGamble(S_ABNORMALITY_END p)
    {
        if (p.AbnormalityId != 100801) return;
        if (!IsViewModelAvailable<WarriorLayoutViewModel>(out var vm)) return;
        //if (!GambleIDs.Contains(p.AbnormalityId)) return;
        //if (!CheckByIconName(p.AbnormalityId, DeadlyGambleIconName)) return; //temporary
        vm.DeadlyGamble.StopEffect();
    }

    void CheckBladeWaltz(S_ABNORMALITY_BEGIN p)
    {
        if (!BladeWaltzIDs.Contains(p.AbnormalityId)) return;
        StartPrecooldown(_bladeWaltz, p.Duration);
    }

    static void CheckTraverseCut(S_ABNORMALITY_BEGIN p)
    {
        if (!TraverseCutIDs.Contains(p.AbnormalityId)) return;
        if (!IsViewModelAvailable<WarriorLayoutViewModel>(out var vm)) return;
        vm.TraverseCut.Val = p.Stacks;
        vm.TraverseCut.InvokeToZero(p.Duration);
    }

    static void CheckTraverseCut(S_ABNORMALITY_REFRESH p)
    {
        if (!TraverseCutIDs.Contains(p.AbnormalityId)) return;
        if (!IsViewModelAvailable<WarriorLayoutViewModel>(out var vm)) return;
        vm.TraverseCut.Val = p.Stacks;
        vm.TraverseCut.InvokeToZero(p.Duration);
    }

    static void CheckTraverseCut(S_ABNORMALITY_END p)
    {
        if (!TraverseCutIDs.Contains(p.AbnormalityId)) return;
        if (!IsViewModelAvailable<WarriorLayoutViewModel>(out var vm)) return;
        vm.TraverseCut.Val = 0;
    }

    static void CheckSwiftGlyphs(S_ABNORMALITY_BEGIN p)
    {
        if (!SwiftGlyphs.Contains(p.AbnormalityId)) return;
        if (!IsViewModelAvailable<WarriorLayoutViewModel>(out var vm)) return;
        vm.SetSwift(p.Duration);
    }

    static void CheckSwiftGlyphs(S_ABNORMALITY_REFRESH p)
    {
        if (!SwiftGlyphs.Contains(p.AbnormalityId)) return;
        if (!IsViewModelAvailable<WarriorLayoutViewModel>(out var vm)) return;
        vm.SetSwift(p.Duration);
    }

    static void CheckSwiftGlyphs(S_ABNORMALITY_END p)
    {
        if (!SwiftGlyphs.Contains(p.AbnormalityId)) return;
        if (!IsViewModelAvailable<WarriorLayoutViewModel>(out var vm)) return;
        vm.SetSwift(0);
    }

    static void CheckArush(S_ABNORMALITY_BEGIN p)
    {
        if (!CheckByIconName(p.AbnormalityId, LancerAbnormalityTracker.AdrenalineRushIconName)) return; //temporary
        if (!IsViewModelAvailable<WarriorLayoutViewModel>(out var vm)) return;
        vm.SetArush(p.Duration);
    }

    static void CheckArush(S_ABNORMALITY_REFRESH p)
    {
        if (!CheckByIconName(p.AbnormalityId, LancerAbnormalityTracker.AdrenalineRushIconName)) return; //temporary
        if (!IsViewModelAvailable<WarriorLayoutViewModel>(out var vm)) return;
        vm.SetArush(p.Duration);
    }

    static void CheckArush(S_ABNORMALITY_END p)
    {
        if (!CheckByIconName(p.AbnormalityId, LancerAbnormalityTracker.AdrenalineRushIconName)) return; //temporary
        if (!IsViewModelAvailable<WarriorLayoutViewModel>(out var vm)) return;
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
