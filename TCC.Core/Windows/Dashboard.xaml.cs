using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Nostrum.Factories;
using TCC.Data.Pc;
using TCC.ViewModels;

namespace TCC.Windows
{
    public partial class Dashboard
    {
        private DashboardViewModel VM { get; }

        public Dashboard(DashboardViewModel vm) : base(false)
        {
            InitializeComponent();
            DataContext = vm;
            VM = DataContext as DashboardViewModel;
            Showed += () => VM.UpdateBuffs();
            Hidden += () => Game.DB.DungeonDatabase.SaveCustomDefs();
        }


        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            HideWindow();
            VM.SaveCharacters();
        }

        private void OnDetailsMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            var an = AnimationFactory.CreateDoubleAnimation(300, 0, easing: true);
            an.Completed += (o, args) => DetailsBorder.IsHitTestVisible = false;
            DetailsBorder.BeginAnimation(OpacityProperty, an);
        }

        public void ShowDetails()
        {
            Dispatcher?.InvokeAsync(() =>
            {
                InventoryFilter.Clear();
                var an = AnimationFactory.CreateDoubleAnimation(300, 1, easing: true, completed:
                    (_, __) => DetailsBorder.IsHitTestVisible = true);
                DetailsBorder.BeginAnimation(OpacityProperty, an);
            });
        }

        private void FilterInventory(object sender, TextChangedEventArgs e)
        {
            var view = (ICollectionView) VM.SelectedCharacterInventory;
            view.Filter = o =>
            {
                var item = ((InventoryItem) o).Item;
                var name = item.Name;
                return name.IndexOf(((TextBox)sender).Text,
                           StringComparison.InvariantCultureIgnoreCase) != -1;
            };
            view.Refresh();
        }

        private void OnTabChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 0 && e.AddedItems[0] is TabItem) OnDetailsMouseButtonDown(null, null);
        }

        private void RemoveCharacter(object sender, RoutedEventArgs e)
        {
            OnDetailsMouseButtonDown(null, null);
            //VM.Characters.Remove(VM.SelectedCharacter);
            VM.SelectedCharacter.Hidden = true;
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