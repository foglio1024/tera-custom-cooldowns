using System;
using System.Collections.Generic;
using System.Linq;
using TCC.Parsing.Messages;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public static class Priest
    {
        private static readonly List<ulong> MarkedTargets = new List<ulong>();
        public static event Action<ulong> TripleNemesisRefreshed;
        public static event Action TripleNemesisExpired;


        public static readonly List<uint> CommonBuffs = new List<uint>
        {
            805100, 805101, 805102,
            805600, 805601, 805602, 805603, 805604,
            800300, 800301, 800302,800303, 800304
        };

        private static readonly uint[] EnergyStarsIDs = { 801500, 801501, 801502, 801503, 98000107 };
        private static readonly int GraceId = 801700;
        private static readonly int TripleNemesisId = 28090;
        private static readonly uint[] EdictIDs = { 805800, 805801, 805802, 805803 };

        public static void CheckTripleNemesis(ulong target)
        {
            if (MarkedTargets.Contains(target))
            {
                MarkedTargets.Remove(target);
                if (MarkedTargets.Count == 0) TripleNemesisExpired?.Invoke();
            }
        }

        public static void CheckTripleNemesis(S_ABNORMALITY_BEGIN p)
        {
            if (TripleNemesisId != p.AbnormalityId) return;
            var target = BossGageWindowViewModel.Instance.NpcList.FirstOrDefault(x => x.EntityId == p.TargetId);
            if (target != null)
            {
                if (!MarkedTargets.Contains(p.TargetId)) MarkedTargets.Add(p.TargetId);
                TripleNemesisRefreshed?.Invoke(p.Duration);
            }
        }
        public static void CheckTripleNemesis(S_ABNORMALITY_REFRESH p)
        {
            if (TripleNemesisId != p.AbnormalityId) return;
            var target = BossGageWindowViewModel.Instance.NpcList.FirstOrDefault(x => x.EntityId == p.TargetId);
            if (target != null)
            {
                if (!MarkedTargets.Contains(p.TargetId)) MarkedTargets.Add(p.TargetId);
                TripleNemesisRefreshed?.Invoke(p.Duration);
            }
        }
        public static void CheckTripleNemesis(S_ABNORMALITY_END p)
        {
            if (TripleNemesisId != p.AbnormalityId) return;
            if (MarkedTargets.Contains(p.TargetId)) MarkedTargets.Remove(p.TargetId);
            if (MarkedTargets.Count == 0) TripleNemesisExpired?.Invoke();
        }

        public static void CheckBuff(S_ABNORMALITY_BEGIN p)
        {
            CheckTripleNemesis(p);
            if (p.TargetId == SessionManager.CurrentPlayer.EntityId && EnergyStarsIDs.Contains(p.AbnormalityId))
            {
                ((PriestBarManager)ClassWindowViewModel.Instance.CurrentManager).EnergyStars.Buff.Start(p.Duration);
                return;
            }
            if (p.TargetId == SessionManager.CurrentPlayer.EntityId && p.AbnormalityId == GraceId)
            {
                ((PriestBarManager)ClassWindowViewModel.Instance.CurrentManager).Grace.Buff.Start(p.Duration);
                return;
            }
            if (p.TargetId == SessionManager.CurrentPlayer.EntityId && EdictIDs.Contains(p.AbnormalityId))
            {
                ((PriestBarManager)ClassWindowViewModel.Instance.CurrentManager).EdictOfJudgment.Buff.Start(p.Duration);
                return;
            }
        }
        public static void CheckBuff(S_ABNORMALITY_REFRESH p)
        {
            CheckTripleNemesis(p);

            if (p.TargetId == SessionManager.CurrentPlayer.EntityId && EnergyStarsIDs.Contains(p.AbnormalityId))
            {
                ((PriestBarManager)ClassWindowViewModel.Instance.CurrentManager).EnergyStars.Buff.Refresh(p.Duration);
                return;
            }
            if (p.TargetId == SessionManager.CurrentPlayer.EntityId && p.AbnormalityId == GraceId)
            {
                ((PriestBarManager)ClassWindowViewModel.Instance.CurrentManager).Grace.Buff.Refresh(p.Duration);
                return;
            }
            if (p.TargetId == SessionManager.CurrentPlayer.EntityId && EdictIDs.Contains(p.AbnormalityId))
            {
                ((PriestBarManager)ClassWindowViewModel.Instance.CurrentManager).EdictOfJudgment.Buff.Refresh(p.Duration);
                return;
            }
        }
        public static void CheckBuffEnd(S_ABNORMALITY_END p)
        {
            CheckTripleNemesis(p);
            if (p.TargetId == SessionManager.CurrentPlayer.EntityId && EnergyStarsIDs.Contains(p.AbnormalityId))
            {
                ((PriestBarManager)ClassWindowViewModel.Instance.CurrentManager).EnergyStars.Buff.Refresh(0);
                return;
            }
            if (p.TargetId == SessionManager.CurrentPlayer.EntityId && p.AbnormalityId == GraceId)
            {
                ((PriestBarManager)ClassWindowViewModel.Instance.CurrentManager).Grace.Buff.Refresh(0);
                return;
            }
            if (p.TargetId == SessionManager.CurrentPlayer.EntityId && EdictIDs.Contains(p.AbnormalityId))
            {
                ((PriestBarManager)ClassWindowViewModel.Instance.CurrentManager).EdictOfJudgment.Buff.Refresh(0);
                return;
            }
        }

        public static void ClearMarkedTargets()
        {
            App.BaseDispatcher.Invoke(() => MarkedTargets.Clear());
        }
    }
}
