using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TCC.ViewModels;

namespace TCC.Controls
{
    /// <summary>
    /// Logica di interazione per WarriorBar.xaml
    /// </summary>
    public partial class WarriorBar : UserControl
    {
        public WarriorBar()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            gambleCD.DataContext = ((WarriorBarManager)DataContext).DeadlyGamble;
            gambleBuff.DataContext = ((WarriorBarManager)DataContext).DeadlyGambleBuff;
            edgeBar.DataContext = ((WarriorBarManager)DataContext).EdgeCounter;
            hpText.DataContext = ((WarriorBarManager)DataContext).HP;
            mpText.DataContext = ((WarriorBarManager)DataContext).MP;
            reText.DataContext = ((WarriorBarManager)DataContext).ST;
        }
    }
}
