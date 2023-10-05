using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GongSolutions.Wpf.DragDrop;
using GongSolutions.Wpf.DragDrop.Utilities;
using TCC.Data;
using TCC.Data.Abnormalities;
using TCC.Data.Skills;
using TCC.ViewModels.Widgets;
using TeraDataLite;

namespace TCC.UI.Windows;

//TODO: make this inherit from TccWindow
public partial class SkillConfigWindow
{
    static SkillConfigWindow? _instance;
    public static SkillConfigWindow Instance => _instance ?? new SkillConfigWindow();


    CooldownWindowViewModel VM { get; }
    public SkillConfigWindow() : base(true)
    {
        _instance = this;

        InitializeComponent();
        DataContext = WindowManager.ViewModels.CooldownsVM;
        VM = (CooldownWindowViewModel) DataContext;

        //Closing += OnClosing;
    }

    //private void OnClosing(object sender, CancelEventArgs e)
    //{
    //    if (Opacity != 0) e.Cancel = true;
    //    ClosewWindow(null, null);
    //}

    public class GenericDragHandler : IDropTarget
    {
        public void DragOver(IDropInfo dropInfo)
        {
        }

        public void Drop(IDropInfo dropInfo)
        {
        }
    }


    public GenericDragHandler DragHandler => new();
    public HiddenSKillsDragHandler HiddenSkillsDropHandler => new();

    public override void HideWindow()
    {
        FocusManager.ForceFocused = false;
        if (VM.Settings != null)
        {
            VM.Settings.ForcedClickable = false;
            VM.Settings.ForcedVisible = false;
        }

        base.HideWindow();

        VM.SaveConfig();
        VM.IsDragging = false;

    }

    void ClosewWindow(object? sender, RoutedEventArgs? e)
    {
        //var an = new DoubleAnimation(0, TimeSpan.FromMilliseconds(200));
        //FocusManager.ForceFocused = false;
        //VM.Settings.ForcedClickable = false;
        //VM.Settings.ForcedVisible = false;

        HideWindow();

        //WindowManager.SkillConfigWindow = null;

        //an.Completed += (s, ev) =>
        //{
        //    Close();
        //    if (App.Settings.ForceSoftwareRendering) RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
        //};
        //BeginAnimation(OpacityProperty, an);
        //VM.SaveConfig();
        //VM.IsDragging = false;
    }

    public override void ShowWindow()
    {
        //if (App.Settings.ForceSoftwareRendering) RenderOptions.ProcessRenderMode = RenderMode.Default;
        FocusManager.ForceFocused = true;
        if (VM.Settings != null)
        {
            VM.Settings.ForcedClickable = true;
            VM.Settings.ForcedVisible = true;
        }

        //WindowManager.SkillConfigWindow = this;

        base.ShowWindow();
        //Dispatcher?.Invoke(() =>
        //{
        //    var animation = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));
        //    if (IsVisible) return;
        //    Opacity = 0;
        //    Show();
        //    Activate();
        //    BeginAnimation(OpacityProperty, animation);
        //});
    }

    void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    void SkillSearch_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (VM.SkillsView == null) return;
        var view = (ICollectionView)VM.SkillsView;
        view.Filter = o => ((Skill)o).ShortName.IndexOf(((TextBox)sender).Text, StringComparison.InvariantCultureIgnoreCase) != -1;
        view.Refresh();
    }

    void ItemSearch_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var view = (ICollectionView)VM.ItemsView;
        view.Filter = o => ((Item)o).Name.IndexOf(((TextBox)sender).Text, StringComparison.InvariantCultureIgnoreCase) != -1;
        view.Refresh();
    }

    void PassivitySearch_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var view = (ICollectionView)VM.AbnormalitiesView;
        view.Filter = o => ((Abnormality)o).Name.IndexOf(((TextBox)sender).Text, StringComparison.InvariantCultureIgnoreCase) != -1;
        view.Refresh();
    }

    void RemoveHiddenSkill(object sender, RoutedEventArgs e)
    {
        VM.RemoveHiddenSkill((Cooldown) ((Button)sender).DataContext);
    }
}
public class HiddenSKillsDragHandler : IDropTarget
{
    public void DragOver(IDropInfo dropInfo)
    {
        dropInfo.Effects = DragDropEffects.Move;
    }

    public void Drop(IDropInfo dropInfo)
    {
        var l = dropInfo.TargetCollection.TryGetList();
        if (l.Cast<Cooldown>().Any(cd =>
            {
                var ret = dropInfo.Data switch
                {
                    Skill s => cd.Skill.IconName == s.IconName,
                    Item i => cd.Skill.IconName == i.IconName,
                    Abnormality a => cd.Skill.IconName == a.IconName,
                    _ => false
                };

                return ret;
            })) return;

        switch (dropInfo.Data)
        {
            case Skill s:
                l.Add(new Cooldown(s, false));
                break;
            case Item i:
                Game.DB!.ItemsDatabase.TryGetItemSkill(i.Id, out var itemSkill);
                l.Add(new Cooldown(itemSkill, false, CooldownType.Item));
                break;
            case Abnormality a:
                l.Add(new Cooldown(new Skill(a.Id,Class.None, a.Name, a.ToolTip){IconName = a.IconName}, false, CooldownType.Passive));
                break;
        }
        //dropInfo.DragInfo.SourceCollection.TryGetList().Remove(dropInfo.Data);
    }
}