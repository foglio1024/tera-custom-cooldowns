using System;
using System.Windows;
using System.Windows.Media;
using TCC.Windows;

namespace TCC.Data
{
    public class MessagePiece : TSPropertyChanged
    {
        private ChatChannel _channel;

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

        private Color ParseColor(string col)
        {
            return System.Windows.Media.Color.FromRgb(
                                Convert.ToByte(col.Substring(0, 2), 16),
                                Convert.ToByte(col.Substring(2, 2), 16),
                                Convert.ToByte(col.Substring(4, 2), 16));
        }
        private Thickness SetThickness(string text)
        {
            double left = 1;
            double right = -1;
            if (text.StartsWith(" "))
            {
                left = 3;
            }
            if (text.EndsWith(" "))
            {
                right = 3;
            }

            return new Thickness(left, 0, right, 0);

        }
        private void SetColor(string color = "")
        {
            _dispatcher.Invoke(() =>
            {
                if(color == "")
                {
                    var conv = new Converters.ChatColorConverter();
                    var col = ((SolidColorBrush)conv.Convert(_channel, null, null, null));
                    Color = col;
                }
                else
                {
                    Color = new SolidColorBrush(ParseColor(color));
                }
            });
        }
        public MessagePiece(string text, MessagePieceType type, ChatChannel ch, string customColor = "", long itemUid = 0, uint itemId = 0, string ownerName = "", BoundType b = BoundType.None)
        {
            _dispatcher = WindowManager.ChatWindow.Dispatcher;

            _channel = ch;
            SetColor(customColor);

            Type = type;
            Text = text;
            BoundType = b;

            Spaces = SetThickness(text);

            ItemUid = itemUid;
            ItemId = itemId;
            OwnerName = ownerName;
        }
        public MessagePiece(Money money, ChatChannel ch)
        {
            _dispatcher = WindowManager.ChatWindow.Dispatcher;
            _dispatcher.Invoke(() =>
            {
                var conv = new Converters.ChatColorConverter();
                var col = ((SolidColorBrush)conv.Convert(ch, null, null, null));
                Color = col;
            }); Type = MessagePieceType.Money;
            Money = money;
        }
    }
}
