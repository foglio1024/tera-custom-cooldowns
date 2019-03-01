using TCC.Settings;
using TCC.ViewModels;

namespace TCC.Windows.Widgets
{
    public partial class CooldownWindow 
    {
        public CooldownWindowViewModel VM { get; }

        public CooldownWindow()
        {
            InitializeComponent();

            DataContext = new CooldownWindowViewModel();
            VM = DataContext as CooldownWindowViewModel;
            ButtonsRef = Buttons;
            MainContent = WindowContent;
            Init(SettingsHolder.CooldownWindowSettings, ignoreSize: true, undimOnFlyingGuardian: false);
        }
    }
}