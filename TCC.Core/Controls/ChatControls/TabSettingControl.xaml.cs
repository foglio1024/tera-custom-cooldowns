using Dragablz;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.Controls.ChatControls
{
    /// <summary>
    /// Interaction logic for TabSettingControl.xaml
    /// </summary>
    public partial class TabSettingControl : UserControl
    {
        private Tab _dc;
        public TabSettingControl()
        {
            InitializeComponent();

        }

        private void TabSettingControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            _dc = DataContext as Tab;
        }

        private void RemoveAuthor(object sender, RoutedEventArgs e)
        {
            _dc.Authors.Remove(((FrameworkElement)sender).DataContext as string);
            _dc.ApplyFilter();
            
        }

        private void RemoveChannel(object sender, RoutedEventArgs e)
        {
            _dc.Channels.Remove((ChatChannel)((FrameworkElement)sender).DataContext);
            _dc.ApplyFilter();

        }

        private void RemoveTab(object sender, RoutedEventArgs e)
        {
            ChatWindowManager.Instance.RemoveTab(_dc);
            //if (ChatWindowManager.Instance.TabVMs.Count >= 1)
            //{
            //    ChatWindowManager.Instance.TabVMs.Remove(_dc);
            //}
        }

        private void AddAuthor(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(NewAuthorTextBox.Text) && !string.Equals(NewAuthorTextBox.Text, "New author..."))
            {
                _dc.Authors.Add(NewAuthorTextBox.Text);
                _dc.ApplyFilter();
            }
        }

        private void AddChannel(object sender, RoutedEventArgs e)
        {
            try
            {
                var ch = ((ChatChannelOnOff)NewChannelComboBox.SelectionBoxItem).Channel;
                if (!_dc.Channels.Contains(ch))
                {
                    _dc.Channels.Add(ch);
                    _dc.ApplyFilter();
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void OnRequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Down) || Keyboard.IsKeyDown(Key.Up))
                return;

            e.Handled = true;
        }

        private void RemoveExAuthor(object sender, RoutedEventArgs e)
        {
            _dc.ExcludedAuthors.Remove(((FrameworkElement)sender).DataContext as string);
            _dc.ApplyFilter();

        }

        private void RemoveExChannel(object sender, RoutedEventArgs e)
        {
            _dc.ExcludedChannels.Remove((ChatChannel)((FrameworkElement)sender).DataContext);
            _dc.ApplyFilter();

        }

        private void AddExAuthor(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(NewExAuthorTextBox.Text) && !string.Equals(NewExAuthorTextBox.Text, "New author..."))
            {
                _dc.ExcludedAuthors.Add(NewExAuthorTextBox.Text);
                _dc.ApplyFilter();
            }
        }

        private void AddExChannel(object sender, RoutedEventArgs e)
        {
            try
            {
                var ch = ((ChatChannelOnOff)NewExChannelComboBox.SelectionBoxItem).Channel;
                if (!_dc.ExcludedChannels.Contains(ch))
                {
                    _dc.ExcludedChannels.Add(ch);
                    _dc.ApplyFilter();
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void NewChannelComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems.Count == 0) return;
                var i = e.AddedItems[0] as ChatChannelOnOff;
                var ch = i.Channel;
                if (!_dc.Channels.Contains(ch))
                {
                    _dc.Channels.Add(ch);
                    _dc.ApplyFilter();
                }
                var s = sender as ComboBox;
                s.SelectedIndex = -1;
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void NewAuthorTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            var s = sender as TextBox;
            if (!string.IsNullOrEmpty(s.Text) && !string.Equals(s.Text, "New author..."))
            {
                if (_dc.Authors.Contains(s.Text)) return;
                _dc.Authors.Add(s.Text);
                _dc.ApplyFilter();
            }

        }

        private void NewAuthorTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).Text = "New author...";
        }

        private void NewExChannelComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems.Count == 0) return;
                var i = e.AddedItems[0] as ChatChannelOnOff;
                var ch = i.Channel;
                if (!_dc.ExcludedChannels.Contains(ch))
                {
                    _dc.ExcludedChannels.Add(ch);
                    _dc.ApplyFilter();
                }
                var s = sender as ComboBox;
                s.SelectedIndex = -1;

            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void NewExAuthorTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            var s = sender as TextBox;
            if (!string.IsNullOrEmpty(s.Text) && !string.Equals(s.Text, "New author..."))
            {
                if (_dc.ExcludedAuthors.Contains(s.Text)) return;
                _dc.ExcludedAuthors.Add(s.Text);
                _dc.ApplyFilter();
            }
        }

        private void NewExAuthorTextBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ((sender as TextBox).Text != "New author...") return;
            (sender as TextBox).Text = "";
        }
    }
}
