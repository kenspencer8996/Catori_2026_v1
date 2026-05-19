
using CatoriApp.Controllers;
using CatoriApp.Objects.Arguments;
using System.Windows.Media.Animation;
using System.Windows.Threading;
namespace CatoriApp.Views.Controls.RobotsDrones
{
    /// <summary>
    /// Interaction logic for RobotCartControl.xaml
    /// </summary>
    public partial class RobotCartControl : UserControl
    {
        public event EventHandler RobotCartControlMouseUp;
        DispatcherTimer _moveCartTimer;
        public event EventHandler<RobotMoverControlDrag> RobotMoverMouseUp;
        public event EventHandler<RobotMoverControlDrag> RobotEnter;
        public event EventHandler<RobotMoverControlDrag> RobotLeave;
        public event EventHandler<RobotMoverControlDrag> RobotAllFinished;

        RobotCartControlController _controller;
        public double _originalLeft ;
        public double _originalTop ;
        public RobotCartControl()
        {
            InitializeComponent();
            _controller = new RobotCartControlController(this);
            _originalLeft = _controller._originalLeft;
            _originalTop = _controller._originalTop;
            _moveCartTimer = new DispatcherTimer();
            _moveCartTimer.Tick += new EventHandler(_moveCartTimer_Tick);
            _moveCartTimer.Interval = new TimeSpan(0, 0, 2);

        }

        private void _moveCartTimer_Tick(object? sender, EventArgs e)
        {
            _moveCartTimer.Stop();
            MoveCartFromTimer();
        }

        public ShoppingCartItemViewModel Model;
        public string HardwareStoreName { get; set; }
        internal void MoveCartToLoadOut()
        {
            _controller.MoveCartToLoadOut();
        }
        internal void MoveCart()
        {
            _moveCartTimer.Start();

        }
        private void MoveCartFromTimer()
        {
            _controller.MoveCartFromTimer();
           
        }
        public void SetFrontiew()
        {
            string filepath = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "stores", "warehoouserollingrobot.png");
            RobotCartImage.Source = UIUtility.GetImageControl(filepath, 100, 100, 0).Source;
        }
        public void SetSideView()
        {
            string filepath = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "stores", "warehouserobobtsideview.png");
            RobotCartImage.Source = UIUtility.GetImageControl(filepath, 100, 100, 0).Source;
        }
        public void SetRearView()
        {
            string filepath = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "stores", "warehouserobotrearview.png");
            RobotCartImage.Source = UIUtility.GetImageControl(filepath, 100, 100, 0).Source;
        }
        private void AddShoppingItem(ShoppingCartItemViewModel model)
        {
            Model = model;
            Image image = new Image();
            image.Source = UIUtility.GetImageBitmap(model.ShopItem.FilePath);
            image.Width = 30;
            image.Height = 15;
        }

        private void ShopItemsPanel_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
          
        }

        private void ShopItemsPanel_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void UserControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (RobotEnter != null)
            {
                RobotMoverControlDrag arg = new RobotMoverControlDrag();

                RobotEnter(this, arg);
            }
        }

        private void UserControl_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            cLogger.Log(" in mouseup ");
            //_isDragging = false;
            //StopDrag(this, new ShopItemontrolDrag() { shopItemControl = null });
            HandleCaptureMouse(false);
            RobotMoverControlDrag args = new RobotMoverControlDrag();
            args.shopItemControl = null;
            if (RobotMoverMouseUp != null)
            {
                RobotMoverMouseUp(this, args);
            }
        }
        private void HandleCaptureMouse(bool capture)
        {
            if (capture)
            {
                this.CaptureMouse();
            }
            else
            {
                this.ReleaseMouseCapture();
            }
        }

        private void UserControl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (RobotLeave != null)
            {
                RobotMoverControlDrag arg = new RobotMoverControlDrag();

                RobotLeave(this, arg);
            }
        }

        public void RobotAllDone()
        {
            if (RobotAllFinished != null)
            {
                RobotMoverControlDrag arg = new RobotMoverControlDrag();
                RobotAllFinished(this, arg);
            }
        }
    }
}


