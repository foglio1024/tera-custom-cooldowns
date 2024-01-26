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

        var currFire = Game.Me.Fire;
        var currIce = Game.Me.Ice;
        var currArc = Game.Me.Arcane;

        if (fire) SetSorcererElements(!currFire, currIce, currArc);
        if (ice) SetSorcererElements(currFire, !currIce, currArc);
        if (arc) SetSorcererElements(currFire, currIce, !currArc);
      
        return;

        static void SetSorcererElements(bool pFire, bool pIce, bool pArcane)
        {
            Game.Me.Fire = pFire;
            Game.Me.Ice = pIce;
            Game.Me.Arcane = pArcane;

            if (App.Settings.ClassWindowSettings.Enabled
                && Game.Me.Class == Class.Sorcerer
                && WindowManager.ViewModels.ClassVM.CurrentManager is SorcererLayoutViewModel sm)
            {
                sm.NotifyElementChanged();
            }

        }

    }

    void SetSorcElementBoost(object sender, RoutedEventArgs e)
    {
        var el = ((Button) sender).Content.ToString()?.Split(' ')[0];

        var fire = el == "Fire";
        var ice = el == "Ice";
        var arc = el == "Arcane";

        var currFire = Game.Me.FireBoost;
        var currIce = Game.Me.IceBoost;
        var currArc = Game.Me.ArcaneBoost;

        if (fire) Game.SetSorcererElementsBoost(!currFire, currIce, currArc);
        if (ice) Game.SetSorcererElementsBoost(currFire, !currIce, currArc);
        if (arc) Game.SetSorcererElementsBoost(currFire, currIce, !currArc);


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
        Game.Me.WarriorStance.CurrentStance = ((Button)sender).Content.ToString() switch
        {
            "Assault" => WarriorStance.Assault,
            "Defensive" => WarriorStance.Defensive,
            "None" => WarriorStance.None,
            _ => Game.Me.WarriorStance.CurrentStance
        };
    }

    void IncreaseEdge(object sender, RoutedEventArgs e)
    {
        if (Game.Me.StacksCounter.IsMaxed) Game.Me.StacksCounter.Val = 0;
        Game.Me.StacksCounter.Val++;

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