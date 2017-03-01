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
            MainWindow.AddSkill(sk);
            sk.Timer.Enabled = true;
        }

        void RemoveSkill(object sender, EventArgs e, SkillCooldown sk)
        {
            sk.Timer.Stop();
            MainWindow.RemoveSkill(sk);
            queue.Remove(sk);
            SkillManager.LastSkillName = string.Empty;

        }

    }
}
