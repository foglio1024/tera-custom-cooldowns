using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using FoglioUtils.Extensions;

namespace TCC.Windows
{
    public partial class FUBH : Window
    {
        public FUBH()
        {
            InitializeComponent();
            DataContext = new FUBHVM();
            Closing += FUBH_Closing;
        }

        private void FUBH_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.TryDragMove();
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Hide();
            App.Settings.Save();
        }

        private void Hyperlink_OnRequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
        }
    }
}
