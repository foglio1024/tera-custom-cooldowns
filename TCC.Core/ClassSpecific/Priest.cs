using System.Collections.Generic;
using System.Linq;
using TCC.Parsing.Messages;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public static class Priest
    {
        public static readonly List<uint> CommonBuffs = new List<uint>
        {
            805100, 805101, 805102,
            805600, 805601, 805602, 805603, 805604,
            800300, 800301, 800302,800303, 800304
        };

        private static readonly uint[] EnergyStarsIDs = { 801500, 801501 , 801502 , 801503, 98000107 };
        private static readonly int GraceId = 801700;
        private static readonly uint[] EdictIDs = { 805800, 805801, 805802, 805803 };
        private static readonly uint[] DivineIDs = { 805700, 805710, 805711, 805712, 805713 };
        internal static void CheckBuff(S_ABNORMALITY_BEGIN p)
        {
            if(p.TargetId == SessionManager.CurrentPlayer.EntityId && EnergyStarsIDs.Contains(p.AbnormalityId))
            {
                ((PriestBarManager)ClassWindowViewModel.Instance.CurrentManager).EnergyStars.Buff.Start(p.Duration);
                return;
            }
            if (p.TargetId == SessionManager.CurrentPlayer.EntityId && p.AbnormalityId == GraceId)
            {
                ((PriestBarManager)ClassWindowViewModel.Instance.CurrentManager).Grace.Buff.Start(p.Duration);
            }
            if (p.TargetId == SessionManager.CurrentPlayer.EntityId && EdictIDs.Contains(p.AbnormalityId))
            {
                ((PriestBarManager)ClassWindowViewModel.Instance.CurrentManager).EdictOfJudgment.Buff.Start(p.Duration);
            }
            if (p.TargetId == SessionManager.CurrentPlayer.EntityId && DivineIDs.Contains(p.AbnormalityId))
            {
                ((PriestBarManager)ClassWindowViewModel.Instance.CurrentManager).DivineCharge.Buff.Start(p.Duration);
            }

        }

        internal static void CheckBuff(S_ABNORMALITY_REFRESH p)
        {
            if (p.TargetId == SessionManager.CurrentPlayer.EntityId && EnergyStarsIDs.Contains(p.AbnormalityId))
            {
                ((PriestBarManager)ClassWindowViewModel.Instance.CurrentManager).EnergyStars.Buff.Refresh(p.Duration);
                return;
            }
            if (p.TargetId == SessionManager.CurrentPlayer.EntityId && p.AbnormalityId == GraceId)
            {
                ((PriestBarManager)ClassWindowViewModel.Instance.CurrentManager).Grace.Buff.Refresh(p.Duration);
            }
            if (p.TargetId == SessionManager.CurrentPlayer.EntityId && EdictIDs.Contains(p.AbnormalityId))
            {
                ((PriestBarManager)ClassWindowViewModel.Instance.CurrentManager).EdictOfJudgment.Buff.Refresh(p.Duration);
            }
            if (p.TargetId == SessionManager.CurrentPlayer.EntityId && DivineIDs.Contains(p.AbnormalityId))
            {
                ((PriestBarManager)ClassWindowViewModel.Instance.CurrentManager).DivineCharge.Buff.Start(p.Duration);
            }
        }

        public static void CheckBuffEnd(S_ABNORMALITY_END p)
        {
            if (p.TargetId == SessionManager.CurrentPlayer.EntityId && EnergyStarsIDs.Contains(p.AbnormalityId))
            {
                ((PriestBarManager)ClassWindowViewModel.Instance.CurrentManager).EnergyStars.Buff.Refresh(0);
                return;
            }
            if (p.TargetId == SessionManager.CurrentPlayer.EntityId && p.AbnormalityId == GraceId)
            {
                ((PriestBarManager)ClassWindowViewModel.Instance.CurrentManager).Grace.Buff.Refresh(0);
            }
            if (p.TargetId == SessionManager.CurrentPlayer.EntityId && EdictIDs.Contains(p.AbnormalityId))
            {
                ((PriestBarManager)ClassWindowViewModel.Instance.CurrentManager).EdictOfJudgment.Buff.Refresh(0);
            }
            if (p.TargetId == SessionManager.CurrentPlayer.EntityId && DivineIDs.Contains(p.AbnormalityId))
            {
                ((PriestBarManager)ClassWindowViewModel.Instance.CurrentManager).DivineCharge.Buff.Refresh(0);
            }
        }
    }
}
