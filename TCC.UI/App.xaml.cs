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
        private void OnStartup(object sender, StartupEventArgs ev)
        {
            System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.High;

            TeraSniffer.Instance.Enabled = true;
            WindowManager.Init();
            LoadSettings();
            PacketRouter.Init();

            TeraSniffer.Instance.NewConnection += (srv) => SkillManager.Clear();
            TeraSniffer.Instance.EndConnection += () => SkillManager.Clear();

            var LoadThread = new Thread(new ThreadStart(() =>
            {
                SkillsDatabase.Populate();
                Console.WriteLine("Skills loaded.");
                BroochesDatabase.SetBroochesIcons();
                Console.WriteLine("Set brooches icons");
                MonsterDatabase.Populate();
                Console.WriteLine("Monsters loaded");
                AbnormalityDatabase.Populate();
                Console.WriteLine("Abnormalities loaded");
                WindowManager.CooldownWindow.LoadingDone();
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
                SetWindowPosition(WindowManager.BossGauge, sr); //0
                SetWindowPosition(WindowManager.BuffBar, sr); //1
                SetWindowPosition(WindowManager.CharacterWindow, sr); //2
                SetWindowPosition(WindowManager.ClassSpecificWindow, sr); //3
                SetWindowPosition(WindowManager.CooldownWindow, sr); //4
                var t = sr.ReadLine(); //5
                if (t.Equals("true"))
                {
                    WindowManager.SetTransparent(true);
                }
                else
                {
                    WindowManager.SetTransparent(false);
                }
                sr.Close();
            }
            else
            {
                WindowManager.SetTransparent(false);
            }
        }

        private static void SetWindowPosition(Window w, StreamReader sr)
        {
            var line = sr.ReadLine();
            var vals = line.Split(',');
            w.Top = Convert.ToDouble(vals[0]);
            w.Left = Convert.ToDouble(vals[1]);
        }

        public static void SaveSettings()
        {
            string[] vals = new string[6];
            AddSetting(WindowManager.BossGauge, vals, 0);
            AddSetting(WindowManager.BuffBar, vals, 1);
            AddSetting(WindowManager.CharacterWindow, vals, 2);
            AddSetting(WindowManager.ClassSpecificWindow, vals, 3);
            AddSetting(WindowManager.CooldownWindow, vals, 4);
            vals[5] = WindowManager.Transparent.ToString().ToLower();
            File.WriteAllLines(Environment.CurrentDirectory + @"/settings.csv", vals);
        }

        private static void AddSetting(Window w, string[] vals, int i)
        {
            vals[i] = String.Format("{0},{1}", w.Top, w.Left);
        }
        public static void CloseApp()
        {
            TeraSniffer.Instance.Enabled = false;
            WindowManager.Dispose();
            SaveSettings();
            Environment.Exit(0);
        }

    }
}
