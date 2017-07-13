using System;
using System.Windows;
using System.Windows.Media;
using TCC.Windows;

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

        private Color ParseColor(string col)
        {
            return System.Windows.Media.Color.FromRgb(
                                Convert.ToByte(col.Substring(0, 2), 16),
                                Convert.ToByte(col.Substring(2, 2), 16),
                                Convert.ToByte(col.Substring(4, 2), 16));
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
                    var conv = new Converters.ChatColorConverter();
                    var col = ((SolidColorBrush)conv.Convert(Channel, null, null, null));
                    Color = col;
                }
                else
                {
                    Color = new SolidColorBrush(ParseColor(color));
                }
            });
        }
        public MessagePiece(string text, MessagePieceType type, ChatChannel ch, string customColor = "") : this(text)
        {
            Channel = ch;

            SetColor(customColor);

            Type = type;
        }
        public MessagePiece(string text)
        {
            _dispatcher = WindowManager.ChatWindow.Dispatcher;
            Text = text;
            Spaces = SetThickness(text);
        }
        public MessagePiece(Money money, ChatChannel ch) : this(text:"")
        {
            SetColor("");
            Type = MessagePieceType.Money;
            Money = money;
        }
    }
}
