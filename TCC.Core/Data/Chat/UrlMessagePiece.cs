using System.Windows.Input;
using Nostrum.WPF;
using TCC.UI.Windows;
using TCC.Utils;

namespace TCC.Data.Chat;

public class UrlMessagePiece : SimpleMessagePiece
{
    bool _isHovered;

    public override bool IsHovered
    {
        get => _isHovered;
        set
        {
            if (_isHovered == value) return;
            _isHovered = value;
            N();
        }
    }
    public override ICommand ClickCommand { get; }

    UrlMessagePiece(string url) : base(url)
    {
        ClickCommand = new RelayCommand(Click);
    }

    public UrlMessagePiece(string text, int fontSize, bool customSize, string col = "") : this(text)
    {
        _fontSize = fontSize;
        _customSize = customSize;
        Color = col;
    }

    void Click()
    {
        try
        {
            Utils.Utilities.OpenUrl(Text);
        }
        catch
        {
            TccMessageBox.Show(SR.UnableToOpenUrl(Text), MessageBoxType.Error);
        }
    }
}