using System.Windows;
using System.Windows.Controls;
using TCC.Data;

namespace TCC.TemplateSelectors
{
    public class ChannelLabelDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NormalChannelDataTemplate { get; set; }
        public DataTemplate WhisperChannelDataTemplate { get; set; }
        public DataTemplate MegaphoneChannelDataTemplate { get; set; }
        public DataTemplate EnchantChannelDataTemplate { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var m = item as ChatMessage;
            if (m == null) return null;
            switch (m.Channel)
            {
                case ChatChannel.SentWhisper:
                    return WhisperChannelDataTemplate;
                case ChatChannel.ReceivedWhisper:
                    return WhisperChannelDataTemplate;
                case ChatChannel.Megaphone:
                    return MegaphoneChannelDataTemplate;
                case ChatChannel.Enchant12:
                    return EnchantChannelDataTemplate;
                case ChatChannel.Enchant15:
                    return EnchantChannelDataTemplate;
                default:
                    return NormalChannelDataTemplate;
            }
        }
    }
}
