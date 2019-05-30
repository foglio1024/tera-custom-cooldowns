using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using TCC.Settings;
using FoglioUtils.Extensions;

namespace TCC.Windows
{
    public class FUBHVM : TSPropertyChanged
    {

        public string CloseMessage => $"Click to {(!_dontshowagain ? "temporarily " : "")}close this window";
        private bool _dontshowagain;

        public bool DontShowAgain
        {
            get => _dontshowagain;
            set
            {
                if(_dontshowagain == value) return;
                _dontshowagain = value;
                N();
                N(nameof(CloseMessage));
                SettingsHolder.DontShowFUBH = value;
            }
        }

    }
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
            SettingsWriter.Save();
        }

        private void Hyperlink_OnRequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
        }
    }
}
