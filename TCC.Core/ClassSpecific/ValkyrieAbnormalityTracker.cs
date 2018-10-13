using System.Collections.Generic;
using TCC.Data;
using TCC.Parsing.Messages;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public class ValkyrieAbnormalityTracker : ClassAbnormalityTracker
    {
        private const uint RagnarokId = 10155130;
        private const uint GrugnirsBiteId = 10155530;
        private static readonly List<uint> GodsfallIds = new List<uint> { 10155510, 10155512 };
        private static readonly List<uint> TwilightWaltzIds = new List<uint> { 10155540, 10155541, 10155542 };

        private Skill _godsfall;
        private Skill _twilightWaltz;
        private Skill _grugnirsBite;

        public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
        {
            if (!p.TargetId.IsMe()) return;
            CheckGrugnirsBite(p);
            CheckTwilightWaltz(p);
            CheckRagnarok(p);
            CheckGodsfall(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_REFRESH p)
        {
            if (!p.TargetId.IsMe()) return;
            CheckRagnarok(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_END p)
        {
            if (!p.TargetId.IsMe()) return;
            CheckRagnarok(p);
        }

        private static void CheckRagnarok(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != RagnarokId) return;
            ((ValkyrieBarManager)ClassWindowViewModel.Instance.CurrentManager).Ragnarok.Buff.Start(p.Duration);
        }
        private static void CheckRagnarok(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId != RagnarokId) return;
            ((ValkyrieBarManager)ClassWindowViewModel.Instance.CurrentManager).Ragnarok.Buff.Refresh(0);
        }
        private static void CheckRagnarok(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId != RagnarokId) return;
            ((ValkyrieBarManager)ClassWindowViewModel.Instance.CurrentManager).Ragnarok.Buff.Refresh(p.Duration);
        }
        private  void CheckTwilightWaltz(S_ABNORMALITY_BEGIN p)
        {
            if (!TwilightWaltzIds.Contains(p.AbnormalityId)) return;
            StartPrecooldown(_twilightWaltz, p.Duration);
        }
        private void CheckGodsfall(S_ABNORMALITY_BEGIN p)
        {
            if (!GodsfallIds.Contains(p.AbnormalityId)) return;
            StartPrecooldown(_godsfall, p.Duration);
        }
        private void CheckGrugnirsBite(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != GrugnirsBiteId) return;
            StartPrecooldown(_grugnirsBite, p.Duration);
        }

        public ValkyrieAbnormalityTracker()
        {
            SessionManager.SkillsDatabase.TryGetSkillByIconName("icon_skills.rageslash_tex", SessionManager.CurrentPlayer.Class, out _twilightWaltz);
            SessionManager.SkillsDatabase.TryGetSkillByIconName("icon_skills.warbegin_tex", SessionManager.CurrentPlayer.Class, out _godsfall);
            SessionManager.SkillsDatabase.TryGetSkillByIconName("icon_skills.halfmoon_tex", SessionManager.CurrentPlayer.Class, out _grugnirsBite);

        }
    }
}
