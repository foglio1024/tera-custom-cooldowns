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
            WindowManager.Init();
            LoadSettings();
            PacketRouter.Init();

            TeraSniffer.Instance.NewConnection += (srv) => SkillManager.Clear();
            TeraSniffer.Instance.EndConnection += () => SkillManager.Clear();

            var LoadThread = new Thread(new ThreadStart(() =>
            {
                SkillsDatabase.Load();
                Console.WriteLine("Skills loaded.");
                BroochesDatabase.SetBroochesIcons();
                Console.WriteLine("Set brooches icons");
                MonsterDatabase.Populate();
                Console.WriteLine("Monsters loaded");
                AbnormalityDatabase.Load();
                Console.WriteLine("Abnormalities loaded");
                WindowManager.CooldownWindow.LoadingDone();
                //Debug();

            }));

            SessionManager.CurrentPlayer.Class = Class.None;

            WindowManager.ShowWindow(WindowManager.CooldownWindow);
            LoadThread.Start();

        }

        static void LoadSettings()
        {
            if (File.Exists(Environment.CurrentDirectory + @"/settings.csv"))
            {
                var sr = File.OpenText(Environment.CurrentDirectory + @"/settings.csv");
                SetWindowParameters(WindowManager.BossGauge, sr); //0
                SetWindowParameters(WindowManager.BuffBar, sr); //1
                SetWindowParameters(WindowManager.CharacterWindow, sr); //2
                //SetWindowParameters(WindowManager.ClassSpecificWindow, sr); //3
                SetWindowParameters(WindowManager.CooldownWindow, sr); //4
                var t = sr.ReadLine(); //5
                if (t.Equals("true"))
                {
                    WindowManager.ChangeClickThru(true);
                }
                else
                {
                    WindowManager.ChangeClickThru(false);
                }
                sr.Close();
            }
            else
            {
                WindowManager.ChangeClickThru(false);

                WindowManager.BossGauge.Visibility = Visibility.Visible;
                WindowManager.BossGauge.Top = 20;
                WindowManager.BossGauge.Left = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width / 2 - 200;

                WindowManager.BuffBar.Visibility = Visibility.Visible;
                WindowManager.BuffBar.Top = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height / 1.5;
                WindowManager.BuffBar.Left = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - 1000;

                WindowManager.CharacterWindow.Visibility = Visibility.Visible;
                WindowManager.CharacterWindow.Top = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - 120;
                WindowManager.CharacterWindow.Left = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width /2 - 200;

                //WindowManager.ClassSpecificWindow.Visibility = Visibility.Visible;
                //WindowManager.ClassSpecificWindow.Top = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height / 2;
                //WindowManager.ClassSpecificWindow.Left = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width /2 - 100;

                WindowManager.CooldownWindow.Visibility = Visibility.Visible;
                WindowManager.CooldownWindow.Top = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height /1.5;
                WindowManager.CooldownWindow.Left = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width /3;

            }
        }

        private static void SetWindowParameters(Window w, StreamReader sr)
        {
            var line = sr.ReadLine();
            var vals = line.Split(',');
            try
            {
                w.Top = Convert.ToDouble(vals[0]);
                w.Left = Convert.ToDouble(vals[1]);
                if (Enum.TryParse(vals[2], out Visibility v))
                {
                    w.Visibility = v;
                }
            }
            catch (Exception)
            {
                             
            }
        }

        public static void SaveSettings()
        {
            string[] vals = new string[5];
            AddSetting(WindowManager.BossGauge, vals, 0);
            AddSetting(WindowManager.BuffBar, vals, 1);
            AddSetting(WindowManager.CharacterWindow, vals, 2);
            //AddSetting(WindowManager.ClassSpecificWindow, vals, 3);
            AddSetting(WindowManager.CooldownWindow, vals, 3);
            vals[4] = WindowManager.Transparent.ToString().ToLower();
            File.WriteAllLines(Environment.CurrentDirectory + @"/settings.csv", vals);
        }

        private static void AddSetting(Window w, string[] vals, int i)
        {
            vals[i] = String.Format("{0},{1},{2}", w.Top, w.Left, w.Visibility.ToString());
        }
        public static void CloseApp()
        {
            TeraSniffer.Instance.Enabled = false;
            WindowManager.Dispose();
            SaveSettings();
            Environment.Exit(0);
        }
        static bool x = true;
        public static void Debug()
        {
            SessionManager.Logged = true;
            SessionManager.LoadingScreen = false;
            EntitiesManager.SpawnNPC(970, 3000, 1, Visibility.Visible, true);
            System.Timers.Timer t = new System.Timers.Timer(1000);
            EntitiesManager.TryGetBossById(1, out Boss b);
            EntitiesManager.SetNPCStatus(1, true);
            t.Elapsed += (se, ev) =>
            {
                if (b.CurrentHP == 0)
                {
                    b.CurrentHP = b.MaxHP;
                }
                else
                {

                    b.CurrentHP = 0;
                }
            };

            t.Enabled = true;


        }
    }
}
