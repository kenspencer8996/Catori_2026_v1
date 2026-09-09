using System.Collections.ObjectModel;
namespace CatoriApp.Core.Objects.Shared
{
    public class ShoppingCartUtility
    {
        public ObservableCollection<ShoppingCartItemViewModel> Items { get; } =
            new ObservableCollection<ShoppingCartItemViewModel>();
        public ShoppingCartUtility()
        {
            Items = new ObservableCollection<ShoppingCartItemViewModel>();
        }
        public void AddItem(ShoppingCartItemViewModel item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            // Find existing item by ItemName (StoreId removed from model)
            ShoppingCartItemViewModel existingItem = FindItemByName(item.ShopItem.Name);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                Items.Add(item);
            }
        }


        private ShoppingCartItemViewModel? FindItemByName(string itemName)
        {
            if (string.IsNullOrEmpty(itemName)) return null;
            return Items.FirstOrDefault(i => string.Equals(i.ShopItem.Name, itemName, StringComparison.OrdinalIgnoreCase));
        }


        public decimal GetTotalAmount()
        {
            decimal total = 0;
            foreach (var item in Items)
            {
                total += item.ShopItem.Price * item.Quantity;
            }
            return total;
        }
        
    }
}


