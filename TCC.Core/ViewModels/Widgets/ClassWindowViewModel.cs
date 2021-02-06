using System;
using TCC.Data.Skills;
using TCC.Analysis;
using TCC.Settings.WindowSettings;
using TCC.UI;
using TCC.Utilities;
using TCC.Utils;
using TeraDataLite;
using TeraPacketParser.Messages;

namespace TCC.ViewModels.Widgets
{
    [TccModule]
    public class ClassWindowViewModel : TccWindowViewModel
    {
        private Class _currentClass = Class.None;
        private BaseClassLayoutVM _currentManager = new NullClassLayoutVM();

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
                    CurrentManager = _currentClass switch
                    {
                        Class.Warrior => new WarriorLayoutVM(),
                        Class.Valkyrie => new ValkyrieLayoutVM(),
                        Class.Archer => new ArcherLayoutVM(),
                        Class.Lancer => new LancerLayoutVM(),
                        Class.Priest => new PriestLayoutVM(),
                        Class.Mystic => new MysticLayoutVM(),
                        Class.Slayer => new SlayerLayoutVM(),
                        Class.Berserker => new BerserkerLayoutVM(),
                        Class.Sorcerer => new SorcererLayoutVM(),
                        Class.Reaper => new ReaperLayoutVM(),
                        Class.Gunner => new GunnerLayoutVM(),
                        Class.Brawler => new BrawlerLayoutVM(),
                        Class.Ninja => new NinjaLayoutVM(),
                        _ => new NullClassLayoutVM()
                    };
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
            }
        }

        public ClassWindowViewModel(ClassWindowSettings settings) : base(settings)
        {
            if (!settings.Enabled) return;
            settings.WarriorShowEdgeChanged += OnWarriorShowEdgeChanged;
            settings.WarriorShowTraverseCutChanged += OnWarriorShowTraverseCutChanged;
            settings.WarriorEdgeModeChanged += OnWarriorEdgeModeChanged;
            settings.SorcererShowElementsChanged += OnSorcererShowElementsChanged;
            settings.ValkyrieShowGodsfallChanged += OnValkyrieShowGodsfallChanged;
            settings.ValkyrieShowRagnarokChanged += OnValkyrieShowRagnarokChanged;
        }

        protected override void OnEnabledChanged(bool enabled)
        {
            base.OnEnabledChanged(enabled);
            if (!enabled)
            {
                CurrentClass = Class.None;
                ((ClassWindowSettings)Settings!).WarriorShowEdgeChanged -= OnWarriorShowEdgeChanged;
                ((ClassWindowSettings)Settings).WarriorShowTraverseCutChanged -= OnWarriorShowTraverseCutChanged;
                ((ClassWindowSettings)Settings).WarriorEdgeModeChanged -= OnWarriorEdgeModeChanged;
                ((ClassWindowSettings)Settings).SorcererShowElementsChanged -= OnSorcererShowElementsChanged;
                ((ClassWindowSettings)Settings).ValkyrieShowGodsfallChanged -= OnValkyrieShowGodsfallChanged;
                ((ClassWindowSettings)Settings).ValkyrieShowRagnarokChanged -= OnValkyrieShowRagnarokChanged;
            }
            else
            {
                ((ClassWindowSettings)Settings!).WarriorShowEdgeChanged += OnWarriorShowEdgeChanged;
                ((ClassWindowSettings)Settings).WarriorShowTraverseCutChanged += OnWarriorShowTraverseCutChanged;
                ((ClassWindowSettings)Settings).WarriorEdgeModeChanged += OnWarriorEdgeModeChanged;
                ((ClassWindowSettings)Settings).SorcererShowElementsChanged += OnSorcererShowElementsChanged;
                ((ClassWindowSettings)Settings).ValkyrieShowGodsfallChanged += OnValkyrieShowGodsfallChanged;
                ((ClassWindowSettings)Settings).ValkyrieShowRagnarokChanged += OnValkyrieShowRagnarokChanged;
            }
        }

        private void OnValkyrieShowRagnarokChanged()
        {
            TccUtils.CurrentClassVM<ValkyrieLayoutVM>()?.ExN(nameof(ValkyrieLayoutVM.ShowRagnarok));
        }

        private void OnValkyrieShowGodsfallChanged()
        {
            TccUtils.CurrentClassVM<ValkyrieLayoutVM>()?.ExN(nameof(ValkyrieLayoutVM.ShowGodsfall));
        }

        private void OnWarriorEdgeModeChanged()
        {
            TccUtils.CurrentClassVM<WarriorLayoutVM>()?.ExN(nameof(WarriorLayoutVM.WarriorEdgeMode));
        }

        private void OnWarriorShowTraverseCutChanged()
        {
            TccUtils.CurrentClassVM<WarriorLayoutVM>()?.ExN(nameof(WarriorLayoutVM.ShowTraverseCut));
        }

        private void OnWarriorShowEdgeChanged()
        {
            TccUtils.CurrentClassVM<WarriorLayoutVM>()?.ExN(nameof(WarriorLayoutVM.ShowEdge));
        }

        private void OnSorcererShowElementsChanged()
        {
            // TODO: delet this
            WindowManager.ViewModels.CharacterVM.ExN(nameof(CharacterWindowViewModel.ShowElements));
        }

        protected override void InstallHooks()
        {
            PacketAnalyzer.Processor.Hook<S_LOGIN>(OnLogin);
            PacketAnalyzer.Processor.Hook<S_GET_USER_LIST>(OnGetUserList);
            PacketAnalyzer.Processor.Hook<S_PLAYER_STAT_UPDATE>(OnPlayerStatUpdate);
            PacketAnalyzer.Processor.Hook<S_PLAYER_CHANGE_STAMINA>(OnPlayerChangeStamina);
            PacketAnalyzer.Processor.Hook<S_START_COOLTIME_SKILL>(OnStartCooltimeSkill);
            PacketAnalyzer.Processor.Hook<S_DECREASE_COOLTIME_SKILL>(OnDecreaseCooltimeSkill);
        }

        protected override void RemoveHooks()
        {
            PacketAnalyzer.Processor.Unhook<S_LOGIN>(OnLogin);
            PacketAnalyzer.Processor.Unhook<S_GET_USER_LIST>(OnGetUserList);
            PacketAnalyzer.Processor.Unhook<S_PLAYER_STAT_UPDATE>(OnPlayerStatUpdate);
            PacketAnalyzer.Processor.Unhook<S_PLAYER_CHANGE_STAMINA>(OnPlayerChangeStamina);
            PacketAnalyzer.Processor.Unhook<S_START_COOLTIME_SKILL>(OnStartCooltimeSkill);
            PacketAnalyzer.Processor.Unhook<S_DECREASE_COOLTIME_SKILL>(OnDecreaseCooltimeSkill);
        }

        private void UpdateSkillCooldown(uint skillId, uint cooldown)
        {
            if (!App.Settings.ClassWindowSettings.Enabled) return;
            if (!Game.DB!.SkillsDatabase.TryGetSkill(skillId, Game.Me.Class, out var skill)) return;
            CurrentManager.StartSpecialSkill(new Cooldown(skill, cooldown));
        }

        private void OnLogin(S_LOGIN m)
        {
            Dispatcher.InvokeAsync(() =>
            {
                CurrentClass = m.CharacterClass;
            });
            if (m.CharacterClass == Class.Valkyrie)
                PacketAnalyzer.Processor.Hook<S_WEAK_POINT>(OnWeakPoint);
            else
                PacketAnalyzer.Processor.Unhook<S_WEAK_POINT>(OnWeakPoint);
        }

        private void OnGetUserList(S_GET_USER_LIST m)
        {
            CurrentClass = Class.None;
        }

        private void OnPlayerStatUpdate(S_PLAYER_STAT_UPDATE m)
        {
            // check enabled?
            switch (CurrentClass)
            {
                //case Class.Warrior when CurrentManager is WarriorLayoutVM wm:
                //    wm.EdgeCounter.Val = m.Edge;
                //    break;
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

        private void OnStartCooltimeSkill(S_START_COOLTIME_SKILL m)
        {
            UpdateSkillCooldown(m.SkillId, m.Cooldown);
        }

        private void OnDecreaseCooltimeSkill(S_DECREASE_COOLTIME_SKILL m)
        {
            UpdateSkillCooldown(m.SkillId, m.Cooldown);
        }
    }
}