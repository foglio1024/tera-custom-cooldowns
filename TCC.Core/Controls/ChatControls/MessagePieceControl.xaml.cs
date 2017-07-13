using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
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
        MessagePiece _context;

        public MessagePieceControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_context != null) return;
            _context = (MessagePiece)DataContext;
        }
        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            switch (_context.Type)
            {
                case MessagePieceType.Item:
                    if (_context.BoundType == BoundType.Equip || _context.ItemUid > 0)
                    {
                        ProxyInterop.SendExTooltipMessage(_context.ItemUid, _context.OwnerName);
                    }
                    else
                    {
                        ProxyInterop.SendNondbItemInfoMessage(_context.ItemId);
                    }
                    break;
                case MessagePieceType.Url:
                    Process.Start(_context.Text);
                    break;
                default:
                    break;
            }
        }
        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_context == null) return;
            switch (_context.Type)
            {
                case MessagePieceType.Item:
                    bgBorder.Background = _context.Color;
                    break;
                case MessagePieceType.Url:
                    bgBorder.Background = _context.Color;
                    break;
                case MessagePieceType.Point_of_interest:
                    bgBorder.Background = _context.Color;
                    WindowManager.ChatWindow.OpenMap(_context);
                    break;
                default:
                    break;
            }

        }
        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            bgBorder.Background = Brushes.Transparent;
        }
    }
}
