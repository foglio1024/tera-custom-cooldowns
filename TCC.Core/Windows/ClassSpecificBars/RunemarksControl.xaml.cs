using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Logica di interazione per RunemarksControl.xaml
    /// </summary>
    public partial class RunemarksControl : UserControl
    {
        public RunemarksControl()
        {
            InitializeComponent();
            baseBorder.Background = new SolidColorBrush(Color.FromRgb(0x20, 0x20, 0x27));

        }

        private void _context_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Val")
            {
                SetRunes(_context.Val);
            }
            else if (e.PropertyName == "Maxed")
            {
                baseBorder.Background = new SolidColorBrush(Color.FromRgb(255,255,255));
            }
        }
        private int _currentRunes = 0;

        private void SetRunes(int newRunes)
        {
            var diff = newRunes - _currentRunes;

            if (diff == 0) return;
            if (diff > 0)
            {
                for (int i = 0; i < diff; i++)
                {
                    dotsContainer.Children[_currentRunes + i].Opacity = 1;
                }
            }
            else
            {
                baseBorder.Background = new SolidColorBrush(Color.FromRgb(0x20, 0x20, 0x27));

                for (int i = dotsContainer.Children.Count - 1; i >= 0; i--)
                {
                    dotsContainer.Children[i].Opacity = 0;
                }
            }
            _currentRunes = newRunes;
        }

        Counter _context;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            _context = (Counter)DataContext;
            _context.PropertyChanged += _context_PropertyChanged;

        }
    }
}
