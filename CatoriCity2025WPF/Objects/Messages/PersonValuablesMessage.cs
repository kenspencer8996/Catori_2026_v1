namespace CatoriCity2025WPF.Objects.Messages
{
    public class PersonValuablesMessage
    {
        public decimal ValuableAmount { get; set; }
        public PersonValuablesMessage(double valuableAmount) 
        {
            ValuableAmount = (decimal)valuableAmount;
        }
    }
}
