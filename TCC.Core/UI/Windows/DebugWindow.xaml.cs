using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Windows;
using TCC.Annotations;
using TCC.Data;
using TCC.Test;
using TCC.Utilities;
using TCC.ViewModels;
using TeraDataLite;
using Button = System.Windows.Controls.Button;

namespace TCC.UI.Windows
{
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public sealed partial class DebugWindow : INotifyPropertyChanged
    {
        public DebugWindow()
        {
            InitializeComponent();
        }

        private int _last;
        private int _sum;
        private int _max;

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

        private int Sum
        {
            get => _sum;
            set { _sum = value; NPC(); }
        }
        public double Avg => _count == 0 ? 0 : Sum / _count;

        private int _count;
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void NPC([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SwitchClass(object sender, RoutedEventArgs e)
        {
            Game.Me.Class = (Class)Enum.Parse(typeof(Class), ((Button) sender).Content.ToString());
            WindowManager.ViewModels.ClassVM.CurrentClass = Game.Me.Class;
            WindowManager.ViewModels.CooldownsVM.ClearSkills();
            WindowManager.ViewModels.CooldownsVM.LoadConfig(Game.Me.Class);
        }

        private void SetSorcElement(object sender, RoutedEventArgs e)
        {
            var el = (sender as Button).Content.ToString();

            var fire = el == "Fire";
            var ice = el == "Ice";
            var arc = el == "Arcane";

            var currFire = Game.Me.Fire;
            var currIce = Game.Me.Ice;
            var currArc = Game.Me.Arcane;

            if (fire) SetSorcererElements(!currFire, currIce, currArc);
            if (ice) SetSorcererElements(currFire, !currIce, currArc);
            if (arc) SetSorcererElements(currFire, currIce, !currArc);

            void SetSorcererElements(bool pFire, bool pIce, bool pArcane)
            {
                Game.Me.Fire = pFire;
                Game.Me.Ice = pIce;
                Game.Me.Arcane = pArcane;

                if (App.Settings.ClassWindowSettings.Enabled
                    && Game.Me.Class == Class.Sorcerer
                    && WindowManager.ViewModels.ClassVM.CurrentManager is SorcererLayoutVM sm)
                {
                    sm.NotifyElementChanged();
                }

            }

        }

        private void SetSorcElementBoost(object sender, RoutedEventArgs e)
        {
            var el = (sender as Button).Content.ToString().Split(' ')[0];

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

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            TccUtils.CurrentClassVM<SorcererLayoutVM>().ManaBoost.Buff.Start(10000);

            //SkillManager.AddSkill(100700, 20000);
            //SkillManager.AddSkill(400120, 20000);
            //SkillManager.AddItemSkill(6298, 10);
            //SkillManager.ResetSkill(400120);
        }

        private void SetStance(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Content.ToString() == "Assault") Game.Me.WarriorStance.CurrentStance = WarriorStance.Assault;
            else if (((Button)sender).Content.ToString() == "Defensive") Game.Me.WarriorStance.CurrentStance = WarriorStance.Defensive;
            else if (((Button)sender).Content.ToString() == "None") Game.Me.WarriorStance.CurrentStance = WarriorStance.None;
        }

        private void IncreaseEdge(object sender, RoutedEventArgs e)
        {
            if (Game.Me.StacksCounter.IsMaxed) Game.Me.StacksCounter.Val = 0;
            Game.Me.StacksCounter.Val++;

        }

        private void RegisterWebhook(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 80; i++)
            {
                var i1 = i;
                Dispatcher.InvokeAsync(() => Tester.RegisterWebhook("user" + i1));
            }
        }

        private void FireWebhook(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 80; i++)
            {
                var i1 = i;
                Dispatcher.InvokeAsync(() => Tester.FireWebhook("user" + i1));
            }
        }
        private void DungeonTest(object sender, RoutedEventArgs e)
        {
            //i = 0;
            //WindowManager.Dashboard.VM.SetDungeons(20000078, new Dictionary<uint, short>() { { 9770U, i++ } });

            Game.Account.Characters[0].VanguardInfo.DailiesDone = App.Random.Next(0, 16);
            Game.Account.Characters[0].VanguardInfo.WeekliesDone = App.Random.Next(0, 16);
        }

        private void NotifyGuildBam(object sender, RoutedEventArgs e)
        {
            Tester.AddFakeSystemMessage("SMT_GQUEST_URGENT_NOTIFY", "questName", "Frygaras", "npcName", "Frygaras", "zoneName", "Zone");
        }

        private void NotifyFieldBoss(object sender, RoutedEventArgs e)
        {
            Tester.AddFakeSystemMessage("SMT_FIELDBOSS_APPEAR", "npcName", "Ortan", "RegionName", "Zone");
        }

        private void NotifyFieldBossDie(object sender, RoutedEventArgs e)
        {
            Tester.AddFakeSystemMessage("SMT_FIELDBOSS_DIE_NOGUILD", "userName", "Foglio", "npcName", "Ortan");
        }

        private void SetAtkSpeed(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Content.ToString() == "Swift") TccUtils.CurrentClassVM<WarriorLayoutVM>().SetSwift(1000);
            else if (((Button)sender).Content.ToString() == "Arush") TccUtils.CurrentClassVM<WarriorLayoutVM>().SetArush(1000);

        }
    }
}
