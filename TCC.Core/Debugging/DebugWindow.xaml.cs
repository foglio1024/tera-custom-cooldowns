using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using TCC.Data;
using TCC.Debug;
using TCC.UI;
using TCC.Utilities;
using TCC.ViewModels.ClassManagers;
using TeraDataLite;

namespace TCC.Debugging;

public sealed partial class DebugWindow : INotifyPropertyChanged
{
    public DebugWindow()
    {
        InitializeComponent();
    }

    int _last;
    int _sum;
    int _max;

    public int Last
    {
        get => _last;
        private set { _last = value; NPC(); }
    }
    public int Max
    {
        get => _max;
        private set { _max = value; NPC(); }
    }

    int Sum
    {
        get => _sum;
        set { _sum = value; NPC(); }
    }
    public double Avg => _count == 0 ? 0 : Sum / (float)_count;

    int _count;
    public void SetQueuedPackets(int val)
    {
        Last = val;
        if (val > Max) Max = val;
        NPC(nameof(Avg));
        if (int.MaxValue - Sum < val)
        {
            Sum = 0;
            _count = 0;
        }
        else Sum += val;

        _count++;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    void NPC([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    void SwitchClass(object sender, RoutedEventArgs e)
    {
        Game.Me.Class = Enum.Parse<Class>(((Button) sender).Content.ToString() ?? "None");
        WindowManager.ViewModels.ClassVM.CurrentClass = Game.Me.Class;
        WindowManager.ViewModels.CooldownsVM.ClearSkills();
        WindowManager.ViewModels.CooldownsVM.LoadConfig(Game.Me.Class);
    }

    void SetSorcElement(object sender, RoutedEventArgs e)
    {
        var el = ((Button) sender).Content.ToString();

        var fire = el == "Fire";
        var ice = el == "Ice";
        var arc = el == "Arcane";

        if (!(App.Settings.ClassWindowSettings.Enabled
            && Game.Me.Class == Class.Sorcerer
            && WindowManager.ViewModels.ClassVM.CurrentManager is SorcererLayoutViewModel sm))
        {
            return;
        }

        var currFire = (sm.Elements & FusionElements.Flame) == FusionElements.Flame;
        var currIce = (sm.Elements & FusionElements.Frost) == FusionElements.Frost;
        var currArc = (sm.Elements & FusionElements.Arcane) == FusionElements.Arcane;

        if (fire) SetSorcererElements(!currFire, currIce, currArc);
        if (ice) SetSorcererElements(currFire, !currIce, currArc);
        if (arc) SetSorcererElements(currFire, currIce, !currArc);
      
        return;

        static void SetSorcererElements(bool pFire, bool pIce, bool pArcane)
        {
            var elements = FusionElements.None;

            if (pFire) elements |= FusionElements.Flame;
            if (pIce) elements |= FusionElements.Frost;
            if (pArcane) elements |= FusionElements.Arcane;

            if (App.Settings.ClassWindowSettings.Enabled
                && Game.Me.Class == Class.Sorcerer
                && WindowManager.ViewModels.ClassVM.CurrentManager is SorcererLayoutViewModel sm)
            {
                sm.SetElements(elements);
            }

        }

    }

    void SetSorcElementBoost(object sender, RoutedEventArgs e)
    {
        var el = ((Button) sender).Content.ToString()?.Split(' ')[0];

        var fire = el == "Fire";
        var ice = el == "Ice";
        var arc = el == "Arcane";

        if (!(App.Settings.ClassWindowSettings.Enabled
              && Game.Me.Class == Class.Sorcerer
              && WindowManager.ViewModels.ClassVM.CurrentManager is SorcererLayoutViewModel sm))
        {
            return;
        }


        var currFire = (sm.Boosts & FusionElements.Flame) == FusionElements.Flame;
        var currIce = (sm.Boosts & FusionElements.Frost) == FusionElements.Frost;
        var currArc = (sm.Boosts & FusionElements.Arcane) == FusionElements.Arcane;

        if (fire) SetBoosts(!currFire, currIce, currArc);
        if (ice) SetBoosts(currFire, !currIce, currArc);
        if (arc) SetBoosts(currFire, currIce, !currArc);

        static void SetBoosts(bool pFire, bool pIce, bool pArcane)
        {
            var elements = FusionElements.None;

            if (pFire) elements |= FusionElements.Flame;
            if (pIce) elements |= FusionElements.Frost;
            if (pArcane) elements |= FusionElements.Arcane;

            if (App.Settings.ClassWindowSettings.Enabled
                && Game.Me.Class == Class.Sorcerer
                && WindowManager.ViewModels.ClassVM.CurrentManager is SorcererLayoutViewModel sm)
            {
                sm.Boosts = elements;
            }
        }


    }

    void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        TccUtils.CurrentClassVM<SorcererLayoutViewModel>()?.ManaBoost.StartEffect(10000);

        //SkillManager.AddSkill(100700, 20000);
        //SkillManager.AddSkill(400120, 20000);
        //SkillManager.AddItemSkill(6298, 10);
        //SkillManager.ResetSkill(400120);
    }

    void SetStance(object sender, RoutedEventArgs e)
    {
        TccUtils.CurrentClassVM<WarriorLayoutViewModel>()!.StanceTracker.CurrentStance = ((Button)sender).Content.ToString() switch
        {
            "Assault" => WarriorStance.Assault,
            "Defensive" => WarriorStance.Defensive,
            _ => WarriorStance.None,
        };
    }

    void IncreaseEdge(object sender, RoutedEventArgs e)
    {
        var edge = TccUtils.CurrentClassVM<WarriorLayoutViewModel>()!.EdgeCounter;
        if (edge.IsMaxed) edge.Val = 0;
        edge.Val++;

    }

    void RegisterWebhook(object sender, RoutedEventArgs e)
    {
        for (var i = 0; i < 80; i++)
        {
            var i1 = i;
            Dispatcher.InvokeAsync(() => Tester.RegisterWebhook("user" + i1));
        }
    }

    void FireWebhook(object sender, RoutedEventArgs e)
    {
        for (var i = 0; i < 80; i++)
        {
            var i1 = i;
            Dispatcher.InvokeAsync(() => Tester.FireWebhook("user" + i1));
        }
    }

    void DungeonTest(object sender, RoutedEventArgs e)
    {
        //i = 0;
        //WindowManager.Dashboard.VM.SetDungeons(20000078, new Dictionary<uint, short>() { { 9770U, i++ } });

        Game.Account.Characters[0].VanguardInfo.DailiesDone = App.Random.Next(0, 16);
        Game.Account.Characters[0].VanguardInfo.WeekliesDone = App.Random.Next(0, 16);
    }

    void MaxEnchant(object sender, RoutedEventArgs e)
    {
        //"@464\vUserName\vHeve\vItemName\v@item:89607?dbid:327641239?enchantCount:11"

        Tester.AddFakeSystemMessage("SMT_MAX_ENCHANT_SUCCEED", "UserName", "Foglio", "ItemName", "@item:89607?dbid:327641239?enchantCount:11");
    }

    void NotifyGuildBam(object sender, RoutedEventArgs e)
    {
        Tester.AddFakeSystemMessage("SMT_GQUEST_URGENT_NOTIFY", "questName", "Frygaras", "npcName", "Frygaras", "zoneName", "Zone");
    }

    void NotifyFieldBoss(object sender, RoutedEventArgs e)
    {
        Tester.AddFakeSystemMessage("SMT_FIELDBOSS_APPEAR", "npcName", "Ortan", "RegionName", "Zone");
    }

    void NotifyFieldBossDie(object sender, RoutedEventArgs e)
    {
        Tester.AddFakeSystemMessage("SMT_FIELDBOSS_DIE_NOGUILD", "userName", "Foglio", "npcName", "Ortan");
    }

    void SetAtkSpeed(object sender, RoutedEventArgs e)
    {
        switch (((Button)sender).Content.ToString())
        {
            case "Swift":
                TccUtils.CurrentClassVM<WarriorLayoutViewModel>()?.SetSwift(1000);
                break;
            case "Arush":
                TccUtils.CurrentClassVM<WarriorLayoutViewModel>()?.SetArush(1000);
                break;
        }
    }

    void Unleash(object sender, RoutedEventArgs e)
    {
        Tester.StartUnleash();
    }

    private void NotifyMysteryMerchant(object sender, RoutedEventArgs e)
    {
        Tester.AddFakeSystemMessage("SMT_WORLDSPAWN_NOTIFY_SPAWN", "npcname", "Gokoro", "RegionName", "Allemantheia");
    }
}