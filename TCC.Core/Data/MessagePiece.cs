using System.Windows;
using System.Windows.Media;
using TCC.ViewModels;

namespace TCC.Data
{
    public class MessagePiece : TSPropertyChanged
    {
        public  ChatChannel Channel;

        long itemUid;
        public long ItemUid
        {
            get => itemUid;
            set
            {
                if (itemUid == value) return;
                itemUid = value;
            }
        }

        uint itemId;
        public uint ItemId
        {
            get => itemId;
            set
            {
                if (itemId == value) return;
                itemId = value;
            }
        }

        public Location Location { get; set; }
        public string RawLink { get; set; }
        public Money Money { get; set; }

        public BoundType BoundType { get; set; }

        public Thickness Spaces { get; set; }

        string ownerName;
        public string OwnerName
        {
            get => ownerName;
            set
            {
                if (ownerName == value) return;
                ownerName = value;
            }
        }

        MessagePieceType type;
        public MessagePieceType Type
        {
            get => type;
            set
            {
                if (type == value) return;
                type = value;
            }
        }

        string text;
        public string Text
        {
            get => text;
            set
            {
                if (text == value) return;
                text = value;
            }
        }

        public string PlainText
        {
            get
            {
                return Text.StartsWith("<")? Text.Substring(1, Text.Length - 2) : Text;
            }
        }

        SolidColorBrush color;
        public SolidColorBrush Color
        {
            get => color;
            set
            {
                if (color == value) return;
                color = value;
            }
        }

        private int _size = 18;
        private bool _customSize;

        public int Size
        {
            get => _customSize ? _size : SettingsManager.FontSize;
            set
            {
                if (_size == value) return;
                _size = value;
                _customSize = value != SettingsManager.FontSize;
                NPC(nameof(Size));
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
            _dispatcher.Invoke(() =>
            {
                if(color == "")
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
            _dispatcher = ChatWindowManager.Instance.GetDispatcher();
            WindowManager.Settings.Dispatcher.Invoke(() => ((SettingsWindowViewModel)WindowManager.Settings.DataContext).PropertyChanged += MessagePiece_PropertyChanged);

            Text = text;
            Spaces = SetThickness(text);
            _customSize = false;

        }

        private void MessagePiece_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "FontSize")
            {
                NPC(nameof(Size));
            }
        }

        public MessagePiece(Money money, ChatChannel ch) : this(text:"")
        {
            SetColor("");
            Type = MessagePieceType.Money;
            Money = money;
            _customSize = false;
        }
    }
}
