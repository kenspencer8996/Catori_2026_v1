using CatoriCity2025WPF.Objects.Arguments;
using CatoriCity2025WPF.Objects.DragDrop;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Threading;

namespace CatoriCity2025WPF.Views.Controls.Robots
{
    /// <summary>
    /// Interaction logic for RobotControl.xaml
    /// </summary>
    public partial class RobotControl : UserControl, IDraggable, IDropTarget
    {
        List<string> robotImagePaths = new List<string>();
        private readonly DispatcherTimer _animateRobotTimer;
        int rowindex = 0;
        public event EventHandler<RobotArg> MouseUpAfterRobotMove;

        public RobotControl()
        {
            InitializeComponent();
            _animateRobotTimer = new DispatcherTimer();
            _animateRobotTimer.Interval = TimeSpan.FromMilliseconds(250); // 20 Hz flicker
            _animateRobotTimer.Tick += _animateRobotTimer_Tick;
            this.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(RobotUC_ForwardMouseDown), true);

        }

        private void RobotUC_ForwardMouseDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("RobotControl is now the source: " + e.Source);
            this.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(RobotUC_ForwardMouseDown), true);
        }

        private void _animateRobotTimer_Tick(object? sender, EventArgs e)
        {
            string thisImage = robotImagePaths[rowindex];
            RobotControlImage.Source = UIUtility.GetImageControl(thisImage, 10, 5, 0).Source;

            rowindex++;
            if (rowindex >= robotImagePaths.Count )
            {
                rowindex = 0;
                _animateRobotTimer.Stop();
            }
        }

        public void AddRobotImage(string robotImage)
        {

            string thisimage = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "Factories\\RobotArms", robotImage);
            robotImagePaths.Add(thisimage);
        }
        public void BuildProduct()
        {
            if (this.Visibility != Visibility.Visible)
            {
                this.Visibility = Visibility.Visible;
            }
            _animateRobotTimer.Start();
        }

        public void OnDragMouseup()
        {
            //todo: add code to handle when the robot is dropped onto a factory
            if (MouseUpAfterRobotMove != null)
            {
                RobotArg arg = new RobotArg
                {
                    Robot = this,
                    X = Canvas.GetLeft(this),
                    Y = Canvas.GetTop(this)
                    
                };
                MouseUpAfterRobotMove.Invoke(this, arg);
            }
        }

        private void RobotUC_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = false; // allow event to bubble up to Canvas
        }
        public UIElement Visual
        {
            get
            {
                return this;
            }
        }

        public Point OriginalPosition
        {
            get
            {
                double left = Canvas.GetLeft(this);
                double top = Canvas.GetTop(this);
                return new Point(left, top);
            }
        }

        private void RobotUC_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = false; // allow event to bubble up to Canvas
        }

        public bool CanDrop(IDraggable element)
        {
            return true;
        }

        public void OnDrop(IDraggable element)
        {
        }

        public void HighlightOn()
        {
        }

        public void HighlightOff()
        {
        }

        public Point GetSnapPoint(IDraggable dragged)
        {
            var feDragged = (FrameworkElement)dragged;
            double x = Canvas.GetLeft(this) + (this.ActualWidth - feDragged.ActualWidth) / 2;
            double y = Canvas.GetTop(this) + (this.ActualHeight - feDragged.ActualHeight) / 2;
            return new Point(x, y);

        }
    }
}

