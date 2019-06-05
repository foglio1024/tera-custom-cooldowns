using TCC.ViewModels;

namespace TCC.Windows.Widgets
{
    public partial class BossWindow
    {
        public BossGageWindowViewModel VM { get; }
        public BossWindow()
        {
            DataContext = new BossGageWindowViewModel();
            VM = DataContext as BossGageWindowViewModel;
            InitializeComponent();
            ButtonsRef = Buttons;
            MainContent = WindowContent;
            Init(App.Settings.BossWindowSettings);
            MouseDoubleClick += (_, __) => VM.CopyToClipboard();
        }
    }
}