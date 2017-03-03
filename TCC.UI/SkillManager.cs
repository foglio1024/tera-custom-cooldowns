using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Messages;
using TCC.UI.Messages;

namespace TCC.UI
{
    public delegate void SkillAddedEventHandler(object sender, EventArgs e, SkillCooldown s);
    public delegate void SkillOverEventHandler(object sender, EventArgs e, SkillCooldown s);
    public delegate void SkillCooldownChangedEventHandler(object sender, EventArgs e, SkillCooldown s);

    public static class SkillManager
    {
        public const int LongSkillTreshold = 40000;

        public static SkillQueue NormalSkillsQueue = new SkillQueue();
        public static SkillQueue LongSkillsQueue = new SkillQueue();

        public static SkillListener NormalSkillsQueueListener = new SkillListener(NormalSkillsQueue);
        public static SkillListener LongSkillsQueueListener = new SkillListener(LongSkillsQueue);
        public static string LastSkillName;

        public static event SkillCooldownChangedEventHandler Changed;

        public static void AddSkill(SkillCooldown sk)
        {
            switch (sk.Type)
            {
                case CooldownType.Skill:
                    string skillName = SkillsDatabase.SkillIdToName(sk.Id, PacketParser.CurrentClass);
                    if (sk.Cooldown == 0)
                    {
                        ResetSkill(sk.Id);
                    }
                    else if (
                            skillName != "Unknown" &&
                            skillName != LastSkillName &&
                            !skillName.Contains("Summon:") &&
                            !skillName.Contains("Flight:") &&
                            !skillName.Contains("greeting") ||
                            skillName == "Summon: Party")
                    {
                        if (sk.Cooldown >= LongSkillTreshold)
                        {
                            LongSkillsQueue.Add(sk);
                            LastSkillName = skillName;
                        }
                        else
                        {
                            NormalSkillsQueue.Add(sk);
                            LastSkillName = skillName;
                        }

                    }
                    break;

                case CooldownType.Item:
                    if(BroochesDatabase.GetBrooch(sk.Id) != null)
                    {
                        LongSkillsQueue.Add(sk);
                    }
                    break;

                default:
                    break;
            }

        }
        public static void ResetSkill(uint id)
        {
            if (LongSkillsQueue.Where(x => x.Id == id).Count() > 0)
            {
                LongSkillsQueue.Where(x => x.Id == id).Single().Timer.Stop();
                LongSkillsQueue.Remove(LongSkillsQueue.Where(x => x.Id == id).Single());
                MainWindow.RemoveLongSkill(new SkillCooldown(id, 0, CooldownType.Skill));
                LastSkillName = string.Empty;
                Console.WriteLine(id + " reset.");
            }
            else if (NormalSkillsQueue.Where(x => x.Id == id).Count() > 0)
            {
                NormalSkillsQueue.Where(x => x.Id == id).Single().Timer.Stop();
                NormalSkillsQueue.Remove(NormalSkillsQueue.Where(x => x.Id == id).Single());
                MainWindow.RemoveNormalSkill(new SkillCooldown(id, 0, CooldownType.Skill));
                LastSkillName = string.Empty;
                Console.WriteLine(id + " reset.");

            }
        }
        public static void ChangeSkillCooldown(SkillCooldown sk)
        {
            if (sk.Cooldown > SkillManager.LongSkillTreshold)
            {
                if (LongSkillsQueue.Where(X => X.Id == sk.Id).Count() > 0)
                {
                    if (sk.Cooldown == 0)
                    {
                        ResetSkill(sk.Id);
                    }
                    else
                    {
                        LongSkillsQueue.Where(x => x.Id == sk.Id).Single().Timer.Interval = sk.Cooldown;

                        Changed?.Invoke(null, null, sk);
                    }
                }
            }
            else
            {
                if (NormalSkillsQueue.Where(X => X.Id == sk.Id).Count() > 0)
                {
                    if (sk.Cooldown == 0)
                    {
                        ResetSkill(sk.Id);
                    }
                    else
                    {
                        NormalSkillsQueue.Where(x => x.Id == sk.Id).Single().Timer.Interval = sk.Cooldown;

                        Changed?.Invoke(null, null, sk);
                    }
                }

            }
        }



        public static void Clear()
        {
            LongSkillsQueue.Clear();
            MainWindow.ClearSkills();
            LastSkillName = string.Empty;
        }
    }
}
