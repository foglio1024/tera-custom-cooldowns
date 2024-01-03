using System.Windows;
using System.Windows.Controls;
using TCC.Data.Chat;

namespace TCC.UI.TemplateSelectors;

public class MessagePieceDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate? SimpleTemplate { get; set; }
    public DataTemplate? ActionTemplate { get; set; }
    public DataTemplate? MoneyTemplate { get; set; }
    public DataTemplate? IconTemplate { get; set; }
    public DataTemplate? UrlTemplate { get; set; }
    public DataTemplate? TranslationIndicatorTemplate { get; set; }

    public override DataTemplate? SelectTemplate(object? item, DependencyObject container)
    {
        return item switch
        {
            ActionMessagePiece => ActionTemplate,
            IconMessagePiece => IconTemplate,
            MoneyMessagePiece => MoneyTemplate,
            UrlMessagePiece => ActionTemplate,
            TranslationIndicatorPiece => TranslationIndicatorTemplate,
            _ => SimpleTemplate
        };
    }
}