using CatoriApp.Core.Objects.DragDrop;
using CatoriApp.Core.Objects.Arguments;
using CatoriApp.Core.Objects.Production;
using CommunityToolkit.Mvvm.Messaging;
using System.Diagnostics;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CatoriUCLibrary.Views.RobotArm
{
    public partial class RoboticArmUC : UserControl, IDraggable, ICanvasDragAnchor
    {
        public event EventHandler? DragCompleted;
        public event EventHandler<RobotPoseCompletedEventArgs>? PoseCompleted;
        private readonly RobotArmController _controller;
        private readonly List<RobotArmSegmentontrol> _segments = new();
        private readonly List<double> _relativeAngles = new();
        private bool _isDragging;
        private double _dragOffset;
        private Point _fixedBase = new(390, 525);
        private RobotArmSegmentontrol? _draggedArmSegment;
        private bool _isPlayingPoses;
        private bool _isSynchronizingJointProperties;
        private bool _isHandlingPickup;
        private ContentControl? _carriedPartHost;
        private Point _handPoint;
        public RoboticArmUC()
        {
            InitializeComponent();
            _controller = new RobotArmController(this);
            WeakReferenceMessenger.Default.Register<AnimationCompleteMessage>(this,
                static (recipient,message)=>((RoboticArmUC)recipient).ReceiveAnimationComplete(message));

        }

        public string? AnimationTargetName
        {
            get => (string?)GetValue(AnimationTargetNameProperty);
            set => SetValue(AnimationTargetNameProperty,value);
        }

        public static readonly DependencyProperty AnimationTargetNameProperty=
            DependencyProperty.Register(nameof(AnimationTargetName),typeof(string),typeof(RoboticArmUC));

        public bool ReleasePartAtDrop { get; set; }=true;
        public string? OutputPartImagePath { get; set; }

        private void ReceiveAnimationComplete(AnimationCompleteMessage message)
        {
            if(message.Action!=ProductionAction.Pickup
                ||!MatchesAnimationTarget(message.TargetName))
                return;

            if(!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action(()=>ReceiveAnimationComplete(message)));
                return;
            }
            _=HandlePickupAsync(message.PartName,message.Part);
        }

        private async Task HandlePickupAsync(string partName,FrameworkElement? part)
        {
            if(_isHandlingPickup)return;
            _isHandlingPickup=true;
            try
            {
                if(part!=null)
                {
                    PickupPart(part);
                    SendPartTransfer(partName,RobotPartTransferStage.PickedUp,part);
                }
                await PlayAllPosesAsync();
                if(!ReleasePartAtDrop)return;
                ApplyOutputPartImage(part);
                var releasedPart=ReleasePart();
                if(releasedPart!=null)
                    SendPartTransfer(partName,RobotPartTransferStage.Dropped,releasedPart);
            }
            finally
            {
                _isHandlingPickup=false;
            }
        }

        private void ApplyOutputPartImage(FrameworkElement? part)
        {
            if(part==null||string.IsNullOrWhiteSpace(OutputPartImagePath))return;
            var image=FindVisualChild<Image>(part);if(image==null)return;
            try { image.Source=new System.Windows.Media.Imaging.BitmapImage(new Uri(OutputPartImagePath,UriKind.RelativeOrAbsolute)); }
            catch(Exception) { }
        }

        private static T? FindVisualChild<T>(DependencyObject parent) where T:DependencyObject
        {
            for(int index=0;index<VisualTreeHelper.GetChildrenCount(parent);index++)
            {
                var child=VisualTreeHelper.GetChild(parent,index);
                if(child is T match)return match;
                var nested=FindVisualChild<T>(child);if(nested!=null)return nested;
            }
            return null;
        }

        private void PickupPart(FrameworkElement part)
        {
            DetachFromParent(part);
            if(_carriedPartHost!=null)RobotRoot.Children.Remove(_carriedPartHost);
            _carriedPartHost=new ContentControl
            {
                Content=part,IsHitTestVisible=false,
                Width=double.IsNaN(part.Width)?part.ActualWidth:part.Width,
                Height=double.IsNaN(part.Height)?part.ActualHeight:part.Height
            };
            Panel.SetZIndex(_carriedPartHost,10000);
            RobotRoot.Children.Add(_carriedPartHost);
            PositionCarriedPart();
        }

        private void PositionCarriedPart()
        {
            if(_carriedPartHost==null)return;
            double width=double.IsNaN(_carriedPartHost.Width)?_carriedPartHost.ActualWidth:_carriedPartHost.Width;
            double height=double.IsNaN(_carriedPartHost.Height)?_carriedPartHost.ActualHeight:_carriedPartHost.Height;
            Canvas.SetLeft(_carriedPartHost,_handPoint.X-width/2);
            Canvas.SetTop(_carriedPartHost,_handPoint.Y-height/2);
        }

        private FrameworkElement? ReleasePart()
        {
            if(_carriedPartHost?.Content is not FrameworkElement part)return null;
            var targetCanvas=FindAncestorCanvas(this);
            Point dropPoint=targetCanvas==null
                ? default
                : RobotRoot.TranslatePoint(_handPoint,targetCanvas);

            // Releasing always removes the visual owned by the arm.  Failure to
            // locate a destination must not leave a stale part stuck to the hand.
            _carriedPartHost.Content=null;
            RobotRoot.Children.Remove(_carriedPartHost);
            _carriedPartHost=null;
            if(targetCanvas==null)return null;

            var releasedHost=new ContentControl { Content=part,IsHitTestVisible=false };
            Canvas.SetLeft(releasedHost,dropPoint.X-Math.Max(0,part.ActualWidth)/2);
            Canvas.SetTop(releasedHost,dropPoint.Y-Math.Max(0,part.ActualHeight)/2);
            Panel.SetZIndex(releasedHost,Panel.GetZIndex(this)+1);
            targetCanvas.Children.Add(releasedHost);
            return part;
        }

        private void SendPartTransfer(string partName,RobotPartTransferStage stage,FrameworkElement part)
        {
            string robotName=string.IsNullOrWhiteSpace(AnimationTargetName)?Name:AnimationTargetName;
            WeakReferenceMessenger.Default.Send(new RobotPartTransferMessage(
                robotName??string.Empty,partName,stage,part));
        }

        private static Canvas? FindAncestorCanvas(DependencyObject child)
        {
            DependencyObject? current=VisualTreeHelper.GetParent(child);
            while(current!=null)
            {
                if(current is Canvas canvas)return canvas;
                current=VisualTreeHelper.GetParent(current);
            }
            return null;
        }

        private static void DetachFromParent(FrameworkElement element)
        {
            switch(element.Parent)
            {
                case ContentControl content when ReferenceEquals(content.Content,element): content.Content=null;break;
                case Panel panel: panel.Children.Remove(element);break;
                case Decorator decorator when ReferenceEquals(decorator.Child,element): decorator.Child=null;break;
            }
        }

        private bool MatchesAnimationTarget(string? targetName)
        {
            if(string.IsNullOrWhiteSpace(targetName))return false;
            string controlName=Name??string.Empty;
            const string suffix="RobotArmUC";
            string logicalName=controlName.EndsWith(suffix,StringComparison.Ordinal)
                ?controlName[..^suffix.Length]
                :controlName;
            return string.Equals(targetName,AnimationTargetName,StringComparison.Ordinal)
                ||string.Equals(targetName,controlName,StringComparison.Ordinal)
                ||string.Equals(targetName,logicalName,StringComparison.Ordinal);
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

        public string? ItemDataJson
        {
            get => (string?)GetValue(ItemDataJsonProperty);
            set => SetValue(ItemDataJsonProperty, value);
        }

        public static readonly DependencyProperty ItemDataJsonProperty =
            DependencyProperty.Register(
                nameof(ItemDataJson),
                typeof(string),
                typeof(RoboticArmUC),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool PlayAllPoses
        {
            get => (bool)GetValue(PlayAllPosesProperty);
            set => SetValue(PlayAllPosesProperty, value);
        }

        public static readonly DependencyProperty PlayAllPosesProperty =
            DependencyProperty.Register(
                nameof(PlayAllPoses),
                typeof(bool),
                typeof(RoboticArmUC),
                new PropertyMetadata(false, OnPlayAllPosesChanged));

        private static async void OnPlayAllPosesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not RoboticArmUC arm || e.NewValue is not true)
                return;

            try
            {
                await arm.PlayAllPosesAsync();
            }
            finally
            {
                arm.SetCurrentValue(PlayAllPosesProperty, false);
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
            if (d is RoboticArmUC arm && !arm._isSynchronizingJointProperties)
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

        public async Task PlayAllPosesAsync(int millisecondsPerPose = 600)
        {
            if (_isPlayingPoses || string.IsNullOrWhiteSpace(ItemDataJson))
                return;

            _isPlayingPoses = true;
            try
            {
                List<RobotPoseDefinition> poses;
                try
                {
                    poses = JsonSerializer.Deserialize<List<RobotPoseDefinition>>(ItemDataJson) ?? [];
                }
                catch (JsonException)
                {
                    MessageLabel.Content = "Robot pose data is not valid JSON.";
                    return;
                }

                for (var index = 0; index < poses.Count; index++)
                {
                    var storedPose = poses[index];
                    var angles = storedPose.Angles??[];
                    var pose = new RobotPose(
                        GetAngle(angles, 0), GetAngle(angles, 1),
                        GetAngle(angles, 2), GetAngle(angles, 3));

                    await MoveToPoseAsync(pose, millisecondsPerPose);
                    PoseCompleted?.Invoke(this, new RobotPoseCompletedEventArgs(
                        storedPose.PoseName, index, poses.Count, pose));
                }
            }
            finally
            {
                _isPlayingPoses = false;
            }
        }

        private static double GetAngle(double[] angles, int index)
        {
            return index < angles.Length ? angles[index] : 0;
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
            if (IsDragEnabled)
                return;

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
            if (IsDragEnabled)
                return;

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

        public bool IsDragEnabled { get; set; }

        public double DragAnchorX => _fixedBase.X;
        public double DragAnchorY => _fixedBase.Y;

        public UIElement Visual => this;

        public Point OriginalPosition
        {
            get
            {
                double left = Canvas.GetLeft(this);
                double top = Canvas.GetTop(this);
                return new Point(
                    double.IsNaN(left) ? 0 : left,
                    double.IsNaN(top) ? 0 : top);
            }
        }

        public void OnDragMouseup()
        {
            DragCompleted?.Invoke(this, EventArgs.Empty);
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
            _handPoint=joint;
            PositionCarriedPart();
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
            _isSynchronizingJointProperties = true;
            try
            {
                SetCurrentValue(Joint1AngleProperty, GetRelativeAngle(0));
                SetCurrentValue(Joint2AngleProperty, GetRelativeAngle(1));
                SetCurrentValue(Joint3AngleProperty, GetRelativeAngle(2));
                SetCurrentValue(Joint4AngleProperty, GetRelativeAngle(3));
            }
            finally
            {
                _isSynchronizingJointProperties = false;
            }
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
