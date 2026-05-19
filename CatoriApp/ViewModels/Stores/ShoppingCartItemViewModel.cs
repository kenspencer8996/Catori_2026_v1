namespace CatoriApp.ViewModels.Stores
{
    public class ShoppingCartItemViewModel : ViewmodelBase
    {
        private int _quantity;
        private string _storeName = string.Empty;
        public ShoppingCartItemViewModel(string storeName)
        {
            StoreName = storeName;
        }
        public ShopItemViewModel ShopItem {  get; set;  }

        public string Name => ShopItem.Name;
        public decimal ItemPrice => ShopItem.Price; 
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                    OnPropertyChanged(nameof(TotalPrice));
                }
            }
        }


        public decimal TotalPrice => ShopItem.Price * Quantity;
        public string StoreName
        {
            get => _storeName;
            set
            {
                if (_storeName != value)
                {
                    _storeName = value;
                    OnPropertyChanged(nameof(StoreName));
                }
            }
        }

    }
}


