using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Data;
using TCC.Parsing.Messages;
using TCC.ViewModels;

namespace TCC
{
    public static class Warrior
    {
        static readonly uint[] GambleIDs = { 100800, 100801, 100802, 100803 };
        static readonly uint[] AstanceIDs = { 100100, 100101, 100102, 100103 };
        static readonly uint[] DstanceIDs = { 100200, 100201, 100202, 100203 };

        public static void CheckBuff(S_ABNORMALITY_BEGIN sAbnormalityBegin)
        {
            if (GambleIDs.Contains(sAbnormalityBegin.Id) && sAbnormalityBegin.CasterId == SessionManager.CurrentPlayer.EntityId)
            {
                ((WarriorBarManager)ClassManager.CurrentClassManager).DeadlyGamble.Buff.Start(sAbnormalityBegin.Duration);
                return;
            }
            if (AstanceIDs.Contains(sAbnormalityBegin.Id) && sAbnormalityBegin.CasterId == SessionManager.CurrentPlayer.EntityId)
            {
                ((WarriorBarManager)ClassManager.CurrentClassManager).Stance.CurrentStance = WarriorStance.Assault;
                return;
            }
            if (DstanceIDs.Contains(sAbnormalityBegin.Id) && sAbnormalityBegin.CasterId == SessionManager.CurrentPlayer.EntityId)
            {
                ((WarriorBarManager)ClassManager.CurrentClassManager).Stance.CurrentStance=WarriorStance.Defensive;
                return;
            }
        }
        public static void CheckBuffEnd(S_ABNORMALITY_END p)
        {
            if (AstanceIDs.Contains(p.Id) && p.Target == SessionManager.CurrentPlayer.EntityId)
            {
                ((WarriorBarManager)ClassManager.CurrentClassManager).Stance.CurrentStance = WarriorStance.None;
                return;
            }
            if (DstanceIDs.Contains(p.Id) && p.Target == SessionManager.CurrentPlayer.EntityId)
            {
                ((WarriorBarManager)ClassManager.CurrentClassManager).Stance.CurrentStance = WarriorStance.None;
                return;
            }
        }
    }
}
