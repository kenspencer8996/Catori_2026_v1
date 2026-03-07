using CatoriCity2025WPF.Controllers;
using CatoriCity2025WPF.Objects.Arguments;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows.Input;

namespace CatoriCity2025WPF.Views
{
    /// <summary>
    /// Interaction logic for StoreHardwareView.xaml
    /// </summary>
    public partial class StoreHardwareInteriorView : Window
    {
        StoreHardwareInteriorViewController _controller;
        bool overrobot = false;
        PersonViewModel _personViewModel;
        RobotCartControl robotMover;
        double _personOriginalLeft;
        double _personOriginalTop;
        public StoreHardwareInteriorView(double personLeft,double personTop)
        {
            InitializeComponent();
            _personOriginalLeft = personLeft;
            _personOriginalTop = personTop;
            _controller = new StoreHardwareInteriorViewController(this);

            robotMover = new RobotCartControl();
            //DroneDeliver1UC.Visibility = Visibility.Hidden;
            DragImage.Visibility = Visibility.Hidden;
            robotMover.RobotEnter += RobotMover_RobotEnter;
            robotMover.RobotLeave += RobotMover_RobotLeave;
            POSUC.CheckoutButtonClickedEvent += POSUC_CheckoutButtonClickedEvent;
            robotMover.RobotAllFinished += RobotMover_RobotAllFinished;
        }

        private void RobotMover_RobotAllFinished(object? sender, RobotMoverControlDrag e)
        {
            // _controller.MoveUpBox();

            _controller.cardboardBoxUC.ShowOpen();
        }

        private void POSUC_CheckoutButtonClickedEvent(object? sender, EventArgs e)
        {
            robotMover.MoveCartToLoadOut();
        }

        public List<ShelfLocationEntity> Shelves = new List<ShelfLocationEntity>();
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            robotMover.RobotCartControlMouseUp += robotMover_robotMoverMouseUp;
            robotMover.HardwareStoreName = _controller.HardwareStoreName;
            double robotleft = robotMover._originalLeft + 200;
            double robottop = robotMover._originalTop + 200;
            MainLayoutHardwareStoreInterior.Children.Add(robotMover);
            Canvas.SetLeft(robotMover, robotleft);
            Canvas.SetTop(robotMover, robottop);
            Canvas.SetZIndex(robotMover, 2000);

            Canvas.SetLeft(POSUC, 750);
            Canvas.SetTop(POSUC, 225);

            MoveCart();
        }
        public double x;
        public double y;
        private void robotMover_robotMoverMouseUp(object? sender, EventArgs e)
        {
            _controller.shoppingCartUtility.AddItem(robotMover.Model);

            POSUC.CheckoutButtonClickedEvent += POSUC_CheckoutButtonClicked;
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
                _controller.LoadPerson(this.Width, this.Height);
            }
        }

        private void MainLayout_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // Get mouse position relative to the window
            Point relativePoint = e.GetPosition(MainLayoutHardwareStoreInterior);

            // Convert to absolute screen coordinates
            //Point absolutePoint = this.PointToScreen(relativePoint);
            double xpos = relativePoint.X;
            double ypos = relativePoint.Y;
            //xpos = 800;
            //ypos = 370;
            if (_controller._isShopItemDragging == false)
                _controller._isShopItemDragging = UIUtility.CheckMouseMoveForDrag(relativePoint, _controller._mouseOffset);
            //cLogger.Log("_controller._isShopItemDragging " + _controller._isShopItemDragging);
            if (_controller._isShopItemDragging == true && _controller._draggedShopItemControl != null)
            {
                //_controller._draggedShopItemControl.SetLocation(xpos, ypos);
                Canvas.SetTop(DragImage, ypos);
                Canvas.SetLeft(DragImage, xpos);
                //cLogger.Log("-----------------------------MainLayout_MouseMove----------------------------------------------");
                if (xpos < (14 + _controller._draggedShopItemControl.Width) &&
                    ypos > (102 + _controller._draggedShopItemControl.Height))
                {
                    _controller._isMouseInDropOnCOunterares = true;
                    //cLogger.Log("======= MainLayout_MouseMove over counter  =========================");
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
                cLogger.Log("  Dragging ended ");
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

            double left = Canvas.GetLeft(_controller._draggedShopItemControl);
            double top = Canvas.GetTop(_controller._draggedShopItemControl);
            Canvas.SetLeft(DragImage, left - 2);
            Canvas.SetTop(DragImage, top - 2);

            _controller._isShopItemDragging = true;
            DragImage.Source = _controller._draggedShopItemControl.MainImage.Source;
            DragImage.Width = _controller._draggedShopItemControl.MainImage.Width;
            DragImage.Height = _controller._draggedShopItemControl.MainImage.Height;

            DragImage.Visibility = Visibility.Visible;
            cLogger.Log("_isShopItemMouseDown " + _controller._isShopItemMouseDown);
        }

        internal void ShopItemMouseUp(object? sender, ShopItemControlDrag e)
        {
            //set in mainwindowsontroller
            cLogger.Log(" ShopItemMouse Up");


            if (overrobot)
            {
                _controller._isShopItemDragging = false;
                _controller._isShopItemMouseDown = false;
                _controller._draggedShopItemControl = null;
                DragImage.Visibility = Visibility.Hidden;

                string modelstring = UIUtility.GetFromClipboard();
                ShopItemViewModel model = GenericSerializer.Deserialize<ShopItemViewModel>(modelstring);
                ShoppingCartItemViewModel shoppingCartItem = new ShoppingCartItemViewModel(_controller.HardwareStoreName);
                shoppingCartItem.ShopItem = model;
                shoppingCartItem.Quantity = 1;
                POSUC.AddItemToCart(shoppingCartItem);
                //AddShoppingItem(shoppingCartItem);
            }
        }

        private void MainLayout_MouseDown(object sender, MouseButtonEventArgs e)
        {
            cLogger.Log(" MainLayout_MouseDown ");

        }



        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            cLogger.Log(" Image_MouseUp ");

        }

        internal void MoveCart()
        {
            robotMover.MoveCart();
        }
        private void RobotMover_RobotLeave(object? sender, RobotMoverControlDrag e)
        {
            overrobot = false;
        }

        private void RobotMover_RobotEnter(object? sender, RobotMoverControlDrag e)
        {
            overrobot = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ResetPersonMesssage resetPerson = 
                new ResetPersonMesssage(_personOriginalLeft, _personOriginalTop);
            WeakReferenceMessenger.Default.Send(resetPerson);

        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
