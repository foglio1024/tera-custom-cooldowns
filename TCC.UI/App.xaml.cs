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
        }

        private void OnStartup(object sender, StartupEventArgs ev)
        {
            var cd = AppDomain.CurrentDomain;
            cd.UnhandledException += GlobalUnhandledExceptionHandler;
            System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.High;
            if (File.Exists(Environment.CurrentDirectory + "/TCCupdater.exe"))
            {
                File.Delete(Environment.CurrentDirectory + "/TCCupdater.exe");
            }
            UpdateManager.CheckAppVersion();
            UpdateManager.CheckDatabaseVersion();



            TeraSniffer.Instance.Enabled = true;
            SettingsManager.LoadSettings();
            WindowManager.Init();
            WindowManager.Settings = new SettingsWindow()
            {
                Name = "Settings"
            };
            FocusManager.FocusTimer.Start();

            PacketProcessor.Init();

            TeraSniffer.Instance.NewConnection += (srv) => SkillManager.Clear();
            TeraSniffer.Instance.EndConnection += () => SkillManager.Clear();

            var LoadThread = new Thread(new ThreadStart(() =>
            {
                SkillsDatabase.Load();
                Console.WriteLine("Skills loaded.");
                BroochesDatabase.SetBroochesIcons();
                Console.WriteLine("Set brooches icons");
                //MonsterDatabase.Populate();
                //Console.WriteLine("Monsters loaded");
                AbnormalityDatabase.Load();
                Console.WriteLine("Abnormalities loaded");
                //Debug();
            }));

            SessionManager.CurrentPlayer.Class = Class.None;
            LoadThread.Start();

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
            SessionManager.CurrentPlayer.MaxHP = 100;
            SessionManager.CurrentPlayer.CurrentHP = 100;
            SessionManager.CurrentPlayer.EntityId = 1;

            for (int i = 0; i < 5; i++)
            {
                var u = new User(WindowManager.GroupWindow.Dispatcher);
                u.Name = "Test D" + i;
                u.UserClass = Class.Warrior;
                u.Laurel = Laurel.Champion;
                u.PlayerId = (uint)i;
                u.Online = true;
                u.Ready = ReadyStatus.Ready;
                ViewModels.GroupWindowManager.Instance.AddOrUpdateMember(u);
            }
            for (int i = 0; i < 1; i++)
            {
                var u = new User(WindowManager.GroupWindow.Dispatcher);
                u.Name = "Test H" + i;
                u.UserClass = Class.Elementalist;
                u.Laurel = Laurel.Champion;
                u.PlayerId = (uint)i*2;
                u.Online = true;
                u.Ready = ReadyStatus.NotReady;
                ViewModels.GroupWindowManager.Instance.AddOrUpdateMember(u);
            }
            for (int i = 0; i < 5; i++)
            {
                var u = new User(WindowManager.GroupWindow.Dispatcher);
                u.Name = "Test T" + i;
                u.UserClass = Class.Lancer;
                u.Laurel = Laurel.Champion;
                u.PlayerId = (uint)i*3;
                u.Online = true;
                ViewModels.GroupWindowManager.Instance.AddOrUpdateMember(u);
            }

        }
    }
}
