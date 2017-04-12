using DamageMeter.Sniffing;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
            //Icon_Skills.ResourceManager.IgnoreCase = true;
            //Icon_Status.ResourceManager.IgnoreCase = true;
            //Icon_Crest.ResourceManager.IgnoreCase = true;

            System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.High;

            TeraSniffer.Instance.Enabled = true;

            WindowManager.Init();
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

        public static void CloseApp()
        {
            TeraSniffer.Instance.Enabled = false;
            WindowManager.Dispose();

            Environment.Exit(0);
        }

    }
}
