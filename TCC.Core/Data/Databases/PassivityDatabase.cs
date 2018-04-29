using System.Collections.Generic;

namespace TCC.Data.Databases
{
    public static class PassivityDatabase
    {
        public static List<uint> Passivities = new List<uint>
        {
             6001 , 6002, 6003, 6004 
        };
        public static bool TryGetPassivitySkill(uint id, out Skill sk)
        {
            var result = false;
            sk = new Skill(0, Class.None, string.Empty, string.Empty);

            if (AbnormalityManager.CurrentDb.Abnormalities.ContainsKey(id))
            {
                var ab = AbnormalityManager.CurrentDb.Abnormalities[id];
                result = true;
                sk = new Skill(id, Class.Common, ab.Name, "");
                sk.SetSkillIcon(ab.IconName);
            }
            return result;

        }
    }
}
