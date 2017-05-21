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
    public partial class ClassWindow : TccWindow
    {
        public ClassWindow()
        {
            InitializeComponent();

        }

        private void TccWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
            SettingsManager.ClassWindowSettings.X = Left;
            SettingsManager.ClassWindowSettings.Y = Top;
            SettingsManager.SaveSettings();
        }

        ClassWindowViewModel _context;
        public ClassWindowViewModel Context
        {
            get => _context;
        }
        private void TccWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitWindow();
            Left = SettingsManager.ClassWindowSettings.X;
            Top = SettingsManager.ClassWindowSettings.Y;
            SetClickThru(SettingsManager.ClassWindowSettings.ClickThru);
            SetVisibility(SettingsManager.ClassWindowSettings.Visibility);

            _context = (ClassWindowViewModel)DataContext;
            _context.PropertyChanged += _context_PropertyChanged;


        }

        private void _context_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            if (e.PropertyName == "CurrentClass")
            {
                switch (_context.CurrentClass)
                {
                    case Class.Warrior:
                        root.ContentTemplate = FindResource("warrior") as DataTemplate;
                        break;
                    case Class.Glaiver:
                        root.ContentTemplate = FindResource("valkyrie") as DataTemplate;
                        break;
                    default:
                        root.ContentTemplate = FindResource("emptyTemplate") as DataTemplate;
                        break;
                }
            }

        }
    }
}
