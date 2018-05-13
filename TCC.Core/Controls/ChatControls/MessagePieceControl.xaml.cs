using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using TCC.Data;

namespace TCC.Controls.ChatControls
{
    /// <summary>
    /// Logica di interazione per MessagePieceControl.xaml
    /// </summary>
    public partial class MessagePieceControl
    {
        private MessagePiece _context;

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
                    Proxy.ChatLinkData(_context.RawLink);
                    break;
                case MessagePieceType.Url:
                    Process.Start(_context.Text);
                    break;
                case MessagePieceType.Point_of_interest:
                    Proxy.ChatLinkData(_context.RawLink);
                    break;
                case MessagePieceType.Quest:
                    Proxy.ChatLinkData(_context.RawLink);
                    break;
            }
        }
        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_context == null) return;
            switch (_context.Type)
            {
                case MessagePieceType.Item:
                    BgBorder.Background = _context.Color;
                    break;
                case MessagePieceType.Url:
                    BgBorder.Background = _context.Color;
                    break;
                case MessagePieceType.Point_of_interest:
                    BgBorder.Background = _context.Color;
                    break;
                case MessagePieceType.Quest:
                    BgBorder.Background = _context.Color;
                    break;
            }

        }
        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            BgBorder.Background = Brushes.Transparent;
        }
    }
}
