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
            //queue.Added += new SkillAddedEventHandler(OnSkillAdded); // start cd
            //queue.Over += new SkillOverEventHandler(RemoveSkill);   // remove skill

            t = new Timer(SkillManager.Ending);
        }
        /// <summary>
        /// Executed when a new skill is added
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        /// <param name="sk">Skill cooldown info</param>
        void OnSkillAdded(SkillCooldownNew sk)
        {
            //sk.Timer.Elapsed += (s, ev) => RemoveSkill(null, null, sk);
            //switch (sk.Type)
            //{
            //    case CooldownType.Skill:
            //        if (sk.Cooldown < SkillManager.LongSkillTreshold)
            //        {
            //            CooldownWindow.AddNormalSkill(sk);
            //        }
            //        else
            //        {
            //            CooldownWindow.AddLongSkill(sk);
            //        }
            //        break;
            //    case CooldownType.Item:
            //        CooldownWindow.AddLongSkill(sk);
            //        break;
            //    default:
            //        break;
            //}
            
            //sk.Timer.Enabled = true;
            ////Console.WriteLine("Running {0}", sk.Id);
        }   
        void RemoveSkill(SkillCooldownNew sk)
        {
            //sk.Timer.Stop();
            //queue.Remove(sk);
            //string name = string.Empty;

            //if (SkillsDatabase.SkillIdToName(sk.Id, SessionManager.CurrentClass)!="Unknown")
            //{
            //    name = SkillsDatabase.SkillIdToName(sk.Id, SessionManager.CurrentClass);
            //}
            //else if (BroochesDatabase.GetBrooch(sk.Id) != null)
            //{
            //    name = BroochesDatabase.GetBrooch(sk.Id).Name;
            //}
  
            //SkillManager.LastSkills.Remove(name);
            //t.Elapsed += (s, o) => RemoveFromMainWindow(sk);
            //t.Start();
            //Console.WriteLine("{0} skill removed", sk.Id);
        }
        void RemoveFromMainWindow(SkillCooldownOld sk)
        {
            //t.Stop();
            //t = new Timer();
            //switch (sk.Type)
            //{
            //    case CooldownType.Skill:
            //        if (sk.Cooldown < SkillManager.LongSkillTreshold)
            //        {
            //            CooldownWindow.RemoveNormalSkill(sk);
            //        }
            //        else
            //        {
            //            CooldownWindow.RemoveLongSkill(sk);
            //        }
                    
            //        break;
            //    case CooldownType.Item:
            //        CooldownWindow.RemoveLongSkill(sk);
            //        break;
            //    default:
            //        break;
            //}

        }
    }
}
