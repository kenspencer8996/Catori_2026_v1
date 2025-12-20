using CatoriCity2025WPF.Objects;

namespace CatoriCity2025WPF.ViewModels
{
    public class FundsViewModel:ViewmodelBase
    {
       // public Image MoneyImage { get; set; } = new Image();
        public decimal _money { get; set; }
        public string BankName { get; set; } = "NA";
        public FundsViewModel()
        {
           // MoneyImage = new Image();
            Money = 0;
        }
        public decimal Money
        {
            get
            {
                return _money;
            }
            set
            {
                _money = value;
                OnPropertyChanged("Money");
            }
        }

        public double X { get; internal set; }
        public double Y { get; internal set; }
    }
}
