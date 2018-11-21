using System.Windows;
using System.Windows.Controls;
using TCC.Data;
using TCC.Data.Chat;

namespace TCC.TemplateSelectors
{
    public class AuthorLabelTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DefaultAuthorTemplate { get; set; }
        public DataTemplate SystemAuthorTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if(item == null) return null;
            var m = (ChatMessage) item;

            switch (m.Channel)
            {
                case ChatChannel.System:
                case ChatChannel.Damage:
                case ChatChannel.Notify:
                case ChatChannel.Event:
                case ChatChannel.Error:
                case ChatChannel.Group:
                case ChatChannel.GuildNotice:
                case ChatChannel.Deathmatch:
                case ChatChannel.ContractAlert:
                case ChatChannel.GroupAlerts:
                case ChatChannel.Loot:
                case ChatChannel.Exp:
                case ChatChannel.Money:
                case ChatChannel.Emote:
                case ChatChannel.TCC:
                case ChatChannel.Death:
                case ChatChannel.Ress:
                case ChatChannel.Quest:
                case ChatChannel.Friend:
                case ChatChannel.SystemDefault:
                case ChatChannel.WorldBoss:
                case ChatChannel.Laurel:
                case ChatChannel.Guardian:
                    return SystemAuthorTemplate;
                case ChatChannel.Private7:
                case ChatChannel.Private8:
                    if(m.Author == "System" ||string.IsNullOrWhiteSpace(m.Author) || string.IsNullOrEmpty(m.Author)) return SystemAuthorTemplate;
                    return DefaultAuthorTemplate;
                default:
                    return DefaultAuthorTemplate;
            }
        }
    }
}
