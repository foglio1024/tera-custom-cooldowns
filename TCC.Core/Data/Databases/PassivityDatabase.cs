using System.Collections.Generic;
using TCC.Data.Skills;
using TeraDataLite;

namespace TCC.Data.Databases
{
    public static class PassivityDatabase
    {
        //TODO: maybe move this to a TeraData tsv file (or merge in hotdot.tsv)
        public static Dictionary<uint, uint> Passivities { get; } = new Dictionary<uint, uint>
        {
            {6001,60}, {6002,60}, {6003,60}, {6004,60}, // dragon
            {6012,60}, {6013,60}, // phoenix
            {6017,60}, {6018,60}, // drake
            {60029,120}, {60030,120}, { 60031,120}, { 60032,120},  //concentration
            {15162,60} // insigna of the punisher
        };
        public static bool TryGetPassivitySkill(uint id, out Skill sk)
        {
            var result = false;
            sk = new Skill(0, Class.None, string.Empty, string.Empty);

            if (Session.DB.AbnormalityDatabase.Abnormalities.TryGetValue(id, out var ab))
            {
                result = true;
                sk = new Skill(id, Class.Common, ab.Name, "") { IconName = ab.IconName };
            }
            return result;

        }
    }
}
