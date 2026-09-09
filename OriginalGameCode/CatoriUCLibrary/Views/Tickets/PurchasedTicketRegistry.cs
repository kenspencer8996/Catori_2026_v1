namespace CatoriUCLibrary.Views.Tickets;

public static class PurchasedTicketRegistry
{
    private static readonly object SyncRoot = new();
    private static readonly List<TicketPurchasedMessage> Tickets = [];

    public static void Add(TicketPurchasedMessage ticket)
    {
        ArgumentNullException.ThrowIfNull(ticket);
        lock (SyncRoot)
        {
            Tickets.Add(ticket);
        }
    }

    public static IReadOnlyList<TicketPurchasedMessage> GetAll()
    {
        lock (SyncRoot)
        {
            return Tickets.ToArray();
        }
    }
}
