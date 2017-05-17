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
using System.Windows.Shapes;
using TCC.ViewModels;

namespace TCC.Windows
{
    /// <summary>
    /// Logica di interazione per WarriorBar.xaml
    /// </summary>
    public partial class WarriorBar : TccWindow
    {
        public WarriorBar()
        {
            InitializeComponent();

        }

        private void TccWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void TccWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitWindow();

            mainSkills.ItemsSource = WarriorBarManager.Instance.MainSkills;
            secSkills.ItemsSource = WarriorBarManager.Instance.SecondarySkills;
            otherSkills.ItemsSource = WarriorBarManager.Instance.OtherSkills;
            gambleCD.DataContext = WarriorBarManager.Instance.DeadlyGamble;
            gambleBuff.DataContext = WarriorBarManager.Instance.DeadlyGambleBuff;
        }
    }
}
