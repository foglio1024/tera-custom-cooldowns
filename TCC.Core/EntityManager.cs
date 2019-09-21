using System.Collections.Generic;

namespace TCC
{
    public static class EntityManager
    {
        public static ulong FoglioEid = 0;

        //TODO: merge this to monsters-override.xml and make it configurable
        public static bool Pass(uint zoneId, uint templateId)
        {
            switch (zoneId)
            {
                case 950 when templateId == 1002: //skip goddamn toy tanks
                case 210 when templateId == 4000: //skip HHP4 lament warriors
                    return false;
                default:
                    return true;
            }
        }
        public static void SetEncounter(float curHP, float maxHP)
        {
            if (maxHP > curHP)
            {
                Game.Encounter = true;
            }
            else if (maxHP == curHP || curHP == 0)
            {
                Game.Encounter = false;
            }
        }
    }
}
