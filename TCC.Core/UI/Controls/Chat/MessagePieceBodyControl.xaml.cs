using System.Windows;
using System.Windows.Input;
using TCC.Data.Chat;

namespace TCC.UI.Controls.Chat;

public partial class MessagePieceBodyControl
{
    MessagePieceBase? _context;

    public MessagePieceBodyControl()
    {
        InitializeComponent();
    }

    void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (_context != null) return;
        _context = (MessagePieceBase)DataContext;
    }

    void UserControl_MouseEnter(object sender, MouseEventArgs e)
    {
        if (_context == null) return;
        _context.IsHovered = true;

    }

    void UserControl_MouseLeave(object sender, MouseEventArgs e)
    {
        if (_context == null) return;
        _context.IsHovered = false;
    }
}