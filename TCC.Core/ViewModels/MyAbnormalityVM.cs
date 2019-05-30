/* Add My Abnormals Setting by HQ
GroupAbnormalityVM  -> MyAbnormalityVM 
GroupAbnormals      -> MyAbnormals

ClassToggle         -> MyClassToggle
 */
using System.Windows.Threading;
using TCC.Data;
using TCC.Data.Abnormalities;
using TCC.Windows;
using TeraDataLite;

namespace TCC.ViewModels
{
    public class MyAbnormalityVM : TSPropertyChanged
    {
        public Abnormality Abnormality { get; }
        public SynchronizedObservableCollection<MyClassToggle> Classes { get; }

        public MyAbnormalityVM(Abnormality ab)
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            Abnormality = ab;
            Classes = new SynchronizedObservableCollection<MyClassToggle>(Dispatcher);
            for (var i = 0; i < 13; i++)
            {
                var ct = new MyClassToggle((Class)i, ab.Id);
                if (Settings.SettingsHolder.MyAbnormals.TryGetValue(ct.Class, out var list)) ct.Selected = list.Contains(ab.Id);
                Classes.Add(ct);
            }
            Classes.Add(new MyClassToggle(Class.Common, ab.Id)
            {
                Selected = Settings.SettingsHolder.MyAbnormals[Class.Common].Contains(ab.Id)
            });

        }
    }
}