using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.TemplateSelectors
{
    public class AbnormalityTemplateSelector : DataTemplateSelector
    {
        public AbnormalityTemplateSelector()
        {
        }

        private Action _lambda;

        public DataTemplate RoundTemplate { get; set; }
        public DataTemplate SquareTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            //_lambda = () =>
            //{
            //    SettingsWindowViewModel.AbnormalityShapeChanged -= _lambda;
            //    //_lambda = null;
            //    var cp = (ContentPresenter) container;
            //    cp.ContentTemplateSelector = null;
            //    cp.ContentTemplateSelector = this;
            //};
            //SettingsWindowViewModel.AbnormalityShapeChanged += _lambda;
            return Settings.AbnormalityShape == AbnormalityShape.Round ? RoundTemplate : SquareTemplate;
        }
    }
}
