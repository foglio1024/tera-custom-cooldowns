using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace TCC.UI
{
    public static class BroochesDatabase
    {
        static List<Skill> BroochSkills = new List<Skill>
        {
            new Skill(51028, Class.Common, "",""),//marrow
            new Skill(51011, Class.Common, "",""),//quatrefoil
            new Skill(19698, Class.Common, "",""),//simple grounding
            new Skill(19699, Class.Common, "",""),//grounding
            new Skill(19702, Class.Common, "",""),//simple empowered
            new Skill(19703, Class.Common, "",""),//empowered
            new Skill(19705, Class.Common, "",""),//simple quickcarve
            new Skill(19706, Class.Common, "",""),//quickcarve
            new Skill(19734, Class.Common, "",""),//simple cleansing
            new Skill(19735, Class.Common, "",""),//cleansing
            new Skill(6466,  Class.Common, "",""), //iridescent threat
            new Skill(6469,  Class.Common, "",""), //iridescent empowered
            new Skill(6472,  Class.Common, "",""), //iridescent quickcarve
            new Skill(80280, Class.Common, "",""),//simple purifying
            new Skill(80281, Class.Common, "",""),//purifying
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
    }
}