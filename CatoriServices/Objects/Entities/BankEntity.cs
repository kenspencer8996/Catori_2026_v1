namespace CatoriServices.Objects.Entities
{
    public class BankEntity
    {
        public int BankId { get; set; }
        public string BusinesskeyImageNameWOExtension { get; set; } = string.Empty;
        public string ImageFileName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal CurrentFunds { get; set; } = 0m;
        public decimal Interestrate { get; set; }
    }
}
