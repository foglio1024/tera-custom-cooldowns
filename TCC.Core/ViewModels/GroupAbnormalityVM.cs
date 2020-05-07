using Nostrum;
using System.Windows.Input;
using System.Windows.Threading;
using TCC.Data.Abnormalities;
using TCC.UI;
using TCC.UI.Controls.Abnormalities;
using TCC.UI.Windows;
using TeraDataLite;

namespace TCC.ViewModels
{
    public class GroupAbnormalityVM : TSPropertyChanged
    {
        private bool _hidden;

        public bool Hidden
        {
            get => _hidden;
            set
            {
                if (_hidden == value) return;
                _hidden = value;
                N();
            }
        }

        public ICommand HiddenCommand { get; }
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

            HiddenCommand = new RelayCommand(_ =>
            {
                Hidden = !Hidden;
                if (Hidden)
                {
                    App.Settings.GroupWindowSettings.Hidden.Add(Abnormality.Id);
                }
                else
                {
                    App.Settings.GroupWindowSettings.Hidden.Remove(Abnormality.Id);
                }
            });
        }
    }
}