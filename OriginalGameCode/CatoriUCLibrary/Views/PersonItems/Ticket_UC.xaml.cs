using CatoriInterfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CatoriUCLibrary.Views.PersonItems;

public partial class Ticket_UC : UserControl, ITicket
{
    private Point _dragStart;

    public Ticket_UC()
    {
        InitializeComponent();
        PurchasedDateTime = DateTime.Now;
    }

    public decimal Cost
    {
        get;
        set;
    }

    public string Destination
    {
        get;
        set;
    } = string.Empty;

    public DateTime PurchasedDateTime
    {
        get;
        set;
    }

    private void Ticket_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _dragStart = e.GetPosition(this);
    }

    private void Ticket_MouseMove(object sender, MouseEventArgs e)
    {
        StartDragWhenReady(e);
    }

    private void StartDragWhenReady(MouseEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed)
        {
            return;
        }
        Point current = e.GetPosition(this);
        if (Math.Abs(current.X - _dragStart.X) < SystemParameters.MinimumHorizontalDragDistance
            && Math.Abs(current.Y - _dragStart.Y) < SystemParameters.MinimumVerticalDragDistance)
        {
            return;
        }
        DragDrop.DoDragDrop(this, this, DragDropEffects.Copy);
    }
}
