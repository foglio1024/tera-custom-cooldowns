using System.Linq;
using TCC.Data;
using TCC.Data.Skills;
using TCC.Parsing.Messages;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public class WarriorAbnormalityTracker : ClassAbnormalityTracker
    {
        private static readonly uint[] GambleIDs = { 100800, 100801, 100802, 100803 };
        private static readonly uint[] AstanceIDs = { 100100, 100101, 100102, 100103 };
        private static readonly uint[] DstanceIDs = { 100200, 100201, 100202, 100203 };
        private static readonly uint[] TraverseCutIDs = { 101300/*, 101301*/ };
        private static readonly uint[] BladeWaltzIDs = { 104100 };
        private static readonly uint[] SwiftGlyphs = { 21010, 21070 };

        private Skill _bladeWaltz;

        public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
        {
            if (!p.TargetId.IsMe()) return;
            CheckAssaultStance(p);
            CheckDefensiveStance(p);
            CheckDeadlyGamble(p);
            CheckTraverseCut(p);
            CheckSwiftGlyphs(p);
            CheckBladeWaltz(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_REFRESH p)
        {
            if (!p.TargetId.IsMe()) return;
            CheckAssaultStance(p);
            CheckDefensiveStance(p);
            CheckDeadlyGamble(p);
            CheckTraverseCut(p);
            CheckSwiftGlyphs(p);
            //CheckTempestAura(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_END p)
        {
            if (!p.TargetId.IsMe()) return;
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
            ((WarriorLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).Stance.CurrentStance = WarriorStance.Assault;
        }
        private static void CheckAssaultStance(S_ABNORMALITY_REFRESH p)
        {
            if (!AstanceIDs.Contains(p.AbnormalityId)) return;
            ((WarriorLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).Stance.CurrentStance = WarriorStance.Assault;
        }
        private static void CheckAssaultStance(S_ABNORMALITY_END p)
        {
            if (!AstanceIDs.Contains(p.AbnormalityId)) return;
            ((WarriorLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).Stance.CurrentStance = WarriorStance.None;
        }

        private static void CheckDefensiveStance(S_ABNORMALITY_BEGIN p)
        {
            if (!DstanceIDs.Contains(p.AbnormalityId)) return;
            ((WarriorLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).Stance.CurrentStance = WarriorStance.Defensive;
        }
        private static void CheckDefensiveStance(S_ABNORMALITY_REFRESH p)
        {
            if (!DstanceIDs.Contains(p.AbnormalityId)) return;
            ((WarriorLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).Stance.CurrentStance = WarriorStance.Defensive;
        }
        private static void CheckDefensiveStance(S_ABNORMALITY_END p)
        {
            if (!DstanceIDs.Contains(p.AbnormalityId)) return;
            ((WarriorLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).Stance.CurrentStance = WarriorStance.None;
        }

        private static void CheckDeadlyGamble(S_ABNORMALITY_BEGIN p)
        {
            if (!GambleIDs.Contains(p.AbnormalityId)) return;
            ((WarriorLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).DeadlyGamble.Buff.Start(p.Duration);
        }
        private static void CheckDeadlyGamble(S_ABNORMALITY_REFRESH p)
        {
            if (!GambleIDs.Contains(p.AbnormalityId)) return;
            ((WarriorLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).DeadlyGamble.Buff.Refresh(p.Duration, CooldownMode.Normal);
        }
        private static void CheckDeadlyGamble(S_ABNORMALITY_END p)
        {
            if (!GambleIDs.Contains(p.AbnormalityId)) return;
            ((WarriorLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).DeadlyGamble.Buff.Refresh(0, CooldownMode.Normal);
        }

        private void CheckBladeWaltz(S_ABNORMALITY_BEGIN p)
        {
            if (!BladeWaltzIDs.Contains(p.AbnormalityId)) return;
            StartPrecooldown(_bladeWaltz, p.Duration);
        }

        public WarriorAbnormalityTracker()
        {
            SessionManager.DB.SkillsDatabase.TryGetSkillByIconName("icon_skills.doublesworddance_tex", SessionManager.CurrentPlayer.Class, out _bladeWaltz);

        }
        private static void CheckTraverseCut(S_ABNORMALITY_BEGIN p)
        {
            if (!TraverseCutIDs.Contains(p.AbnormalityId)) return;
            ((WarriorLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).TraverseCut.Val = p.Stacks;
            ((WarriorLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).TraverseCut.InvokeToZero(p.Duration);
        }
        private static void CheckTraverseCut(S_ABNORMALITY_REFRESH p)
        {
            if (!TraverseCutIDs.Contains(p.AbnormalityId)) return;
            ((WarriorLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).TraverseCut.Val = p.Stacks;
            ((WarriorLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).TraverseCut.InvokeToZero(p.Duration);
        }
        private static void CheckTraverseCut(S_ABNORMALITY_END p)
        {
            if (!TraverseCutIDs.Contains(p.AbnormalityId)) return;
            ((WarriorLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).TraverseCut.Val = 0;
        }

        private static void CheckSwiftGlyphs(S_ABNORMALITY_BEGIN p)
        {
            if (!SwiftGlyphs.Contains(p.AbnormalityId)) return;
            ((WarriorLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).Swift.Start(p.Duration);
            ((WarriorLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).SwiftProc = true;
        }
        private static void CheckSwiftGlyphs(S_ABNORMALITY_REFRESH p)
        {
            if (!SwiftGlyphs.Contains(p.AbnormalityId)) return;
            ((WarriorLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).Swift.Start(p.Duration);
            ((WarriorLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).SwiftProc = true;
        }
        private static void CheckSwiftGlyphs(S_ABNORMALITY_END p)
        {
            if (!SwiftGlyphs.Contains(p.AbnormalityId)) return;
            ((WarriorLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).Swift.Refresh(0, CooldownMode.Normal);
            ((WarriorLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).SwiftProc = false;
        }
    }
}
/*
        private static readonly uint[] TempestAuraIDs = { 103000, 103102, 103120, 103131 };
        private static readonly uint[] ShadowTempestIDs = { 103104, 103130 };

        private static void CheckTempestAura(S_ABNORMALITY_BEGIN p)
        {
            if (!TempestAuraIDs.Contains(p.AbnormalityId)) return;
            ((WarriorLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).TempestAura.Val = p.Stacks;
        }
        private static void CheckTempestAura(S_ABNORMALITY_REFRESH p)
        {
            if (!TempestAuraIDs.Contains(p.AbnormalityId)) return;
            ((WarriorLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).TempestAura.Val = p.Stacks;
        }
        private static void CheckTempestAura(S_ABNORMALITY_END p)
        {
            if (!TempestAuraIDs.Contains(p.AbnormalityId)) return;
            ((WarriorLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).TempestAura.Val = 0;
        }
        private static void CheckShadowTempest(S_ABNORMALITY_BEGIN p)
        {
            if (!ShadowTempestIDs.Contains(p.AbnormalityId)) return;
            ((WarriorLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).TempestAura.ToZero(p.Duration);
        }
*/
