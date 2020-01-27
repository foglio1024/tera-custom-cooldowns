using System.Windows;
using System.Windows.Controls.Primitives;
using Nostrum.Factories;
using TCC.Publisher.ViewModels;

namespace TCC.Publisher.Windows
{
    public partial class PublisherWindow : Window
    {
        private PublisherVM _vm { get; }

        public PublisherWindow()
        {
            InitializeComponent();

            _vm = new PublisherVM();
            DataContext = _vm;
            _vm.ProgressChanged += OnProgressChanged;
            _vm.Finished += OnFinished;
        }

        private void OnFinished()
        {
            ProgressBarEnd.BeginAnimation(RangeBase.ValueProperty, AnimationFactory.CreateDoubleAnimation(250, 100, easing: true));
        }

        private void OnProgressChanged()
        {
            ProgressBar.BeginAnimation(RangeBase.ValueProperty, AnimationFactory.CreateDoubleAnimation(250, _vm.Progress, easing: true));
        }
    }
}
