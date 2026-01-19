using CatoriCity2025WPF.Objects.Arguments;
using System.Windows.Input;
using CatoriCity2025WPF.Controllers;
using System.Windows.Media.Animation;

namespace CatoriCity2025WPF.Views.Controls
{
    /// <summary>
    /// Interaction logic for StoreHardwareInteriorControl.xaml
    /// </summary>
    public partial class StoreHardwareInteriorControl
    {
        StoreHardwareInteriorControlController _controller;
        public StoreHardwareInteriorControl()
        {
             InitializeComponent();
           _controller = new StoreHardwareInteriorControlController(this);

        }
        public List<ShelfLocationEntity> Shelves = new List<ShelfLocationEntity>();
        private void UC_Loaded(object sender, RoutedEventArgs e)
        {
            robotMover.RobotCartControlMouseUp += robotMover_robotMoverMouseUp;
            robotMover.HardwareStoreName = _controller.HardwareStoreName;
            Canvas.SetLeft(robotMover, robotMover._originalLeft);
            Canvas.SetTop(robotMover, robotMover._originalTop);
            Canvas.SetZIndex(robotMover, 2000);

            Canvas.SetLeft(POSUC, 750);
            Canvas.SetTop(POSUC, 225);
        }
        public double x;
        public double y;
        private void robotMover_robotMoverMouseUp(object? sender, EventArgs e)
        {
            _controller.shoppingCartUtility.AddItem(robotMover.Model);

            POSUC.CheckoutButtonClicked += POSUC_CheckoutButtonClicked;
        }

        private void POSUC_CheckoutButtonClicked(object? sender, EventArgs e)
        {
            // Fix: _contentLoaded is a bool, not an object with CheckOut method.
            // You likely meant to call CheckOut on a different object.
            // Replace _contentLoaded.CheckOut(); with the correct reference.

            // Example fix (replace 'YourCheckoutObject' with the correct instance):
            // YourCheckoutObject.CheckOut();

            // If you intended to use POSUC for checkout:
            POSUC.CheckOut();

            // If you need clarification, please provide the correct object that should handle CheckOut.
        }

        PersonViewModel _model;
        public PersonViewModel Model
        {
            get
            {
                return _model;
            }
            set
            {
                _model = value;
                _controller.LoadPerson(this.Width,this.Height);
            }
        }

        private void MainLayout_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Point position = e.GetPosition(MainLayout);
            double xpos = position.X;
            double ypos = position.Y;
            if (_controller._isShopItemDragging == true && _controller._draggedShopItemControl != null)
            {
                _controller._draggedShopItemControl.SetLocation(position.X, position.Y);
                //cLogger.Log("-----------------------------MainLayout_MouseMove----------------------------------------------");
                //cLogger.Log("---------------------End MainLayout_MouseMove--------------------------------------------------");
                if (xpos < (14 + _controller._draggedShopItemControl.Width) &&
                    ypos > (102 + _controller._draggedShopItemControl.Height) )
                {
                    _controller._isMouseInDropOnCOunterares = true;
                    cLogger.Log("======= MainLayout_MouseMove over counter  =========================");
                }
            }
            else
            {                 
                _controller._isMouseInDropOnCOunterares = false;
            }
        }
      

      

        private void MainLayout_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            cLogger.Log(" MainLayout_MouseUp  ");

            if (_controller._isShopItemDragging)
            {
                if (_controller._isMouseInDropOnCOunterares)
                {
                    cLogger.Log("  Dropped on counter area ");
                    string modelstring = UIUtility.GetFromClipboard();
                    ShopItemViewModel model = GenericSerializer.Deserialize<ShopItemViewModel>(modelstring);
                    ShoppingCartItemViewModel shoppingCartItem = new ShoppingCartItemViewModel(_controller.HardwareStoreName);
                    shoppingCartItem.ShopItem = model;
                    shoppingCartItem.Quantity = 1;
                    _controller.shoppingCartUtility.AddItem(shoppingCartItem);
                }
                else
                {
                    cLogger.Log("  Not dropped on counter area ");
                }
                cLogger.Log("  Dragging ended " );
                _controller._isShopItemDragging = false;
                _controller._draggedShopItemControl = null;
            }
        }

        internal void ShopItemMouseDown(object? sender, ShopItemControlDrag e)
        {
            cLogger.Log("  ShopItemMouse down ");
            _controller._isShopItemMouseDown = true;
            _controller._draggedShopItemControl = e.shopItemControl;
            _controller._mouseOffset = e.MouseArgs.GetPosition(_controller._draggedShopItemControl);

            _controller._isShopItemDragging = true;
            
            cLogger.Log("_isShopItemMouseDown " + _controller._isShopItemMouseDown  );

        }

        internal void ShopItemMouseUp(object? sender, ShopItemControlDrag e)
        {
            //set in mainwindowsontroller
            cLogger.Log(" ShopItemMouse Up");
            _controller._isShopItemDragging = false;
            _controller._isShopItemMouseDown = false;
            _controller._draggedShopItemControl = null;

            string modelstring = UIUtility.GetFromClipboard();
            ShopItemViewModel model = GenericSerializer.Deserialize<ShopItemViewModel>(modelstring);
            ShoppingCartItemViewModel shoppingCartItem = new ShoppingCartItemViewModel(_controller.HardwareStoreName);
            shoppingCartItem.ShopItem = model;
            shoppingCartItem.Quantity = 1;
            POSUC.AddItemToCart(shoppingCartItem);
            //AddShoppingItem(shoppingCartItem);
        }

        private void MainLayout_MouseDown(object sender, MouseButtonEventArgs e)
        {
            cLogger.Log(" MainLayout_MouseDown ");

        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            cLogger.Log(" Image_MouseEnter ");

        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            cLogger.Log(" Image_MouseLeave ");

        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            cLogger.Log(" Image_MouseUp ");

        }

        internal  void MoveCart()
        {
            robotMover.MoveCart();
        }
    }
}
