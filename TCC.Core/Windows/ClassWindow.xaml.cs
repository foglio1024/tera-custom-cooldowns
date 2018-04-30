using System.Windows;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.Windows
{
    /// <summary>
    /// Logica di interazione per WarriorBar.xaml
    /// </summary>
    public partial class ClassWindow : TccWindow
    {
        public ClassWindow()
        {
            InitializeComponent();
            ButtonsRef = Buttons;
            MainContentRef = content;
            InitWindow(SettingsManager.ClassWindowSettings, ignoreSize: true);
        }

        public ClassWindowViewModel Context { get; private set; }

        private void TccWindow_Loaded(object sender, RoutedEventArgs e)
        {

            Context = (ClassWindowViewModel)DataContext;
            Context.PropertyChanged += _context_PropertyChanged;
        }

        private void _context_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentClass")
            {
                switch (Context.CurrentClass)
                {
                    case Class.Warrior:
                        content.ContentTemplate = FindResource("warrior") as DataTemplate;
                        break;
                    case Class.Glaiver:
                        content.ContentTemplate = FindResource("valkyrie") as DataTemplate;
                        break;
                    case Class.Archer:
                        content.ContentTemplate = FindResource("archer") as DataTemplate;
                        break;
                    case Class.Lancer:
                        content.ContentTemplate = FindResource("lancer") as DataTemplate;
                        break;
                    case Class.Priest:
                        content.ContentTemplate = FindResource("priest") as DataTemplate;
                        break;
                    case Class.Elementalist:
                        content.ContentTemplate = FindResource("mystic") as DataTemplate;
                        break;
                    case Class.Assassin:
                        content.ContentTemplate = FindResource("ninja") as DataTemplate;
                        break;
                    case Class.Engineer:
                        content.ContentTemplate = FindResource("gunner") as DataTemplate;
                        break;
                    case Class.Fighter:
                        content.ContentTemplate = FindResource("brawler") as DataTemplate;
                        break;
                    case Class.Soulless:
                        content.ContentTemplate = FindResource("reaper") as DataTemplate;
                        break;
                    case Class.Sorcerer:
                        content.ContentTemplate = FindResource("sorcerer") as DataTemplate;
                        break;
                    case Class.Berserker:
                        content.ContentTemplate = FindResource("berserker") as DataTemplate;
                        break;
                    case Class.Slayer:
                        content.ContentTemplate = FindResource("slayer") as DataTemplate;
                        break;
                    default:
                        content.ContentTemplate = FindResource("emptyTemplate") as DataTemplate;
                        break;
                }
            }
        }
    }
}
