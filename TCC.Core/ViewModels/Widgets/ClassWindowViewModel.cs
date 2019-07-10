using System;
using TCC.Parsing;
using TCC.Settings;
using TeraDataLite;
using TeraPacketParser.Messages;

namespace TCC.ViewModels.Widgets
{
    [TccModule]
    public class ClassWindowViewModel : TccWindowViewModel
    {
        private Class _currentClass = Class.None;
        private BaseClassLayoutVM _currentManager = new NullClassManager();

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

        public ClassWindowViewModel(WindowSettings settings) : base(settings) { }

        protected override void InstallHooks()
        {
            PacketAnalyzer.NewProcessor.Hook<S_LOGIN>(OnLogin);
            PacketAnalyzer.NewProcessor.Hook<S_RETURN_TO_LOBBY>(OnReturnToLobby);
            PacketAnalyzer.NewProcessor.Hook<S_PLAYER_STAT_UPDATE>(OnPlayerStatUpdate);
            PacketAnalyzer.NewProcessor.Hook<S_PLAYER_CHANGE_STAMINA>(OnPlayerChangeStamina);
            PacketAnalyzer.NewProcessor.Hook<S_ABNORMALITY_BEGIN>(OnAbnormalityBegin);
            PacketAnalyzer.NewProcessor.Hook<S_ABNORMALITY_REFRESH>(OnAbnormalityRefresh);
            PacketAnalyzer.NewProcessor.Hook<S_ABNORMALITY_END>(OnAbnormalityEnd);
        }

        protected override void RemoveHooks()
        {
            PacketAnalyzer.NewProcessor.Unhook<S_LOGIN>(OnLogin);
            PacketAnalyzer.NewProcessor.Unhook<S_RETURN_TO_LOBBY>(OnReturnToLobby);
            PacketAnalyzer.NewProcessor.Unhook<S_PLAYER_STAT_UPDATE>(OnPlayerStatUpdate);
            PacketAnalyzer.NewProcessor.Unhook<S_PLAYER_CHANGE_STAMINA>(OnPlayerChangeStamina);
            PacketAnalyzer.NewProcessor.Unhook<S_ABNORMALITY_BEGIN>(OnAbnormalityBegin);
            PacketAnalyzer.NewProcessor.Unhook<S_ABNORMALITY_REFRESH>(OnAbnormalityRefresh);
            PacketAnalyzer.NewProcessor.Unhook<S_ABNORMALITY_END>(OnAbnormalityEnd);
        }

        private void OnLogin(S_LOGIN m)
        {
            CurrentClass = m.CharacterClass; // todo: check for enabled?
            if (m.CharacterClass == Class.Valkyrie)
            {
                PacketAnalyzer.NewProcessor.Hook<S_WEAK_POINT>(OnWeakPoint);
            }
            else
            {
                PacketAnalyzer.NewProcessor.Unhook<S_WEAK_POINT>(OnWeakPoint);
            }
        }
        private void OnReturnToLobby(S_RETURN_TO_LOBBY m)
        {
            CurrentClass = Class.None;
        }
        private void OnPlayerStatUpdate(S_PLAYER_STAT_UPDATE m)
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
        }
        private void OnPlayerChangeStamina(S_PLAYER_CHANGE_STAMINA m)
        {
            CurrentManager.SetMaxST(Convert.ToInt32(m.MaxST));
            CurrentManager.SetST(Convert.ToInt32(m.CurrentST));
        }
        private void OnWeakPoint(S_WEAK_POINT p)
        {
            if (!(CurrentManager is ValkyrieLayoutVM vvm)) return;
            vvm.RunemarksCounter.Val = p.TotalRunemarks;
        }
        private void OnAbnormalityBegin(S_ABNORMALITY_BEGIN p)
        {
            AbnormalityManager.CurrentAbnormalityTracker?.CheckAbnormality(p);
        }
        private void OnAbnormalityRefresh(S_ABNORMALITY_REFRESH p)
        {
            AbnormalityManager.CurrentAbnormalityTracker?.CheckAbnormality(p);
        }
        private void OnAbnormalityEnd(S_ABNORMALITY_END p)
        {
            AbnormalityManager.CurrentAbnormalityTracker?.CheckAbnormality(p);
        }
    }
}