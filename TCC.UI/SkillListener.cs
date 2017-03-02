using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.UI
{
    public class SkillListener
    {
        private SkillQueue queue;

        public SkillListener(SkillQueue q)
        {
            queue = q;
            queue.Added += new SkillAddedEventHandler(RunSkill); // start cd
            queue.Over += new SkillOverEventHandler(RemoveSkill);   // remove skill
        }

        void RunSkill(object sender, EventArgs e, SkillCooldown sk)
        {
            sk.Timer.Elapsed += (s, ev) => RemoveSkill(null, null, sk);
            switch (sk.Type)
            {
                case CooldownType.Skill:
                    if (sk.Cooldown < SkillManager.LongSkillTreshold)
                    {
                        MainWindow.AddNormalSkill(sk);
                    }
                    else
                    {
                        MainWindow.AddLongSkill(sk);
                    }
                    break;
                case CooldownType.Item:
                    MainWindow.AddLongSkill(sk);
                    break;
                default:
                    break;
            }
            
            sk.Timer.Enabled = true;
        }

        void RemoveSkill(object sender, EventArgs e, SkillCooldown sk)
        {
            sk.Timer.Stop();
            switch (sk.Type)
            {
                case CooldownType.Skill:
                    if (sk.Cooldown < SkillManager.LongSkillTreshold)
                    {
                        MainWindow.RemoveNormalSkill(sk);
                    }
                    else
                    {
                        MainWindow.RemoveLongSkill(sk);
                    }
                    SkillManager.LastSkillName = string.Empty;
                    break;
                case CooldownType.Item:
                    MainWindow.RemoveLongSkill(sk);
                    break;
                default:
                    break;
            }
            queue.Remove(sk);

        }

    }
}
