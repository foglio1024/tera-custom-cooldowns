using System.Linq;
using TCC.Data;
using TCC.Parsing.Messages;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public static class Warrior
    {
        private static readonly uint[] GambleIDs = { 100800, 100801, 100802, 100803 };
        private static readonly uint[] AstanceIDs = { 100100, 100101, 100102, 100103 };
        private static readonly uint[] DstanceIDs = { 100200, 100201, 100202, 100203 };
        private static readonly uint[] TraverseCutIDs = { 101300, 101301 };
        private static readonly uint[] TempestAuraIDs = { 103000, 103102, 103120, 103131 };
        private static readonly uint[] ShadowTempestIDs = { 103104, 103130 };
        private static readonly uint[] BladeWaltzIDs = { 104100 };

        public static void CheckBuff(S_ABNORMALITY_BEGIN p)
        {
            if (p.TargetId != SessionManager.CurrentPlayer.EntityId) return;
            if (GambleIDs.Contains(p.AbnormalityId))
            {
                ((WarriorBarManager)ClassWindowViewModel.Instance.CurrentManager).DeadlyGamble.Buff.Start(p.Duration);
                return;
            }
            if (AstanceIDs.Contains(p.AbnormalityId))
            {
                ((WarriorBarManager)ClassWindowViewModel.Instance.CurrentManager).Stance.CurrentStance = WarriorStance.Assault;
                return;
            }
            if (DstanceIDs.Contains(p.AbnormalityId))
            {
                ((WarriorBarManager)ClassWindowViewModel.Instance.CurrentManager).Stance.CurrentStance = WarriorStance.Defensive;
                return;
            }
            if (TraverseCutIDs.Contains(p.AbnormalityId))
            {
                ((WarriorBarManager)ClassWindowViewModel.Instance.CurrentManager).TraverseCut.Val = p.Stacks;
                ((WarriorBarManager)ClassWindowViewModel.Instance.CurrentManager).TraverseCut.ToZero(p.Duration);

                return;
            }
            if (TempestAuraIDs.Contains(p.AbnormalityId))
            {
                ((WarriorBarManager)ClassWindowViewModel.Instance.CurrentManager).TempestAura.Val = p.Stacks;
                return;
            }
            if (ShadowTempestIDs.Contains(p.AbnormalityId))
            {
                ((WarriorBarManager)ClassWindowViewModel.Instance.CurrentManager).TempestAura.ToZero(p.Duration);
                return;
            }
            if (BladeWaltzIDs.Contains(p.AbnormalityId))
            {
                if (!SessionManager.SkillsDatabase.TryGetSkillByIconName("icon_skills.doublesworddance_tex",
                    SessionManager.CurrentPlayer.Class, out var sk)) return;
                CooldownWindowViewModel.Instance.AddOrRefresh(new SkillCooldown(sk, p.Duration, CooldownType.Skill, CooldownWindowViewModel.Instance.GetDispatcher(),true, true));
                //((WarriorBarManager) ClassWindowViewModel.Instance.CurrentManager).TraverseCut.Val = p.Stacks;
                return;
            }
        }
        public static void CheckBuff(S_ABNORMALITY_REFRESH p)
        {
            if (p.TargetId != SessionManager.CurrentPlayer.EntityId) return;
            if (GambleIDs.Contains(p.AbnormalityId))
            {
                ((WarriorBarManager)ClassWindowViewModel.Instance.CurrentManager).DeadlyGamble.Buff.Refresh(p.Duration);
                return;
            }
            if (AstanceIDs.Contains(p.AbnormalityId))
            {
                ((WarriorBarManager)ClassWindowViewModel.Instance.CurrentManager).Stance.CurrentStance = WarriorStance.Assault;
                return;
            }
            if (DstanceIDs.Contains(p.AbnormalityId))
            {
                ((WarriorBarManager)ClassWindowViewModel.Instance.CurrentManager).Stance.CurrentStance = WarriorStance.Defensive;
                return;
            }
            if (TraverseCutIDs.Contains(p.AbnormalityId))
            {
                ((WarriorBarManager)ClassWindowViewModel.Instance.CurrentManager).TraverseCut.Val = p.Stacks;
                ((WarriorBarManager)ClassWindowViewModel.Instance.CurrentManager).TraverseCut.ToZero(p.Duration);

                return;
            }
            if (TempestAuraIDs.Contains(p.AbnormalityId))
            {
                ((WarriorBarManager)ClassWindowViewModel.Instance.CurrentManager).TempestAura.Val = p.Stacks;
                return;
            }
            if (BladeWaltzIDs.Contains(p.AbnormalityId))
            {
                //((WarriorBarManager)ClassWindowViewModel.Instance.CurrentManager).TraverseCut.Val = p.Stacks;
                return;
            }
        }
        public static void CheckBuffEnd(S_ABNORMALITY_END p)
        {
            if (p.TargetId != SessionManager.CurrentPlayer.EntityId) return;
            if (AstanceIDs.Contains(p.AbnormalityId))
            {
                ((WarriorBarManager)ClassWindowViewModel.Instance.CurrentManager).Stance.CurrentStance = WarriorStance.None;
                return;
            }
            if (DstanceIDs.Contains(p.AbnormalityId))
            {
                ((WarriorBarManager)ClassWindowViewModel.Instance.CurrentManager).Stance.CurrentStance = WarriorStance.None;
                return;
            }
            if (TraverseCutIDs.Contains(p.AbnormalityId))
            {
                ((WarriorBarManager)ClassWindowViewModel.Instance.CurrentManager).TraverseCut.Val = 0;
                return;
            }
            if (TempestAuraIDs.Contains(p.AbnormalityId))
            {
                ((WarriorBarManager)ClassWindowViewModel.Instance.CurrentManager).TempestAura.Val = 0;
                return;
            }
            if (BladeWaltzIDs.Contains(p.AbnormalityId))
            {
                //((WarriorBarManager)ClassWindowViewModel.Instance.CurrentManager).TraverseCut.Val = 0;
                return;
            }
        }
    }
}
