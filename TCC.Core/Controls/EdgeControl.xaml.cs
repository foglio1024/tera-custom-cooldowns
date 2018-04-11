using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using TCC.ViewModels;

namespace TCC.Controls
{
    /// <summary>
    /// Logica di interazione per EdgeControl.xaml
    /// </summary>
    public partial class EdgeControl : UserControl
    {
        public EdgeControl()
        {
            InitializeComponent();
            //baseBorder.Background = (SolidColorBrush)App.Current.FindResource("Colors.App.DefaultBackground");

        }
        private int _currentEdge = 0;
        void SetEdge(int newEdge)
        {
            var diff = newEdge - _currentEdge;

            if (diff == 0) return;
            if (diff > 0)
            {
                for (int i = 0; i < diff; i++)
                {
                    edgeContainer.Children[_currentEdge + i].Opacity = 1;
                }
            }
            else
            {
                //baseBorder.Background = (SolidColorBrush)App.Current.FindResource("Colors.App.DefaultBackground");
                maxBorder.Opacity = 0;

                for (int i = edgeContainer.Children.Count - 1; i >= 0; i--)
                {
                    edgeContainer.Children[i].Opacity = 0;
                }
            }
            _currentEdge = newEdge;
        }

        Counter _context;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            _context = (Counter)DataContext;
            _context.PropertyChanged += _context_PropertyChanged;
        }

        private void _context_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Val")
            {
                SetEdge(_context.Val);
            }
            else if (e.PropertyName == "Maxed")
            {
                //baseBorder.Background = new SolidColorBrush(Colors.Red);
                maxBorder.Opacity = 1;
            }
        }

    }
}
