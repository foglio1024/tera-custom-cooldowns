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

    public partial class MainWindow : Window
    {
        public static MainWindow Instance;
        public static EdgeWindow EdgeGauge;
        public SkillsModel NormalSkillsModel;
        public SkillsModel LongSkillsModel;
        DispatcherTimer FocusTimer;
        System.Windows.Forms.NotifyIcon NI;
        ContextMenu CM;
        bool transparent;
        public MainWindow()
        {
            Instance = this;

            NI = new System.Windows.Forms.NotifyIcon()
            {
                Icon = Properties.Resources.tcc,
                Visible = true
            };

            NI.MouseDown += NI_MouseDown;

            CM = new ContextMenu();
            var c = new MenuItem() { Header = "Close" };
            var d = new MenuItem() { Header = "Click through" };
            c.Click += (s, ev) => CloseApp();
            d.Click += (s, ev) =>
            {
                if (transparent)
                {
                    FocusManager.UndoTransparent(new WindowInteropHelper(this).Handle);
                    d.IsChecked = false;
                    transparent = false;
                }
                else
                {
                    FocusManager.MakeTransparent(new WindowInteropHelper(this).Handle);
                    d.IsChecked = true;
                    transparent = true;
                }

            };
            CM.Items.Add(c);
            CM.Items.Add(d);
            NormalSkillsModel = new SkillsModel();
            LongSkillsModel = new SkillsModel();
            TeraSniffer.Instance.MessageReceived += PacketParser.MessageReceived;
            TeraSniffer.Instance.Enabled = true;
            TeraSniffer.Instance.NewConnection += (srv) => SkillManager.Clear();
            TeraSniffer.Instance.EndConnection += () => SkillManager.Clear();
            SkillsDatabase.Progress += UpdateLoadGauge;
            PacketParser.CurrentClass = Class.Common;
            EdgeGauge = new EdgeWindow();
            ShowEdgeGauge();
            InitializeComponent();

            NormalSkillsPanel.ItemsSource = NormalSkillsModel.SkillIndicators;
            NormalSkillsPanel.DataContext = NormalSkillsModel;
            LongSkillsPanel.ItemsSource = LongSkillsModel.SkillIndicators;
            LongSkillsPanel.DataContext = LongSkillsModel;


        }

        private void NI_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if(e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                CM.IsOpen = true;
            }
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Left = this.Left;
            Properties.Settings.Default.Top = this.Top;
            Properties.Settings.Default.ELeft = EdgeGauge.Left;
            Properties.Settings.Default.ETop = EdgeGauge.Top;
            Properties.Settings.Default.Transparent = transparent;

            Properties.Settings.Default.Save();

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

        public void CloseApp()
        {
            TeraSniffer.Instance.Enabled = false;

            FocusTimer.Stop();
            Close();
            NI.Visible = false;
            Environment.Exit(0);

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Top = Properties.Settings.Default.Top;
            this.Left = Properties.Settings.Default.Left;
            if(Properties.Settings.Default.Transparent)
            {
                this.transparent = true;
                ((MenuItem)CM.Items[1]).IsChecked = true;
                FocusManager.MakeTransparent(new WindowInteropHelper(this).Handle);
            }
            else
            {
                this.transparent = false;
                ((MenuItem)CM.Items[1]).IsChecked = false;
            }



            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            FocusManager.MakeUnfocusable(hwnd);
            FocusManager.HideFromToolBar(hwnd);

            var a = new DoubleAnimation(0,359.9, TimeSpan.FromSeconds(.7))
            {
                EasingFunction = new QuadraticEase()
            };
            a.Completed += (s, o) =>
            {
                loadArc.Visibility = Visibility.Hidden;
                FocusTimer.Start();
            };


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
            
            FocusTimer = new DispatcherTimer();
            FocusTimer.Interval = TimeSpan.FromSeconds(1);
            FocusTimer.Tick += CheckForegroundWindow;


        }

        public static void ShowEdgeGauge()
        {
            Instance.Dispatcher.Invoke(() =>
            {
                EdgeGauge.Show();
                Console.WriteLine("Edge showed");
            });
        }
        public static void HideEdgeGauge()
        {
            Instance.Dispatcher.Invoke(() =>
            {
                EdgeGauge.Hide();
                Console.WriteLine("Edge hidden");
            });
        }
        public static void DimEdgeGauge()
        {
            Instance.Dispatcher.Invoke(() =>
            {
                var a = new DoubleAnimation(0, TimeSpan.FromMilliseconds(300))
                {
                    EasingFunction = new QuadraticEase()
                };
                EdgeGauge.BeginAnimation(OpacityProperty, a);
                Console.WriteLine("Edge dim");
            });

        }
        public static void UndimEdgeGauge()
        {
            Instance.Dispatcher.Invoke(() =>
            {
                var a = new DoubleAnimation(1, TimeSpan.FromMilliseconds(300))
                {
                    EasingFunction = new QuadraticEase()
                };
                EdgeGauge.BeginAnimation(OpacityProperty, a);
                Console.WriteLine("Edge undim");

            });

        }
        private void CheckForegroundWindow(object sender, EventArgs e)
        {
            IntPtr hwnd = FocusManager.GetForegroundWindow();
            FocusManager.GetWindowThreadProcessId(hwnd, out uint procId);
            var proc = Process.GetProcessById((int)procId);

            if(proc.ProcessName == "TERA" || proc.ProcessName == "TCC" || proc.ProcessName == "devenv")
            {
                this.Show();
                if(PacketParser.CurrentClass == Class.Warrior)
                {
                    ShowEdgeGauge();
                }
            }
            else
            {
                this.Hide();
                HideEdgeGauge();
            }
        }
    }
}

/*
* C_START_SKILL            0xCBC5
* S_START_COOLTIME_SKILL   0x7BA8
* C_USE_ITEM               0x750F
* S_START_COOLTIME_ITEM    0xAD63
*/