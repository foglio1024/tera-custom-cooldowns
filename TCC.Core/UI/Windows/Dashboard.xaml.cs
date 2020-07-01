using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Nostrum.Factories;
using TCC.Data.Pc;
using TCC.ViewModels;

namespace TCC.UI.Windows
{
    public partial class Dashboard
    {
        private readonly DoubleAnimation _detailsShowAnim;
        private readonly DoubleAnimation _detailsHideAnim;
        private readonly DashboardViewModel _vm;

        public Dashboard(DashboardViewModel vm) : base(false)
        {
            InitializeComponent();
            DataContext = vm;
            _vm = vm;
            Showed += _vm.UpdateBuffs;
            Hidden += Game.DB.DungeonDatabase.SaveCustomDefs;
            _detailsShowAnim = AnimationFactory.CreateDoubleAnimation(300, 1, easing: true, completed: (_, __) => DetailsBorder.IsHitTestVisible = true);
            _detailsHideAnim = AnimationFactory.CreateDoubleAnimation(300, 0, easing: true, completed: (_, __) => DetailsBorder.IsHitTestVisible = false);
        }

        public void ShowDetails()
        {
            Dispatcher?.InvokeAsync(() =>
            {
                InventoryFilter.Clear();
                DetailsBorder.BeginAnimation(OpacityProperty, _detailsShowAnim);
            });
        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            HideWindow();
            DashboardViewModel.SaveCharacters();
        }
        private void OnDetailsMouseButtonDown(object? sender, MouseButtonEventArgs? e)
        {
            DetailsBorder.BeginAnimation(OpacityProperty, _detailsHideAnim);
        }
        private void FilterInventory(object sender, TextChangedEventArgs e)
        {
            var view = (ICollectionView?)_vm.SelectedCharacterInventory;
            if (view == null) return;
            view.Filter = o =>
            {
                var item = ((InventoryItem) o).Item;
                var name = item.Name;
                return name.IndexOf(((TextBox) sender).Text,
                    StringComparison.InvariantCultureIgnoreCase) != -1;
            };
            view.Refresh();
        }
        private void OnTabChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0 || !(e.AddedItems[0] is TabItem)) return;
            OnDetailsMouseButtonDown(null, null);
        }
        private void RemoveCharacter(object sender, RoutedEventArgs e)
        {
            OnDetailsMouseButtonDown(null, null);
            if (_vm.SelectedCharacter == null) return;
            _vm.SelectedCharacter.Hidden = true;
        }
        private void OpenMergedInventory(object sender, RoutedEventArgs e)
        {
            new MergedInventoryWindow
            { Topmost = true, Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner }.ShowDialog();
        }
        private void OnMenuButtonClick(object sender, RoutedEventArgs e)
        {
            //MenuPopup.IsOpen = true;
        }
    }
}