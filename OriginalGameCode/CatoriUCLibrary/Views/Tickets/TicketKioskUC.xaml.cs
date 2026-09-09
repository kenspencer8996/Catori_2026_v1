using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CatoriApp.Core.Objects.DragDrop;
using CatoriInterfaces;
using CommunityToolkit.Mvvm.Messaging;

namespace CatoriUCLibrary.Views.Tickets;

public sealed class TicketPurchaseRequestedEventArgs(
    string destination,
    decimal cost,
    DateTime purchasedDateTime) : EventArgs
{
    public string Destination
    {
        get;
    } = destination;

    public decimal Cost
    {
        get;
    } = cost;

    public DateTime PurchasedDateTime
    {
        get;
    } = purchasedDateTime;
}

public partial class TicketKioskUC : UserControl, IDraggable, ICanvasDragAnchor, IDragStartFilter
{
    private bool _isDragging;
    private Point _dragOffset;

    public TicketKioskUC()
    {
        InitializeComponent();
    }

    public string Destination
    {
        get => (string)GetValue(DestinationProperty);
        set => SetValue(DestinationProperty, value);
    }

    public static readonly DependencyProperty DestinationProperty = DependencyProperty.Register(
        nameof(Destination),
        typeof(string),
        typeof(TicketKioskUC),
        new PropertyMetadata("Beach"));

    public decimal Cost
    {
        get => (decimal)GetValue(CostProperty);
        set => SetValue(CostProperty, value);
    }

    public static readonly DependencyProperty CostProperty = DependencyProperty.Register(
        nameof(Cost),
        typeof(decimal),
        typeof(TicketKioskUC),
        new PropertyMetadata(0m));

    public event EventHandler<TicketPurchaseRequestedEventArgs>? BuyTicketRequested;
    public event EventHandler? DragCompleted;
    public event EventHandler? Selected;
    public event EventHandler? PropertiesRequested;

    public bool IsDragEnabled
    {
        get;
        set;
    } = true;

    public UIElement Visual => this;

    public Point OriginalPosition => new(
        Normalize(Canvas.GetLeft(this)),
        Normalize(Canvas.GetTop(this)));

    public double DragAnchorX => ActualWidth > 0 ? ActualWidth / 2 : Width / 2;

    public double DragAnchorY => ActualHeight > 0 ? ActualHeight / 2 : Height / 2;

    public bool CanStartDrag(DependencyObject? originalSource)
    {
        return FindParent<Button>(originalSource) == null;
    }

    public void OnDragMouseup()
    {
        DragCompleted?.Invoke(this, EventArgs.Empty);
    }

    public void SetPurchaseStatus(string message, bool isError = false)
    {
        PurchaseStatusTextBlock.Text = message;
        PurchaseStatusTextBlock.Foreground = isError
            ? System.Windows.Media.Brushes.Yellow
            : System.Windows.Media.Brushes.White;
        PurchaseStatusTextBlock.Visibility = string.IsNullOrWhiteSpace(message)
            ? Visibility.Collapsed
            : Visibility.Visible;
    }

    public void PublishPurchasedTicket(ITicket ticket, string passengerName)
    {
        ArgumentNullException.ThrowIfNull(ticket);
        var message = new TicketPurchasedMessage(
            ticket.Destination,
            ticket.Cost,
            ticket.PurchasedDateTime,
            passengerName);
        PurchasedTicketRegistry.Add(message);
        WeakReferenceMessenger.Default.Send(message);
    }

    private void BuyTicketButton_Click(object sender, RoutedEventArgs e)
    {
        BuyTicketRequested?.Invoke(
            this,
            new TicketPurchaseRequestedEventArgs(Destination, Cost, DateTime.Now));
    }

    private void TicketKiosk_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        Selected?.Invoke(this, EventArgs.Empty);
        if (!IsDragEnabled || Parent is not Canvas canvas || !CanStartDrag(e.OriginalSource as DependencyObject))
        {
            return;
        }
        Point pointer = e.GetPosition(canvas);
        _dragOffset = new Point(
            pointer.X - Normalize(Canvas.GetLeft(this)),
            pointer.Y - Normalize(Canvas.GetTop(this)));
        _isDragging = true;
        CaptureMouse();
        e.Handled = true;
    }

    private void TicketKiosk_MouseMove(object sender, MouseEventArgs e)
    {
        if (!_isDragging || e.LeftButton != MouseButtonState.Pressed || Parent is not Canvas canvas)
        {
            return;
        }
        Point pointer = e.GetPosition(canvas);
        Canvas.SetLeft(this, Math.Max(0, pointer.X - _dragOffset.X));
        Canvas.SetTop(this, Math.Max(0, pointer.Y - _dragOffset.Y));
        e.Handled = true;
    }

    private void TicketKiosk_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (!_isDragging)
        {
            return;
        }
        _isDragging = false;
        ReleaseMouseCapture();
        OnDragMouseup();
        e.Handled = true;
    }

    private static T? FindParent<T>(DependencyObject? source) where T : DependencyObject
    {
        for (DependencyObject? current = source; current != null; current = System.Windows.Media.VisualTreeHelper.GetParent(current))
        {
            if (current is T match)
            {
                return match;
            }
        }
        return null;
    }

    private static double Normalize(double value)
    {
        return double.IsNaN(value) ? 0 : value;
    }
}
