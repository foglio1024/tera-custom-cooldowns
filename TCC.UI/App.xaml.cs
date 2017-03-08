using DamageMeter.Sniffing;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace TCC
{
    /// <summary>
    /// Logica di interazione per App.xaml
    /// </summary>
    public partial class App : Application
    {
        

        private void OnStartup(object sender, StartupEventArgs ev)
        {
            TeraSniffer.Instance.Enabled = true;

            WindowManager.Init();

            TeraSniffer.Instance.MessageReceived += PacketParser.MessageReceived;
            TeraSniffer.Instance.NewConnection += (srv) => SkillManager.Clear();
            TeraSniffer.Instance.EndConnection += () => SkillManager.Clear();

            var LoadThread = new Thread(new ThreadStart(() =>
            {
                SkillsDatabase.Populate();
                BroochesDatabase.SetBroochesIcons();
                WindowManager.CDBar.LoadingDone();
            }));

            PacketParser.CurrentClass = Class.None;

            WindowManager.CDBar.Show();
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
