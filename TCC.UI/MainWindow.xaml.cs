using DamageMeter.Sniffing;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Tera.Game;
using System;

namespace TCC.UI
{


    public class SkillsModel
    {
        public ObservableCollection<SkillIndicator> SkillIndicators;
        public SkillsModel()
        {
            SkillIndicators = new ObservableCollection<SkillIndicator>();
        }
    }

    public class SkillIndicator
    {
        public Skill Skill { get; set; }
        public int Cooldown { get; set; }

        public SkillIndicator(Skill sk, int cd)
        {
            Skill = sk;
            Cooldown = cd;
        }
    }

    public partial class MainWindow : Window
    {
        public static MainWindow Instance;
        public SkillsModel skModel;
        public MainWindow()
        {
            Instance = this;
            
            SkillsDatabase.Populate();
            skModel = new SkillsModel();

            TeraSniffer.Instance.MessageReceived += PacketParser.MessageReceived;
            TeraSniffer.Instance.NewConnection += Instance_NewConnection;
            TeraSniffer.Instance.Enabled = true;
       
            InitializeComponent();
            SP.ItemsSource = skModel.SkillIndicators;
            SP.DataContext = skModel;
        }

        internal static void ClearSkills()
        {
            Instance.Dispatcher.Invoke(() =>
            {
                Instance.skModel.SkillIndicators.Clear();
            });
        }
        
        public static void AddSkill(SkillCooldown sk)
        {
            Instance.Dispatcher.Invoke(() =>
            {
                Instance.skModel.SkillIndicators.Add(new SkillIndicator(SkillsDatabase.GetSkill(sk.Id, PacketParser.CurrentClass), (int)sk.Cooldown));
            });
        }
        public static void RemoveSkill(SkillCooldown sk)
        {
            Instance.Dispatcher.Invoke(() =>
            {
                try
                {
                    Instance.skModel.SkillIndicators.Remove(Instance.skModel.SkillIndicators.Where(x => x.Skill.Id == sk.Id).Single());
                }
                catch (Exception)
                {
                    Console.WriteLine("Can't remove indicator.");
                }
            });
        }
        
        private void Instance_NewConnection(Server obj)
        {
            Dispatcher.Invoke(()=> TB_ConnectionStatus.Text = obj.Name);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TeraSniffer.Instance.Enabled = false;
        }

        private void TB_ConnectionStatus_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //System.Console.WriteLine("Adding skill");
            //SkillManager.SQ.Add(new SkillCooldown(100700, 5000));
        }

        private void Window_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}

/*
* C_START_SKILL            0xCBC5
* S_START_COOLTIME_SKILL   0x7BA8
* C_USE_ITEM               0x750F
* S_START_COOLTIME_ITEM    0xAD63
*/