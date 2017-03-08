using DamageMeter.Sniffing;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;


namespace TCC
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

    public partial class CooldownsBarWindow : Window
    {
        public static CooldownsBarWindow Instance;
        public SkillsModel NormalSkillsModel;
        public SkillsModel LongSkillsModel;
        public CooldownsBarWindow()
        {
            Instance = this;

            NormalSkillsModel = new SkillsModel();
            LongSkillsModel = new SkillsModel();

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
                Console.WriteLine("Added {0} indicator.", SkillsDatabase.GetSkill(sk.Id, PacketParser.CurrentClass).Name);
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
                        Console.WriteLine("Added {0} indicator.", SkillsDatabase.GetSkill(sk.Id, PacketParser.CurrentClass).Name);

                        break;
                    case CooldownType.Item:
                        Instance.LongSkillsModel.SkillIndicators.Add(new SkillIndicator(BroochesDatabase.GetBrooch(sk.Id), (int)sk.Cooldown));
                        Console.WriteLine("Added {0} indicator.", BroochesDatabase.GetBrooch(sk.Id).Name);

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
                    Instance.NormalSkillsModel.SkillIndicators.Remove(Instance.NormalSkillsModel.SkillIndicators.Where(x => x.Skill.Id == sk.Id).First());
                    Console.WriteLine("Removed {0} indicator.", SkillsDatabase.GetSkill(sk.Id, PacketParser.CurrentClass).Name);

                }
                catch (Exception)
                {
                    Console.WriteLine("Can't remove {0} indicator.", SkillsDatabase.GetSkill(sk.Id, PacketParser.CurrentClass).Name);
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
                            Instance.LongSkillsModel.SkillIndicators.Remove(Instance.LongSkillsModel.SkillIndicators.Where(x => x.Skill.Id == sk.Id).First());
                            Console.WriteLine("Removed {0} indicator.", SkillsDatabase.SkillIdToName(sk.Id, PacketParser.CurrentClass));
                            break;

                        case CooldownType.Item:
                            Instance.LongSkillsModel.SkillIndicators.Remove(Instance.LongSkillsModel.SkillIndicators.Where(x => x.Skill.Id == sk.Id).First());
                            Console.WriteLine("Removed {0} indicator.", BroochesDatabase.GetBrooch(sk.Id).Name);
                            break;

                        default:
                            break;
                    }

                }
                catch (Exception)
                {
                    Console.WriteLine("Can't remove {0} indicator.", SkillsDatabase.SkillIdToName(sk.Id, PacketParser.CurrentClass));
                }
            });
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //SkillManager.AddSkill(new SkillCooldown(100700, 2000, CooldownType.Skill)); //test skill
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
            this.Top = Properties.Settings.Default.CooldownBarTop;
            this.Left = Properties.Settings.Default.CooldownBarLeft;

            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            FocusManager.MakeUnfocusable(hwnd);
            FocusManager.HideFromToolBar(hwnd);

        }
        public void LoadingDone()
        {
            Dispatcher.Invoke(() =>
            {
                var a = new DoubleAnimation(0, 359.9, TimeSpan.FromSeconds(.7))
                {
                    EasingFunction = new QuadraticEase()
                };
                a.Completed += (s, o) =>
                {
                    loadArc.Visibility = Visibility.Hidden;
                    FocusManager.FocusTimer.Start();
                    this.Hide();
                };
                loadArc.Stroke = new SolidColorBrush(Color.FromArgb(255, 100, 255, 100));
                loadArc.BeginAnimation(Arc.StartAngleProperty, a);
            });
        }
    }
}

/*
* C_START_SKILL            0xCBC5
* S_START_COOLTIME_SKILL   0x7BA8
* C_USE_ITEM               0x750F
* S_START_COOLTIME_ITEM    0xAD63
*/