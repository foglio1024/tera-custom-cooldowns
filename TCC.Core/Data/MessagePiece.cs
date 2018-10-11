using System.Windows;
using System.Windows.Media;
using TCC.Annotations;
using TCC.ViewModels;

namespace TCC.Data
{
    public class MessagePiece : TSPropertyChanged
    {
        public readonly ChatChannel Channel;

        public long ItemUid { get; set; }
        public uint ItemId { get; set; }
        public Location Location { [UsedImplicitly] get; set; }
        public string RawLink { get; set; }
        public Money Money { get; set; }

        public BoundType BoundType { get; set; }

        public Thickness Spaces { get; set; }

        public string OwnerName { get; set; }

        public MessagePieceType Type { get; set; }

        public string Text { get; set; }

        public string PlainText => Text.StartsWith("<") ? Text.Substring(1, Text.Length - 2) : Text;

        public SolidColorBrush Color { get; set; }

        private int _size = 18;
        private bool _customSize;
        private bool _isVisible;

        public int Size
        {
            get => _customSize ? _size : Settings.FontSize;
            set
            {
                if (_size == value) return;
                _size = value;
                _customSize = value != Settings.FontSize;
                NPC(nameof(Size));
            }
        }

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (value) SettingsWindowViewModel.FontSizeChanged += OnFontSizeChanged;
                else       SettingsWindowViewModel.FontSizeChanged -= OnFontSizeChanged;

                if (_isVisible == value) return;
                _isVisible = value;
                NPC();
            }
        }

        private Thickness SetThickness(string text)
        {
            double left = 0;
            double right = 0;
            if (text.StartsWith(" "))
            {
                left = 0;
                right = -1;
            }
            if (text.EndsWith(" "))
            {
                right = 4;
                left = -1;
            }

            return new Thickness(left, 0, right, 0);

        }
        public void SetColor(string color = "")
        {
            Dispatcher.Invoke(() =>
            {
                if (color == "")
                {
                    var conv = new Converters.ChatChannelToColor();
                    var col = ((SolidColorBrush)conv.Convert(Channel, null, null, null));
                    Color = col;
                }
                else
                {
                    try
                    {
                        Color = new SolidColorBrush(Utils.ParseColor(color));
                    }
                    catch
                    {
                        var conv = new Converters.ChatChannelToColor();
                        var col = ((SolidColorBrush)conv.Convert(Channel, null, null, null));
                        Color = col;
                    }
                }
            });
        }
        public MessagePiece(string text, MessagePieceType type, ChatChannel ch, int size, bool customSize, string customColor = "") : this(text)
        {
            Channel = ch;
            SetColor(customColor);
            Type = type;

            _size = size;
            _customSize = customSize;

        }
        public MessagePiece(string text)
        {
            Dispatcher = ChatWindowManager.Instance.GetDispatcher();
            Text = text;
            Spaces = SetThickness(text);
            _customSize = false;

        }

        private void OnFontSizeChanged()
        {
            NPC(nameof(Size));
        }

        public MessagePiece(Money money) : this(text: "")
        {
            SetColor();
            Type = MessagePieceType.Money;
            Money = money;
            _customSize = false;
        }
    }
}
