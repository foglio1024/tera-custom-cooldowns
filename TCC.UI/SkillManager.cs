using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using TCC.Messages;

namespace TCC
{
    public delegate void SkillAddedEventHandler(SkillCooldown s);
    public delegate void SkillOverEventHandler( SkillCooldown s);

    public delegate void SkillCooldownChangedEventHandler(SkillCooldown s);
    public delegate void SkillResetEventHandler(Skill s);

    public static class SkillManager
    {
        public const int LongSkillTreshold = 40000;
        public const int Ending = 120;

        public static event SkillCooldownChangedEventHandler Changed;
        public static event SkillResetEventHandler Reset;
        public static event SkillCooldownChangedEventHandler Refresh;

        public static ObservableCollection<SkillCooldown> NormalSkillsQueue = new ObservableCollection<SkillCooldown>();
        public static ObservableCollection<SkillCooldown> LongSkillsQueue = new ObservableCollection<SkillCooldown>();

        public static List<string> LastSkills = new List<string>();

        static bool Filter(string name)
        {
            if (name != "Unknown" &&
                !name.Contains("Summon:") &&
                !name.Contains("Flight:") &&
                !name.Contains("Super Rocket Jump")&&
                !name.Contains("greeting") ||
                name == "Summon: Party") return true;
            else return false;

        }
        public static void AddSkillDirectly(Skill sk, uint cd)
        {
            RouteSkill(new SkillCooldown(sk, (int)cd, CooldownType.Skill));
        }

        static void RouteSkill(SkillCooldown skillCooldown)
        {
            if (skillCooldown.Cooldown == 0)
            {
                ResetSkill(skillCooldown.Skill);
            }

            if (NormalSkillsQueue.ToList().Any(x => x!=null && x.Skill.Name == skillCooldown.Skill.Name) || LongSkillsQueue.ToList().Any(x => x!=null && x.Skill.Name == skillCooldown.Skill.Name))
            {
                Refresh?.Invoke(skillCooldown);
            }
            else
            {
                if (skillCooldown.Cooldown < LongSkillTreshold)
                {
                    
                    WindowManager.CooldownWindow.Dispatcher.Invoke(() => NormalSkillsQueue.Add(skillCooldown));
                }
                else
                {
                    WindowManager.CooldownWindow.Dispatcher.Invoke(() => LongSkillsQueue.Add(skillCooldown));
                }
            }
        }
        public static void AddSkill(uint id, uint cd)
        {
            if(SkillsDatabase.TryGetSkill(id, SessionManager.CurrentPlayer.Class, out Skill skill))
            {
                if (!Filter(skill.Name))
                {
                    return;
                }
                RouteSkill(new SkillCooldown(skill, (int)cd, CooldownType.Skill));
            }
        }
        public static void AddBrooch(uint id, uint cd)
        {
            if (BroochesDatabase.TryGetBrooch(id, out Skill brooch))
            {
                RouteSkill(new SkillCooldown(brooch, (int)cd, CooldownType.Item));
            }

        }

        public static void ResetSkill(Skill sk)
        {
            Reset?.Invoke(sk);
        }
        public static void ChangeSkillCooldown(uint id, int cd)
        {
            if (SkillsDatabase.TryGetSkill(id, SessionManager.CurrentPlayer.Class, out Skill skill))
            {
                Changed?.Invoke(new SkillCooldown(skill, cd, CooldownType.Skill));
            }

            //if (sk.Cooldown > SkillManager.LongSkillTreshold)
            //{
            //    if (LongSkillsQueue.Where(X => X.Id == sk.Id).Count() > 0)
            //    {
            //        if (sk.Cooldown == 0)
            //        {
            //            ResetSkill(sk.Id);
            //        }
            //        else
            //        {
            //            LongSkillsQueue.Where(x => x.Id == sk.Id).Single().Timer.Interval = sk.Cooldown;

            //            Changed?.Invoke(null, null, sk);
            //        }
            //    }
            //}
            //else
            //{
            //    if (NormalSkillsQueue.Where(X => X.Id == sk.Id).Count() > 0)
            //    {
            //        if (sk.Cooldown == 0)
            //        {
            //            ResetSkill(sk.Id);
            //        }
            //        else
            //        {
            //            try
            //            {
            //                NormalSkillsQueue.Where(x => x.Id == sk.Id).Single().Timer.Interval = sk.Cooldown;

            //            }
            //            catch (Exception)
            //            {

            //            }

            //            Changed?.Invoke(null, null, sk);
            //        }
            //    }

            //}

            //try
            //{
            //    //Console.WriteLine("{0} cooldown reduced.", SkillsDatabase.GetSkill(sk.Id, PacketParser.CurrentClass).Name);

            //}
            //catch (Exception)
            //{

            //    Console.WriteLine("{0}'s skill cooldown reduced.", sk.Id);
            //}
        }

        public static void Clear()
        {          
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                NormalSkillsQueue.Clear();
                LongSkillsQueue.Clear();
                SessionManager.CurrentPlayer.Buffs.Clear();
                SessionManager.CurrentPlayer.Debuffs.Clear();
                SessionManager.CurrentPlayer.InfBuffs.Clear();
            }));

            SessionManager.CurrentPlayer.Class = Class.None;
            SessionManager.CurrentPlayer.EntityId = 0;
            LastSkills.Clear();
            SessionManager.Logged = false;
        }
    }
}
