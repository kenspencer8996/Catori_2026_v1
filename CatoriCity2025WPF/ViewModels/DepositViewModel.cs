using CatoriServices.Objects.Entities;
using System.ComponentModel;

namespace CatoriCity2025WPF.ViewModels
{
    public class DepositViewModel
    {
        private DepositEntity _entity;

        public DepositViewModel()
        {
            _entity = new DepositEntity();
        }

        public void  ToDepositViewModel(DepositEntity entity)
        {
            _entity = entity ?? throw new ArgumentNullException(nameof(entity));
        }
        public string BusinessName
        { 
            get
            {
                return _entity.BusinessName;
            }
            set
            {
                _entity.BusinessName = value;
            }
        }

        public int DepositId
        {
            get => _entity.DepositId;
            set
            {
                if (_entity.DepositId != value)
                {
                    _entity.DepositId = value;
                    OnPropertyChanged(nameof(DepositId));
                }
            }
        }

        public int BankId
        {
            get => _entity.BankId;
            set
            {
                if (_entity.BankId != value)
                {
                    _entity.BankId = value;
                    OnPropertyChanged(nameof(BankId));
                }
            }
        }

        public int PersonId
        {
            get => _entity.PersonId;
            set
            {
                if (_entity.PersonId != value)
                {
                    _entity.PersonId = value;
                    OnPropertyChanged(nameof(PersonId));
                }
            }
        }

        public decimal Amount
        {
            get => _entity.Amount;
            set
            {
                if (_entity.Amount != value)
                {
                    _entity.Amount = value;
                    OnPropertyChanged(nameof(Amount));
                }
            }
        }

        public DepositEntity Entity => _entity;

        public DepositEntity ToEntity()
        {
            return new DepositEntity
            {
                DepositId = this.DepositId,
                BankId = this.BankId,
                PersonId = this.PersonId,
                Amount = this.Amount
            };
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString() => $"Deposit #{DepositId} Person:{PersonId} Business:{BankId} Amount:{Amount}";

    }
}
