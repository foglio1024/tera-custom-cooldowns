using System.Windows.Input;
using TCC.ViewModels;

namespace TCC.Windows
{
    public partial class BossWindow
    {

        public BossWindow()
        {
            InitializeComponent();
            ButtonsRef = Buttons;
            MainContent = WindowContent;
            Init(Settings.BossWindowSettings);
        }

        private void TccWindow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BossGageWindowViewModel.Instance.CopyToClipboard();
        }
    }
}