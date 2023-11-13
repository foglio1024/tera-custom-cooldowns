using System;
using JetBrains.Annotations;
using TCC.Data.Skills;
using TCC.Settings.WindowSettings;
using TCC.UI;
using TCC.Utilities;
using TCC.Utils;
using TCC.ViewModels.ClassManagers;
using TeraDataLite;
using TeraPacketParser.Analysis;
using TeraPacketParser.Messages;

namespace TCC.ViewModels.Widgets;

[TccModule]
[UsedImplicitly]
public class ClassWindowViewModel : TccWindowViewModel
{
    Class _currentClass = Class.None;
    BaseClassLayoutViewModel _currentManager = new NullClassLayoutViewModel();

    public Class CurrentClass
    {
        get => _currentClass;
        set
        {
            if (_currentClass == value) return;
            _currentClass = value;

            _dispatcher.Invoke(() =>
            {
                CurrentManager.Dispose();
                CurrentManager = _currentClass switch
                {
                    Class.Warrior => new WarriorLayoutViewModel(),
                    Class.Valkyrie => new ValkyrieLayoutViewModel(),
                    Class.Archer => new ArcherLayoutViewModel(),
                    Class.Lancer => new LancerLayoutViewModel(),
                    Class.Priest => new PriestLayoutViewModel(),
                    Class.Mystic => new MysticLayoutVM(),
                    Class.Slayer => new SlayerLayoutViewModel(),
                    Class.Berserker => new BerserkerLayoutViewModel(),
                    Class.Sorcerer => new SorcererLayoutVM(),
                    Class.Reaper => new ReaperLayoutViewModel(),
                    Class.Gunner => new GunnerLayoutViewModel(),
                    Class.Brawler => new BrawlerLayoutViewModel(),
                    Class.Ninja => new NinjaLayoutViewModel(),
                    _ => new NullClassLayoutViewModel()
                };
            });
            N();
        }
    }

    public BaseClassLayoutViewModel CurrentManager
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
        settings.WarriorShowInfuriateChanged += OnWarriorShowInfuriateChanged;
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
            ((ClassWindowSettings)Settings!).WarriorShowEdgeChanged -= OnWarriorShowEdgeChanged;
            ((ClassWindowSettings)Settings).WarriorShowTraverseCutChanged -= OnWarriorShowTraverseCutChanged;
            ((ClassWindowSettings)Settings).WarriorShowInfuriateChanged -= OnWarriorShowInfuriateChanged;
            ((ClassWindowSettings)Settings).WarriorEdgeModeChanged -= OnWarriorEdgeModeChanged;
            ((ClassWindowSettings)Settings).SorcererShowElementsChanged -= OnSorcererShowElementsChanged;
            ((ClassWindowSettings)Settings).ValkyrieShowGodsfallChanged -= OnValkyrieShowGodsfallChanged;
            ((ClassWindowSettings)Settings).ValkyrieShowRagnarokChanged -= OnValkyrieShowRagnarokChanged;
            CurrentClass = Class.None;
        }
        else
        {
            ((ClassWindowSettings)Settings!).WarriorShowEdgeChanged += OnWarriorShowEdgeChanged;
            ((ClassWindowSettings)Settings).WarriorShowTraverseCutChanged += OnWarriorShowTraverseCutChanged;
            ((ClassWindowSettings)Settings).WarriorShowInfuriateChanged += OnWarriorShowInfuriateChanged;
            ((ClassWindowSettings)Settings).WarriorEdgeModeChanged += OnWarriorEdgeModeChanged;
            ((ClassWindowSettings)Settings).SorcererShowElementsChanged += OnSorcererShowElementsChanged;
            ((ClassWindowSettings)Settings).ValkyrieShowGodsfallChanged += OnValkyrieShowGodsfallChanged;
            ((ClassWindowSettings)Settings).ValkyrieShowRagnarokChanged += OnValkyrieShowRagnarokChanged;
            CurrentClass = Game.Me.Class;
        }
    }

    void OnValkyrieShowRagnarokChanged()
    {
        TccUtils.CurrentClassVM<ValkyrieLayoutViewModel>()?.ExN(nameof(ValkyrieLayoutViewModel.ShowRagnarok));
    }

    void OnValkyrieShowGodsfallChanged()
    {
        TccUtils.CurrentClassVM<ValkyrieLayoutViewModel>()?.ExN(nameof(ValkyrieLayoutViewModel.ShowGodsfall));
    }

    void OnWarriorEdgeModeChanged()
    {
        TccUtils.CurrentClassVM<WarriorLayoutViewModel>()?.ExN(nameof(WarriorLayoutViewModel.WarriorEdgeMode));
    }

    void OnWarriorShowTraverseCutChanged()
    {
        TccUtils.CurrentClassVM<WarriorLayoutViewModel>()?.ExN(nameof(WarriorLayoutViewModel.ShowTraverseCut));
    }

    void OnWarriorShowInfuriateChanged()
    {
        TccUtils.CurrentClassVM<WarriorLayoutViewModel>()?.ExN(nameof(WarriorLayoutViewModel.ShowInfuriate));
    }

    void OnWarriorShowEdgeChanged()
    {
        TccUtils.CurrentClassVM<WarriorLayoutViewModel>()?.ExN(nameof(WarriorLayoutViewModel.ShowEdge));
    }

    void OnSorcererShowElementsChanged()
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
        PacketAnalyzer.Processor.Hook<S_CREST_MESSAGE>(OnCrestMessage);
    }

    protected override void RemoveHooks()
    {
        PacketAnalyzer.Processor.Unhook<S_LOGIN>(OnLogin);
        PacketAnalyzer.Processor.Unhook<S_GET_USER_LIST>(OnGetUserList);
        PacketAnalyzer.Processor.Unhook<S_PLAYER_STAT_UPDATE>(OnPlayerStatUpdate);
        PacketAnalyzer.Processor.Unhook<S_PLAYER_CHANGE_STAMINA>(OnPlayerChangeStamina);
        PacketAnalyzer.Processor.Unhook<S_START_COOLTIME_SKILL>(OnStartCooltimeSkill);
        PacketAnalyzer.Processor.Unhook<S_DECREASE_COOLTIME_SKILL>(OnDecreaseCooltimeSkill);
        PacketAnalyzer.Processor.Unhook<S_CREST_MESSAGE>(OnCrestMessage);
    }

    void OnLogin(S_LOGIN m)
    {
        _dispatcher.InvokeAsync(() =>
        {
            CurrentClass = m.CharacterClass;
        });
        if (m.CharacterClass == Class.Valkyrie)
            PacketAnalyzer.Processor.Hook<S_WEAK_POINT>(OnWeakPoint);
        else
            PacketAnalyzer.Processor.Unhook<S_WEAK_POINT>(OnWeakPoint);
    }

    void OnGetUserList(S_GET_USER_LIST m)
    {
        CurrentClass = Class.None;
    }

    void OnPlayerStatUpdate(S_PLAYER_STAT_UPDATE m)
    {
        // check enabled?
        switch (CurrentClass)
        {
            case Class.Sorcerer when CurrentManager is SorcererLayoutVM sm:
                sm.NotifyElementChanged();
                break;
        }
    }

    void OnPlayerChangeStamina(S_PLAYER_CHANGE_STAMINA m)
    {
        CurrentManager.SetMaxST(Convert.ToInt32(m.MaxST));
        CurrentManager.SetST(Convert.ToInt32(m.CurrentST));
    }

    void OnWeakPoint(S_WEAK_POINT p)
    {
        if (CurrentManager is not ValkyrieLayoutViewModel vvm) return;
        vvm.RunemarksCounter.Val = p.TotalRunemarks;
    }

    void OnStartCooltimeSkill(S_START_COOLTIME_SKILL m)
    {
        if (!Game.DB!.SkillsDatabase.TryGetSkill(m.SkillId, Game.Me.Class, out var skill)) return;
        CurrentManager.StartSpecialSkill(new Cooldown(skill, m.Cooldown));
    }

    void OnDecreaseCooltimeSkill(S_DECREASE_COOLTIME_SKILL m)
    {
        if (!Game.DB!.SkillsDatabase.TryGetSkill(m.SkillId, Game.Me.Class, out var skill)) return;
        CurrentManager.ChangeSpecialSkill(skill, m.Cooldown);
    }

    void OnCrestMessage(S_CREST_MESSAGE m)
    {
        if (m.Type != 6) return;
        if (!Game.DB!.SkillsDatabase.TryGetSkill(m.SkillId, Game.Me.Class, out var skill)) return;
        CurrentManager.ResetSpecialSkill(skill);
    }
}