using System.Windows.Input;
using Nostrum.WPF;
using Nostrum.WPF.ThreadSafe;
using TCC.Data.Abnormalities;
using TCC.UI.Windows;
using TeraDataLite;

namespace TCC.ViewModels;

public class GroupAbnormalityViewModel : ThreadSafeObservableObject
{
    bool _hidden;

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
    public ThreadSafeObservableCollection<ClassToggle> Classes { get; }

    public GroupAbnormalityViewModel(Abnormality ab)
    {
        Abnormality = ab;

        Classes = new ThreadSafeObservableCollection<ClassToggle>(_dispatcher);
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