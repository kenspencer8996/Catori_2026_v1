using CatoriServices.Objects.Services.Finance;

namespace CatoriApp.Game.Objects.Travel;

public sealed class TravelService
{
    public static TravelService Current { get; } = new();

    private readonly FinanceService _financeService = new();

    public TravelTicket? ActiveTicket { get; private set; }

    public event EventHandler<TravelTicket>? TicketPurchased;

    public TravelTicket BuyTicket(int personId, TravelDestination destination)
    {
        ArgumentNullException.ThrowIfNull(destination);
        if (personId <= 0)
            throw new ArgumentOutOfRangeException(nameof(personId), "A valid traveler is required.");
        if (destination.Price <= 0)
            throw new ArgumentOutOfRangeException(nameof(destination), "The ticket price must be positive.");

        MoneyOperationResult result = _financeService.PurchaseTravelTicket(
            personId,
            destination.Price,
            destination.Mode.ToString(),
            destination.Key,
            destination.DisplayName);

        var ticket = new TravelTicket(destination, personId, DateTime.Now, result.RecordId, result.WalletBalanceCents);
        ActiveTicket = ticket;
        TicketPurchased?.Invoke(this, ticket);
        return ticket;
    }

    public bool TryConsumeActiveTicket(TravelMode mode, out TravelTicket? ticket)
    {
        ticket = ActiveTicket;
        if (ticket?.Destination.Mode != mode)
            return false;

        ActiveTicket = null;
        return true;
    }
}
