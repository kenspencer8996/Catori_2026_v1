using CatoriServices.Objects.Entities;

namespace CatoriCity2025WPF.ViewModels
{
    public class BankCustomerFundsViewModel :ViewmodelBase
    {
        private int _bankCustomerFundsId;
        private decimal _amount;
        private DateTime _lastUpdated = DateTime.MinValue;

        public int BankCustomerFundsId
        {
            get => _bankCustomerFundsId;
            set
            {
                if (_bankCustomerFundsId != value)
                {
                    _bankCustomerFundsId = value;
                    OnPropertyChanged(nameof(BankCustomerFundsId));
                }
            }
        }

        public decimal Amount
        {
            get => _amount;
            set
            {
                if (_amount != value)
                {
                    _amount = value;
                    OnPropertyChanged(nameof(Amount));
                }
            }
        }

        public DateTime LastUpdated
        {
            get => _lastUpdated;
            set
            {
                if (_lastUpdated != value)
                {
                    _lastUpdated = value;
                    OnPropertyChanged(nameof(LastUpdated));
                    OnPropertyChanged(nameof(LastUpdatedString));
                }
            }
        }

        // Friendly formatted string for bindings
        public string LastUpdatedString => LastUpdated == DateTime.MinValue
            ? string.Empty
            : LastUpdated.ToString("g");
        public BankCustomerFundsViewModel()
        {
            
        }
        public BankCustomerFundsViewModel(BankCustomerFundsEntity entity)
        {
            ToModel(entity);
        }

        public BankCustomerFundsEntity GetEntity()
        {
            return new BankCustomerFundsEntity
            {
                BankCustomerFundsId = this.BankCustomerFundsId,
                Amount = Convert.ToDouble(this.Amount),
                LastUpdated = this.LastUpdated.ToString("o")
            };
        }

        public void ToModel(BankCustomerFundsEntity entity)
        {
            if (entity == null) return;
            BankCustomerFundsId = entity.BankCustomerFundsId;
            Amount = Convert.ToDecimal(entity.Amount);
            if (DateTime.TryParse(entity.LastUpdated, out var dt))
                LastUpdated = dt;
            else
                LastUpdated = DateTime.MinValue;
        }
    }
}
