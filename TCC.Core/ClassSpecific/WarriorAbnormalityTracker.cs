using System.Linq;
using TCC.Data;
using TCC.Data.Skills;
using TCC.ViewModels;
using TeraPacketParser.Messages;

namespace TCC.ClassSpecific
{
    public class WarriorAbnormalityTracker : ClassAbnormalityTracker
    {
        //private static readonly uint[] GambleIDs = { 100800, 100801, 100802, 100803 };
        private static readonly uint[] AstanceIDs = { 100100, 100101, 100102, 100103 };
        private static readonly uint[] DstanceIDs = { 100200, 100201, 100202, 100203 };
        private static readonly uint[] TraverseCutIDs = { 101300/*, 101301*/ };
        private static readonly uint[] BladeWaltzIDs = { 104100 };
        private static readonly uint[] SwiftGlyphs = { 21010, 21070 };
        public const string DeadlyGambleIconName = "icon_skills.deadlywill_tex";

        private Skill _bladeWaltz;

        public WarriorAbnormalityTracker()
        {
            Game.DB.SkillsDatabase.TryGetSkillByIconName("icon_skills.doublesworddance_tex", Game.Me.Class, out _bladeWaltz);
        }

        public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
        {
            if (!Game.IsMe(p.TargetId)) return;
            CheckAssaultStance(p);
            CheckDefensiveStance(p);
            CheckDeadlyGamble(p);
            CheckTraverseCut(p);
            CheckSwiftGlyphs(p);
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
            //CheckTempestAura(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_END p)
        {
            if (!Game.IsMe(p.TargetId)) return;
            CheckTraverseCut(p);
            CheckSwiftGlyphs(p);
            CheckDefensiveStance(p);
            CheckAssaultStance(p);
            CheckDeadlyGamble(p);
            //CheckTempestAura(p);
        }

        private static void CheckAssaultStance(S_ABNORMALITY_BEGIN p)
        {
            if (!AstanceIDs.Contains(p.AbnormalityId)) return;
            TccUtils.CurrentClassVM<WarriorLayoutVM>().Stance.CurrentStance = WarriorStance.Assault;
        }
        private static void CheckAssaultStance(S_ABNORMALITY_REFRESH p)
        {
            if (!AstanceIDs.Contains(p.AbnormalityId)) return;
            TccUtils.CurrentClassVM<WarriorLayoutVM>().Stance.CurrentStance = WarriorStance.Assault;
        }
        private static void CheckAssaultStance(S_ABNORMALITY_END p)
        {
            if (!AstanceIDs.Contains(p.AbnormalityId)) return;
            TccUtils.CurrentClassVM<WarriorLayoutVM>().Stance.CurrentStance = WarriorStance.None;
        }

        private static void CheckDefensiveStance(S_ABNORMALITY_BEGIN p)
        {
            if (!DstanceIDs.Contains(p.AbnormalityId)) return;
            TccUtils.CurrentClassVM<WarriorLayoutVM>().Stance.CurrentStance = WarriorStance.Defensive;
        }
        private static void CheckDefensiveStance(S_ABNORMALITY_REFRESH p)
        {
            if (!DstanceIDs.Contains(p.AbnormalityId)) return;
            TccUtils.CurrentClassVM<WarriorLayoutVM>().Stance.CurrentStance = WarriorStance.Defensive;
        }
        private static void CheckDefensiveStance(S_ABNORMALITY_END p)
        {
            if (!DstanceIDs.Contains(p.AbnormalityId)) return;
            TccUtils.CurrentClassVM<WarriorLayoutVM>().Stance.CurrentStance = WarriorStance.None;
        }

        private static void CheckDeadlyGamble(S_ABNORMALITY_BEGIN p)
        {
            //if (!GambleIDs.Contains(p.AbnormalityId)) return;
            if (!CheckByIconName(p.AbnormalityId, DeadlyGambleIconName)) return; //temporary
            TccUtils.CurrentClassVM<WarriorLayoutVM>().DeadlyGamble.Buff.Start(p.Duration);
        }
        private static void CheckDeadlyGamble(S_ABNORMALITY_REFRESH p)
        {
            //if (!GambleIDs.Contains(p.AbnormalityId)) return;
            if (!CheckByIconName(p.AbnormalityId, DeadlyGambleIconName)) return; //temporary
            TccUtils.CurrentClassVM<WarriorLayoutVM>().DeadlyGamble.Buff.Refresh(p.Duration, CooldownMode.Normal);
        }
        private static void CheckDeadlyGamble(S_ABNORMALITY_END p)
        {
            //if (!GambleIDs.Contains(p.AbnormalityId)) return;
            if (!CheckByIconName(p.AbnormalityId, DeadlyGambleIconName)) return; //temporary
            TccUtils.CurrentClassVM<WarriorLayoutVM>().DeadlyGamble.Buff.Refresh(0, CooldownMode.Normal);
        }

        private void CheckBladeWaltz(S_ABNORMALITY_BEGIN p)
        {
            if (!BladeWaltzIDs.Contains(p.AbnormalityId)) return;
            StartPrecooldown(_bladeWaltz, p.Duration);
        }

        private static void CheckTraverseCut(S_ABNORMALITY_BEGIN p)
        {
            if (!TraverseCutIDs.Contains(p.AbnormalityId)) return;
            TccUtils.CurrentClassVM<WarriorLayoutVM>().TraverseCut.Val = p.Stacks;
            TccUtils.CurrentClassVM<WarriorLayoutVM>().TraverseCut.InvokeToZero(p.Duration);
        }
        private static void CheckTraverseCut(S_ABNORMALITY_REFRESH p)
        {
            if (!TraverseCutIDs.Contains(p.AbnormalityId)) return;
            TccUtils.CurrentClassVM<WarriorLayoutVM>().TraverseCut.Val = p.Stacks;
            TccUtils.CurrentClassVM<WarriorLayoutVM>().TraverseCut.InvokeToZero(p.Duration);
        }
        private static void CheckTraverseCut(S_ABNORMALITY_END p)
        {
            if (!TraverseCutIDs.Contains(p.AbnormalityId)) return;
            TccUtils.CurrentClassVM<WarriorLayoutVM>().TraverseCut.Val = 0;
        }

        private static void CheckSwiftGlyphs(S_ABNORMALITY_BEGIN p)
        {
            if (!SwiftGlyphs.Contains(p.AbnormalityId)) return;
            TccUtils.CurrentClassVM<WarriorLayoutVM>().Swift.Start(p.Duration);
            TccUtils.CurrentClassVM<WarriorLayoutVM>().SwiftProc = true;
        }
        private static void CheckSwiftGlyphs(S_ABNORMALITY_REFRESH p)
        {
            if (!SwiftGlyphs.Contains(p.AbnormalityId)) return;
            TccUtils.CurrentClassVM<WarriorLayoutVM>().Swift.Start(p.Duration);
            TccUtils.CurrentClassVM<WarriorLayoutVM>().SwiftProc = true;
        }
        private static void CheckSwiftGlyphs(S_ABNORMALITY_END p)
        {
            if (!SwiftGlyphs.Contains(p.AbnormalityId)) return;
            TccUtils.CurrentClassVM<WarriorLayoutVM>().Swift.Refresh(0, CooldownMode.Normal);
            TccUtils.CurrentClassVM<WarriorLayoutVM>().SwiftProc = false;
        }
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
