using System.Collections.Generic;

namespace TCC.Data.Chat
{
    public static class ChatUtils
    {
        public static string GetItemColor(Item i)
        {
            var CommonColor = "FFFFFF";
            var UncommonColor = "4DCB30";
            var RareColor = "009ED9";
            var SuperiorColor = "EEBE00";

            switch (i.RareGrade)
            {
                case RareGrade.Common:
                    return CommonColor;
                case RareGrade.Uncommon:
                    return UncommonColor;
                case RareGrade.Rare:
                    return RareColor;
                case RareGrade.Superior:
                    return SuperiorColor;
                default:
                    return "";
            }
        }
        public static uint GetId(Dictionary<string, string> d, string paramName)
        {
            return uint.Parse(d[paramName]);
        }

        public static long GetItemUid(Dictionary<string, string> d)
        {
            return d.ContainsKey("dbid") ? long.Parse(d["dbid"]) : 0;
        }
    }
}
