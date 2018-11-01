using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GongSolutions.Wpf.DragDrop.Utilities;

namespace TCC.Controls.Classes.Elements
{


    /// <summary>
    /// Logica di interazione per EdgeRhombControl.xaml
    /// </summary>
    public partial class EdgeRhombControl : EdgeControlBase
    {
        public EdgeRhombControl()
        {
            InitializeComponent();
        }

        public override List<FrameworkElement> EdgeElements
        {
            get
            {
                var ret = new List<FrameworkElement>();
                foreach (FrameworkElement child in Container.Children)
                {
                    ret.Add(child);
                }
                return ret.ToList();
            }
        }
    }
}
