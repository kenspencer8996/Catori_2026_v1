namespace CatoriUCLibrary.Objects.Economy;

public sealed class EconomySave
{
    public int Version { get; set; } = 1;
    public Dictionary<string, decimal> Balances { get; set; } = new(StringComparer.Ordinal);
    public Dictionary<string, MoneyTransaction> Transactions { get; set; } = new(StringComparer.Ordinal);
}
