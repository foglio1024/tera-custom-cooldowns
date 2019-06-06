using TCC.ViewModels;

namespace TCC.Windows.Widgets
{
    public partial class BossWindow
    {
        private NpcWindowViewModel VM { get; }
        public BossWindow(NpcWindowViewModel vm)
        {
            DataContext = vm;
            VM = DataContext as NpcWindowViewModel;
            InitializeComponent();
            ButtonsRef = Buttons;
            MainContent = WindowContent;
            Init(App.Settings.NpcWindowSettings);
            MouseDoubleClick += (_, __) => VM.CopyToClipboard();
        }
    }
}