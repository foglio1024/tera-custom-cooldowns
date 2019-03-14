using System;
using System.Windows;
using System.Windows.Media.Animation;
using TCC.ViewModels;

namespace TCC.Controls.Classes
{
    /// <summary>
    /// Logica di interazione per ArcherLayout.xaml
    /// </summary>
    public partial class ArcherLayout
    {
        private ArcherLayoutVM _context;
        private DoubleAnimation _an;
        private DoubleAnimation _an2;

        public ArcherLayout()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _context = (ArcherLayoutVM)DataContext;
            _an = new DoubleAnimation { Duration = TimeSpan.FromMilliseconds(150) };
            _an2 = new DoubleAnimation();

            Timeline.SetDesiredFrameRate(_an, 20);
            Timeline.SetDesiredFrameRate(_an2, 30);

            _context.Focus.EmpoweredBuffStarted += OnFocusXStarted;
            _context.Focus.BaseStacksChanged += OnStacksChanged;
        }


        private void OnStacksChanged(int stacks)
        {
            Dispatcher.Invoke(() => _an2.To = stacks / 10D * 280 + 42);
            Dispatcher.Invoke(() => _an2.Duration = TimeSpan.FromMilliseconds(150));
            Dispatcher.Invoke(() => SecReArc.BeginAnimation(Arc.EndAngleProperty, _an2));

            //if (ArcherFocusTracker.IsFocusXRunning)
            //{
            //}
            //else
            //{
            //    //Dispatcher.Invoke(() => MainReArc.BeginAnimation(Arc.EndAngleProperty, _an2));
            //}
        }

        private void OnFocusXStarted(long duration)
        {
            Dispatcher.Invoke(() =>
            {
                _an.From = 318;
                _an.To = 42;
                _an.Duration = TimeSpan.FromMilliseconds(duration);
                MainReArc.BeginAnimation(Arc.EndAngleProperty, _an);
            });
        }
    }
}
