namespace CatoriUCLibrary.Views.Tickets;

public sealed record TicketPurchasedMessage(
    string Destination,
    decimal Cost,
    DateTime PurchasedDateTime,
    string PassengerName);
