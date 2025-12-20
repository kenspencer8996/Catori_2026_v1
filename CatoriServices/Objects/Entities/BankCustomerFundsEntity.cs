namespace CatoriServices.Objects.Entities
{
    public class BankCustomerFundsEntity
    {
        public int BankCustomerFundsId { get; set; }
        public double Amount { get; set; } = 0;
        public string LastUpdated { get; set; } = string.Empty;
    }
}
