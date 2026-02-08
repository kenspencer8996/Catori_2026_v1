using CatoriCity2025WPF.Controllers;
using CatoriCity2025WPF.Objects.Arguments;
using CommunityToolkit.Mvvm.Messaging;

namespace CatoriCity2025WPF.Views.Controls
{
    /// <summary>
    /// Interaction logic for POSUI_UC.xaml
    /// </summary>
    public partial class POSUI_UC : UserControl
    {
        POSUI_UCController _controller;
        public event EventHandler CheckoutButtonClickedEvent;
        public POSUI_UC()
        {
            InitializeComponent();
        }
        public void SetupPOC(int personid)
        {
            _controller = new POSUI_UCController(this,personid);
        }
        public void AddItemToCart(ShoppingCartItemViewModel item)
        {
            _controller.AddItemToCart(item);
        }   

        private void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            CheckoutButton.IsEnabled = false;
            POCArgs pOCArgs = new POCArgs();
            pOCArgs.Items = _controller.Items;
            WeakReferenceMessenger.Default.Send<POCArgs>(pOCArgs);
            _controller.SaveCartItems();
            if (CheckoutButtonClickedEvent != null)
            {
                CheckoutButtonClickedEvent(this, EventArgs.Empty);
            }
            _controller.Items.Clear();
        }

        internal void CheckOut()
        {
           
        }
    }
}
