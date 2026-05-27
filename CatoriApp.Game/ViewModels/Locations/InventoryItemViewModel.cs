using System;
using System.Collections.Generic;
using System.Text;
namespace CatoriApp.Game.ViewModels.Locations
{
    public class InventoryItemViewModel : ViewmodelBase
    {
        private int _inventoryId;
        private int _productId;
        private string _productName = "";
        private string _location = "";
        private decimal _quantityOnHand;
        private DateTime _lastUpdated = DateTime.Now;

        public int InventoryId
        {
            get => _inventoryId;
            set => SetProperty(ref _inventoryId, value);
        }

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

        public string Location
        {
            get => _location;
            set => SetProperty(ref _location, value);
        }

        public decimal QuantityOnHand
        {
            get => _quantityOnHand;
            set => SetProperty(ref _quantityOnHand, value);
        }

        public DateTime LastUpdated
        {
            get => _lastUpdated;
            set => SetProperty(ref _lastUpdated, value);
        }

    }
}


