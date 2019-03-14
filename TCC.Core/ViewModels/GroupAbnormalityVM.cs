using System.Windows.Threading;
using TCC.Data;
using TCC.Data.Abnormalities;
using TCC.Settings;
using TCC.Windows;

namespace TCC.ViewModels
{
    public class GroupAbnormalityVM : TSPropertyChanged
    {
        public Abnormality Abnormality { get; }
        public SynchronizedObservableCollection<ClassToggle> Classes { get; }

        public GroupAbnormalityVM(Abnormality ab)
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            Abnormality = ab;
            Classes = new SynchronizedObservableCollection<ClassToggle>(Dispatcher);
            for (var i = 0; i < 13; i++)
            {
                var ct = new ClassToggle((Class)i, ab.Id);
                if (SettingsHolder.GroupAbnormals.TryGetValue(ct.Class, out var list)) ct.Selected = list.Contains(ab.Id);
                Classes.Add(ct);
            }
            Classes.Add(new ClassToggle(Class.Common, ab.Id)
            {
                Selected = SettingsHolder.GroupAbnormals[Class.Common].Contains(ab.Id)
            });

        }
    }
}