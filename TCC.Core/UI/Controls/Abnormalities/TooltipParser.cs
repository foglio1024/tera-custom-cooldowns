using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Nostrum.WPF;
using Colors = TCC.R.Colors;

namespace TCC.UI.Controls.Abnormalities;

public class TooltipParser
{
    private const string GoodMarker = "H_W_GOOD";
    private const string BadMarker = "H_W_BAD";
    private const string CustomMarker = "H_W_CSTM";
    private const string EndMarker = "COLOR_END";

    private const string CarriageReturn1 = "$BR";
    private const string CarriageReturn2 = "<br>";

    private string _t;

    private readonly List<Inline> _ret;

    public TooltipParser(string s)
    {
        _ret = new List<Inline>();
        _t = s;
        Clean();
        CorrectOrder();
        ReplaceHTML();
    }

    public List<Inline> Parse()
    {
        var pieces = SplitOn(_t, EndMarker);

        for (var i = 0; i < pieces.Length; i++)
        {
            var piece = pieces[i];

            if (i == pieces.Length - 1)
            {
                Add(piece);
            }
            else
            {
                if (ParseGood(piece)) continue;
                if (ParseBad(piece)) continue;
                if (ParseCustom(piece)) continue;
                System.Diagnostics.Debug.WriteLine("Failed to parse piece");
            }
        }

        return _ret;
    }

    private void Clean()
    {
        _t = _t.Replace(CarriageReturn1, "\n")
            .Replace(CarriageReturn2, "\n")
            .Replace("color = ", "color=")
            .Replace("=\"", "='")
            .Replace("\">", "'>");
    }

    private void CorrectOrder()
    {
        var correctionSplit = _t.Split('$');
        var swapped = false;
        for (var i = 0; i < correctionSplit.Length - 1; i++)
        {
            if (!correctionSplit[i].StartsWith(EndMarker)
                || correctionSplit[i - 1].StartsWith(GoodMarker)
                || correctionSplit[i - 1].StartsWith(BadMarker)) continue;

            if (correctionSplit[i + 1].StartsWith(GoodMarker))
            {
                correctionSplit[i] = correctionSplit[i].Replace(EndMarker, $"${GoodMarker}");
                correctionSplit[i + 1] = correctionSplit[i + 1].Replace(GoodMarker, $"${EndMarker}");
                swapped = true;
            }
            else if (correctionSplit[i + 1].StartsWith(BadMarker))
            {
                correctionSplit[i] = correctionSplit[i].Replace(EndMarker, $"${BadMarker}");
                correctionSplit[i + 1] = correctionSplit[i + 1].Replace(BadMarker, $"${EndMarker}");
                swapped = true;
            }
        }

        if (!swapped) return;
        _t = "";
        foreach (var s1 in correctionSplit) _t += s1;
    }

    private void ReplaceHTML()
    {
        while (_t.Contains("<font"))
            _t = _t.Replace("<font color='", $"${CustomMarker}")
                .Replace("'>", "")
                .Replace("</font>", $"${EndMarker}");
    }

    private bool ParseGood(string piece)
    {
        if (!IsGood(piece)) return false;
        var d = SplitOn(piece, GoodMarker);

        Add(d[0]);
        AddFormatted(d[1], Colors.TooltipGoodColor);
        return true;
    }

    private bool ParseBad(string piece)
    {
        if (!IsBad(piece)) return false;
        var d = SplitOn(piece, BadMarker);

        Add(d[0]);
        AddFormatted(d[1], Colors.TooltipBadColor);
        return true;
    }

    private bool ParseCustom(string piece)
    {
        if (!IsCustom(piece)) return false;
        var d = SplitOn(piece, CustomMarker);

        var txt = d[1][7..];
        var col = d[1][..7];
        var cstm = new {Text = txt, Color = col};
        Add(d[0]);
        AddFormatted(cstm.Text, MiscUtils.ParseColor(cstm.Color));

        return true;
    }

    private void Add(string content)
    {
        _ret.Add(new Run(content));
    }

    private void AddFormatted(string content, Color color)
    {
        _ret.Add(new Run(content) {Foreground = new SolidColorBrush(color), FontWeight = FontWeights.DemiBold});
    }

    private static string[] SplitOn(string input, string marker)
    {
        return input.Split(new[] {$"${marker}"}, StringSplitOptions.None);
    }

    private static bool IsGood(string input)
    {
        return input.Contains($"${GoodMarker}");
    }

    private static bool IsBad(string input)
    {
        return input.Contains($"${BadMarker}");
    }

    private static bool IsCustom(string input)
    {
        return input.Contains($"${CustomMarker}");
    }
}