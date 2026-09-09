namespace CatoriApp.Game.Objects.Travel;

public enum TravelMode
{
    Air,
    Rail,
    Road,
    Sea
}

public sealed record TravelDestination(
    string Key,
    string DisplayName,
    string LocationName,
    decimal Price,
    string Icon,
    TravelMode Mode = TravelMode.Air);

public sealed record TravelTicket(
    TravelDestination Destination,
    int PersonId,
    DateTime PurchasedAt,
    long? TransactionId,
    long WalletBalanceCents);
