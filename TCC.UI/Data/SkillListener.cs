using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TCC
{
    public class SkillListener
    {
        SkillQueue queue;
        Timer t;

        public SkillListener(SkillQueue q)
        {
            queue = q;
            queue.Added += new SkillAddedEventHandler(RunSkill); // start cd
            queue.Over += new SkillOverEventHandler(RemoveSkill);   // remove skill

            t = new Timer(SkillManager.Ending);
        }

        void RunSkill(object sender, EventArgs e, SkillCooldown sk)
        {
            sk.Timer.Elapsed += (s, ev) => RemoveSkill(null, null, sk);
            switch (sk.Type)
            {
                case CooldownType.Skill:
                    if (sk.Cooldown < SkillManager.LongSkillTreshold)
                    {
                        CooldownsBarWindow.AddNormalSkill(sk);
                    }
                    else
                    {
                        CooldownsBarWindow.AddLongSkill(sk);
                    }
                    break;
                case CooldownType.Item:
                    CooldownsBarWindow.AddLongSkill(sk);
                    break;
                default:
                    break;
            }
            
            sk.Timer.Enabled = true;
            //Console.WriteLine("Running {0}", sk.Id);
        }
        
        void RemoveSkill(object sender, EventArgs e, SkillCooldown sk)
        {
            sk.Timer.Stop();
            queue.Remove(sk);
            string name = string.Empty;

            if (SkillsDatabase.GetSkill(sk.Id, PacketParser.CurrentClass)!=null)
            {
                name = SkillsDatabase.GetSkill(sk.Id, PacketParser.CurrentClass).Name;
            }
            else if (BroochesDatabase.GetBrooch(sk.Id) != null)
            {
                name = BroochesDatabase.GetBrooch(sk.Id).Name;
            }
  
            SkillManager.LastSkills.Remove(name);
            t.Elapsed += (s, o) => RemoveFromMainWindow(sk);
            t.Start();
            //Console.WriteLine("{0} skill removed", sk.Id);
        }

        void RemoveFromMainWindow(SkillCooldown sk)
        {
            t.Stop();
            t = new Timer();
            switch (sk.Type)
            {
                case CooldownType.Skill:
                    if (sk.Cooldown < SkillManager.LongSkillTreshold)
                    {
                        CooldownsBarWindow.RemoveNormalSkill(sk);
                    }
                    else
                    {
                        CooldownsBarWindow.RemoveLongSkill(sk);
                    }
                    
                    break;
                case CooldownType.Item:
                    CooldownsBarWindow.RemoveLongSkill(sk);
                    break;
                default:
                    break;
            }

        }
    }
}
