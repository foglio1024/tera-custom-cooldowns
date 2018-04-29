using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.Data
{
    public static class InventoryManager
    {
        private const uint StartId = 88273;
        private const uint TotalWeapons = 13;
        private const uint TotalArmors = 13;
        private const uint TotalGloves = 3;
        private const uint TotalBoots = 3;
        private const uint TotalJewels = 3 + 3 + 1;

        private static readonly uint TotalTierItems = TotalWeapons + TotalArmors + TotalGloves + TotalBoots + 1;
        private static readonly uint TotalItems = TotalTierItems * 4 + TotalJewels * 4;

        public static bool TryParseGear(uint id, out Tuple<GearTier, GearPiece> parsedGear)
        {
            parsedGear = null;
            if (id < StartId || id >= StartId + TotalItems) return false;

            var offset = id - StartId;  //offset = 97
            uint tier, tierOffset, tierIndex, itemOffsetInTier;
            if (offset < TotalTierItems * 4)
            {
                tier = offset / TotalTierItems; // tier = 2 (High)
                tierOffset = tier * TotalTierItems; //tierOffset = 66
                tierIndex = StartId + tierOffset; // tierIndex = 88339
                itemOffsetInTier = id - tierIndex; // itemOffsetInTier = 31
                if (itemOffsetInTier < TotalWeapons)
                {
                    parsedGear = new Tuple<GearTier, GearPiece>((GearTier)tier, GearPiece.Weapon);
                    return true; 
                }

                if (itemOffsetInTier < TotalWeapons + TotalArmors)
                {
                    parsedGear = new Tuple<GearTier, GearPiece>((GearTier)tier, GearPiece.Armor); //31<26
                    return true;

                }

                if (itemOffsetInTier < TotalWeapons + TotalArmors + TotalGloves)
                {
                    parsedGear = new Tuple<GearTier, GearPiece>((GearTier)tier, GearPiece.Hands); //31<29
                    return true;

                }

                if (itemOffsetInTier < TotalWeapons + TotalArmors + TotalGloves + TotalBoots)
                {
                    parsedGear = new Tuple<GearTier, GearPiece>((GearTier)tier, GearPiece.Feet); //31<31
                    return true;

                }

                if (itemOffsetInTier < TotalWeapons + TotalArmors + TotalGloves + TotalBoots + 1)
                {
                    parsedGear = new Tuple<GearTier, GearPiece>((GearTier)tier, GearPiece.Belt); //31<32
                    return true;

                }
                return false;
            }
            else //88412, offset = 139
            {
                tier = (offset - TotalTierItems * 4) / TotalJewels; // tier = 1 (Mid)
                tierOffset = tier * TotalJewels; //7
                tierIndex = StartId + tierOffset + TotalTierItems * 4; //88412
                itemOffsetInTier = id - tierIndex; //0

                parsedGear = new Tuple<GearTier, GearPiece>((GearTier)tier, (GearPiece)(itemOffsetInTier + 5));
                return true;
            }
            return false;
        }

    }

}

