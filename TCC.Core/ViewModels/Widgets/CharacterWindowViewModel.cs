using System.ComponentModel;
using System.Windows.Threading;
using TCC.Data.Pc;
using TCC.Parsing;
using TCC.Settings;
using TCC.Utils;
using TeraDataLite;
using TeraPacketParser.Messages;

namespace TCC.ViewModels.Widgets
{
    //TODO: remove references to other modules?
    [TccModule]
    public class CharacterWindowViewModel : TccWindowViewModel
    {
        public Player Player => Game.Me;
        public bool CompactMode => App.Settings.CharacterWindowSettings.CompactMode;
        public bool ShowRe =>
            (!App.Settings.ClassWindowSettings.Visible || !App.Settings.ClassWindowSettings.Enabled) &&
            (Player.Class == Class.Brawler || Player.Class == Class.Gunner ||
             Player.Class == Class.Ninja || Player.Class == Class.Valkyrie);
        public bool ShowElements => Player.Class == Class.Sorcerer &&
                                    (!App.Settings.ClassWindowSettings.Visible
                                     || !App.Settings.ClassWindowSettings.Enabled
                                     || !App.Settings.ClassWindowSettings.SorcererReplacesElementsInCharWindow);

        public CharacterWindowViewModel(WindowSettings settings) : base(settings)
        {
            Game.Me.PropertyChanged += MePropertyChanged;
            App.Settings.ClassWindowSettings.EnabledChanged += ClassWindowSettings_EnabledChanged;
            App.Settings.ClassWindowSettings.VisibilityChanged += ClassWindowSettings_EnabledChanged;
            ((CharacterWindowSettings)settings).CompactModeChanged += InvokeCompactModeChanged;
        }

        protected override void InstallHooks()
        {
            PacketAnalyzer.Processor.Hook<S_LOGIN>(OnLogin);
            PacketAnalyzer.Processor.Hook<S_PLAYER_STAT_UPDATE>(OnPlayerStatUpdate);
            PacketAnalyzer.Processor.Hook<S_RETURN_TO_LOBBY>(OnReturnToLobby);
            PacketAnalyzer.Processor.Hook<S_LOAD_TOPO>(OnLoadTopo);
            PacketAnalyzer.Processor.Hook<S_CREATURE_LIFE>(OnCreatureLife);
            PacketAnalyzer.Processor.Hook<S_GET_USER_LIST>(OnGetUserList);
            PacketAnalyzer.Processor.Hook<S_ABNORMALITY_DAMAGE_ABSORB>(OnAbnormalityDamageAbsorb);
            PacketAnalyzer.Processor.Hook<S_CREATURE_CHANGE_HP>(OnCreatureChangeHp);
            PacketAnalyzer.Processor.Hook<S_PLAYER_CHANGE_MP>(OnPlayerChangeMp);
            PacketAnalyzer.Processor.Hook<S_PLAYER_CHANGE_STAMINA>(OnPlayerChangeStamina);
        }

        protected override void RemoveHooks()
        {
            PacketAnalyzer.Processor.Unhook<S_LOGIN>(OnLogin);
            PacketAnalyzer.Processor.Unhook<S_PLAYER_STAT_UPDATE>(OnPlayerStatUpdate);
            PacketAnalyzer.Processor.Unhook<S_RETURN_TO_LOBBY>(OnReturnToLobby);
            PacketAnalyzer.Processor.Unhook<S_LOAD_TOPO>(OnLoadTopo);
            PacketAnalyzer.Processor.Unhook<S_CREATURE_LIFE>(OnCreatureLife);
            PacketAnalyzer.Processor.Unhook<S_GET_USER_LIST>(OnGetUserList);
            PacketAnalyzer.Processor.Unhook<S_ABNORMALITY_DAMAGE_ABSORB>(OnAbnormalityDamageAbsorb);
            PacketAnalyzer.Processor.Unhook<S_CREATURE_CHANGE_HP>(OnCreatureChangeHp);
            PacketAnalyzer.Processor.Unhook<S_PLAYER_CHANGE_MP>(OnPlayerChangeMp);
            PacketAnalyzer.Processor.Unhook<S_PLAYER_CHANGE_STAMINA>(OnPlayerChangeStamina);
        }

        private void OnLogin(S_LOGIN m)
        {
            Player.ClearAbnormalities();
            Player.EntityId = m.EntityId;
            Player.PlayerId = m.PlayerId;
            Player.ServerId = m.ServerId;
            Player.Name = m.Name;
            Player.Level = m.Level;
        }
        private void OnPlayerChangeStamina(S_PLAYER_CHANGE_STAMINA m)
        {
            Player.MaxST = m.MaxST;
            Player.CurrentST = m.CurrentST;
        }
        private void OnPlayerChangeMp(S_PLAYER_CHANGE_MP m)
        {
            if (!Game.IsMe(m.Target)) return;
            Player.MaxMP = m.MaxMP;
            Player.CurrentMP = m.CurrentMP;
        }
        private void OnCreatureChangeHp(S_CREATURE_CHANGE_HP m)
        {
            if (!Game.IsMe(m.Target)) return;
            Player.MaxHP = m.MaxHP;
            Player.CurrentHP = m.CurrentHP;
        }
        private void OnAbnormalityDamageAbsorb(S_ABNORMALITY_DAMAGE_ABSORB p)
        {
            // todo: add chat message too
            if (!Game.IsMe(p.Target) || Player.CurrentShield < 0) return;
            Player.DamageShield(p.Damage);
        }
        private void OnGetUserList(S_GET_USER_LIST m)
        {
            Player.ClearAbnormalities();
        }
        private void OnCreatureLife(S_CREATURE_LIFE m)
        {
            if (!Game.IsMe(m.Target)) return;
            Player.IsAlive = m.Alive;
        }
        private void OnLoadTopo(S_LOAD_TOPO m)
        {
            Player.ClearAbnormalities();
        }
        private void OnReturnToLobby(S_RETURN_TO_LOBBY m)
        {
            Player.ClearAbnormalities();
        }
        private void OnPlayerStatUpdate(S_PLAYER_STAT_UPDATE m)
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

            if (Player.Class != Class.Sorcerer) return;
            Player.Fire = m.Fire;
            Player.Ice = m.Ice;
            Player.Arcane = m.Arcane;
        }

        private void ClassWindowSettings_EnabledChanged(bool enabled)
        {
            N(nameof(ShowRe));
            N(nameof(ShowElements));
        }

        private void MePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            N(e.PropertyName);
            if (e.PropertyName != nameof(Data.Pc.Player.Class)) return;
            N(nameof(ShowRe));
            N(nameof(ShowElements));
        }

        public void InvokeCompactModeChanged()
        {
            N(nameof(CompactMode));
            Dispatcher.InvokeAsync(() =>
            {
                WindowManager.CharacterWindow.Left = App.Settings.CharacterWindowSettings.CompactMode
                    ? WindowManager.CharacterWindow.Left + 175
                    : WindowManager.CharacterWindow.Left - 175;
            }, DispatcherPriority.Background);
        }
    }
}