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
                    !skillName.Contains("greeting"))
            {
                if (sk.Cooldown >= LongSkillTreshold)
                {
                    LongSkillsQueue.Add(new SkillCooldown(sk.Id, sk.Cooldown));
                    LastSkillName = skillName;
                }
                else
                {
                    NormalSkillsQueue.Add(new SkillCooldown(sk.Id, sk.Cooldown));
                    LastSkillName = skillName;
                }

            }

        }
        public static void ResetSkill(uint id)
        {
            if (LongSkillsQueue.Where(x => x.Id == id).Count() > 0)
            {
                LongSkillsQueue.Where(x => x.Id == id).Single().Timer.Stop();
                LongSkillsQueue.Remove(LongSkillsQueue.Where(x => x.Id == id).Single());
                MainWindow.RemoveNormalSkill(new SkillCooldown(id, 0));
                LastSkillName = string.Empty;
                Console.WriteLine(id + " reset.");
            }
        }
        public static void ChangeSkillCooldown(SkillCooldown sk)
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
            
        

        public static void Clear()
        {
            LongSkillsQueue.Clear();
            MainWindow.ClearSkills();
            LastSkillName = string.Empty;
        }
    }
}
