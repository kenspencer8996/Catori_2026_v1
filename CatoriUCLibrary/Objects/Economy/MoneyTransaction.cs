namespace CatoriUCLibrary.Objects.Economy;

public sealed class MoneyTransaction
{
    public string AvatarId { get; set; } = "";
    public decimal Amount { get; set; }
    public string Description { get; set; } = "";
    public DateTime TimestampUtc { get; set; }
}
