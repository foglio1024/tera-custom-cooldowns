using FoglioUtils.Extensions;

using System.Linq;

using TCC.Data;
using TCC.ViewModels;
using TeraDataLite;
using TeraPacketParser.Messages;

namespace TCC.ClassSpecific
{
    public class MysticAbnormalityTracker : ClassAbnormalityTracker
    {
        private const int HurricaneId = 60010;
        private const int HurricaneDuration = 120000;
        private static readonly int VowId = 700100;
        private static readonly int VocId = 27160;

        private static readonly uint[] CritAuraIDs = { 700600, 700601, 700602, 700603 };
        private static readonly uint[] ManaAuraIDs = { 700300 };
        private static readonly uint[] CritResAuraIDs = { 700200, 700201, 700202, 700203 };
        private static readonly uint[] SwiftAuraIDs = { 700700, 700701 };
        private static readonly uint[] ElementalizeIDs = { 702000 };


        public static void CheckHurricane(S_ABNORMALITY_BEGIN msg)
        {
            if (msg.AbnormalityId != HurricaneId || !SessionManager.IsMe(msg.CasterId)) return;
            SessionManager.DB.SkillsDatabase.TryGetSkill(HurricaneId, Class.Common, out var hurricane);
            SkillManager.AddSkillDirectly(hurricane, HurricaneDuration);
        }

        public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
        {
            CheckVoc(p);
            if (!SessionManager.IsMe(p.TargetId)) return;
            if (CritAuraIDs.Contains(p.AbnormalityId))
            {
                TccUtils.CurrentClassVM<MysticLayoutVM>().Auras.CritAura = true;
            }
            else if (ManaAuraIDs.Contains(p.AbnormalityId) )
            {
                TccUtils.CurrentClassVM<MysticLayoutVM>().Auras.ManaAura = true;
            }
            else if (CritResAuraIDs.Contains(p.AbnormalityId) )
            {
                TccUtils.CurrentClassVM<MysticLayoutVM>().Auras.CritResAura = true;
            }
            else if (SwiftAuraIDs.Contains(p.AbnormalityId) )
            {
                TccUtils.CurrentClassVM<MysticLayoutVM>().Auras.SwiftAura = true;
            }
            else if (p.AbnormalityId == VowId )
            {
                TccUtils.CurrentClassVM<MysticLayoutVM>().Vow.Buff.Start(p.Duration);
            }
            else if (ElementalizeIDs.Contains(p.AbnormalityId) )
            {
                TccUtils.CurrentClassVM<MysticLayoutVM>().Elementalize = true;
            }

        }
        public override void CheckAbnormality(S_ABNORMALITY_REFRESH p)
        {
            CheckVoc(p);

            if (!SessionManager.IsMe(p.TargetId)) return;

            if (CritAuraIDs.Contains(p.AbnormalityId) )
            {
                TccUtils.CurrentClassVM<MysticLayoutVM>().Auras.CritAura = true;
            }
            else if (ManaAuraIDs.Contains(p.AbnormalityId) )
            {
                TccUtils.CurrentClassVM<MysticLayoutVM>().Auras.ManaAura = true;
            }
            else if (CritResAuraIDs.Contains(p.AbnormalityId) )
            {
                TccUtils.CurrentClassVM<MysticLayoutVM>().Auras.CritResAura = true;
            }
            else if (SwiftAuraIDs.Contains(p.AbnormalityId) )
            {
                TccUtils.CurrentClassVM<MysticLayoutVM>().Auras.SwiftAura = true;
            }
            else if (p.AbnormalityId == VowId )
            {
                TccUtils.CurrentClassVM<MysticLayoutVM>().Vow.Buff.Refresh(p.Duration, CooldownMode.Normal);
            }
            else if (ElementalizeIDs.Contains(p.AbnormalityId))
            {
                TccUtils.CurrentClassVM<MysticLayoutVM>().Elementalize = true;
            }
        }
        public override void CheckAbnormality(S_ABNORMALITY_END p)
        {
            CheckVoc(p);

            if (!SessionManager.IsMe(p.TargetId)) return;

            if (CritAuraIDs.Contains(p.AbnormalityId) )
            {
                TccUtils.CurrentClassVM<MysticLayoutVM>().Auras.CritAura = false;
            }
            else if (ManaAuraIDs.Contains(p.AbnormalityId) )
            {
                TccUtils.CurrentClassVM<MysticLayoutVM>().Auras.ManaAura = false;
            }
            else if (CritResAuraIDs.Contains(p.AbnormalityId) )
            {
                TccUtils.CurrentClassVM<MysticLayoutVM>().Auras.CritResAura = false;
            }
            else if (SwiftAuraIDs.Contains(p.AbnormalityId) )
            {
                TccUtils.CurrentClassVM<MysticLayoutVM>().Auras.SwiftAura = false;
            }
            else if (p.AbnormalityId == VowId )
            {
                TccUtils.CurrentClassVM<MysticLayoutVM>().Vow.Buff.Refresh(0, CooldownMode.Normal);
            }
            else if (ElementalizeIDs.Contains(p.AbnormalityId))
            {
                TccUtils.CurrentClassVM<MysticLayoutVM>().Elementalize = false;
            }
        }

        public static void CheckVoc(ulong target)
        {
            if (MarkedTargets.Contains(target))
            {
                MarkedTargets.Remove(target);
                if (MarkedTargets.Count == 0) InvokeMarkingExpired();
            }
        }
        private static void CheckVoc(S_ABNORMALITY_BEGIN p)
        {
            if (VocId != p.AbnormalityId) return;
            var target = WindowManager.BossWindow.VM.NpcList.ToSyncList().FirstOrDefault(x => x.EntityId == p.TargetId);
            if (target != null)
            {
                if (!MarkedTargets.Contains(p.TargetId)) MarkedTargets.Add(p.TargetId);
                InvokeMarkingRefreshed(p.Duration);
            }
        }
        private static void CheckVoc(S_ABNORMALITY_REFRESH p)
        {
            if (VocId != p.AbnormalityId) return;
            var target = WindowManager.BossWindow.VM.NpcList.ToSyncList().FirstOrDefault(x => x.EntityId == p.TargetId);
            if (target != null)
            {
                if (!MarkedTargets.Contains(p.TargetId)) MarkedTargets.Add(p.TargetId);
                InvokeMarkingRefreshed(p.Duration);
            }
        }
        private static void CheckVoc(S_ABNORMALITY_END p)
        {
            if (VocId != p.AbnormalityId) return;
            if (MarkedTargets.Contains(p.TargetId)) MarkedTargets.Remove(p.TargetId);
            if (MarkedTargets.Count == 0) InvokeMarkingExpired();
        }
    }
}
