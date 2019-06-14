using System;
using System.Windows.Threading;
using TCC.Data.Pc;
using TCC.Parsing;
using TeraDataLite;
using TeraPacketParser.Messages;

namespace TCC.ViewModels
{
    public class CharacterWindowViewModel : TccWindowViewModel
    {
        public Player Player => Session.Me;


        public bool CompactMode => App.Settings.CharacterWindowSettings.CompactMode;

        public bool ShowRe => (!App.Settings.ClassWindowSettings.Visible || !App.Settings.ClassWindowSettings.Enabled) &&
                              (Player.Class == Class.Brawler || Player.Class == Class.Gunner ||
                               Player.Class == Class.Ninja || Player.Class == Class.Valkyrie);

        public bool ShowElements => Player.Class == Class.Sorcerer &&
                                 (!App.Settings.ClassWindowSettings.Visible
                                 || !App.Settings.ClassWindowSettings.Enabled
                                 || !App.Settings.ClassWindowSettings.SorcererReplacesElementsInCharWindow);

        public CharacterWindowViewModel()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;

            Session.Me.PropertyChanged += MePropertyChanged;
            App.Settings.ClassWindowSettings.EnabledChanged += ClassWindowSettings_EnabledChanged;
            App.Settings.ClassWindowSettings.VisibilityChanged += ClassWindowSettings_EnabledChanged;

        }

        protected override void InstallHooks()
        {
            PacketAnalyzer.NewProcessor.Hook<S_PLAYER_STAT_UPDATE>(m =>
            {
                Player.ItemLevel = m.Ilvl;
                Player.Level = m.Level;
                Player.CritFactor = m.BonusCritFactor;
                Player.Coins = m.Coins;
                Player.MaxCoins = m.MaxCoins;
                Player.MaxHP = m.MaxHP;
                Player.MaxMP = m.MaxMP;
                Player.MaxST = m.MaxST + m.BonusST;
                Player.CurrentHP = m.CurrentHP;
                Player.CurrentMP = m.CurrentMP;
                Player.CurrentST = m.CurrentST;

                if (Player.Class == Class.Sorcerer)
                {
                    Player.Fire = m.Fire;
                    Player.Ice = m.Ice;
                    Player.Arcane = m.Arcane;
                }
            });
            PacketAnalyzer.NewProcessor.Hook<S_LOGIN>(m =>
            {
                Player.ClearAbnormalities();
                Player.EntityId = m.EntityId;
                Player.PlayerId = m.PlayerId;
                Player.ServerId = m.ServerId;
                Player.Name = m.Name;
                Player.Level = m.Level;
            });
            PacketAnalyzer.NewProcessor.Hook<S_RETURN_TO_LOBBY>(m =>
            {
                Player.ClearAbnormalities();
            });
            PacketAnalyzer.NewProcessor.Hook<S_LOAD_TOPO>(m =>
            {
                Player.ClearAbnormalities();
            });
            PacketAnalyzer.NewProcessor.Hook<S_CREATURE_LIFE>(m =>
            {
                if (!Session.IsMe(m.Target)) return;
                Player.IsAlive = m.Alive;
            });
            PacketAnalyzer.NewProcessor.Hook<S_GET_USER_LIST>(m =>
            {
                Player.ClearAbnormalities();
            });
            PacketAnalyzer.NewProcessor.Hook<S_ABNORMALITY_DAMAGE_ABSORB>(p =>
            {
                // todo: add chat message too
                if (!Session.IsMe(p.Target) || Player.CurrentShield < 0) return;
                Player.DamageShield(p.Damage);
            });
            PacketAnalyzer.NewProcessor.Hook<S_CREATURE_CHANGE_HP>(m =>
            {
                if (!Session.IsMe(m.Target)) return;
                Player.MaxHP = m.MaxHP;
                Player.CurrentHP = m.CurrentHP;
            });
            PacketAnalyzer.NewProcessor.Hook<S_PLAYER_CHANGE_MP>(m =>
            {
                if (!Session.IsMe(m.Target)) return;
                Player.MaxMP = m.MaxMP;
                Player.CurrentMP = m.CurrentMP;
            });
            PacketAnalyzer.NewProcessor.Hook<S_PLAYER_CHANGE_STAMINA>(m =>
            {
                Player.MaxST = m.MaxST;
                Player.CurrentST = m.CurrentST;
            });


        }

        private void ClassWindowSettings_EnabledChanged()
        {
            N(nameof(ShowRe));
            N(nameof(ShowElements));
        }

        private void MePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            N(e.PropertyName);
            if (e.PropertyName != nameof(Data.Pc.Player.Class)) return;
            N(nameof(ShowRe));
            N(nameof(ShowElements));
        }

        public void InvokeCompactModeChanged()
        {
            N(nameof(CompactMode));
            Dispatcher.BeginInvoke(new Action(() =>
            {
                WindowManager.CharacterWindow.Left = App.Settings.CharacterWindowSettings.CompactMode
                ? WindowManager.CharacterWindow.Left + 175
                : WindowManager.CharacterWindow.Left - 175;
            }), DispatcherPriority.Background);
        }
    }

}

