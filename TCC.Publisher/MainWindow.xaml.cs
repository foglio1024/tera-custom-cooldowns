using System;
using System.Windows;
using System.Windows.Controls;
using FoglioUtils;
using FoglioUtils.Extensions;

namespace TCC.Publisher
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public readonly SynchronizedObservableCollection<string> Log;
        public MainWindow()
        {
            Log = new SynchronizedObservableCollection<string>();
            InitializeComponent();
            LogList.ItemsSource = Log;
        }
        public void AddLine(string msg)
        {
            msg = msg.Replace("\n", "");

            Log.Add($"[{DateTime.Now.ToShortTimeString()}] {msg}");
            LogList.GetChild<ScrollViewer>().ScrollToBottom();
        }
        private void Generate(object sender, RoutedEventArgs e)
        {
            Publisher.Generate();
        }

        private async void Push(object sender, RoutedEventArgs e)
        {
            var confirm = MessageBox.Show("Confirm zip push?", "TCC publisher", MessageBoxButton.YesNo);
            if (confirm != MessageBoxResult.Yes) return;
            await Publisher.Upload();
        }

        public void AppendToLine(string msg)
        {
            msg = msg.Replace("\n", "");
            var last = Log[Log.Count - 1];
            Log.RemoveAt(Log.Count - 1);
            Log.Add(last + msg);
            LogList.GetChild<ScrollViewer>().ScrollToBottom();
        }

        private async void Release(object sender, RoutedEventArgs e)
        {
            var confirm = MessageBox.Show("Confirm release creation?", "TCC publisher", MessageBoxButton.YesNo);
            if (confirm != MessageBoxResult.Yes) return;
            if (string.IsNullOrWhiteSpace(ReleaseNotesTb.Text))
            {
                var emptyNotesConf = MessageBox.Show("Release notes field is empty, continue anyway?", "TCC publisher", MessageBoxButton.YesNo);
                if (emptyNotesConf != MessageBoxResult.Yes) return;
            }
            await Publisher.CreateRelease();

        }

        private void GetVersion(object sender, RoutedEventArgs e)
        {
            Publisher.GetVersion();
        }
    }
}
