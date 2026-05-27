using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CatoriUCLibrary.Views.RobotArm
{
    public partial class RoboticArmUC : UserControl
    {
        private readonly RobotArmController _controller;
        private readonly List<RobotArmSegmentontrol> _segments = new();
        private readonly List<double> _relativeAngles = new();
        private bool _isDragging;
        private double _dragOffset;
        private Point _fixedBase = new(390, 525);
        private RobotArmSegmentontrol? _draggedArmSegment;

        public RoboticArmUC()
        {
            InitializeComponent();
            _controller = new RobotArmController(this);
        }

        public Point FixedBase
        {
            get => _fixedBase;
            set
            {
                _fixedBase = value;
                RenderConfiguredPose();
            }
        }

        public IList<RobotArmSegmentDefinition> SegmentDefinitions { get; } = new List<RobotArmSegmentDefinition>
        {
            new() { Name = "Shoulder", ImagePath = "robotArmLongBlue.png", InitialAngle = -90, JointLength = 124 },
            new() { Name = "UpperArm", ImagePath = "robotArmLongBlue.png", InitialAngle = 0, JointLength = 124 },
            new() { Name = "Forearm", ImagePath = "robotArmLongBlue.png", InitialAngle = 0, JointLength = 124 },
            new() { Name = "Hand", ImagePath = "RobotArmHandBlue.png", InitialAngle = 0, JointLength = 124 }
        };

        public void SetupRobot(RobotColorEnum color)
        {
            _controller.SetupRobot(color);
            RebuildSegments();
            RenderConfiguredPose();
        }

        public void ConfigureSegments(IEnumerable<RobotArmSegmentDefinition> definitions)
        {
            SegmentDefinitions.Clear();

            foreach (RobotArmSegmentDefinition definition in definitions)
            {
                SegmentDefinitions.Add(definition);
            }

            RebuildSegments();
            RenderConfiguredPose();
        }

        public double Joint1Angle
        {
            get => (double)GetValue(Joint1AngleProperty);
            set => SetValue(Joint1AngleProperty, value);
        }

        public static readonly DependencyProperty Joint1AngleProperty =
            DependencyProperty.Register(nameof(Joint1Angle), typeof(double), typeof(RoboticArmUC),
                new FrameworkPropertyMetadata(-90.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnJointAngleChanged));

        public double Joint2Angle
        {
            get => (double)GetValue(Joint2AngleProperty);
            set => SetValue(Joint2AngleProperty, value);
        }

        public static readonly DependencyProperty Joint2AngleProperty =
            DependencyProperty.Register(nameof(Joint2Angle), typeof(double), typeof(RoboticArmUC),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnJointAngleChanged));

        public double Joint3Angle
        {
            get => (double)GetValue(Joint3AngleProperty);
            set => SetValue(Joint3AngleProperty, value);
        }

        public static readonly DependencyProperty Joint3AngleProperty =
            DependencyProperty.Register(nameof(Joint3Angle), typeof(double), typeof(RoboticArmUC),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnJointAngleChanged));

        public double Joint4Angle
        {
            get => (double)GetValue(Joint4AngleProperty);
            set => SetValue(Joint4AngleProperty, value);
        }

        public static readonly DependencyProperty Joint4AngleProperty =
            DependencyProperty.Register(nameof(Joint4Angle), typeof(double), typeof(RoboticArmUC),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnJointAngleChanged));

        private static void OnJointAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RoboticArmUC arm)
            {
                arm.ApplyPoseFromProperties();
            }
        }

        public void SetPose(int angle1, int angle2, int angle3, int angleh)
        {
            SetPose(new[] { (double)angle1, angle2, angle3, angleh });
        }

        public void SetPose(params double[] relativeAngles)
        {
            if (_segments.Count == 0)
                return;

            for (int i = 0; i < _segments.Count; i++)
            {
                _relativeAngles[i] = i < relativeAngles.Length ? relativeAngles[i] : 0;
            }

            RenderConfiguredPose();
            SyncJointProperties();
        }

        public RobotPose GetCurrentPose()
        {
            return new RobotPose(
                GetRelativeAngle(0),
                GetRelativeAngle(1),
                GetRelativeAngle(2),
                GetRelativeAngle(3));
        }

        public async Task MoveToPoseAsync(RobotPose targetPose, TimeSpan duration)
        {
            await MoveToPoseAsync(targetPose, (int)duration.TotalMilliseconds);
        }

        public async Task MoveToPoseAsync(RobotPose target, int milliseconds = 600)
        {
            RobotPose start = GetCurrentPose();
            Stopwatch sw = Stopwatch.StartNew();

            while (sw.ElapsedMilliseconds < milliseconds)
            {
                double t = sw.ElapsedMilliseconds / (double)milliseconds;
                t = EaseInOut(t);

                SetPose(
                    Lerp(start.Joint1, target.Joint1, t),
                    Lerp(start.Joint2, target.Joint2, t),
                    Lerp(start.Joint3, target.Joint3, t),
                    Lerp(start.JointHand, target.JointHand, t));

                await Task.Delay(16);
            }

            SetPose(target.Joint1, target.Joint2, target.Joint3, target.JointHand);
        }

        public void MoveTo(Point target)
        {
            if (_segments.Count == 0)
                return;

            double angle = GetAngleFromPivot(_fixedBase, target);
            _relativeAngles[0] = angle;
            RenderConfiguredPose();
            SyncJointProperties();
        }

        private void RebuildSegments()
        {
            RobotRoot.Children.Clear();
            RobotRoot.Children.Add(MessageLabel);
            _segments.Clear();
            _relativeAngles.Clear();

            foreach (RobotArmSegmentDefinition definition in SegmentDefinitions)
            {
                RobotArmSegmentontrol segment = new()
                {
                    Name = definition.Name,
                    Width = definition.Width,
                    Height = definition.Height,
                    SegmentLength = definition.JointLength
                };

                segment.SetSegmentImage(definition.ImagePath);
                segment.SegmentMouseDown += Segment_MouseDown;
                RobotRoot.Children.Insert(RobotRoot.Children.Count - 1, segment);
                _segments.Add(segment);
                _relativeAngles.Add(definition.InitialAngle);
            }

            SyncJointProperties();
        }

        private void Segment_MouseDown(object? sender, SegmentMouseDownArgs e)
        {
            _draggedArmSegment = sender as RobotArmSegmentontrol;
            UserControl_MouseDown(this, e.MouseArgs);
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging || _draggedArmSegment == null)
                return;

            int draggedIndex = _segments.IndexOf(_draggedArmSegment);
            if (draggedIndex < 0)
                return;

            Point mouse = e.GetPosition(RobotRoot);
            Point pivot = GetSegmentPivot(_draggedArmSegment);
            double worldAngle = GetAngleFromPivot(pivot, mouse) + _dragOffset;
            double parentWorldAngle = GetWorldAngleBefore(draggedIndex);

            _relativeAngles[draggedIndex] = worldAngle - parentWorldAngle;
            RenderConfiguredPose();
            SyncJointProperties();
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_draggedArmSegment == null)
            {
                MessageLabel.Content = "You must click on a segment to drag it.";
                return;
            }

            MessageLabel.Content = string.Empty;
            Point mouse = e.GetPosition(RobotRoot);
            Point pivot = GetSegmentPivot(_draggedArmSegment);
            double mouseAngle = GetAngleFromPivot(pivot, mouse);

            _dragOffset = _draggedArmSegment.SegmentAngle - mouseAngle;
            _isDragging = true;
            RobotRoot.CaptureMouse();
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            Mouse.Capture(null);
            RobotRoot.ReleaseMouseCapture();
            _draggedArmSegment = null;
        }

        private void RenderConfiguredPose()
        {
            if (_segments.Count == 0)
                return;

            Point joint = _fixedBase;
            double worldAngle = 0;

            for (int i = 0; i < _segments.Count; i++)
            {
                RobotArmSegmentontrol segment = _segments[i];

                worldAngle += _relativeAngles[i];
                Canvas.SetLeft(segment, joint.X);
                Canvas.SetTop(segment, joint.Y - GetSegmentHalfHeight(segment));
                segment.SegmentAngle = worldAngle;

                joint = GetSegmentEnd(joint, worldAngle, segment.SegmentLength);
            }
        }

        private void ApplyPoseFromProperties()
        {
            if (_segments.Count == 0)
                return;

            double[] values = { Joint1Angle, Joint2Angle, Joint3Angle, Joint4Angle };
            for (int i = 0; i < _relativeAngles.Count && i < values.Length; i++)
            {
                _relativeAngles[i] = values[i];
            }

            RenderConfiguredPose();
        }

        private void SyncJointProperties()
        {
            SetCurrentValue(Joint1AngleProperty, GetRelativeAngle(0));
            SetCurrentValue(Joint2AngleProperty, GetRelativeAngle(1));
            SetCurrentValue(Joint3AngleProperty, GetRelativeAngle(2));
            SetCurrentValue(Joint4AngleProperty, GetRelativeAngle(3));
        }

        private double GetRelativeAngle(int index)
        {
            return index < _relativeAngles.Count ? _relativeAngles[index] : 0;
        }

        private double GetWorldAngleBefore(int segmentIndex)
        {
            double angle = 0;
            for (int i = 0; i < segmentIndex; i++)
            {
                angle += _relativeAngles[i];
            }

            return angle;
        }

        private static Point GetSegmentPivot(RobotArmSegmentontrol segment)
        {
            return new Point(
                Canvas.GetLeft(segment),
                Canvas.GetTop(segment) + GetSegmentHalfHeight(segment));
        }

        private static double GetAngleFromPivot(Point pivot, Point mouse)
        {
            double dx = mouse.X - pivot.X;
            double dy = mouse.Y - pivot.Y;

            return Math.Atan2(dy, dx) * 180.0 / Math.PI;
        }

        private static Point GetSegmentEnd(Point start, double angleDegrees, double length)
        {
            double radians = angleDegrees * Math.PI / 180.0;

            return new Point(
                start.X + Math.Cos(radians) * length,
                start.Y + Math.Sin(radians) * length);
        }

        private static double GetSegmentHalfHeight(RobotArmSegmentontrol segment)
        {
            double height = segment.ActualHeight > 0 ? segment.ActualHeight : segment.Height;
            return height / 2;
        }

        private static double Lerp(double from, double to, double t)
        {
            return from + (to - from) * t;
        }

        private static double EaseInOut(double t)
        {
            return t < 0.5
                ? 2 * t * t
                : 1 - Math.Pow(-2 * t + 2, 2) / 2;
        }
    }
}
