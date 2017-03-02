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
        public SkillsModel NormalSkillsModel;
        public SkillsModel LongSkillsModel;
        public MainWindow()
        {
            Instance = this;
            
            SkillsDatabase.Populate();

            NormalSkillsModel = new SkillsModel();
            LongSkillsModel = new SkillsModel();

            TeraSniffer.Instance.MessageReceived += PacketParser.MessageReceived;
            TeraSniffer.Instance.Enabled = true;
       
            InitializeComponent();
            NormalSkillsPanel.ItemsSource = NormalSkillsModel.SkillIndicators;
            NormalSkillsPanel.DataContext = NormalSkillsModel;
            LongSkillsPanel.ItemsSource = LongSkillsModel.SkillIndicators;
            LongSkillsPanel.DataContext = LongSkillsModel;
        }

        public static void ClearSkills()
        {
            Instance.Dispatcher.Invoke(() =>
            {
                Instance.NormalSkillsModel.SkillIndicators.Clear();
                Instance.LongSkillsModel.SkillIndicators.Clear();
            });
        }

        public static void AddNormalSkill(SkillCooldown sk)
        {
            Instance.Dispatcher.Invoke(() =>
            {
                Instance.NormalSkillsModel.SkillIndicators.Add(new SkillIndicator(SkillsDatabase.GetSkill(sk.Id, PacketParser.CurrentClass), (int)sk.Cooldown));
            });
        }
        public static void AddLongSkill(SkillCooldown sk)
        {
            Instance.Dispatcher.Invoke(() =>
            {
                Instance.LongSkillsModel.SkillIndicators.Add(new SkillIndicator(SkillsDatabase.GetSkill(sk.Id, PacketParser.CurrentClass), (int)sk.Cooldown));
            });
        }

        public static void RemoveNormalSkill(SkillCooldown sk)
        {
            Instance.Dispatcher.Invoke(() =>
            {
                try
                {
                    Instance.NormalSkillsModel.SkillIndicators.Remove(Instance.NormalSkillsModel.SkillIndicators.Where(x => x.Skill.Id == sk.Id).Single());
                }
                catch (Exception)
                {
                    Console.WriteLine("Can't remove indicator.");
                }
            });
        }
        public static void RemoveLongSkill(SkillCooldown sk)
        {
            Instance.Dispatcher.Invoke(() =>
            {
                try
                {
                    Instance.LongSkillsModel.SkillIndicators.Remove(Instance.LongSkillsModel.SkillIndicators.Where(x => x.Skill.Id == sk.Id).Single());
                }
                catch (Exception)
                {
                    Console.WriteLine("Can't remove indicator.");
                }
            });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TeraSniffer.Instance.Enabled = false;
        }

        private void TB_ConnectionStatus_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Console.WriteLine("Adding skill");
            SkillManager.NormalSkillsQueue.Add(new SkillCooldown(100700, 2565));
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
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