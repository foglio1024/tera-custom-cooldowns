using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Shapes;
using TCC.Controls.Dashboard;
using TCC.Data.Pc;
using TCC.ViewModels;

namespace TCC.Windows

{
    /// <summary>
    /// Logica di interazione per Dashboard.xaml
    /// </summary>
    public partial class Dashboard : TccWindow
    {
        public DashboardViewModel VM => Dispatcher.Invoke(() => DataContext as DashboardViewModel);
        public IntPtr Handle => Dispatcher.Invoke(() => new WindowInteropHelper(this).Handle);

        public Dashboard()
        {
            InitializeComponent();
            DataContext = new DashboardViewModel();
        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            HideWindow();
            VM.SaveCharacters();
        }


        private void ChangeView(object sender, RoutedEventArgs e)
        {
            VM.DetailView = !VM.DetailView;
        }

        private void OnTitleBarMouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private Popup _last;

        private void CreatePopup(FrameworkElement sender)
        {
            if (_last != null) _last.IsOpen = false;
            _last = new Popup
            {
                Placement = PlacementMode.Right,
                AllowsTransparency = true,
                Child = new VanguardInfoPopup()
                {
                    DataContext = sender?.DataContext
                },
                PlacementTarget = sender,
                IsOpen = true
            };
            _last.VerticalOffset = -((FrameworkElement)_last.Child).ActualHeight + 8;
        }

        private void ShowVanguardCreditsPopup(object sender, MouseEventArgs e)
        {
            CreatePopup((FrameworkElement)sender);

            var gauge = (sender as RectangleBarGauge);
            var dc = gauge?.DataContext as Character;

            ((VanguardInfoPopup) _last.Child).Icon.Children.Add(new Ellipse() { Fill = gauge?.Color });
            ((VanguardInfoPopup) _last.Child).Text.Text = $"Vanguard credits: {dc?.VanguardCredits}";
        }

        private void ShowVanguardDailyPopup(object sender, MouseEventArgs e)
        {
            CreatePopup((FrameworkElement)sender);
            var gauge = (sender as RectangleBarGauge);
            var dc = gauge?.DataContext as Character;
            ((VanguardInfoPopup)_last.Child).Icon.Children.Add(new Ellipse() { Fill = gauge?.Color });
            ((VanguardInfoPopup)_last.Child).Text.Text = $"Vanguard daily quests: {dc?.VanguardDailiesDone}";

        }

        private void ShowVanguardWeeklyPopup(object sender, MouseEventArgs e)
        {
            CreatePopup((FrameworkElement)sender);
            var gauge = (sender as RectangleBarGauge);
            var dc = gauge?.DataContext as Character;
            ((VanguardInfoPopup)_last.Child).Icon.Children.Add(new Ellipse() { Fill = gauge?.Color });
            ((VanguardInfoPopup)_last.Child).Text.Text = $"Vanguard weekly quests: {dc?.VanguardWeekliesDone}";

        }

        private void ShowGuardianCreditsPopup(object sender, MouseEventArgs e)
        {
            CreatePopup((FrameworkElement)sender);
            var gauge = (sender as RectangleBarGauge);
            var dc = gauge?.DataContext as Character;
            ((VanguardInfoPopup)_last.Child).Icon.Children.Add(new Ellipse() { Fill = gauge?.Color });
            ((VanguardInfoPopup)_last.Child).Text.Text = $"Guardian credits: {dc?.GuardianCredits}";


        }

        private void ShowGuardianQuestsPopup(object sender, MouseEventArgs e)
        {
            CreatePopup((FrameworkElement)sender);
            var gauge = (sender as RectangleBarGauge);
            var dc = gauge?.DataContext as Character;
            ((VanguardInfoPopup)_last.Child).Icon.Children.Add(new Ellipse() { Fill = gauge?.Color });
            ((VanguardInfoPopup)_last.Child).Text.Text = $"Guardian daily quests: {dc?.ClaimedGuardianQuests}";


        }

        private void ShowDragonwingPopup(object sender, MouseEventArgs e)
        {
            CreatePopup((FrameworkElement)sender);
            var gauge = (sender as RectangleBarGauge);
            var dc = gauge?.DataContext as Character;
            ((VanguardInfoPopup)_last.Child).Icon.Children.Add(new Ellipse() { Fill = gauge?.Color });
            ((VanguardInfoPopup)_last.Child).Text.Text = $"Dragonwing scales: {dc?.DragonwingScales}";


        }

        private void ShowScrollPiecesPopup(object sender, MouseEventArgs e)
        {
            CreatePopup((FrameworkElement)sender);
            var gauge = (sender as RectangleBarGauge);
            var dc = gauge?.DataContext as Character;
            ((VanguardInfoPopup)_last.Child).Icon.Children.Add(new Ellipse() { Fill = gauge?.Color });
            ((VanguardInfoPopup)_last.Child).Text.Text = $"Pieces of dragon scroll: {dc?.PiecesOfDragonScroll}";


        }

        private void ShowElleonMarksPopup(object sender, MouseEventArgs e)
        {
            CreatePopup((FrameworkElement)sender);
            var gauge = (sender as RectangleBarGauge);
            var dc = gauge?.DataContext as Character;
            ((VanguardInfoPopup)_last.Child).Icon.Children.Add(new Ellipse() { Fill = gauge?.Color });
            ((VanguardInfoPopup)_last.Child).Text.Text = $"Elleon Marks: {dc?.ElleonMarks}";


        }

        private void CloseInfoPopup(object sender, MouseEventArgs e)
        {
            if (_last != null) _last.IsOpen = false;
        }
    }
}
