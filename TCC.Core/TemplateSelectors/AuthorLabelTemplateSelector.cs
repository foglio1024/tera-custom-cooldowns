using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TCC.Data;

namespace TCC.TemplateSelectors
{
    public class AuthorLabelTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DefaultAuthorTemplate { get; set; }
        public DataTemplate SystemAuthorTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if(item == null) return null;
            ChatMessage m = item as ChatMessage;

            switch (m.Channel)
            {
                case ChatChannel.System:
                    return SystemAuthorTemplate;
                case ChatChannel.Notify:
                    return SystemAuthorTemplate;
                case ChatChannel.Event:
                    return SystemAuthorTemplate;
                case ChatChannel.Error:
                    return SystemAuthorTemplate;
                case ChatChannel.Group:
                    return SystemAuthorTemplate;
                case ChatChannel.GuildNotice:
                    return SystemAuthorTemplate;
                case ChatChannel.Deathmatch:
                    return SystemAuthorTemplate;
                case ChatChannel.ContractAlert:
                    return SystemAuthorTemplate;
                case ChatChannel.GroupAlerts:
                    return SystemAuthorTemplate;
                case ChatChannel.Loot:
                    return SystemAuthorTemplate;
                case ChatChannel.Exp:
                    return SystemAuthorTemplate;
                case ChatChannel.Money:
                    return SystemAuthorTemplate;
                case ChatChannel.Emote:
                    return SystemAuthorTemplate;
                case ChatChannel.TCC:
                    return SystemAuthorTemplate;
                default:
                    return DefaultAuthorTemplate;
            }
        }
    }
}
