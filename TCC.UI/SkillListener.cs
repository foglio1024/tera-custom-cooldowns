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
            if(sk.Cooldown < SkillManager.LongSkillTreshold)
            {
                MainWindow.AddNormalSkill(sk);
            }
            else
            {
                MainWindow.AddLongSkill(sk);
            }

            sk.Timer.Enabled = true;
        }

        void RemoveSkill(object sender, EventArgs e, SkillCooldown sk)
        {
            sk.Timer.Stop();
            if(sk.Cooldown < SkillManager.LongSkillTreshold)
            {
                MainWindow.RemoveNormalSkill(sk);
            }
            else
            {
                MainWindow.RemoveLongSkill(sk);
            }
            queue.Remove(sk);
            SkillManager.LastSkillName = string.Empty;

        }

    }
}
