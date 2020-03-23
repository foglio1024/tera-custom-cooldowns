using TCC.ViewModels.Widgets;

namespace TCC.UI.Windows.Widgets
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
            BoundaryRef = Boundary;
            MainContent = WindowContent;
            Init(App.Settings.NpcWindowSettings);
            MouseDoubleClick += (_, __) => VM.CopyToClipboard();
        }
    }
}