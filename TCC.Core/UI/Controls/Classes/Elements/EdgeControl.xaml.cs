using System.ComponentModel;
using System.Threading;
using System.Windows;
using TCC.Data;

namespace TCC.UI.Controls.Classes.Elements
{
    public partial class EdgeControl
    {
        private Counter? _context;
        public EdgeControl()
        {
            InitializeComponent();

        }
        private int _currentEdge;

        private void SetEdge(int newEdge)
        {
            var diff = newEdge - _currentEdge;

            if (diff == 0) return;
            if (diff > 0)
            {
                if (newEdge == 10)
                {
                    foreach (var child in EdgeContainer.Children)
                    {
                        if (child != null) ((FrameworkElement)child).Opacity = 1;
                    }
                }
                for (var i = 0; i < diff; i++)
                {
                    if (_currentEdge + i < EdgeContainer.Children.Count - 1) EdgeContainer.Children[_currentEdge + i].Opacity = 1;
                }
            }
            else
            {
                MaxBorder.Opacity = 0;

                for (var i = EdgeContainer.Children.Count - 1; i >= 0; i--)
                {
                    EdgeContainer.Children[i].Opacity = 0;
                }
            }
            _currentEdge = newEdge;
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            //lazy way of making sure that DataContext is not null
            //var classMgr = TccUtils.CurrentClassVM<WarriorLayoutVM>();
            _context = Game.Me.StacksCounter; // classMgr?.EdgeCounter;
            while (_context == null)
            {
                _context = (Counter)DataContext;
                Thread.Sleep(500);
            }
            _context.PropertyChanged += _context_PropertyChanged;
        }

        private void _context_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_context == null) return;

            switch (e.PropertyName)
            {
                case nameof(Counter.Val):
                    SetEdge(_context.Val);
                    break;
                case nameof(Counter.IsMaxed):
                    MaxBorder.Opacity = _context.IsMaxed ? 1 : 0;
                    break;
            }
        }

    }
}
