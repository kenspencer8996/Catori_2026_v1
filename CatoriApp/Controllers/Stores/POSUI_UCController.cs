using System.Collections.ObjectModel;
using System.Windows.Threading;
namespace CatoriApp.Controllers.Stores
{
    public class POSUI_UCController
    {
        POSUI_UC _view;
        public ObservableCollection<ShoppingCartItemViewModel> Items = new ObservableCollection<ShoppingCartItemViewModel>();
        private readonly DispatcherTimer _timer;
        int _personId;
        public POSUI_UCController(POSUI_UC view, int personId)
        {
            _view = view;
            _personId = personId;
            _view.CartDataGrid.ItemsSource = Items;
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }
        private void Timer_Tick(object? sender, EventArgs e)
        {
            // Perform periodic work on UI thread
            CalculateandSetTotal();
        }

        public void AddItemToCart(ShoppingCartItemViewModel item)
        {
            Items.Add(item);

            decimal total = CalculateandSetTotal();
            if (total > GlobalAllApps.CurrentPerson.Funds)
                _view.CheckoutButton.IsEnabled = false;
            else
                _view.CheckoutButton.IsEnabled = true;

        }
        public decimal CalculateandSetTotal()
        {
            decimal total = 0;
            foreach (var item in Items)
            {
                total += item.ShopItem.Price;
            }
            _view.TotalTextBox.Text = $"${total:F2}";
            return total;
        }
        public void SaveCartItems()
        {
            PersonProductsOwnedService personProductsOwnedService = new PersonProductsOwnedService();
            foreach (var item in Items) 
            {
                personProductsOwnedService.UpsertAsync(new PersonProductsOwnedViewModel
                {
                    PersonId = _personId,
                    ShopItemId = item.ShopItem.ShopItemId,
                    Quantity = item.Quantity
                });
            }
        }
    }
}

