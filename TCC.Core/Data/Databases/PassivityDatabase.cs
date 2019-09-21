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
            {15162,60}, // insigna of the punisher

            // bracing force
            {13000, 180},{13001, 180},{13002, 180},
            {13003, 180},{13004, 180},{13005, 180},
            {13006, 180},{13007, 180},{13008, 180},
            {13009, 180},{13010, 180},{13011, 180},
            {13012, 180},{13013, 180},{13014, 180},
            {13015, 180},{13016, 180},{13017, 180},
            {13018, 180},{13019, 180},{13020, 180},
            {13021, 180},{13022, 180},{13023, 180},
            {13024, 180},{13025, 180},{13026, 180},
            {13027, 180},{13028, 180},{13029, 180},
            {13030, 180},{13031, 180},{13032, 180},
            {13033, 180},{13034, 180},{13035, 180},
            {13036, 180},{13037, 180},

        };
        public static bool TryGetPassivitySkill(uint id, out Skill sk)
        {
            var result = false;
            sk = new Skill(0, Class.None, string.Empty, string.Empty);

            if (!Game.DB.AbnormalityDatabase.Abnormalities.TryGetValue(id, out var ab)) return result;
            result = true;
            sk = new Skill(id, Class.Common, ab.Name, "") { IconName = ab.IconName };
            return result;

        }
    }
}
