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
using System.Threading;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;

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

            NormalSkillsModel = new SkillsModel();
            LongSkillsModel = new SkillsModel();

            TeraSniffer.Instance.MessageReceived += PacketParser.MessageReceived;
            TeraSniffer.Instance.Enabled = true;
            TeraSniffer.Instance.NewConnection += (srv) => SkillManager.Clear();
            TeraSniffer.Instance.EndConnection += () => SkillManager.Clear();
            SkillsDatabase.Progress += UpdateLoadGauge;

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
                switch (sk.Type)
                {
                    case CooldownType.Skill:
                        Instance.LongSkillsModel.SkillIndicators.Add(new SkillIndicator(SkillsDatabase.GetSkill(sk.Id, PacketParser.CurrentClass), (int)sk.Cooldown));
                        break;
                    case CooldownType.Item:
                        Instance.LongSkillsModel.SkillIndicators.Add(new SkillIndicator(BroochesDatabase.GetBrooch(sk.Id), (int)sk.Cooldown));
                        break;
                    default:
                        break;
                }
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
                    switch (sk.Type)
                    {
                        case CooldownType.Skill:
                            Instance.LongSkillsModel.SkillIndicators.Remove(Instance.LongSkillsModel.SkillIndicators.Where(x => x.Skill.Id == sk.Id).Single());
                            break;
                        case CooldownType.Item:
                            Instance.LongSkillsModel.SkillIndicators.Remove(Instance.LongSkillsModel.SkillIndicators.Where(x => x.Skill.Id == sk.Id).Single());
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Can't remove indicator.");
                }
            });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }
        private void Window_RightClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Properties.Settings.Default.Left = this.Left;
            Properties.Settings.Default.Top = this.Top;

            Properties.Settings.Default.Save();

            TeraSniffer.Instance.Enabled = false;

            Close();
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Window_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ClearSkills();
        }

        public void UpdateLoadGauge(double val)
        {
            Dispatcher.Invoke(() =>
            {
                var v = val * 359.9 / 100;
                var a = new DoubleAnimation(Instance.loadArc.EndAngle, v, TimeSpan.FromMilliseconds(350))
                {
                    EasingFunction = new QuadraticEase()
                };
                Instance.loadArc.BeginAnimation(Arc.EndAngleProperty, a);
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Top = Properties.Settings.Default.Top;
            this.Left = Properties.Settings.Default.Left;
            var a = new DoubleAnimation(0,359.9, TimeSpan.FromSeconds(.5))
            {
                EasingFunction = new QuadraticEase()
            };
            a.Completed += (s, o) => loadArc.Visibility = Visibility.Hidden;
            var t = new Thread(new ThreadStart(() =>
            {
                SkillsDatabase.Populate();
                BroochesDatabase.SetBroochesIcons();
                Dispatcher.Invoke(() =>
                {
                    loadArc.Stroke = new SolidColorBrush(Color.FromArgb(255, 100, 255, 100));
                    loadArc.BeginAnimation(Arc.StartAngleProperty, a);
                });
            }));
            t.Start();

        }
    }
}

/*
* C_START_SKILL            0xCBC5
* S_START_COOLTIME_SKILL   0x7BA8
* C_USE_ITEM               0x750F
* S_START_COOLTIME_ITEM    0xAD63
*/