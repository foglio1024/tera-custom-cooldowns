using System.Collections.Generic;
using System.Linq;

namespace TCC.Data.Databases
{
    public static class ItemSkillsDatabase
    {
        static List<Skill> ItemSkills = new List<Skill>
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
            new Skill(6001, Class.Common, "Dragon Power", ""),
            new Skill(6002, Class.Common, "Ancient Dragon Power", "")
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
            "Icon_Skills.F2P_VIPCustomer_Tex",
            "icon_status.passive_dragonblood",
            "icon_status.passive_dragonblood2"
        };

        public static void SetBroochesIcons()
        {
            for (int i = 0; i < ItemSkills.Count(); i++)
            {
                ItemSkills[i].SetSkillIcon(IconsList[i]);
            }
        }
        public static Skill GetBrooch(uint id)
        {
            if (ItemSkills.Where(x => x.Id == id).Count() > 0)
            {
                return ItemSkills.Where(x => x.Id == id).Single();
            }
            else return null;

        }

        internal static bool TryGetItemSkill(uint itemId, out Skill brooch)
        {
            bool result = false;
            brooch = new Skill(0, Class.None, string.Empty, string.Empty);
            if (ItemSkills.Any(x => x.Id == itemId))
            {
                result = true;
                brooch = ItemSkills.FirstOrDefault(x => x.Id == itemId);
            }
            return result;

        }
    }
}