using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
namespace CatoriApp.ViewModels.Manufacturing
{
    public class ProductViewModel: ViewmodelBase
    {
        private int _productId;
        private string _productName = "";
        private string _productCode = "";
        private ProductType _productType = ProductType.Finished;
        private string _unitOfMeasure = "pcs";
        private decimal _costPerUnit;
        private DateTime _createdAt = DateTime.Now;

        public int ProductId
        {
            get => _productId;
            set => SetProperty(ref _productId, value);
        }

        public string ProductName
        {
            get => _productName;
            set => SetProperty(ref _productName, value);
        }

        public string ProductCode
        {
            get => _productCode;
            set => SetProperty(ref _productCode, value);
        }

        public ProductType ProductType
        {
            get => _productType;
            set => SetProperty(ref _productType, value);
        }

        public string UnitOfMeasure
        {
            get => _unitOfMeasure;
            set => SetProperty(ref _unitOfMeasure, value);
        }

        public decimal CostPerUnit
        {
            get => _costPerUnit;
            set => SetProperty(ref _costPerUnit, value);
        }

        public DateTime CreatedAt
        {
            get => _createdAt;
            set => SetProperty(ref _createdAt, value);
        }

        public ObservableCollection<BomItemViewModel> BomItems { get; } = new();
        public ObservableCollection<InventoryItemViewModel> InventoryItems { get; } = new();

    }
}


