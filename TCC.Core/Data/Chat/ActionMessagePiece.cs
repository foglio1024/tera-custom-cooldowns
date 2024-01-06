using System.Linq;
using System.Windows.Input;
using Nostrum.WPF;
using TCC.Interop.Proxy;

namespace TCC.Data.Chat;

public class ActionMessagePiece : SimpleMessagePiece
{
    bool _isHovered;

    public override bool IsHovered
    {
        get => _isHovered;
        set
        {
            if (_isHovered == value) return;

            _isHovered = value;
            Container?.Pieces.ToSyncList()
                    .OfType<ActionMessagePiece>()
                    .Where(x => x.ChatLinkAction == ChatLinkAction)
                    .ToList()
                    .ForEach(x => x.IsHovered = value);
            InvokePropertyChanged();
        }
    }
    public string ChatLinkAction { get; }
    public override ICommand ClickCommand { get; }

    public ActionMessagePiece(string text, string action) : base(text)
    {
        ChatLinkAction = action;
        ClickCommand = new RelayCommand(_ => StubInterface.Instance.StubClient.ChatLinkAction(ChatLinkAction));
    }
}
