using System.Collections.Generic;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using TCC.Utilities.Extensions;

namespace TCC.Data.Chat
{
    public static class ChatUtils
    {
        public static string GradeToColorString(RareGrade g)
        {
            switch (g)
            {
                case RareGrade.Common:   return R.Colors.ItemCommonColor.ToHex();
                case RareGrade.Uncommon: return R.Colors.ItemUncommonColor.ToHex();
                case RareGrade.Rare:     return R.Colors.ItemRareColor.ToHex();
                case RareGrade.Superior: return R.Colors.ItemSuperiorColor.ToHex();
                default: return "";
            }
        }
        public static uint GetId(Dictionary<string, string> d, string paramName)
        {
            return uint.Parse(d[paramName]);
        }

        public static long GetItemUid(Dictionary<string, string> d)
        {
            return d.TryGetValue("dbid", out var value) ? long.Parse(value) : 0;
        }

        public static Dictionary<string, string> SplitDirectives(string m)
        {
            var parameters = m.Split('\v');
            if (parameters.Length == 1) return null;
            var retDict = new Dictionary<string, string>();
            for (var i = 1; i < parameters.Length - 1; i += 2)
            {
                retDict.Add(parameters[i], parameters[i + 1]);
            }
            return retDict;
        }
        public static Dictionary<string, string> BuildParametersDictionary(string p)
        {
            //@464UserNameChippyAdded12ItemName@item:88176?dbid:273547775?masterpiece
            //@1613ItemAmount5ItemName@item:179072?dbid:254819647
            var stringPairs = p.Replace("@", "").Split('?');
            var retDict = new Dictionary<string, string>();
            foreach (var stringPair in stringPairs)
            {
                var paremterPair = stringPair.Split(':');
                if (paremterPair.Length == 1)
                {
                    if (paremterPair[0] == "masterpiece") retDict["masterpiece"] = "Masterwork";
                    if (paremterPair[0] == "awakened") retDict["awakened"] = "Awakened";
                    continue;
                }
                retDict[paremterPair[0]] = paremterPair[1];
            }
            return retDict;
        }

        public static string ReplaceParameters(string txt, Dictionary<string, string> pars, bool all)
        {
            var result = "";
            if (!all)
            {
                foreach (var keyVal in pars)
                {
                    var regex = new Regex(Regex.Escape($"{{{keyVal.Key}}}"));
                    result = regex.Replace(txt, $"{{{keyVal.Value}}}", 1);
                    if (txt == result) result = txt.ReplaceFirstOccurrenceCaseInsensitive($"{{{keyVal.Key}}}", $"{{{keyVal.Value}}}");
                    if (txt == result) result = txt.ReplaceFirstOccurrenceCaseInsensitive($"{{{keyVal.Key}", $"{{{keyVal.Value}");
                    txt = result;
                }
            }
            else
            {
                foreach (var keyVal in pars)
                {
                    var regex = new Regex(Regex.Escape($"{{{keyVal.Key}}}"));
                    result = regex.Replace(txt, $"{{{keyVal.Value}}}", 1);
                    if (txt == result) result = txt.ReplaceCaseInsensitive($"{{{keyVal.Key}}}", $"{{{keyVal.Value}}}");
                    if (txt == result) result = txt.ReplaceCaseInsensitive($"{{{keyVal.Key}", $"{{{keyVal.Value}");
                    txt = result;
                }
            }
            return result;
        }

        public static string GetCustomColor(HtmlNode node)
        {
            var ret = "";
            if (node.HasAttributes)
            {
                ret = node.GetAttributeValue("color", "").Substring(1);
                while (ret.Length < 6) { ret = "0" + ret; }
            }
            return ret;
        }
    }
}
