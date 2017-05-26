using DamageMeter.Sniffing;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using TCC.Data;
using TCC.Parsing;
using TCC.Properties;
using TCC.ViewModels;
using TCC.Windows;

namespace TCC
{
    /// <summary>
    /// Logica di interazione per App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static void GlobalUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            File.WriteAllText(Environment.CurrentDirectory + "/error.txt", "##### CRASH #####\r\n" + ex.Message + "\r\n" +
                     ex.StackTrace + "\r\n" + ex.Source + "\r\n" + ex + "\r\n" + ex.Data + "\r\n" + ex.InnerException +
                     "\r\n" + ex.TargetSite);
            MessageBox.Show("An error occured and TCC will now close. Check error.txt for more info.", "TCC", MessageBoxButton.OK, MessageBoxImage.Error);
            if(WindowManager.TrayIcon != null)
            {
                WindowManager.TrayIcon.Dispose();
            }
        }

        private void OnStartup(object sender, StartupEventArgs ev)
        {
            var cd = AppDomain.CurrentDomain;
            cd.UnhandledException += GlobalUnhandledExceptionHandler;
            System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.Normal;
            if (File.Exists(Environment.CurrentDirectory + "/TCCupdater.exe"))
            {
                File.Delete(Environment.CurrentDirectory + "/TCCupdater.exe");
            }
            UpdateManager.CheckAppVersion();
            UpdateManager.CheckDatabaseVersion();

            SkillsDatabase.Load();
            BroochesDatabase.SetBroochesIcons();
            AbnormalityDatabase.Load();

            TeraSniffer.Instance.Enabled = true;
            SettingsManager.LoadSettings();
            WindowManager.Init();
            FocusManager.FocusTimer.Start();

            PacketProcessor.Init();

            TeraSniffer.Instance.NewConnection += (srv) =>
            {
                SkillManager.Clear();
                WindowManager.TrayIcon.Icon = WindowManager.ConnectedIcon;

            };
            TeraSniffer.Instance.EndConnection += () =>
            {
                SkillManager.Clear();
                WindowManager.TrayIcon.Icon = WindowManager.DefaultIcon;
            };

            //Debug();
            SessionManager.CurrentPlayer.Class = Class.None;
        }

        public static void CloseApp()
        {
            TeraSniffer.Instance.Enabled = false;
            SettingsManager.SaveSettings();
            WindowManager.Dispose();
            Environment.Exit(0);
        }
        static bool x = true;
        public static void Debug()
        {
            SessionManager.Logged = true;
            SessionManager.LoadingScreen = false;
            WindowManager.ClassWindow.Dispatcher.Invoke(() =>
            {
                ((ClassWindowViewModel)WindowManager.ClassWindow.DataContext).ClearSkills();
                ((ClassWindowViewModel)WindowManager.ClassWindow.DataContext).CurrentClass = Class.Warrior;
            });


            //for (int i = 0; i < 5; i++)
            //{
            //    var u = new User(WindowManager.GroupWindow.Dispatcher);
            //    u.Name = "Test D" + i;
            //    u.UserClass = Class.Warrior;
            //    u.Laurel = Laurel.Champion;
            //    u.PlayerId = (uint)i;
            //    u.Online = true;
            //    u.Ready = ReadyStatus.Ready;
            //    ViewModels.GroupWindowManager.Instance.AddOrUpdateMember(u);
            //}
            //for (int i = 0; i < 1; i++)
            //{
            //    var u = new User(WindowManager.GroupWindow.Dispatcher);
            //    u.Name = "Test H" + i;
            //    u.UserClass = Class.Elementalist;
            //    u.Laurel = Laurel.Champion;
            //    u.PlayerId = (uint)i*2;
            //    u.Online = true;
            //    u.Ready = ReadyStatus.NotReady;
            //    ViewModels.GroupWindowManager.Instance.AddOrUpdateMember(u);
            //}
            //for (int i = 0; i < 5; i++)
            //{
            //    var u = new User(WindowManager.GroupWindow.Dispatcher);
            //    u.Name = "Test T" + i;
            //    u.UserClass = Class.Lancer;
            //    u.Laurel = Laurel.Champion;
            //    u.PlayerId = (uint)i*3;
            //    u.Online = true;
            //    ViewModels.GroupWindowManager.Instance.AddOrUpdateMember(u);
            //}

        }
    }
}
