using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.Controls.Chat
{
    /// <summary>
    /// Interaction logic for TabSettingControl.xaml
    /// </summary>
    public partial class TabSettingControl
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

        private void NewChannelComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems.Count == 0) return;
                if (e.AddedItems[0] is ChatChannelOnOff i)
                {
                    var ch = i.Channel;
                    if (!_dc.Channels.Contains(ch))
                    {
                        _dc.Channels.Add(ch);
                        _dc.ApplyFilter();
                    }
                }

                if (sender is ComboBox s) s.SelectedIndex = -1;
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
            if (string.IsNullOrEmpty(s?.Text) || string.Equals(s.Text, "New author...")) return;
            if (_dc.Authors.Contains(s.Text)) return;
            _dc.Authors.Add(s.Text);
            _dc.ApplyFilter();

        }

        private void NewAuthorTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ((TextBox) sender).Text = "New author...";
        }

        private void NewExChannelComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems.Count == 0) return;
                if (e.AddedItems[0] is ChatChannelOnOff i)
                {
                    var ch = i.Channel;
                    if (!_dc.ExcludedChannels.Contains(ch))
                    {
                        _dc.ExcludedChannels.Add(ch);
                        _dc.ApplyFilter();
                    }
                }

                if (sender is ComboBox s) s.SelectedIndex = -1;
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void NewExAuthorTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            if (!(sender is TextBox s) ||
                (string.IsNullOrEmpty(s.Text) || string.Equals(s.Text, "New author..."))) return;
            if (_dc.ExcludedAuthors.Contains(s.Text)) return;
            _dc.ExcludedAuthors.Add(s.Text);
            _dc.ApplyFilter();
        }

        private void NewExAuthorTextBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ((sender as TextBox)?.Text != "New author...") return;
            ((TextBox) sender).Text = "";
        }
    }
}
