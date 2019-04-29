using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TCC.Data;

namespace TCC.TemplateSelectors
{
    // When I'll come back to this I'll ask myself how high and drunk I was when I wrote this :lul:
    public class GroupWindowTemplateSelector : DataTemplateSelector, IValueConverter 
    {
        public DataTemplate SingleColumn { get; set; }
        public DataTemplate RoleColumns { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return item != null && (GroupWindowLayout) item == GroupWindowLayout.SingleColumn ? SingleColumn : RoleColumns;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return SelectTemplate(value, null);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
