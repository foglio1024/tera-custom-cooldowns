/* Add My Abnormals Setting by HQ
GroupAbnormalityVM  -> MyAbnormalityVM 
GroupAbnormals      -> MyAbnormals

ClassToggle         -> MyClassToggle
 */
using System.Windows.Threading;
using TCC.Data;
using TCC.Data.Abnormalities;
using TCC.Windows;

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
            for (int i = 0; i < 13; i++)
            {
                var ct = new MyClassToggle((Class)i, ab.Id);
                if (Settings.Settings.MyAbnormals.ContainsKey(ct.Class)) ct.Selected = Settings.Settings.MyAbnormals[ct.Class].Contains(ab.Id);
                Classes.Add(ct);
            }
            Classes.Add(new MyClassToggle(Class.Common, ab.Id)
            {
                Selected = Settings.Settings.MyAbnormals[Class.Common].Contains(ab.Id)
            });

        }
    }
}