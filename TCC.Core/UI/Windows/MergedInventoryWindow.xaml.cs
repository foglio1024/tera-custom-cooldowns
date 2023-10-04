using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Nostrum.WPF.Factories;
using Nostrum.WPF.ThreadSafe;
using TCC.Data;
using TCC.Data.Pc;

namespace TCC.UI.Windows;

public class MergedInventoryViewModel : ThreadSafeObservableObject
{
    public ThreadSafeObservableCollection<MergedInventoryItem> MergedInventory { get; }
    public ICollectionViewLiveShaping MergedInventoryView { get; }
    public MergedInventoryViewModel()
    {
        MergedInventory = new ThreadSafeObservableCollection<MergedInventoryItem>();
        MergedInventoryView = CollectionViewFactory.CreateLiveCollectionView(MergedInventory, 
                                  sortFilters: new[]
                                  {
                                      new SortDescription($"{nameof(MergedInventoryItem.Item)}.{nameof(InventoryItem.Item)}.{nameof(Item.Id)}", ListSortDirection.Ascending),
                                      new SortDescription($"{nameof(MergedInventoryItem.Item)}.{nameof(InventoryItem.Item)}.{nameof(Item.RareGrade)}", ListSortDirection.Ascending),
                                  })
                              ?? throw new Exception("Failed to create LiveCollectionView");
    }

    double _totalProgress;

    public double TotalProgress
    {
        get => _totalProgress*100;
        set
        {
            _totalProgress = value;
            N();
        }
    }

    public void LoadItems()
    {
        var totalItemsAmount = 0;
        foreach (var ch in Game.Account.Characters.Where(c => !c.Hidden))
        {
            totalItemsAmount += ch.Inventory.Count;
        }
        var itemsParsed = 0;
        Task.Factory.StartNew(() =>
        {
            Game.Account.Characters.Where(c => !c.Hidden).ToList().ForEach(ch =>
            {
                _dispatcher.InvokeAsync(() =>
                {
                    ch.Inventory.ToList().ForEach(item =>
                    {
                        _dispatcher.InvokeAsync(() =>
                        {
                            var existing = MergedInventory.FirstOrDefault(x => x.Item?.Item.Id == item.Item.Id);
                            if (existing == null)
                            {
                                var newItem = new MergedInventoryItem();
                                newItem.Items.Add(new InventoryItemWithOwner(item, ch));
                                MergedInventory.Add(newItem);
                            }
                            else
                            {
                                var ex = existing.Items.FirstOrDefault(x => x.Owner == ch);
                                if (ex != null)
                                {
                                    ex.Item.Amount = item.Amount;
                                }
                                else
                                {
                                    existing.Items.Add(new InventoryItemWithOwner(item, ch));
                                }
                            }
                            itemsParsed++;
                            TotalProgress = itemsParsed / (double)totalItemsAmount;
                        }, DispatcherPriority.DataBind);
                    });
                }, DispatcherPriority.Background);
            });
        });
    }
}
public class InventoryItemWithOwner
{
    public InventoryItem Item { get; }
    public Character Owner { get; }
    public InventoryItemWithOwner(InventoryItem i, Character o)
    {
        Item = i;
        Owner = o;
    }
}
public class MergedInventoryItem : ThreadSafeObservableObject
{
    public InventoryItem? Item => Items.Count > 0 ? Items[0].Item : null;
    public ThreadSafeObservableCollection<InventoryItemWithOwner> Items { get; }
    public int TotalAmount
    {
        get
        {
            var ret = 0;
            Items.ToList().ForEach(i => ret += i.Item.Amount);
            return ret;
        }
    }

    public MergedInventoryItem()
    {
        Items = new ThreadSafeObservableCollection<InventoryItemWithOwner>();
        Items.CollectionChanged += (_, _) => N(nameof(TotalAmount));
    }
}
public partial class MergedInventoryWindow
{
    public MergedInventoryWindow()
    {
        InitializeComponent();
        DataContext = new MergedInventoryViewModel();
        ((MergedInventoryViewModel) DataContext).Dispatcher = Dispatcher;
        Loaded += OnLoaded;

    }

    void OnLoaded(object sender, RoutedEventArgs e)
    {
        ((MergedInventoryViewModel) DataContext).LoadItems();
    }

    void OnTitleBarMouseDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();

    }

    void OnCloseButtonClick(object sender, RoutedEventArgs e)
    {
        Close();
    }

    void FilterInventory(object sender, TextChangedEventArgs e)
    {
        Dispatcher?.InvokeAsync(() =>
        {
            var view = (ICollectionView)((MergedInventoryViewModel) DataContext).MergedInventoryView;
            view.Filter = o =>
            {
                var item = ((MergedInventoryItem)o).Item?.Item;
                var name = item?.Name;
                return name != null && name.IndexOf(((TextBox)sender).Text, StringComparison.InvariantCultureIgnoreCase) != -1;
            };
            view.Refresh();
        }, DispatcherPriority.DataBind);

    }
}