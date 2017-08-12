using TCC.Parsing.Messages;
using TCC.ViewModels;

namespace TCC.ClassSpecific
{
    public static class Valkyrie
    {
        public static void CheckRagnarok(S_ABNORMALITY_BEGIN p)
        {
            if(p.CasterId == SessionManager.CurrentPlayer.EntityId && p.AbnormalityId == 10155130)
            {
                ((ValkyrieBarManager)ClassManager.CurrentClassManager).Ragnarok.Buff.Start(p.Duration);
            }
        }
    }
}
