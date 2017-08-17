using DamageMeter.Sniffing;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using TCC.Data;
using TCC.Data.Databases;
using TCC.Parsing;
using TCC.ViewModels;
using TCC.Windows;

namespace TCC
{
    /// <summary>
    /// Logica di interazione per App.xaml
    /// </summary>
    public partial class App
    {
        private static void GlobalUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            File.WriteAllText(Environment.CurrentDirectory + "/error.txt",
                "##### CRASH #####\r\n" + ex.Message + "\r\n" +
                ex.StackTrace + "\r\n" + ex.Source + "\r\n" + ex + "\r\n" + ex.Data + "\r\n" + ex.InnerException +
                "\r\n" + ex.TargetSite);
            MessageBox.Show("An error occured and TCC will now close. Check error.txt for more info.", "TCC",
                MessageBoxButton.OK, MessageBoxImage.Error);
            if (WindowManager.TrayIcon != null)
            {
                WindowManager.TrayIcon.Dispose();
            }
        }

        internal static void Restart()
        {
            SettingsManager.SaveSettings();
            Process.Start("TCC.exe");
            CloseApp();
        }

        private void OnStartup(object sender, StartupEventArgs ev)
        {
            var cd = AppDomain.CurrentDomain;
            cd.UnhandledException += GlobalUnhandledExceptionHandler;
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.Normal;
            if (File.Exists(Environment.CurrentDirectory + "/TCCupdater.exe"))
            {
                File.Delete(Environment.CurrentDirectory + "/TCCupdater.exe");
            }
            UpdateManager.CheckAppVersion();
            UpdateManager.CheckDatabaseVersion();

            SkillsDatabase.Load();
            BroochesDatabase.SetBroochesIcons();
            SettingsManager.LoadWindowSettings();
            SettingsManager.LoadSettings();
            WindowManager.Init();
            WindowManager.Settings = new SettingsWindow() { Name = "Settings" };
            FocusManager.FocusTimer.Start();

            PacketProcessor.Init();

            TeraSniffer.Instance.NewConnection += (srv) =>
            {
                SkillManager.Clear();
                WindowManager.TrayIcon.Icon = WindowManager.ConnectedIcon;
                ChatWindowViewModel.Instance.AddChatMessage(
                    new ChatMessage(ChatChannel.TCC, "System", "<FONT>Connected to server.</FONT>"));
            };
            TeraSniffer.Instance.EndConnection += () =>
            {
                ChatWindowViewModel.Instance.AddChatMessage(new ChatMessage(ChatChannel.TCC, "System",
                    "<FONT>Disconnected from server.</FONT>"));
                GroupWindowViewModel.Instance.ClearAllAbnormalities();
                BuffBarWindowViewModel.Instance.Player.ClearAbnormalities();
                EntitiesManager.ClearNPC();

                SkillManager.Clear();
                WindowManager.TrayIcon.Icon = WindowManager.DefaultIcon;
                ProxyInterop.CloseConnection();
            };

            SessionManager.CurrentPlayer.Class = Class.None;
            SessionManager.CurrentPlayer.Name = "player";
            var v = Assembly.GetExecutingAssembly().GetName().Version;
            var ver = $"TCC v{v.Major}.{v.Minor}.{v.Build}";


            ChatWindowViewModel.Instance.AddChatMessage(
                new ChatMessage(ChatChannel.TCC, "System", $"<FONT>{ver}</FONT>"));

            TeraSniffer.Instance.Enabled = true;
            TimeManager.Instance.SetServerTimeZone(SettingsManager.LastRegion);
        }

        public static void CloseApp()
        {
            TeraSniffer.Instance.Enabled = false;
            SettingsManager.SaveSettings();
            WindowManager.Dispose();
            ProxyInterop.CloseConnection();
            Environment.Exit(0);
        }

    }
}
