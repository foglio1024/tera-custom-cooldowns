using System.Windows.Threading;
using TCC.Data;
using TCC.Data.Abnormalities;
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
            for (int i = 0; i < 13; i++)
            {
                var ct = new ClassToggle((Class)i, ab.Id);
                if (Settings.SettingsStorage.GroupAbnormals.ContainsKey(ct.Class)) ct.Selected = Settings.SettingsStorage.GroupAbnormals[ct.Class].Contains(ab.Id);
                Classes.Add(ct);
            }
            Classes.Add(new ClassToggle(Class.Common, ab.Id)
            {
                Selected = Settings.SettingsStorage.GroupAbnormals[Class.Common].Contains(ab.Id)
            });

        }
    }
}