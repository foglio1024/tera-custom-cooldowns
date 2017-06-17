using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TCC.Data;

namespace TCC.Controls
{
    /// <summary>
    /// Logica di interazione per MessagePieceControl.xaml
    /// </summary>
    public partial class MessagePieceControl : UserControl
    {
        public MessagePieceControl()
        {
            InitializeComponent();
        }

        MessagePiece _context;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _context = (MessagePiece)DataContext;
        }

        private void OutlinedTextBlock_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_context.Type != MessagePieceType.Item) return;
            ProxyInterop.SendExTooltipMessage(_context.ItemUid, _context.OwnerName);
        }
    }

    public class TypeToCursorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (MessagePieceType)value;
            Cursor c = Cursors.Arrow;
            switch (val)
            {
                case MessagePieceType.Simple:
                    c = Cursors.Arrow;
                    break;
                case MessagePieceType.Item:
                    c = Cursors.Hand;
                    break;
                case MessagePieceType.Quest:
                    c = Cursors.Hand;
                    break;
            }
            return c;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
