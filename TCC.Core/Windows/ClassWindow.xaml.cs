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
                        content.ContentTemplate = FindResource("Warrior") as DataTemplate;
                        break;
                    case Class.Glaiver:
                        content.ContentTemplate = FindResource("Valkyrie") as DataTemplate;
                        break;
                    case Class.Archer:
                        content.ContentTemplate = FindResource("Archer") as DataTemplate;
                        break;
                    case Class.Lancer:
                        content.ContentTemplate = FindResource("Lancer") as DataTemplate;
                        break;
                    case Class.Priest:
                        content.ContentTemplate = FindResource("Priest") as DataTemplate;
                        break;
                    case Class.Elementalist:
                        content.ContentTemplate = FindResource("Mystic") as DataTemplate;
                        break;
                    case Class.Assassin:
                        content.ContentTemplate = FindResource("Ninja") as DataTemplate;
                        break;
                    case Class.Engineer:
                        content.ContentTemplate = FindResource("Gunner") as DataTemplate;
                        break;
                    case Class.Fighter:
                        content.ContentTemplate = FindResource("Brawler") as DataTemplate;
                        break;
                    case Class.Soulless:
                        content.ContentTemplate = FindResource("Reaper") as DataTemplate;
                        break;
                    case Class.Sorcerer:
                        content.ContentTemplate = FindResource("Sorcerer") as DataTemplate;
                        break;
                    case Class.Berserker:
                        content.ContentTemplate = FindResource("Berserker") as DataTemplate;
                        break;
                    case Class.Slayer:
                        content.ContentTemplate = FindResource("Slayer") as DataTemplate;
                        break;
                    default:
                        content.ContentTemplate = FindResource("EmptyTemplate") as DataTemplate;
                        break;
                }
            }
        }
    }
}
