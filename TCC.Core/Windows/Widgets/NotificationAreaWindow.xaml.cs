using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TCC.Windows.Widgets
{
    public partial class NotificationAreaWindow
    {
        public NotificationAreaWindow(NotificationAreaViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
            ButtonsRef = Buttons;
            MainContent = WindowContent;
            BoundaryRef = Boundary;
            Init(vm.Settings);
            MainContent.Opacity = 1;
        }
    }
}
