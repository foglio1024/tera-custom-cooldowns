using System;
using System.Collections.Generic;
using TCC.Data.Skills;
using TCC.Utils;
using TCC.ViewModels;
using TeraPacketParser.Messages;

namespace TCC.Data.Abnormalities
{
    public class ValkyrieAbnormalityTracker : AbnormalityTracker
    {
        private const uint RagnarokId = 10155130;
        private const uint GrugnirsBiteId = 10155530;
        private const uint GodsfallId = 10155512;
        private static readonly List<uint> GodsfallPreCdIds = new() { 10155510, 10155512 };
        private static readonly List<uint> TwilightWaltzIds = new() { 10155530, 10155540, 10155541, 10155542 };

        private readonly Skill _godsfall;
        private readonly Skill _twilightWaltz;
        private readonly Skill _grugnirsBite;

        public ValkyrieAbnormalityTracker()
        {
            Game.DB!.SkillsDatabase.TryGetSkillByIconName("icon_skills.rageslash_tex", Game.Me.Class, out var tw);
            Game.DB!.SkillsDatabase.TryGetSkillByIconName("icon_skills.warbegin_tex", Game.Me.Class, out var gf);
            Game.DB!.SkillsDatabase.TryGetSkillByIconName("icon_skills.halfmoon_tex", Game.Me.Class, out var gb);

            _twilightWaltz = tw ?? throw new NullReferenceException("Skill not found!");
            _godsfall = gf ?? throw new NullReferenceException("Skill not found!");
            _grugnirsBite = gb ?? throw new NullReferenceException("Skill not found!");
        }

        public override void CheckAbnormality(S_ABNORMALITY_BEGIN p)
        {
            if (!Game.IsMe(p.TargetId)) return;
            CheckGrugnirsBite(p);
            CheckTwilightWaltz(p);
            CheckRagnarok(p);
            CheckGodsfallPrecd(p);
            CheckGodsfall(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_REFRESH p)
        {
            if (!Game.IsMe(p.TargetId)) return;
            CheckRagnarok(p);
            CheckGodsfall(p);
        }
        public override void CheckAbnormality(S_ABNORMALITY_END p)
        {
            if (!Game.IsMe(p.TargetId)) return;
            CheckRagnarok(p);
            CheckGodsfall(p);
        }

        private static void CheckRagnarok(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != RagnarokId) return;
            if (!IsViewModelAvailable<ValkyrieLayoutVM>(out var vm)) return;

            vm!.Ragnarok.StartEffect(p.Duration);
        }
        private static void CheckRagnarok(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId != RagnarokId) return;
            if (!IsViewModelAvailable<ValkyrieLayoutVM>(out var vm)) return;
            vm!.Ragnarok.StopEffect();
        }
        private static void CheckRagnarok(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId != RagnarokId) return;
            if (!IsViewModelAvailable<ValkyrieLayoutVM>(out var vm)) return;
            vm!.Ragnarok.RefreshEffect(p.Duration);
        }

        private static void CheckGodsfall(S_ABNORMALITY_BEGIN p)
        {
            if (p.AbnormalityId != GodsfallId) return;
            if (!IsViewModelAvailable<ValkyrieLayoutVM>(out var vm)) return;
            vm!.Godsfall.StartEffect(p.Duration);
        }
        private static void CheckGodsfall(S_ABNORMALITY_REFRESH p)
        {
            if (p.AbnormalityId != GodsfallId) return;
            if (!IsViewModelAvailable<ValkyrieLayoutVM>(out var vm)) return;
            vm!.Godsfall.RefreshEffect(p.Duration);
        }
        private static void CheckGodsfall(S_ABNORMALITY_END p)
        {
            if (p.AbnormalityId != GodsfallId) return;
            if (!IsViewModelAvailable<ValkyrieLayoutVM>(out var vm)) return;
            vm!.Godsfall.StopEffect();
        }

        private void CheckTwilightWaltz(S_ABNORMALITY_BEGIN p)
        {
            if (!TwilightWaltzIds.Contains(p.AbnormalityId)) return;
            if (p.AbnormalityId == 10155530 && p.Duration == 7000) return; // ewww
            Log.CW($"Starting TW {p.AbnormalityId} {p.Duration}");
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

    }
}
