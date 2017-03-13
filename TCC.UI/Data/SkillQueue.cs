using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC
{
    public class SkillQueue : ObservableCollection<SkillCooldownNew>
    {
        public event SkillAddedEventHandler Added;
        public event SkillOverEventHandler Over;

        protected virtual void OnNewSkill(EventArgs e)
        {
            Added?.Invoke(this.Last());
        }

        protected virtual void OnSkillEnded(EventArgs e)
        {
            Over?.Invoke(this.Last());
        }

        public new void Add(SkillCooldownNew s)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                base.Add(s);
            });
            OnNewSkill(EventArgs.Empty);
        }
    }
}
