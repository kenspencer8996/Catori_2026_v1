namespace CatoriCity2025WPF.Objects.Arguments
{
    public class DepositMessageArgument
    {
        public int BankId { get; set; }
        public string BusinessName { get; set; }
        public decimal Amount { get; set; }
        public PersonViewModel Person { get; set; }
    }
}
