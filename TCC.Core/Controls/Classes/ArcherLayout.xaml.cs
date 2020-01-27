using System;
using System.Windows;
using System.Windows.Media.Animation;
using Nostrum.Controls;
using Nostrum.Factories;
using TCC.ViewModels;

namespace TCC.Controls.Classes
{
    public partial class ArcherLayout
    {
        private ArcherLayoutVM _context;
        private DoubleAnimation _an;
        private DoubleAnimation _an2;

        public ArcherLayout()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _context = (ArcherLayoutVM)DataContext;
            _an = AnimationFactory.CreateDoubleAnimation(150, 42, 318, framerate: 20);
            _an2 = AnimationFactory.CreateDoubleAnimation(150, 0, framerate: 30);

            _context.Focus.EmpoweredBuffStarted += OnFocusXStarted;
            _context.Focus.BaseStacksChanged += OnStacksChanged;
        }


        private void OnStacksChanged(int stacks)
        {
            Dispatcher?.InvokeAsync(() =>
            {
                _an2.To = stacks / 10D * 280 + 42;
                _an2.Duration = TimeSpan.FromMilliseconds(150);
                SecReArc.BeginAnimation(Arc.EndAngleProperty, _an2);
            });

        }

        private void OnFocusXStarted(long duration)
        {
            Dispatcher?.Invoke(() =>
            {
                _an.Duration = TimeSpan.FromMilliseconds(duration);
                MainReArc.BeginAnimation(Arc.EndAngleProperty, _an);
            });
        }
    }
}
