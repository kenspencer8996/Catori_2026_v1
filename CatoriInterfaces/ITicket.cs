namespace CatoriInterfaces;

public interface ITicket
{
    decimal Cost
    {
        get;
    }

    string Destination
    {
        get;
    }

    DateTime PurchasedDateTime
    {
        get;
    }
}
