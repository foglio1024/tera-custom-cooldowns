using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Dragablz;
using TCC.Data;
using TCC.Data.Pc;
using TCC.TeraCommon.Game.Messages.Client;
using TCC.ViewModels;
using TCC.Windows;

namespace TCC.Controls.Dashboard
{
    /// <summary>
    /// Logica di interazione per DungeonView.xaml
    /// </summary>
    public partial class DungeonView : UserControl
    {
        public DungeonView()
        {
            InitializeComponent();
            IsVisibleChanged += (_, __) => { (DataContext as DashboardViewModel).LoadDungeonsCommand.Execute(null); };
        }

        private void DungeonColumns_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var headerSw = Utils.GetChild<ScrollViewer>(DungeonHeaders);
            var namesSw = Utils.GetChild<ScrollViewer>(CharacterNames);

            headerSw.ScrollToHorizontalOffset(e.HorizontalOffset);
            namesSw.ScrollToVerticalOffset(e.VerticalOffset);

        }
        private void OnEntryMouseEnter(object sender, MouseEventArgs e)
        {
            var cd = (sender as FrameworkElement)?.DataContext as DungeonCooldownViewModel;

            var chara = cd.Owner;
            var dung = cd.Cooldown.Dungeon;

            var dng = WindowManager.Dashboard.VM.Columns.FirstOrDefault(x => x.Dungeon.Id == dung.Id);
            if (dng != null) dng.Hilight = true;

            var ch = WindowManager.Dashboard.VM.CharacterViewModels.ToList().FirstOrDefault(x => x.Character.Id == chara.Id);
            if (ch != null) ch.Hilight = true;
        }
        private void OnEntryMouseLeave(object sender, MouseEventArgs e)
        {
            var cd = (sender as FrameworkElement)?.DataContext as DungeonCooldownViewModel;
            var chara = cd.Owner;
            var dung = cd.Cooldown.Dungeon;
            WindowManager.Dashboard.VM.Columns.FirstOrDefault(x => x.Dungeon.Id == dung.Id).Hilight = false;
            WindowManager.Dashboard.VM.CharacterViewModels.ToList().FirstOrDefault(x => x.Character.Id == chara.Id).Hilight = false;

        }
        private void OnDungeonEditButtonClick(object sender, RoutedEventArgs e)
        {
            new DungeonEditWindow() { Topmost = true, Owner = WindowManager.Dashboard}.ShowDialog();
        }
    }

    public class DungeonColumnViewModel : TSPropertyChanged
    {
        private bool _hilight;

        public Dungeon Dungeon { get; set; }

        public List<DungeonCooldownViewModel> DungeonsList { get; private set; }
        public bool Hilight
        {
            get => _hilight;
            set
            {
                if (_hilight == value) return;
                _hilight = value;
                NPC();
            }
        }

        public DungeonColumnViewModel()
        {
            DungeonsList = new List<DungeonCooldownViewModel>();
        }
    }

    public class DungeonCooldownViewModel : TSPropertyChanged
    {
        public DungeonCooldown Cooldown { get; set; }
        public Character Owner { get; set; }
    }

    public class CharacterViewModel : TSPropertyChanged
    {
        private bool _hilight;

        public bool Hilight
        {
            get => _hilight;
            set
            {
                if (_hilight == value) return;
                _hilight = value;
                NPC();
            }
        }
        public Character Character { get; set; }

    }


}
