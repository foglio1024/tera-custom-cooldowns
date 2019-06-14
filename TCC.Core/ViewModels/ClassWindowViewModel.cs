using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using TCC.Parsing;
using TeraDataLite;
using TeraPacketParser.Messages;

namespace TCC.ViewModels
{
    public class ClassWindowViewModel : TccWindowViewModel
    {
        //private static ClassWindowViewModel _instance;
        //public static ClassWindowViewModel Instance => _instance ?? (_instance = new ClassWindowViewModel());

        public ClassWindowViewModel()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
        }

        private void HandleRunemarks(S_WEAK_POINT p)
        {
            if (!(CurrentManager is ValkyrieLayoutVM vvm)) return;
            vvm.RunemarksCounter.Val = p.TotalRunemarks;
        }

        protected override void InstallHooks()
        {
            PacketAnalyzer.NewProcessor.Hook<S_PLAYER_CHANGE_STAMINA>(m =>
            {
                CurrentManager.SetMaxST(Convert.ToInt32(m.MaxST));
                CurrentManager.SetST(Convert.ToInt32(m.CurrentST));
            });
            PacketAnalyzer.NewProcessor.Hook<S_LOGIN>(m =>
            {
                CurrentClass = m.CharacterClass; // todo: check for enabled?
                if (m.CharacterClass == Class.Valkyrie)
                {
                    PacketAnalyzer.NewProcessor.Hook<S_WEAK_POINT>(HandleRunemarks);
                }
                else
                {
                    PacketAnalyzer.NewProcessor.Unhook<S_WEAK_POINT>(HandleRunemarks);
                }
            });
            PacketAnalyzer.NewProcessor.Hook<S_RETURN_TO_LOBBY>(m =>
            {
                CurrentClass = Class.None;
            });
            PacketAnalyzer.NewProcessor.Hook<S_PLAYER_STAT_UPDATE>(m =>
            {
                // check enabled?
                switch (CurrentClass)
                {
                    case Class.Warrior when CurrentManager is WarriorLayoutVM wm:
                        wm.EdgeCounter.Val = m.Edge;
                        break;
                    case Class.Sorcerer when CurrentManager is SorcererLayoutVM sm:
                        sm.NotifyElementChanged();
                        break;
                }
            });
            PacketAnalyzer.NewProcessor.Hook<S_ABNORMALITY_BEGIN>(p =>
            {
                AbnormalityManager.CurrentAbnormalityTracker?.CheckAbnormality(p);
            });
            PacketAnalyzer.NewProcessor.Hook<S_ABNORMALITY_REFRESH>(p =>
            {
                AbnormalityManager.CurrentAbnormalityTracker?.CheckAbnormality(p);
            });
            PacketAnalyzer.NewProcessor.Hook<S_ABNORMALITY_END>(p =>
            {
                AbnormalityManager.CurrentAbnormalityTracker?.CheckAbnormality(p);
            });

        }

        //public bool IsTeraOnTop => WindowManager.IsTccVisible;
        private Class _currentClass = Class.None;
        public Class CurrentClass
        {
            get => _currentClass;
            set
            {
                if (_currentClass == value) return;
                _currentClass = value;

                Dispatcher.Invoke(() =>
                {
                    CurrentManager.Dispose();
                    switch (_currentClass)
                    {
                        case Class.Warrior:
                            CurrentManager = new WarriorLayoutVM();
                            break;
                        case Class.Valkyrie:
                            CurrentManager = new ValkyrieLayoutVM();
                            break;
                        case Class.Archer:
                            CurrentManager = new ArcherLayoutVM();
                            break;
                        case Class.Lancer:
                            CurrentManager = new LancerLayoutVM();
                            break;
                        case Class.Priest:
                            CurrentManager = new PriestLayoutVM();
                            break;
                        case Class.Mystic:
                            CurrentManager = new MysticLayoutVM();
                            break;
                        case Class.Slayer:
                            CurrentManager = new SlayerLayoutVM();
                            break;
                        case Class.Berserker:
                            CurrentManager = new BerserkerLayoutVM();
                            break;
                        case Class.Sorcerer:
                            CurrentManager = new SorcererLayoutVM();
                            break;
                        case Class.Reaper:
                            CurrentManager = new ReaperLayoutVM();
                            break;
                        case Class.Gunner:
                            CurrentManager = new GunnerLayoutVM();
                            break;
                        case Class.Brawler:
                            CurrentManager = new BrawlerLayoutVM();
                            break;
                        case Class.Ninja:
                            CurrentManager = new NinjaLayoutVM();
                            break;
                        default:
                            CurrentManager = new NullClassManager();
                            break;
                    }
                });
                N();
            }
        }

        private BaseClassLayoutVM _currentManager = new NullClassManager();
        public BaseClassLayoutVM CurrentManager
        {
            get => _currentManager;
            set
            {
                if (_currentManager == value) return;
                _currentManager = value;
                CurrentManager = _currentManager;
                N();
                CurrentManager.LoadSpecialSkills();
            }
        }
    }

    public class ClassWindowTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Warrior { get; set; }
        public DataTemplate Archer { get; set; }
        public DataTemplate Ninja { get; set; }
        public DataTemplate Mystic { get; set; }
        public DataTemplate Priest { get; set; }
        public DataTemplate Lancer { get; set; }
        public DataTemplate Brawler { get; set; }
        public DataTemplate Sorcerer { get; set; }
        public DataTemplate Slayer { get; set; }
        public DataTemplate Berserker { get; set; }
        public DataTemplate Gunner { get; set; }
        public DataTemplate Valkyrie { get; set; }
        public DataTemplate Reaper { get; set; }

        public DataTemplate None { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            switch (WindowManager.ViewModels.Class.CurrentClass)
            {
                case Class.Warrior: return Warrior;
                case Class.Lancer: return Lancer;
                case Class.Slayer: return Slayer;
                case Class.Berserker: return Berserker;
                case Class.Sorcerer: return Sorcerer;
                case Class.Archer: return Archer;
                case Class.Priest: return Priest;
                case Class.Mystic: return Mystic;
                case Class.Reaper: return Reaper;
                case Class.Gunner: return Gunner;
                case Class.Brawler: return Brawler;
                case Class.Ninja: return Ninja;
                case Class.Valkyrie: return Valkyrie;
                default: return None;
            }
        }
    }
}



