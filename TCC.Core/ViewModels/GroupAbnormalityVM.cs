using System.Windows.Threading;
using Nostrum;
using TCC.Data.Abnormalities;
using TCC.Windows;
using TeraDataLite;

namespace TCC.ViewModels
{
    public class GroupAbnormalityVM : TSPropertyChanged
    {
        public Abnormality Abnormality { get; }
        public TSObservableCollection<ClassToggle> Classes { get; }

        public GroupAbnormalityVM(Abnormality ab)
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            Abnormality = ab;
            Classes = new TSObservableCollection<ClassToggle>(Dispatcher);
            for (var i = 0; i < 13; i++)
            {
                var ct = new ClassToggle((Class)i, ab.Id);
                if (App.Settings.GroupWindowSettings.GroupAbnormals.TryGetValue(ct.Class, out var list)) ct.Selected = list.Contains(ab.Id);
                Classes.Add(ct);
            }
            Classes.Add(new ClassToggle(Class.Common, ab.Id)
            {
                Selected = App.Settings.GroupWindowSettings.GroupAbnormals[Class.Common].Contains(ab.Id)
            });

        }
    }
}