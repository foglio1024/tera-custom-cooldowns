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
            LoadSettings();
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

        static void LoadSettings()
        {
            if (File.Exists(Environment.CurrentDirectory + @"/settings.csv"))
            {
                var sr = File.OpenText(Environment.CurrentDirectory + @"/settings.csv");

                SetWindowParameters(SettingsManager.BossGaugeWindowSettings, sr); //0
                SetWindowParameters(SettingsManager.BuffBarWindowSettings, sr); //1
                SetWindowParameters(SettingsManager.CharacterWindowSettings, sr); //2
                SetWindowParameters(SettingsManager.CooldownWindowSettings, sr); //4

                //var t = sr.ReadLine(); //5
                //if (t.Equals("true"))
                //{
                //    WindowManager.ChangeClickThru(true);
                //}
                //else
                //{
                //    WindowManager.ChangeClickThru(false);
                //}
                sr.Close();
            }
        }

        private static void SetWindowParameters(WindowSettings ws, StreamReader sr)
        {
            var line = sr.ReadLine();
            var vals = line.Split(',');
            try
            {
                ws.Y = Convert.ToDouble(vals[0]);
                ws.X = Convert.ToDouble(vals[1]);
                if (Enum.TryParse(vals[2], out Visibility v))
                {
                    ws.Visibility = v;
                }
                if(Boolean.TryParse(vals[3], out bool ct))
                {
                    ws.ClickThru = ct;
                }

            }
            catch (Exception)
            {

            }
        }

        public static void SaveSettings()
        {
            string[] vals = new string[5];
            AddSetting(SettingsManager.BossGaugeWindowSettings, vals, 0);
            AddSetting(SettingsManager.BuffBarWindowSettings, vals, 1);
            AddSetting(SettingsManager.CharacterWindowSettings, vals, 2);
            AddSetting(SettingsManager.CooldownWindowSettings, vals, 3);

            File.WriteAllLines(Environment.CurrentDirectory + @"/settings.csv", vals);
        }

        private static void AddSetting(WindowSettings ws, string[] vals, int i)
        {
            vals[i] = String.Format("{0},{1},{2},{3}", ws.Y, ws.X, ws.Visibility.ToString(), ws.ClickThru.ToString());
        }
        public static void CloseApp()
        {
            TeraSniffer.Instance.Enabled = false;
            SaveSettings();
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

            for (int i = 0; i < 1; i++)
            {
                var u = new User(WindowManager.GroupWindow.Dispatcher);
                u.Name = "Test D" + i;
                u.UserClass = Class.Warrior;
                u.Laurel = Laurel.Champion;
                u.PlayerId = (uint)i;
                ViewModels.GroupWindowManager.Instance.AddOrUpdateMember(u);
            }
            for (int i = 0; i < 1; i++)
            {
                var u = new User(WindowManager.GroupWindow.Dispatcher);
                u.Name = "Test H" + i;
                u.UserClass = Class.Elementalist;
                u.Laurel = Laurel.Champion;
                u.PlayerId = (uint)i*2;
                ViewModels.GroupWindowManager.Instance.AddOrUpdateMember(u);
            }
            for (int i = 0; i < 10; i++)
            {
                var u = new User(WindowManager.GroupWindow.Dispatcher);
                u.Name = "Test T" + i;
                u.UserClass = Class.Lancer;
                u.Laurel = Laurel.Champion;
                u.PlayerId = (uint)i*3;
                ViewModels.GroupWindowManager.Instance.AddOrUpdateMember(u);
            }

        }
    }
}
