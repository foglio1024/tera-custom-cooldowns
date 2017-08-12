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

        public static void CheckBuff(S_ABNORMALITY_BEGIN p)
        {
            if (GambleIDs.Contains(p.AbnormalityId) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((WarriorBarManager)ClassManager.CurrentClassManager).DeadlyGamble.Buff.Start(p.Duration);
                return;
            }
            if (AstanceIDs.Contains(p.AbnormalityId) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((WarriorBarManager)ClassManager.CurrentClassManager).Stance.CurrentStance = WarriorStance.Assault;
                return;
            }
            if (DstanceIDs.Contains(p.AbnormalityId) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((WarriorBarManager)ClassManager.CurrentClassManager).Stance.CurrentStance=WarriorStance.Defensive;
            }
        }
        public static void CheckBuff(S_ABNORMALITY_REFRESH p)
        {
            if (GambleIDs.Contains(p.AbnormalityId) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((WarriorBarManager)ClassManager.CurrentClassManager).DeadlyGamble.Buff.Refresh(p.Duration);
                return;
            }
            if (AstanceIDs.Contains(p.AbnormalityId) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((WarriorBarManager)ClassManager.CurrentClassManager).Stance.CurrentStance = WarriorStance.Assault;
                return;
            }
            if (DstanceIDs.Contains(p.AbnormalityId) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((WarriorBarManager)ClassManager.CurrentClassManager).Stance.CurrentStance = WarriorStance.Defensive;
            }
        }
        public static void CheckBuffEnd(S_ABNORMALITY_END p)
        {
            if (AstanceIDs.Contains(p.AbnormalityId) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((WarriorBarManager)ClassManager.CurrentClassManager).Stance.CurrentStance = WarriorStance.None;
                return;
            }
            if (DstanceIDs.Contains(p.AbnormalityId) && p.TargetId == SessionManager.CurrentPlayer.EntityId)
            {
                ((WarriorBarManager)ClassManager.CurrentClassManager).Stance.CurrentStance = WarriorStance.None;
            }
        }
    }
}
