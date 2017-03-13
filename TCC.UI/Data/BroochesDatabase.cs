using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace TCC
{
    public static class BroochesDatabase
    {
        static List<Skill> BroochSkills = new List<Skill>
        {
            new Skill(51028, Class.Common, "Marrow brooch",""),//marrow
            new Skill(51011, Class.Common, "Quatrefoil brooch",""),//quatrefoil
            new Skill(19698, Class.Common, "Simple grounding brooch",""),//simple grounding
            new Skill(19699, Class.Common, "Grounding brooch",""),//grounding
            new Skill(19702, Class.Common, "Simple empowered brooch",""),//simple empowered
            new Skill(19703, Class.Common, "Empowered brooch",""),//empowered
            new Skill(19705, Class.Common, "Simple quickcarve brooch",""),//simple quickcarve
            new Skill(19706, Class.Common, "Quickcarve brooch",""),//quickcarve
            new Skill(19734, Class.Common, "Simple cleansing brooch",""),//simple cleansing
            new Skill(19735, Class.Common, "Cleansing brooch",""),//cleansing
            new Skill(6466,  Class.Common, "Iridescent treatening brooch",""), //iridescent threat
            new Skill(6469,  Class.Common, "Iridescent powerful brooch",""), //iridescent empowered
            new Skill(6472,  Class.Common, "Iridescent keen brooch",""), //iridescent quickcarve
            new Skill(80280, Class.Common, "Simple purifying brooch",""),//simple purifying
            new Skill(80281, Class.Common, "Purifying brooch",""),//purifying
        };
        static List<string> IconsList = new List<string>
        {
            "Icon_Skills.brooch_vergos_Tex",
            "Icon_Skills.parts_magicweaponC8_Tex",
            "Icon_Skills.Goddess_brooch_Tex",
            "Icon_Skills.Goddess_brooch_Tex",
            "Icon_Skills.Goddess_brooch_Tex",
            "Icon_Skills.Goddess_brooch_Tex",
            "Icon_Skills.Goddess_brooch_Tex",
            "Icon_Skills.Goddess_brooch_Tex",
            "Icon_Skills.stoneoflife_Tex",
            "Icon_Skills.stoneoflife_Tex",
            "Icon_Skills.Beautiful_Wings_brooch_Cachebar_Tex",
            "Icon_Skills.Beautiful_Wings_brooch_Cachebar_Tex",
            "Icon_Skills.Beautiful_Wings_brooch_Cachebar_Tex",
            "Icon_Skills.F2P_VIPCustomer_Tex",
            "Icon_Skills.F2P_VIPCustomer_Tex"
        };

        public static void SetBroochesIcons()
        {
            for (int i = 0; i < BroochSkills.Count(); i++)
            {
                BroochSkills[i].SetSkillIcon(IconsList[i]);
            }
        }
        public static Skill GetBrooch(uint id)
        {
            if (BroochSkills.Where(x => x.Id == id).Count() > 0)
            {
                return BroochSkills.Where(x => x.Id == id).Single();
            }
            else return null;

        }

        internal static bool TryGetBrooch(uint itemId, out Skill brooch)
        {
            bool result = false;
            brooch = new Skill(0, Class.None, string.Empty, string.Empty);
            if (BroochSkills.Where(x => x.Id == itemId).Count() > 0)
            {
                result = true;
                brooch = BroochSkills.Where(x => x.Id == itemId).Single();
            }
            return result;

        }
    }
}