using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using TCC.Controls;
using TCC.Parsing;
using TCC.ViewModels;
using TCC.Windows;

namespace TCC.Data
{
    public struct PrivateChatChannel
    {
        public uint Id;
        public string Name;
        public int Index;
        public bool Joined;
        public PrivateChatChannel(uint id, string name, int index)
        {
            Id = id;
            Name = name;
            Index = index;
            Joined = true;
        }
    }
    public class ChatMessage : TSPropertyChanged
    {
        private ChatChannel channel;
        public ChatChannel Channel
        {
            get => channel;
            set
            {
                if (channel == value) return;
                channel = value;
                NotifyPropertyChanged(nameof(Channel));
            }
        }

        private string timestamp;
        public string Timestamp
        {
            get => timestamp;
            set
            {
                if (timestamp == value) return;
                timestamp = value;
                NotifyPropertyChanged(nameof(timestamp));
            }
        }

        private string rawMessage;
        public string RawMessage
        {
            get => rawMessage;
            set
            {
                if (rawMessage == value) return;
                rawMessage = value;
                NotifyPropertyChanged(nameof(RawMessage));
            }
        }

        private string author;
        public string Author
        {
            get => author;
            set
            {
                if (author == value) return;
                author = value;
                NotifyPropertyChanged(nameof(Author));
            }
        }

        private bool containsPlayerName;
        public bool ContainsPlayerName
        {
            get { return containsPlayerName; }
            set
            {
                if (containsPlayerName == value) return;
                containsPlayerName = value;
            }
        }

        public static PrivateChatChannel[] PrivateChannels = new PrivateChatChannel[8];

        private SynchronizedObservableCollection<MessagePiece> pieces;
        public SynchronizedObservableCollection<MessagePiece> Pieces
        {
            get => pieces;
            set
            {
                if (pieces == value) return;
                pieces = value;
                NotifyPropertyChanged(nameof(Pieces));
            }
        }

        public ChatMessage(ChatChannel ch, string auth, string msg, bool priv = false, uint privChannelNumber = 0)
        {
            _dispatcher = WindowManager.ChatWindow.Dispatcher;
            Pieces = new SynchronizedObservableCollection<MessagePiece>(_dispatcher);

            if (!priv)
            {
                Channel = ch;

            }
            else
            {
                var i = PrivateChannels.FirstOrDefault(x => x.Id == privChannelNumber).Index;
                Channel = (ChatChannel)(PrivateChannels[i].Index + 11);
            }

            RawMessage = msg;
            Author = auth;
            Timestamp = DateTime.Now.ToShortTimeString();
            if (Channel == ChatChannel.Greet)
            {
                ParseDirectMessage(RawMessage, ch);
            }
            else if (Channel == ChatChannel.Emote)
            {
                ParseSocialMessage(msg);
            }
            else
            {
                ParseFormattedMessage(msg);
            }
        }

        public ChatMessage(string systemMessage, SystemMessage m)
        {
            _dispatcher = WindowManager.ChatWindow.Dispatcher;
            Pieces = new SynchronizedObservableCollection<MessagePiece>(_dispatcher);

            var msg = systemMessage.Split('\v');
            var ch = (ChatChannel)m.ChatChannel;
            var text = m.Message;

            Channel = ch;
            RawMessage = systemMessage;
            Author = "System";
            Timestamp = DateTime.Now.ToShortTimeString();

            if (msg.Length == 1)
            {
                //init simple msg
               _dispatcher.Invoke(()=> Pieces.Add(new MessagePiece(text, MessagePieceType.Simple, ch)));
            }
            else
            {
                for (int i = 1; i < msg.Length; i += 2)
                {
                    var key = msg[i];
                    var value = msg[i + 1];

                    text = text.Replace("{" + key + "}", value);
                }
                //init parametrized message
                _dispatcher.Invoke(()=>Pieces.Add(new MessagePiece(text, MessagePieceType.Simple, ch)));
            }
        }

        string pieceOpeningTag = "<FONT";
        string pieceClosingTag = "</FONT>";

        private string ParsePiece(string msg)
        {
            var start = msg.IndexOf(pieceOpeningTag) + pieceOpeningTag.Length;
            if (msg[start] == '>')
            {
                start++;
                var end = msg.IndexOf(pieceClosingTag, start);
                var text = msg.Substring(start, end - start);
                if (Regex.IsMatch(SessionManager.CurrentPlayer.Name, Regex.Escape(text), RegexOptions.IgnoreCase)) ContainsPlayerName = true;

                if (text.IndexOf("WTS", StringComparison.InvariantCultureIgnoreCase) >= 0 && Channel == ChatChannel.Global) Channel = ChatChannel.Trade;
                if (text.IndexOf("WTB", StringComparison.InvariantCultureIgnoreCase) >= 0 && Channel == ChatChannel.Global) Channel = ChatChannel.Trade;
                _dispatcher.Invoke(() => Pieces.Add(new MessagePiece(ReplaceGtLt(text), MessagePieceType.Simple, Channel)));

                return msg = msg.Substring(end + pieceClosingTag.Length);
            }
            else
            {
                var colorString = msg.Substring(msg.IndexOf("COLOR=") + 8, 6);

                var textStart = msg.IndexOf("&lt;");
                var textEnd = msg.IndexOf("&gt;") + 4;
                var text = ReplaceGtLt(msg.Substring(textStart, textEnd - textStart));
                MessagePieceType t;
                if (msg[msg.IndexOf("#####") - 1] == '1')
                {
                    t = MessagePieceType.Item;
                }
                else
                {
                    t = MessagePieceType.Quest;
                }

                int id = 0;
                ulong uid = 0;
                string ownerName = "";
                if (t != MessagePieceType.Quest)
                {
                    var idStart = msg.IndexOf("#####") + 5;
                    var idEnd = msg.IndexOf("@", idStart);
                    id = int.Parse(msg.Substring(idStart, idEnd - idStart));

                    var uidStart = msg.IndexOf("@", msg.IndexOf("#####") + 6) + 1;
                    var uidLength = msg.IndexOf("@", uidStart) - uidStart;
                    uid = ulong.Parse(msg.Substring(uidStart, uidLength));

                    var ownerNameStart = msg.IndexOf("@", uidStart + uidLength) + 1;
                    var ownerNameEnd = msg.IndexOf(">", ownerNameStart) - 1;
                    ownerName = msg.Substring(ownerNameStart, ownerNameEnd - ownerNameStart);
                }

                _dispatcher.Invoke(() => Pieces.Add(new MessagePiece(text, t, Channel, colorString, uid, id, ownerName)));

                return msg = msg.Substring(msg.IndexOf(pieceClosingTag) + pieceClosingTag.Length);

            }

        }

        private void ParseFormattedMessage(string msg)
        {
            var piecesCount = Regex.Matches(msg, pieceClosingTag).Count;
            for (int i = 0; i < piecesCount; i++)
            {
                msg = ParsePiece(msg); //adds piece to list and cuts msg
            }
        }
        private void ParseDirectMessage(string msg, ChatChannel ch)
        {
            _dispatcher.Invoke(() => Pieces.Add(new MessagePiece(msg, MessagePieceType.Simple, ch)));
        }
        private void ParseSocialMessage(string msg)
        {
            string begin = "@social:";
            var start = msg.IndexOf(begin);
            if (start == -1)
            {
                _dispatcher.Invoke(() => Pieces.Add(new MessagePiece(Author + " " + msg, MessagePieceType.Simple, Channel)));
                return;
            }
            start += begin.Length;
            var id = UInt32.Parse(msg.Substring(start));
            var text = SocialDatabase.Social[id].Replace("{Name}", Author);
            _dispatcher.Invoke(() => Pieces.Add(new MessagePiece(text, MessagePieceType.Simple, Channel)));

        }
        private string ReplaceGtLt(string msg)
        {
            msg = msg.Replace("&lt;", "<");
            msg = msg.Replace("&gt;", ">");
            return msg;
        }
    }
    public enum MessagePieceType
    {
        Simple,
        Item,
        Quest,
        Point_of_interest
    }
    public class MessagePiece : TSPropertyChanged
    {
        ulong itemUid;
        public ulong ItemUid
        {
            get => itemUid;
            set
            {
                if (itemUid == value) return;
                itemUid = value;
            }
        }

        int itemId;
        public int ItemId
        {
            get => itemId;
            set
            {
                if (itemId == value) return;
                itemId = value;
            }
        }

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

        public MessagePiece(string text, MessagePieceType type, ChatChannel ch, string customColor = "", ulong itemUid = 0, int itemId = 0, string ownerName = "")
        {
            _dispatcher = WindowManager.ChatWindow.Dispatcher;

            Type = type;
            Text = text;

            if (customColor == "")
            {
                var conv = new ChatColorConverter();
                var col = ((SolidColorBrush)conv.Convert(ch, null, null, null));
                Color = col;
            }
            else
            {
                Color = new SolidColorBrush(ParseColor(customColor));
            }

            ItemUid = itemUid;
            ItemId = itemId;
            OwnerName = ownerName;
        }
    }
}
