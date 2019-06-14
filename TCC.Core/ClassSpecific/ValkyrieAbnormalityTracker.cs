using System.Collections.Generic;
using TCC.Data;
using TCC.Data.Skills;
using TCC.ViewModels;
using TeraPacketParser.Messages;

namespace TCC.ClassSpecific
{
    public class ValkyrieAbnormalityTracker : ClassAbnormalityTracker
    {
        private const uint RagnarokId = 10155130;
        private const uint GrugnirsBiteId = 10155530;
        private const uint GodsfallId = 10155512;
        private static readonly List<uint> GodsfallPreCdIds = new List<uint> { 10155510, 10155512 };
        private static readonly List<uint> TwilightWaltzIds = new List<uint> { 10155540, 10155541, 10155542 };

        private readonly Skill _godsfall;
        private readonly Skill _twilightWaltz;
        private readonly Skill _grugnirsBite;

        public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
        {
            if (!Session.IsMe(p.TargetId)) return;
            CheckGrugnirsBite(p);
            CheckTwilightWaltz(p);
            CheckRagnarok(p);
            CheckGodsfallPrecd(p);
            CheckGodsfall(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_REFRESH p)
        {
            if (!Session.IsMe(p.TargetId)) return;
            CheckRagnarok(p);
            CheckGodsfall(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_END p)
        {
            if (!Session.IsMe(p.TargetId)) return;
            CheckRagnarok(p);
            CheckGodsfall(p);
        }

        private static void CheckRagnarok(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != RagnarokId) return;
            TccUtils.CurrentClassVM<ValkyrieLayoutVM>().Ragnarok.Buff.Start(p.Duration);
        }
        private static void CheckRagnarok(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId != RagnarokId) return;
            TccUtils.CurrentClassVM<ValkyrieLayoutVM>().Ragnarok.Buff.Refresh(0, CooldownMode.Normal);
        }
        private static void CheckRagnarok(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId != RagnarokId) return;
            TccUtils.CurrentClassVM<ValkyrieLayoutVM>().Ragnarok.Buff.Refresh(p.Duration, CooldownMode.Normal);
        }

        private static void CheckGodsfall(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != GodsfallId) return;
            TccUtils.CurrentClassVM<ValkyrieLayoutVM>().Godsfall.Buff.Start(p.Duration);
        }
        private static void CheckGodsfall(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId != GodsfallId) return;
            TccUtils.CurrentClassVM<ValkyrieLayoutVM>().Godsfall.Buff.Refresh(p.Duration, CooldownMode.Normal);
        }
        private static void CheckGodsfall(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId != GodsfallId) return;
            TccUtils.CurrentClassVM<ValkyrieLayoutVM>().Godsfall.Buff.Refresh(0, CooldownMode.Normal);
        }

        private  void CheckTwilightWaltz(S_ABNORMALITY_BEGIN p)
        {
            if (!TwilightWaltzIds.Contains(p.AbnormalityId)) return;
            StartPrecooldown(_twilightWaltz, p.Duration);
        }
        private void CheckGodsfallPrecd(S_ABNORMALITY_BEGIN p)
        {
            if (!GodsfallPreCdIds.Contains(p.AbnormalityId)) return;
            StartPrecooldown(_godsfall, p.Duration);
        }
        private void CheckGrugnirsBite(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != GrugnirsBiteId) return;
            StartPrecooldown(_grugnirsBite, p.Duration);
        }

        public ValkyrieAbnormalityTracker()
        {
            Session.DB.SkillsDatabase.TryGetSkillByIconName("icon_skills.rageslash_tex", Session.Me.Class, out _twilightWaltz);
            Session.DB.SkillsDatabase.TryGetSkillByIconName("icon_skills.warbegin_tex", Session.Me.Class, out _godsfall);
            Session.DB.SkillsDatabase.TryGetSkillByIconName("icon_skills.halfmoon_tex", Session.Me.Class, out _grugnirsBite);

        }
    }
}
