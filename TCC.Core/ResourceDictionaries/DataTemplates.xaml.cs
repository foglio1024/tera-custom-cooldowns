﻿using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TCC.Data;
using TCC.Data.Pc;
using TCC.Interop.Proxy;

namespace TCC.ResourceDictionaries
{
    public partial class DataTemplates
    {
        private void OnCharacterNameMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowManager.ViewModels.DashboardVM.SelectCharacter((sender as FrameworkElement)?.DataContext as Character);
        }

        private void LfgMessage_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            var lfg = (Listing) ((FrameworkElement) sender).DataContext;
            var msg = lfg.Message;
            var isRaid = lfg.IsRaid;

            if (lfg.Temp) WindowManager.ViewModels.LfgVM.Listings.Remove(lfg);

            ProxyInterface.Instance.Stub.RegisterListing(msg, isRaid);
            Keyboard.ClearFocus();
            FocusManager.MakeUnfocusable(WindowManager.LfgListWindow.Handle);

            Task.Delay(200).ContinueWith(t => ProxyInterface.Instance.Stub.RequestListings());
        }

        private void LfgMessage_OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            FocusManager.UndoUnfocusable(WindowManager.LfgListWindow.Handle);
            WindowManager.LfgListWindow.Activate();
        }

        private void LfgMessage_OnTbLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            FocusManager.MakeUnfocusable(WindowManager.LfgListWindow.Handle);
        }

        private void LfgMessage_OnTbLoaded(object sender, RoutedEventArgs e)
        {
            FocusManager.UndoUnfocusable(WindowManager.LfgListWindow.Handle);
            WindowManager.LfgListWindow.Activate();

            ((TextBox) sender).Focus();
            Keyboard.Focus((TextBox) sender);
        }

        private void LfgMessage_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FocusManager.UndoUnfocusable(WindowManager.LfgListWindow.Handle);
            ((TextBox)sender).Focus();
            Keyboard.Focus((TextBox)sender);
            WindowManager.LfgListWindow.Activate();


        }

        private void LfgPopup_OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is Listing l) l.IsPopupOpen = false;
            else FocusManager.PauseTopmost = false;
        }
    }
}
