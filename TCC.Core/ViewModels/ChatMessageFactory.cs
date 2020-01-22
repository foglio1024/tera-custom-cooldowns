using System;
using System.Text;
using System.Windows.Threading;
using TCC.Data;
using TCC.Data.Chat;

namespace TCC.ViewModels
{
    public class ChatMessageFactory
    {
        private readonly Dispatcher _dispatcher;
        public ChatMessageFactory(Dispatcher d)
        {
            _dispatcher = d;
        }

        public ChatMessage CreateMessage(ChatChannel ch = ChatChannel.Say)
        {
            return _dispatcher.InvokeAsync(() => new ChatMessage(ch)).Result;
        }
        public ChatMessage CreateMessage(ChatChannel ch, string author, string msg, ulong authorGameId = 0)
        {
            return _dispatcher.InvokeAsync(() => new ChatMessage(ch, author, msg, authorGameId)).Result;
        }
        public ChatMessage CreateSystemMessage(string template, SystemMessageData msg, ChatChannel ch, string authorOverride = "System")
        {
            return _dispatcher.InvokeAsync(() => new SystemMessage(template, msg, ch) { Author = authorOverride }).Result;
        }
        public ChatMessage CreateLfgMessage(uint authorId, string author, string msg)
        {
            return _dispatcher.InvokeAsync(() => new LfgMessage(authorId, author, msg)).Result;
        }
        public ChatMessage CreateEnchantSystemMessage(string systemMessage)
        {
            return _dispatcher.InvokeAsync(() =>
            {
                var msg = CreateMessage(ChatChannel.Enchant);
                var e = "";

                if (systemMessage.Contains("enchantCount:"))
                {
                    var s = systemMessage.IndexOf("enchantCount:", StringComparison.InvariantCultureIgnoreCase);
                    var ench = systemMessage.Substring(s + "enchantCount:".Length, 1);
                    e = $"+{ench} ";
                }

                var prm = ChatUtils.SplitDirectives(systemMessage);

                msg.Author = prm["UserName"];
                var txt = "{ItemName}";
                txt = ChatUtils.ReplaceParameters(txt, prm, true);
                txt = txt.Replace("{", "");
                txt = txt.Replace("}", "");
                var mp = MessagePieceBuilder.BuildSysMsgItem(txt);
                var sb = new StringBuilder();
                sb.Append("<");
                sb.Append(e);
                sb.Append(mp.Text.Substring(1));
                mp.Text = sb.ToString();
                msg.AddPiece(new SimpleMessagePiece("Successfully enchanted ", App.Settings.FontSize, false, "cccccc"));
                msg.AddPiece(mp);

                return msg;

            }).Result;
        }
    }
}