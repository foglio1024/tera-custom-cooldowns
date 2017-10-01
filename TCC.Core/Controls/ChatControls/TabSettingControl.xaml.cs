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
            _dc = (Tab)DataContext;
        }

        private void RemoveAuthor(object sender, RoutedEventArgs e)
        {
            _dc.Authors.Remove(((FrameworkElement)sender).DataContext as string);
        }

        private void RemoveChannel(object sender, RoutedEventArgs e)
        {
            _dc.Channels.Remove((ChatChannel)((FrameworkElement)sender).DataContext);
        }

        private void RemoveTab(object sender, RoutedEventArgs e)
        {
            if (ChatWindowViewModel.Instance.Tabs.Count >= 1)
            {
                ChatWindowViewModel.Instance.Tabs.Remove(_dc);
            }
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
        }

        private void RemoveExChannel(object sender, RoutedEventArgs e)
        {
            _dc.ExcludedChannels.Remove((ChatChannel)((FrameworkElement)sender).DataContext);
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
    }
}
