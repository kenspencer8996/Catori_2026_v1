
using CatoriCity2025WPF.Objects.Arguments;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace CatoriCity2025WPF.Views.Controls
{
    /// <summary>
    /// Interaction logic for RobotCartControl.xaml
    /// </summary>
    public partial class RobotCartControl : UserControl
    {
        public event EventHandler RobotCartControlMouseUp;
        DispatcherTimer _moveCartTimer;
        public double _originalLeft = 350;
        public double _originalTop = 520;
        public event EventHandler<RobotMoverControlDrag> RobotMoverMouseUp;
        public RobotCartControl()
        {
            InitializeComponent();
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
        internal void MoveCart()
        {
            _moveCartTimer.Start();

        }
        private void MoveCartFromTimer()
        {
            //show side view
            SetSideView();
            int seconds = 2;
            Storyboard sb = new Storyboard();
            DoubleAnimation daleft = AnimationHelper.GetDoubleAnimation(_originalLeft, 500, seconds * 1000);
            daleft.EasingFunction = new ExponentialEase
            {
                EasingMode = EasingMode.EaseOut,
                Exponent = 5
            };
            Storyboard.SetTarget(daleft, this);
            Storyboard.SetTargetProperty(daleft, new PropertyPath("(Canvas.Left)"));
            sb.Children.Add(daleft);

            DoubleAnimation dtop = AnimationHelper.GetDoubleAnimation(_originalTop, 550, seconds * 1000);
            Storyboard.SetTarget(dtop, this);
            Storyboard.SetTargetProperty(dtop, new PropertyPath("(Canvas.Top)"));
            dtop.EasingFunction = new ExponentialEase
            {
                EasingMode = EasingMode.EaseOut,
                Exponent = 5
            };
            sb.Children.Add(dtop);

            DoubleAnimation daleft2 = AnimationHelper.GetDoubleAnimation(1000, 1220, seconds * 1000);
            Storyboard.SetTarget(daleft2, this);
            Storyboard.SetTargetProperty(daleft2, new PropertyPath("(Canvas.Left)"));
            sb.Children.Add(daleft2);

            DoubleAnimation dtop2 = AnimationHelper.GetDoubleAnimation(550, 400, seconds * 1000);
            Storyboard.SetTarget(dtop2, this);
            Storyboard.SetTargetProperty(dtop2, new PropertyPath("(Canvas.Top)"));
            sb.Children.Add(dtop2);

            sb.Completed += (s, e) =>
            {
                //show rear view
                SetreadView();
                Task.Delay(500).ContinueWith(t =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        //show front view
                        SetFrontiew();
                    });
                });
            };  
            sb.Begin();
        }
        private void SetFrontiew()
        {
            string filepath = System.IO.Path.Combine(GlobalStuff.ImageFolder, "stores", "warehoouserollingrobot.png");
            RobotCartImage.Source = UIUtility.GetImageControl(filepath, 100, 100, 0).Source;
        }
        private void SetSideView()
        {
            string filepath = System.IO.Path.Combine(GlobalStuff.ImageFolder, "stores", "warehouserobobtsideview.png");
            RobotCartImage.Source = UIUtility.GetImageControl(filepath, 100, 100, 0).Source;
        }
        private void SetreadView()
        {
            string filepath = System.IO.Path.Combine(GlobalStuff.ImageFolder, "stores", "warehouserobotrearview.png");
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

    }
}
