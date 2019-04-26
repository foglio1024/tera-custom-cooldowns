using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using TCC.Data;
using TCC.Data.Chat;
using TCC.Interop;
using TCC.Interop.Proxy;
using TCC.Windows;

namespace TCC.Controls.Chat
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
                    ProxyInterface.Instance.Stub.ChatLinkAction(_context.RawLink); //ProxyOld.ChatLinkData(_context.RawLink);
                    break;
                case MessagePieceType.Url:
                    try
                    {
                        Process.Start(_context.Text);
                    }
                    catch 
                    {
                        TccMessageBox.Show("Unable to open URL.", MessageBoxType.Error);
                    }
                    break;
                case MessagePieceType.PointOfInterest:
                case MessagePieceType.Achievement:
                case MessagePieceType.Quest:
                    ProxyInterface.Instance.Stub.ChatLinkAction(_context.RawLink); //ProxyOld.ChatLinkData(_context.RawLink);
                    break;
            }
        }
        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_context == null) return;
            switch (_context.Type)
            {
                case MessagePieceType.Item:
                case MessagePieceType.Url:
                case MessagePieceType.PointOfInterest:
                case MessagePieceType.Quest:
                case MessagePieceType.Achievement:
                    //BgBorder.BorderBrush = _context.Color;
                    _context.IsHovered = true;
                    break;
            }

        }
        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            //BgBorder.Background = Brushes.Transparent;
            _context.IsHovered = false;

        }
    }
}
