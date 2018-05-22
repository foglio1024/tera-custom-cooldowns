using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TCC.Data;

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
            BaseBorder.Background = new SolidColorBrush(Color.FromRgb(0x20, 0x20, 0x27));

        }

        private void _context_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Val")
            {
                SetRunes(_context.Val);
            }
            else if (e.PropertyName == "Maxed")
            {
                //baseBorder.Background = new SolidColorBrush(Color.FromRgb(0xff,0x98,0xbb));
                MaxBorder.Opacity = 1;
            }
        }
        private int _currentRunes = 0;

        private void SetRunes(int newRunes)
        {
            var diff = newRunes - _currentRunes;

            if (diff == 0) return;
            if (diff > 0)
            {
                for (var i = 0; i < diff; i++)
                {
                    DotsContainer.Children[_currentRunes + i].Opacity = 1;
                }
            }
            else
            {
                //baseBorder.Background = new SolidColorBrush(Color.FromRgb(0x20, 0x20, 0x27));
                MaxBorder.Opacity = 0;

                for (var i = DotsContainer.Children.Count - 1; i >= 0; i--)
                {
                    DotsContainer.Children[i].Opacity = 0;
                }
            }
            _currentRunes = newRunes;
        }

        private Counter _context;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            //lazy way of making sure that DataContext is not null
            while (_context == null)
            {
                _context = (Counter)DataContext;
                Thread.Sleep(500);
            }
            _context.PropertyChanged += _context_PropertyChanged;

        }
    }
}
