using System;
using System.Collections.Generic;
using System.Text;

namespace CatoriApp.ViewModels
{
    public class BomItemViewModel : ViewmodelBase
    {
        private int _bomId;
        private int _parentProductId;
        private int _componentId;
        private string _componentName = "";
        private string _componentCode = "";
        private decimal _quantity;
        private decimal _scrapFactor;
        private DateTime _effectiveDate = DateTime.Today;
        private DateTime? _expiryDate;

        public int BomId
        {
            get => _bomId;
            set => SetProperty(ref _bomId, value);
        }

        public int ParentProductId
        {
            get => _parentProductId;
            set => SetProperty(ref _parentProductId, value);
        }

        public int ComponentId
        {
            get => _componentId;
            set => SetProperty(ref _componentId, value);
        }

        public string ComponentName
        {
            get => _componentName;
            set => SetProperty(ref _componentName, value);
        }

        public string ComponentCode
        {
            get => _componentCode;
            set => SetProperty(ref _componentCode, value);
        }

        public decimal Quantity
        {
            get => _quantity;
            set
            {
                if (SetProperty(ref _quantity, value))
                    OnPropertyChanged(nameof(QuantityIncludingScrap));
            }
        }

        public decimal ScrapFactor
        {
            get => _scrapFactor;
            set
            {
                if (SetProperty(ref _scrapFactor, value))
                    OnPropertyChanged(nameof(QuantityIncludingScrap));
            }
        }

        public decimal QuantityIncludingScrap
            => Quantity + (Quantity * ScrapFactor / 100m);

        public DateTime EffectiveDate
        {
            get => _effectiveDate;
            set => SetProperty(ref _effectiveDate, value);
        }

        public DateTime? ExpiryDate
        {
            get => _expiryDate;
            set => SetProperty(ref _expiryDate, value);
        }

    }
}

