using System.Windows;
using System.Windows.Controls;
using TCC.Data.Chat;
using TCC.Utils;

namespace TCC.UI.TemplateSelectors
{
    public class ChannelLabelDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? NormalChannelDataTemplate { get; set; }
        public DataTemplate? WhisperChannelDataTemplate { get; set; }
        public DataTemplate? MegaphoneChannelDataTemplate { get; set; }
        public DataTemplate? EnchantChannelDataTemplate { get; set; }
        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if (!(item is ChatMessage m)) return null;
            return m.Channel switch
            {
                ChatChannel.SentWhisper => WhisperChannelDataTemplate,
                ChatChannel.ReceivedWhisper => WhisperChannelDataTemplate,
                ChatChannel.Megaphone => MegaphoneChannelDataTemplate,
                _ => NormalChannelDataTemplate
            };
        }
    }
}
