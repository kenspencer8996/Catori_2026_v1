namespace CatoriServices.Objects.Entities
{
    public class DepositEntity
    {
        public int DepositId { get; set; }
        public int PersonId { get; set; }
        public decimal Amount { get; set; }
        public string BusinessName { get; set; }
        public int BankId { get; set; }
    }
}
