using HtmlAgilityPack;
using Nostrum.Extensions;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace TCC.Utils;

public static class ChatUtils
{
        
    public static uint GetId(Dictionary<string, string> d, string paramName)
    {
        return uint.Parse(d[paramName]);
    }

    public static long GetItemUid(Dictionary<string, string> d)
    {
        return d.TryGetValue("dbid", out var value) ? long.Parse(value) : 0;
    }

    public static Dictionary<string, string>? SplitDirectives(string m)
    {
        var parameters = m.Split('\v');
        return SplitDirectives(parameters);
    }
    public static Dictionary<string, string>? SplitDirectives(params string[] parameters)
    {
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
            foreach (var (key, value) in pars)
            {
                var regex = new Regex(Regex.Escape($"{{{key}}}"));
                result = regex.Replace(txt, $"{{{value}}}", 1);
                if (txt == result) result = txt.ReplaceFirstOccurrenceCaseInsensitive($"{{{key}}}", $"{{{value}}}");
                if (txt == result) result = txt.ReplaceFirstOccurrenceCaseInsensitive($"{{{key}", $"{{{value}");
                txt = result;
            }
        }
        else
        {
            foreach (var (key, value) in pars)
            {
                var regex = new Regex(Regex.Escape($"{{{key}}}"));
                result = regex.Replace(txt, $"{{{value}}}", int.MaxValue);
                if (txt == result) result = txt.ReplaceCaseInsensitive($"{{{key}}}", $"{{{value}}}");
                if (txt == result) result = txt.ReplaceCaseInsensitive($"{{{key}", $"{{{value}");
                txt = result;
            }
        }
        return result;
    }

    public static string GetCustomColor(HtmlNode node)
    {
        var ret = "";
        if (!node.HasAttributes) return ret;
        ret = node.GetAttributeValue("color", "").Substring(1);
        while (ret.Length < 6) { ret = "0" + ret; }
        return ret;
    }

    public static string GetPlainText(string msg)
    {
        var sb = new StringBuilder();
        try
        {
            var html = new HtmlDocument(); html.LoadHtml(msg);
            var htmlPieces = html.DocumentNode.ChildNodes;

            foreach (var htmlPiece in htmlPieces)
            {
                sb.Append(htmlPiece.InnerText);
            }
        }
        catch
        {
            sb.Append(msg);
        }

        return sb.ToString();
    }

    public static string Font(string msg, string color = "", int size = -1)
    {
        if (color.StartsWith("#")) color = color.Replace("#", "");
        return $"<font{(color == "" ? "" : $" color=\"#{color}\"")}{(size == -1 ? "" : $" size=\"{size}\"")}>{msg}</font>";
    }
}