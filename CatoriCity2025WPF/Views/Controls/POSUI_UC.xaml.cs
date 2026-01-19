using CatoriCity2025WPF.Controllers;

namespace CatoriCity2025WPF.Views.Controls
{
    /// <summary>
    /// Interaction logic for POSUI_UC.xaml
    /// </summary>
    public partial class POSUI_UC : UserControl
    {
        POSUI_UCController _controller;
        public event EventHandler CheckoutButtonClicked;
        public POSUI_UC()
        {
            InitializeComponent();
            _controller = new POSUI_UCController(this);
        }
        public void AddItemToCart(ShoppingCartItemViewModel item)
        {
            _controller.AddItemToCart(item);
        }   

        private void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (CheckoutButtonClicked != null)
            {
                CheckoutButtonClicked(this, EventArgs.Empty);
            }
        }

        internal void CheckOut()
        {
           
        }
    }
}
