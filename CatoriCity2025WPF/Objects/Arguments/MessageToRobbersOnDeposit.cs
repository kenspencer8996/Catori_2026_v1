using CatoriCity2025WPF.Viewmodels;

namespace CatoriCity2025WPF.Objects.Arguments
{
    public class MessageToRobbersOnDeposit
    {
        public int BankId;
        public double X;
        public double Y;
        public decimal Amount;

        public MessageToRobbersOnDeposit(int bankId, double x, double y,decimal amount) //: base(value)
        {
            BankId = bankId;
            this.X = x;
            this.Y = y;
            Amount = amount;
        }
    }
}
