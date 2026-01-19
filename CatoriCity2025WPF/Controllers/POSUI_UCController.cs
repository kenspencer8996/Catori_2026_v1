using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace CatoriCity2025WPF.Controllers
{
    public class POSUI_UCController
    {
        POSUI_UC _view;
        public ObservableCollection<ShoppingCartItemViewModel> Items = new ObservableCollection<ShoppingCartItemViewModel>();
        private readonly DispatcherTimer _timer;

        public POSUI_UCController(POSUI_UC view)
        {
            _view = view;
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
            if (total > GlobalStuff.CurrentPerson.Funds)
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
    }
}
