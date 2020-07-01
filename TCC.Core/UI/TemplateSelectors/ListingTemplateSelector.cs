using System.Windows;
using System.Windows.Controls;
using TCC.Data;

namespace TCC.UI.TemplateSelectors
{
    public class ListingTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? Default { get; set; }
        public DataTemplate? Temp { get; set; }

        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            var lfg = (Listing) item;
            return lfg != null && lfg.Temp ? Temp : Default;
        }
    }
}