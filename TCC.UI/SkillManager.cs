using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Messages;

namespace TCC
{
    public delegate void SkillAddedEventHandler(SkillCooldownNew s);
    public delegate void SkillOverEventHandler( SkillCooldownNew s);

    public delegate void SkillCooldownChangedEventHandler(SkillCooldownNew s);
    public delegate void SkillResetEventHandler(Skill s);

    public static class SkillManager
    {
        public const int LongSkillTreshold = 40000;
        public const int Ending = 250;

        public static event SkillCooldownChangedEventHandler Changed;
        public static event SkillResetEventHandler Reset;

        public static SkillQueue NormalSkillsQueue = new SkillQueue();
        public static SkillQueue LongSkillsQueue = new SkillQueue();

        static SkillListener NormalSkillsQueueListener = new SkillListener(NormalSkillsQueue);
        static SkillListener LongSkillsQueueListener = new SkillListener(LongSkillsQueue);

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

        public static void AddSkill(Skill skill, int cooldown)
        {
            LongSkillsQueue.Add(new SkillCooldownNew(skill, cooldown, CooldownType.Skill));
        }
        public static void AddSkill(S_START_COOLTIME_SKILL packet)
        {

            if(SkillsDatabase.TryGetSkill(packet.SkillId, SessionManager.CurrentClass, out Skill skill))
            {
                if (!Filter(skill.Name))
                {
                    return;
                }
                SkillCooldownNew skillCooldown = new SkillCooldownNew(skill, (int)packet.Cooldown, CooldownType.Skill);
                //Console.WriteLine("Received {0} - {1}", skillCooldown.Skill.Id, skillCooldown.Skill.Name);

                if (skillCooldown.Cooldown == 0)
                {
                    ResetSkill(skillCooldown.Skill);
                    //Console.WriteLine("Resetting {0} - {1}", skill.Id, skill.Name);
                }
                else if (NormalSkillsQueue.Where(x => x.Skill.Name == skillCooldown.Skill.Name).Count() > 0 ||
                         LongSkillsQueue.Where(x => x.Skill.Name == skillCooldown.Skill.Name).Count() > 0)
                {
                    return;
                }
                else if (skillCooldown.Cooldown < LongSkillTreshold)
                {
                    NormalSkillsQueue.Add(skillCooldown);
                }
                else
                {
                    LongSkillsQueue.Add(skillCooldown);
                }
            }


            //string skillName = SkillsDatabase.SkillIdToName(sk.Id, SessionManager.CurrentClass);
            //if (sk.Cooldown == 0)
            //{
            //    ResetSkill(sk.Id);
            //}
            //else if (Filter(skillName) && !LastSkills.Contains(skillName))
            //{
            //    if (sk.Cooldown >= LongSkillTreshold)
            //    {
            //        LongSkillsQueue.Add(sk);
            //    }
            //    else
            //    {
            //        NormalSkillsQueue.Add(sk);
            //    }
            //    LastSkills.Add(skillName);
            //}
            //else
            //{
            //    Console.WriteLine("Skill not added: {0} {1}", sk.Id, SessionManager.CurrentClass);

            //}
        }
        public static void AddBrooch(S_START_COOLTIME_ITEM packet)
        {
            if (BroochesDatabase.TryGetBrooch(packet.ItemId, out Skill brooch))
            {
                SkillCooldownNew broochCooldown = new SkillCooldownNew(brooch, (int)packet.Cooldown, CooldownType.Item);
                //Console.WriteLine("Received {0} - {1}", broochCooldown.Skill.Id, broochCooldown.Skill.Name);
                if (LongSkillsQueue.Where(x => x.Skill.Name == broochCooldown.Skill.Name).Count() > 0)
                {
                    return;
                }
                else
                {
                    LongSkillsQueue.Add(broochCooldown);
                }
            }
            //if (BroochesDatabase.GetBrooch(sk.Id) != null)
            //{
            //    var name = BroochesDatabase.GetBrooch(sk.Id).Name;
            //    if (!LastSkills.Contains(name))
            //    {
            //        LongSkillsQueue.Add(sk);
            //        LastSkills.Add(name);
            //        //Console.WriteLine("{0} added.", name);
            //    }
            //    else
            //    {
            //        Console.WriteLine("Brooch not added: {0}", sk.Id);
            //    }
            //}

        }

        public static void ResetSkill(Skill sk)
        {
            Reset?.Invoke(sk);
        }
        public static void ChangeSkillCooldown(S_DECREASE_COOLTIME_SKILL packet)
        {
            if (SkillsDatabase.TryGetSkill(packet.SkillId, SessionManager.CurrentClass, out Skill skill))
            {
                Changed?.Invoke(new SkillCooldownNew(skill, (int)packet.Cooldown, CooldownType.Skill));
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
            }));

            SessionManager.CurrentClass = Class.None;
            SessionManager.CurrentCharId = 0;
            //CooldownWindow.ClearSkills();
            WindowManager.HideWindow(WindowManager.ClassSpecificGauge);
            WindowManager.ClassSpecificGauge = null;
            LastSkills.Clear();
            SessionManager.Logged = false;
            //Console.WriteLine("Manager cleared.");
        }
    }
}
