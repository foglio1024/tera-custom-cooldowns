using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.UI
{
    public class SkillQueue : List<SkillCooldown>
    {
        public event SkillAddedEventHandler Added;
        public event SkillOverEventHandler Over;

        protected virtual void OnNewSkill(EventArgs e)
        {
            Added?.Invoke(this, e, this.Last());
        }

        protected virtual void OnSkillEnded(EventArgs e)
        {
            Over?.Invoke(this, e, this.Last());
        }

        public new void Add(SkillCooldown s)
        {
            base.Add(s);
            OnNewSkill(EventArgs.Empty);
        }
    }
}
