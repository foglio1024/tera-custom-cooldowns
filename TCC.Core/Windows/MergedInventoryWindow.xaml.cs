using FoglioUtils;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using TCC.Data.Pc;

namespace TCC.Windows
{

    public class MergedInventoryViewModel : TSPropertyChanged
    {
        public SynchronizedObservableCollection<MergedInventoryItem> MergedInventory { get; }
        public ICollectionViewLiveShaping MergedInventoryView { get; }
        public MergedInventoryViewModel()
        {
            MergedInventory = new SynchronizedObservableCollection<MergedInventoryItem>();
            MergedInventoryView = CollectionViewUtils.InitLiveView(o => o != null, MergedInventory, new string[] { }, new[]
            {
                new SortDescription("Item.Item.Id", ListSortDirection.Ascending),
                new SortDescription("Item.Item.RareGrade", ListSortDirection.Ascending),
            });
            ((ICollectionView)MergedInventoryView).CollectionChanged += Reeeee;
        }

        private void Reeeee(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) { }
        private double _totalProgress;

        public double TotalProgress
        {
            get { return _totalProgress*100; }
            set
            {
                _totalProgress = value;
                N();
            }
        }

        public void LoadItems()
        {
            var totalItemsAmount = 0;
            foreach (var ch in WindowManager.Dashboard.VM.Characters.Where(c => !c.Hidden))
            {
                totalItemsAmount += ch.Inventory.Count;
            }
            var itemsParsed = 0;
            Task.Factory.StartNew(() =>
            {
                WindowManager.Dashboard.VM.Characters.Where(c => !c.Hidden).ToList().ForEach(ch =>
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        ch.Inventory.ToList().ForEach(item =>
                        {
                            Dispatcher.BeginInvoke(new Action(() =>
                            {
                                var existing = MergedInventory.FirstOrDefault(x => x.Item.Item.Id == item.Item.Id);
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
                            }), DispatcherPriority.DataBind);
                        });
                    }), DispatcherPriority.Background);
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
    public class MergedInventoryItem : TSPropertyChanged
    {
        public InventoryItem Item => Items.Count > 0 ? Items[0].Item : null;
        public SynchronizedObservableCollection<InventoryItemWithOwner> Items { get; }
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
            Items = new SynchronizedObservableCollection<InventoryItemWithOwner>();
            Items.CollectionChanged += (_, __) => N(nameof(TotalAmount));
        }
    }
    public partial class MergedInventoryWindow
    {
        public MergedInventoryWindow()
        {
            InitializeComponent();
            DataContext = new MergedInventoryViewModel();
            (DataContext as MergedInventoryViewModel).SetDispatcher(Dispatcher);
            Loaded += OnLoaded;

        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ((MergedInventoryViewModel) DataContext).LoadItems();
        }

        private void OnTitleBarMouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();

        }
        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void FilterInventory(object sender, TextChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var view = (ICollectionView)((MergedInventoryViewModel) DataContext).MergedInventoryView;
                view.Filter = o =>
                {
                    var item = ((MergedInventoryItem)o).Item.Item;
                    var name = item.Name;
                    return name.IndexOf(((TextBox)sender).Text,
                                   StringComparison.InvariantCultureIgnoreCase) != -1;
                };
                view.Refresh();
            }), DispatcherPriority.DataBind);

        }
    }
}
