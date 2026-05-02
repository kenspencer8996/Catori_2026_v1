using CatoriCity2025WPF.Views.Controls.Robots.RobotArm.Robot;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CatoriCity2025WPF.Views.Controls.Robots.RobotArm
{
    /// <summary>
    /// Interaction logic for RobotArm.xaml
    /// </summary>
    public partial class RobotArmControl : UserControl
    {
        RobotArmController _controller;
        /// <summary>
        /// Kinematics for a 4-segment robotic arm with a tool at the end. Each segment can rotate independently, allowing for complex movements and precise control of the tool's position and orientation in 2D space.
        /// </summary>
        public RobotArmControl()
        {
            InitializeComponent();
            _controller = new RobotArmController(this);

        }
        double _angle1;
        double _angle2;
        double _angle3;
        double _angle4;
        private bool _isDragging;
        private double _dragOffset;
        private List<Point> _joints;
        private List<double> _lengths;
        private List<Canvas> _segmentCanvases;
        private List<ArmSegment> _armSegments;
        private Point _fixedBase;
        private RobotArmSegmentControl _draggedArmSegment;
        double len1;
        double len2 ;
        double len3 ;
        double len4 ;
        //private const double PivotYOffset = -12;
        private const double PivotYOffset = 12;
        //private const double JointOverlap = 22;
        private const double JointOverlap = 12;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            len1 = RobotArmSeg1.ActualWidth + 10;// - JointOverlap;
            len2 = RobotArmSeg2.ActualWidth - JointOverlap;
            len3 = RobotArmSeg3.ActualWidth - JointOverlap;
            len4 = RobotArmSeg4.ActualWidth - JointOverlap;
        }
        public void SetupRobot(RobotColorEnum color)
        {             
            _controller.SetupRobot(color);
            _fixedBase = new Point(100, 100); // wherever your base should be
            _fixedBase = new Point(200, 200);

            RobotArmSeg1.SegmentMouseDown += RobotArmSeg_SegmentMouseDown;
            RobotArmSeg1.SetSegmentImage("robotArmLongBlue.png");
            RobotArmSeg2.SegmentMouseDown += RobotArmSeg_SegmentMouseDown;
            RobotArmSeg2.SetSegmentImage("robotArmLongBlue.png");
            RobotArmSeg3.SegmentMouseDown += RobotArmSeg_SegmentMouseDown;
            RobotArmSeg3.SetSegmentImage("robotArmLongBlue.png");
            RobotArmSeg4.SegmentMouseDown += RobotArmSeg_SegmentMouseDown;
            RobotArmSeg4.SetSegmentImage("RobotArmHandBlue.png");
            _joints = new List<Point>();
            _joints.Add(new Point(0, 0));
            _joints.Add(new Point(0, 0));
            _joints.Add(new Point(0, 0));
            _joints.Add(new Point(0, 0));
            _joints.Add(new Point(0, 0));

            //_lengths = new List<double>();
            //_lengths.Add(Segment1Length);
            //_lengths.Add(Segment2Length);
            //_lengths.Add(Segment3Length);
            //_lengths.Add(Segment4Length);

          

            _armSegments = new List<ArmSegment>();
            ArmSegment segment1 = new ArmSegment();
            ArmSegment segment2 = new ArmSegment();
            ArmSegment segment3 = new ArmSegment();
            ArmSegment segment4 = new ArmSegment();
            _armSegments.Add(segment1);
            _armSegments.Add(segment2);
            _armSegments.Add(segment3);
            _armSegments.Add(segment4);

            AddTestElipse();
        }
       
        private void RobotArmSeg_SegmentMouseDown(object? sender, SegmentMouseDownArgs e)
        {
            switch (e.SegmentName)
            {
                case "RobotArmSeg1":
                    _draggedArmSegment = RobotArmSeg1;
                    break;
                case "RobotArmSeg2":
                    _draggedArmSegment = RobotArmSeg2;
                    break;
                case "RobotArmSeg3":
                    _draggedArmSegment = RobotArmSeg3;
                    break;
                case "RobotArmSeg4":
                    _draggedArmSegment = RobotArmSeg4;
                    break;
                default:
                    break;
            }
          

            UserControl_MouseDown(this, e.MouseArgs);
        }

        private void AddTestElipse()
        {
            foreach (var joint in _joints)
            {
                var ellipse = new Ellipse
                {
                    Width = 6,
                    Height = 6
                };

                Canvas.SetLeft(ellipse, joint.X - 3);
                Canvas.SetTop(ellipse, joint.Y - 3);

                RobotRoot.Children.Add(ellipse);
            }
        }
        //public FrameworkElement HandHostElement => JointHandHost;
        //public void MoveArm1(int angle) 
        //{
        //    Joint1Angle = angle;
        //}
        //public void MoveArm2(int angle)
        //{
        //    //Joint2Angle = angle;
        //}
        //public void MoveArm3(int angle)
        //{
        //    //Joint3Angle = angle;
        //}
        //public void MoveArm4(int angle)
        //{
        //    //Joint4Angle = angle;
        //}
        //public ImageSource BaseImageSource
        //{
        //    get => (ImageSource)GetValue(BaseImageSourceProperty);
        //    set => SetValue(BaseImageSourceProperty, value);
        //}

        //public static readonly DependencyProperty BaseImageSourceProperty =
        //    DependencyProperty.Register(nameof(BaseImageSource), typeof(ImageSource), typeof(RobotArm));

        //public ImageSource Segment1ImageSource
        //{
        //    get => (ImageSource)GetValue(Segment1ImageSourceProperty);
        //    set => SetValue(Segment1ImageSourceProperty, value);
        //}

        //public static readonly DependencyProperty Segment1ImageSourceProperty =
        //    DependencyProperty.Register(nameof(Segment1ImageSource), typeof(ImageSource), typeof(RobotArm));

        //public ImageSource Segment2ImageSource
        //{
        //    get => (ImageSource)GetValue(Segment2ImageSourceProperty);
        //    set => SetValue(Segment2ImageSourceProperty, value);
        //}

        //public static readonly DependencyProperty Segment2ImageSourceProperty =
        //    DependencyProperty.Register(nameof(Segment2ImageSource), typeof(ImageSource), typeof(RobotArm));

        //public ImageSource Segment3ImageSource
        //{
        //    get => (ImageSource)GetValue(Segment3ImageSourceProperty);
        //    set => SetValue(Segment3ImageSourceProperty, value);
        //}

        //public static readonly DependencyProperty Segment3ImageSourceProperty =
        //    DependencyProperty.Register(nameof(Segment3ImageSource), typeof(ImageSource), typeof(RobotArm));

        //public ImageSource Segment4ImageSource
        //{
        //    get => (ImageSource)GetValue(Segment4ImageSourceProperty);
        //    set => SetValue(Segment4ImageSourceProperty, value);
        //}

        //public static readonly DependencyProperty Segment4ImageSourceProperty =
        //    DependencyProperty.Register(nameof(Segment4ImageSource), typeof(ImageSource), typeof(RobotArm));

        //public ImageSource ToolImageSource
        //{
        //    get => (ImageSource)GetValue(ToolImageSourceProperty);
        //    set => SetValue(ToolImageSourceProperty, value);
        //}

        //public static readonly DependencyProperty ToolImageSourceProperty =
        //    DependencyProperty.Register(nameof(ToolImageSource), typeof(ImageSource), typeof(RobotArm));

        //public double Segment1Length
        //{
        //    get => (double)GetValue(Segment1LengthProperty);
        //    set => SetValue(Segment1LengthProperty, value);
        //}

        //public static readonly DependencyProperty Segment1LengthProperty =
        //    DependencyProperty.Register(nameof(Segment1Length), typeof(double), typeof(RobotArm), new PropertyMetadata(120.0));

        //public double Segment2Length
        //{
        //    get => (double)GetValue(Segment2LengthProperty);
        //    set => SetValue(Segment2LengthProperty, value);
        //}

        //public static readonly DependencyProperty Segment2LengthProperty =
        //    DependencyProperty.Register(nameof(Segment2Length), typeof(double), typeof(RobotArm), new PropertyMetadata(100.0));

        //public double Segment3Length
        //{
        //    get => (double)GetValue(Segment3LengthProperty);
        //    set => SetValue(Segment3LengthProperty, value);
        //}

        //public static readonly DependencyProperty Segment3LengthProperty =
        //    DependencyProperty.Register(nameof(Segment3Length), typeof(double), typeof(RobotArm), new PropertyMetadata(80.0));

        //public double Segment4Length
        //{
        //    get => (double)GetValue(Segment4LengthProperty);
        //    set => SetValue(Segment4LengthProperty, value);
        //}

        //public static readonly DependencyProperty Segment4LengthProperty =
        //    DependencyProperty.Register(nameof(Segment4Length), typeof(double), typeof(RobotArm), new PropertyMetadata(60.0));

        public double Joint1Angle
        {
            get => (double)GetValue(Joint1AngleProperty);
            set => SetValue(Joint1AngleProperty, value);
        }

        public static readonly DependencyProperty Joint1AngleProperty =
            DependencyProperty.Register(
                nameof(Joint1Angle),
                typeof(double),
                typeof(RobotArmControl),
                new FrameworkPropertyMetadata(
                    0.0,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnJoint1AngleChanged));

        private static void OnJoint1AngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RobotArmControl control)
            {
                control.UpdateJoint1Rotation();
            }
        }
        private void UpdateJoint1Rotation()
        {
            //if (Joint1Rotate != null)
            //{
            //    Joint1Rotate.Angle = Joint1Angle;

            //}
        }
        //public double Joint2Angle
        //{
        //    get => (double)GetValue(Joint2AngleProperty);
        //    set => SetValue(Joint2AngleProperty, value);
        //}

        //public static readonly DependencyProperty Joint2AngleProperty =
        //    DependencyProperty.Register(
        //        nameof(Joint2Angle),
        //        typeof(double),
        //        typeof(RobotArm),
        //        new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnJoint2AngleChanged));

        //private static void OnJoint2AngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    if (d is RobotArm control)
        //    {
        //        control.UpdateJoint2Rotation();
        //    }
        //}

        //private void UpdateJoint2Rotation()
        //{
        //    if (Joint2Rotate != null)
        //    {
        //        Joint2Rotate.Angle = Joint2Angle;

        //    }
        //}
        //public double Joint3Angle
        //{
        //    get => (double)GetValue(Joint3AngleProperty);
        //    set => SetValue(Joint3AngleProperty, value);
        //}

        //public static readonly DependencyProperty Joint3AngleProperty =
        //    DependencyProperty.Register(
        //        nameof(Joint3Angle),
        //        typeof(double),
        //        typeof(RobotArm),
        //        new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnJoint3AngleChanged));

        //private static void OnJoint3AngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    if (d is RobotArm control)
        //    {
        //        control.UpdateJoint3Rotation();
        //    }
        //}

        //private void UpdateJoint3Rotation()
        //{
        //    if (Joint3Rotate != null)
        //    {
        //        Joint3Rotate.Angle = Joint3Angle;

        //    }
        //}
        #region "Move Methods"

        //public void MoveToHomePose()
        //{
        //    Joint1Angle = HomePose.Joint1;
        //    Joint2Angle = HomePose.Joint2;
        //    Joint3Angle = HomePose.Joint3;
        //    Joint4Angle = HomePose.JointHand;
        //}
        //public void MoveToPickupBPose()
        //{
        //    Joint1Angle = PickupBPose.Joint1;
        //    Joint2Angle = PickupBPose.Joint2;
        //    Joint3Angle = PickupBPose.Joint3;
        //    Joint4Angle = PickupBPose.JointHand;
        //}
        //public void MoveToLiftBPose()
        //{
        //    Joint1Angle = LiftBPose.Joint1;
        //    Joint2Angle = LiftBPose.Joint2;
        //    Joint3Angle = LiftBPose.Joint3;
        //    Joint4Angle = LiftBPose.JointHand;
        //}
        //public void MoveToDropOnAPose()
        //{
        //    Joint1Angle = DropOnAPose.Joint1;
        //    Joint2Angle = DropOnAPose.Joint2;
        //    Joint3Angle = DropOnAPose.Joint3;
        //    Joint4Angle = DropOnAPose.JointHand;
        //}
        #endregion
        //private RobotPose HomePose = new RobotPose (-90,  0,  0,  0 );
        //private RobotPose PickupBPose = new RobotPose ( 20, 35,  -40, 10 );
        //private RobotPose LiftBPose = new RobotPose ( 10,  5,  -15,  0 );
        //private RobotPose DropOnAPose = new RobotPose(-25, 40, -20, 5);
        //public double Joint4Angle
        //{
        //    get => (double)GetValue(Joint4AngleProperty);
        //    set => SetValue(Joint4AngleProperty, value);
        //}

        //public static readonly DependencyProperty Joint4AngleProperty =
        //    DependencyProperty.Register(
        //        nameof(Joint4Angle),
        //        typeof(double),
        //        typeof(RobotArm),
        //        new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnJointHandAngleChanged));
        ////private static void OnJointHandAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    if (d is RobotArm control)
        //    {
        //        control.UpdateJointHandRotation();
        //    }
        //}
        //private void UpdateJointHandRotation()
        //{
        //    if (JointHandRotate != null)
        //    {
        //        //JointHandRotate.Angle = Joint4Angle;

        //    }
        //}

        //public RobotPose GetCurrentPose()
        //{
        //    return new RobotPose(
        //        Joint1Rotate.Angle,
        //        Joint2Rotate.Angle,
        //        Joint3Rotate.Angle,
        //        JointHandRotate.Angle);
        //}

        //public void ApplyPose(RobotPose pose)
        //{
        //    if (pose == null) throw new ArgumentNullException(nameof(pose));

        //    Joint1Rotate.Angle = pose.Joint1;
        //    Joint2Rotate.Angle = pose.Joint2;
        //    Joint3Rotate.Angle = pose.Joint3;
        //    JointHandRotate.Angle = pose.JointHand;
        //}
        public async Task MoveToPoseAsync(RobotPose targetPose, TimeSpan duration)
        {
            var tcs = new TaskCompletionSource<bool>();

            int completed = 0;

            void OnDone(object? s, EventArgs e)
            {
                completed++;
                if (completed == 4)
                    tcs.TrySetResult(true);
            }

            //AnimateToAngle(Joint1Rotate, targetPose.Joint1, duration, OnDone);
            //AnimateToAngle(Joint2Rotate, targetPose.Joint2, duration, OnDone);
            //AnimateToAngle(Joint3Rotate, targetPose.Joint3, duration, OnDone);
            //AnimateToAngle(JointHandRotate, targetPose.JointHand, duration, OnDone);

            await tcs.Task;
        }

        private void AnimateToAngle(RotateTransform rotate, double targetAngle, TimeSpan duration, EventHandler done)
        {
            double currentAngle = rotate.Angle;

            var anim = new DoubleAnimation
            {
                From = currentAngle,
                To = targetAngle,
                Duration = duration,
                FillBehavior = FillBehavior.Stop
            };
            anim.EasingFunction = new CubicEase
            {
                EasingMode = EasingMode.EaseInOut
            };
            anim.Completed += (s, e) =>
            {
                rotate.Angle = targetAngle;
                rotate.BeginAnimation(RotateTransform.AngleProperty, null);
                done(s, e);
            };

            rotate.BeginAnimation(RotateTransform.AngleProperty, anim);
        }
        public void MoveTo(Point target)
        {
            SolveIK(target);
            RenderArm();
        }

       
        int maxIterations = 10;

        void SolveIK(Point target)
        {
            for (int iter = 0; iter < maxIterations; iter++)
            {
                // backward
                _joints[^1] = target;

                for (int i = _joints.Count - 2; i >= 0; i--)
                {
                    double r = Distance(_joints[i + 1], _joints[i]);
                    if (r < 0.001) r = 0.001;

                    double lambda = _lengths[i] / r;

                    _joints[i] = new Point(
                        (1 - lambda) * _joints[i + 1].X + lambda * _joints[i].X,
                        (1 - lambda) * _joints[i + 1].Y + lambda * _joints[i].Y
                    );
                }

                // 🔴 THIS is critical
                _joints[0] = _fixedBase;

                // forward
                for (int i = 0; i < _joints.Count - 1; i++)
                {
                    double r = Distance(_joints[i + 1], _joints[i]);
                    if (r < 0.001) r = 0.001;

                    double lambda = _lengths[i] / r;

                    _joints[i + 1] = new Point(
                        (1 - lambda) * _joints[i].X + lambda * _joints[i + 1].X,
                        (1 - lambda) * _joints[i].Y + lambda * _joints[i + 1].Y
                    );
                }

                // 🔴 enforce again (important)
                _joints[0] = _fixedBase;
            }
        }

        private static double Distance(Point a, Point b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
        void UpdateAnglesFromJoints()
        {
            //Joint1Angle = _joints[0];
            for (int i = 0; i < _joints.Count - 1; i++)
            {
                var a = _joints[i];
                var b = _joints[i + 1];

                double angle = Math.Atan2(b.Y - a.Y, b.X - a.X) * 180 / Math.PI;

                ArmSegment arm = _armSegments[i];
                arm.Angle = angle;
            }
        }
        private void RenderArm()
        {
            RobotRoot.Children.Clear();

            // 🔴 ALWAYS visible test line
            var test = new Line
            {
                X1 = 0,
                Y1 = 0,
                X2 = 300,
                Y2 = 300,
                Stroke = Brushes.Red,
                StrokeThickness = 5
            };
            RobotRoot.Children.Add(test);

            // 🔍 dump joints
            foreach (var j in _joints)
            {
                Debug.WriteLine($"Joint: {j.X}, {j.Y}");
            }

            // 🔵 draw arm with offset so it's guaranteed onscreen
            for (int i = 0; i < _joints.Count - 1; i++)
            {
                var start = _joints[i];
                var end = _joints[i + 1];

                var line = new Line
                {
                    X1 = start.X + 150,
                    Y1 = start.Y + 150,
                    X2 = end.X + 150,
                    Y2 = end.Y + 150,
                    Stroke = Brushes.Blue,
                    StrokeThickness = 4
                };

                RobotRoot.Children.Add(line);
            }
        }
        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging || _draggedArmSegment == null)
                return;

            Point mouse = e.GetPosition(RobotRoot);
            Point pivot = GetSegmentPivot(_draggedArmSegment);

            double desiredWorldAngle = GetAngleFromPivot(pivot, mouse) + _dragOffset;

            switch (_draggedArmSegment.Name)
            {
                case "RobotArmSeg1":
                    _angle1 = desiredWorldAngle;
                    break;

                case "RobotArmSeg2":
                    _angle2 = desiredWorldAngle - _angle1;
                    break;

                case "RobotArmSeg3":
                    _angle3 = desiredWorldAngle - (_angle1 + _angle2);

                    break;

                case "RobotArmSeg4":
                    _angle4 = desiredWorldAngle - (_angle1 + _angle2 + _angle3);
                    ;
                    break;
            }

            SetPose((int)_angle1, (int)_angle2, (int)_angle3, (int)_angle4);
        }
        private Point GetSegmentPivot(RobotArmSegmentControl seg)
        {
            return new Point(
                Canvas.GetLeft(seg),
                Canvas.GetTop(seg) + PivotYOffset);
        }
      
        private double GetAngleFromPivot(Point pivot, Point mouse)
        {
            double dx = mouse.X - pivot.X;
            double dy = mouse.Y - pivot.Y;

            return Math.Atan2(dy, dx) * 180.0 / Math.PI;
        }
        private void UserControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SyncAnglesFromUI();   // NEW

            Point mouse = e.GetPosition(RobotRoot);
            Point pivot = GetSegmentPivot(_draggedArmSegment);
            //Point pivot = new Point(
            //    Canvas.GetLeft(_draggedArmSegment),
            //    Canvas.GetTop(_draggedArmSegment) + (_draggedArmSegment.ActualHeight / 2));

            double mouseAngle = GetAngleFromPivot(pivot, mouse);

            _dragOffset = _draggedArmSegment.SegmentAngle - mouseAngle;
            // _joints[0] = new Point(_joints[0].X, Math.Max(0, _joints[0].Y));
            _isDragging = true;
            RobotRoot.CaptureMouse();
        }
        private void SyncAnglesFromUI()
        {
            _angle1 = RobotArmSeg1.SegmentAngle;
            _angle2 = RobotArmSeg2.SegmentAngle - _angle1;
            _angle3 = RobotArmSeg3.SegmentAngle - _angle1 - _angle2;
            _angle4 = RobotArmSeg4.SegmentAngle - _angle1 - _angle2 - _angle3;
        }
        private void UserControl_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _isDragging = false;
            Mouse.Capture(null);
            RobotRoot.ReleaseMouseCapture();
        }

        private void SyncJointsToUI()
        {
            for (int i = 0; i < _armSegments.Count; i++)
            {
                var segment = _segmentCanvases[i];

                double x = Canvas.GetLeft(segment);
                double y = Canvas.GetTop(segment);

                _joints[i] = new Point(x, y);

                if (i == _segmentCanvases.Count - 1)
                {
                    double angle = ((RotateTransform)segment.RenderTransform).Angle * Math.PI / 180;

                    double endX = x + Math.Cos(angle) * _lengths[i];
                    double endY = y + Math.Sin(angle) * _lengths[i];

                    _joints[i + 1] = new Point(endX, endY);
                }
            }
        }


        private Point GetSegmentEnd(double startX, double startY, double angleDegrees, double length)
        {
            double radians = angleDegrees * Math.PI / 180.0;

            return new Point(
                startX + Math.Cos(radians) * length,
                startY + Math.Sin(radians) * length);
        }
        private void SetPosePartial(int angle1, int angle2 = 0, int angle3 = 0, int angleh = 0)
        {
            double seg1angle = angle1;
            double seg2angle = angle2;
            double seg3angle = angle3;
            double seghandle = angleh;
            if (angle1 == 0) seg1angle = RobotArmSeg1.SegmentRotate.Angle;
            if (angle2 == 0) seg2angle = RobotArmSeg2.SegmentRotate.Angle;
            if (angle3 == 0) seg3angle = RobotArmSeg3.SegmentRotate.Angle;
            if (angleh == 0) seghandle = RobotArmSeg4.SegmentRotate.Angle;
            SetPose((int)seg1angle, (int)seg2angle, (int)seg3angle, (int)seghandle);
        }
        public void SetPose(int angle1, int angle2, int angle3, int angleh)
        {
            _angle1 = angle1;
            _angle2 = angle2;
            _angle3 = angle3;
            _angle4 = angleh;

            double baseX = 400;
            //double baseY = 535;
            double baseY = 545;
            //double len1 = RobotArmSeg1.ActualWidth;
            //double len2 = RobotArmSeg2.ActualWidth;
            //double len3 = RobotArmSeg3.ActualWidth;
            //double len4 = RobotArmSeg4.ActualWidth;

            double world1 = angle1;
            double world2 = angle1 + angle2;
            double world3 = angle1 + angle2 + angle3;
            double world4 = angle1 + angle2 + angle3 + angleh;

            // Seg1
            Canvas.SetLeft(RobotArmSeg1, baseX);
            Canvas.SetTop(RobotArmSeg1, baseY - 12);
            RobotArmSeg1.SegmentAngle = world1;

            // Seg2
            Point joint2 = GetSegmentEnd(baseX, baseY, world1, len1);
            Canvas.SetLeft(RobotArmSeg2, joint2.X);
            Canvas.SetTop(RobotArmSeg2, joint2.Y - PivotYOffset);
            RobotArmSeg2.SegmentAngle = world2;

            // Seg3
            Point joint3 = GetSegmentEnd(joint2.X, joint2.Y, world2, len2);
            Canvas.SetLeft(RobotArmSeg3, joint3.X);
            Canvas.SetTop(RobotArmSeg3, joint3.Y - PivotYOffset);
            RobotArmSeg3.SegmentAngle = world3;

            // Seg4
            Point joint4 = GetSegmentEnd(joint3.X, joint3.Y, world3, len3);
            Canvas.SetLeft(RobotArmSeg4, joint4.X);
            Canvas.SetTop(RobotArmSeg4, joint4.Y - PivotYOffset);
            RobotArmSeg4.SegmentAngle = world4;
        }

        private Point MoveSegment2(double startx,double starty,
            int parentAngle, double angleDegrees, double length)
        {
            Point joint2 = GetSegmentEnd(startx, starty, RobotArmSeg1.SegmentRotate.Angle, length);
            
            RobotArmSeg2.SegmentRotate.Angle = angleDegrees;
            //Point thisJoint = GetSegmentEnd(joint1X, joint1Y, parentangle, 140);

            Canvas.SetLeft(RobotArmSeg2, joint2.X);
            Canvas.SetTop(RobotArmSeg2, joint2.Y - 20);
            return joint2;
        }
        private Point MoveSegment3(double startx, double starty,
            int parentAngle, double angleDegrees, double length)
        {
            Point joint2 = GetSegmentEnd(startx, starty, RobotArmSeg2.SegmentRotate.Angle, length);

            RobotArmSeg3.SegmentRotate.Angle = angleDegrees;

            Canvas.SetLeft(RobotArmSeg3, joint2.X);
            Canvas.SetTop(RobotArmSeg3, joint2.Y - 20);
            return joint2;
        }
        private Point MoveSegmentHand(double startx, double starty,
          int parentAngle, double angleDegrees, double length)
        {
            Point joint2 = GetSegmentEnd(startx, starty, RobotArmSeg4.SegmentRotate.Angle, length);

            RobotArmSeg4.SegmentRotate.Angle = angleDegrees;

            Canvas.SetLeft(RobotArmSeg4, joint2.X);
            Canvas.SetTop(RobotArmSeg4, joint2.Y - 20);
            return joint2;
        }

       
    }
}
