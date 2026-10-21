using CatoriUCLibrary.Views;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CatoriApp.Core.Objects.DragDrop;
using CatoriInterfaces;
using CommunityToolkit.Mvvm.Messaging;
using CatoriUCLibrary.Views.Person;

namespace CatoriUCLibrary.Views.Tickets;

public sealed class TicketPurchaseRequestedEventArgs(
    string destination,
    decimal cost,
    DateTime purchasedDateTime) : EventArgs, ITicket
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
