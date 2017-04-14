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
                SkillsDatabase.Load();
                Console.WriteLine("Skills loaded.");
                BroochesDatabase.SetBroochesIcons();
                Console.WriteLine("Set brooches icons");
                MonsterDatabase.Populate();
                Console.WriteLine("Monsters loaded");
                AbnormalityDatabase.Load();
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
                SetWindowParameters(WindowManager.BossGauge, sr); //0
                SetWindowParameters(WindowManager.BuffBar, sr); //1
                SetWindowParameters(WindowManager.CharacterWindow, sr); //2
                SetWindowParameters(WindowManager.ClassSpecificWindow, sr); //3
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
            }
        }

        private static void SetWindowParameters(Window w, StreamReader sr)
        {
            var line = sr.ReadLine();
            var vals = line.Split(',');
            w.Top = Convert.ToDouble(vals[0]);
            w.Left = Convert.ToDouble(vals[1]);
            if(Enum.TryParse(vals[2], out Visibility v))
            {
                w.Visibility = v;
            }
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
            vals[i] = String.Format("{0},{1},{2}", w.Top, w.Left, w.Visibility.ToString());
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
