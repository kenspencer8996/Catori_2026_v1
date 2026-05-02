using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CatoriCity2025WPF.Views.Controls.Robots.RobotArm.Robot
{
    public class AssemblyCellController
    {
        private readonly RobotArmControl _robot;
        private readonly Canvas _hostCanvas;

        private readonly FrameworkElement _partAVisual;
        private readonly FrameworkElement _partBVisual;
        private readonly FrameworkElement _carriedPartBVisual;
        private readonly FrameworkElement _assembledPartVisual;
        private bool _isCarryingPart;
        public RobotJobState State { get; private set; } = RobotJobState.Idle;

        public RobotPose HomePose { get; set; } = new RobotPose(0, 0, 0, 0);
        public RobotPose PickupBPose { get; set; } = new RobotPose(50, 25, -35, 10);
        public RobotPose LiftBPose { get; set; } = new RobotPose(20, 0, -10, 0);
        public RobotPose DropOnAPose { get; set; } = new RobotPose(-20, 35, -25, 0);
        public RobotPose LowerOnAPose { get; set; } = new RobotPose(-10, 50, -35, 5);

        public AssemblyCellController(
            RobotArmControl robot,
            Canvas hostCanvas,
            FrameworkElement partAVisual,
            FrameworkElement partBVisual,
            FrameworkElement carriedPartBVisual,
            FrameworkElement assembledPartVisual)
        {
            _robot = robot ?? throw new ArgumentNullException(nameof(robot));
            _hostCanvas = hostCanvas ?? throw new ArgumentNullException(nameof(hostCanvas));
            _partAVisual = partAVisual ?? throw new ArgumentNullException(nameof(partAVisual));
            _partBVisual = partBVisual ?? throw new ArgumentNullException(nameof(partBVisual));
            _carriedPartBVisual = carriedPartBVisual ?? throw new ArgumentNullException(nameof(carriedPartBVisual));
            _assembledPartVisual = assembledPartVisual ?? throw new ArgumentNullException(nameof(assembledPartVisual));
        }

        public async Task RunCycleAsync(RobotJobState runState )
        {
            //int delaybetweenSteps = 500;
            //State = RobotJobState.WaitingForParts;
            //if (_partAVisual.Visibility != Visibility.Visible || _partBVisual.Visibility != Visibility.Visible)
            //{
            //    State = RobotJobState.Idle;
            //    return;
            //}
            //if (runState == RobotJobState.MoveToPickupB || runState == RobotJobState.All)
            //{
            //    Debug.WriteLine($"MoveToPickupB J1: {_robot.Joint1Rotate.Angle}, Target J1: {PickupBPose.Joint1}");
            //    //_robot.Joint1Rotate.Angle = -135;
            //    State = RobotJobState.MoveToPickupB;
            //    await _robot.MoveToPoseAsync(PickupBPose, TimeSpan.FromMilliseconds(800));
            //    //PickupBPose.Joint1 = -135;
            //    //await _robot.MoveToPoseAsync(PickupBPose, TimeSpan.FromMilliseconds(800));

            //    State = RobotJobState.GrabPartB;
            //    GrabPartB();
            //    await Task.Delay(delaybetweenSteps);
            //}
            //if (runState == RobotJobState.LiftPartB || runState == RobotJobState.All)
            //{
            //    Debug.WriteLine($"LiftPartB J2: {_robot.Joint2Rotate.Angle}, Target J2: {PickupBPose.Joint2}");
            //    State = RobotJobState.LiftPartB;
            //    await _robot.MoveToPoseAsync(LiftBPose, TimeSpan.FromMilliseconds(1600));
            //    UpdateCarriedPartPosition();
            //}

            //if (runState == RobotJobState.MoveToDropOnA || runState == RobotJobState.All)
            //{
            //    Debug.WriteLine($"MoveToDropOnA J3: {_robot.Joint3Rotate.Angle}, Target J3: {PickupBPose.Joint3}");
            //    State = RobotJobState.MoveToDropOnA;
            //    await _robot.MoveToPoseAsync(DropOnAPose, TimeSpan.FromMilliseconds(1900));
            //    UpdateCarriedPartPosition();
            //}
            //if (runState == RobotJobState.LowerPartB || runState == RobotJobState.All)
            //{
            //    Debug.WriteLine($"LowerPartB J4: {_robot.JointHandRotate.Angle}, Target J4: {PickupBPose.JointHand}");
            //    State = RobotJobState.LowerPartB;
            //    await _robot.MoveToPoseAsync(LowerOnAPose, TimeSpan.FromMilliseconds(1500));
            //    UpdateCarriedPartPosition();
            //    await Task.Delay(delaybetweenSteps);
            //}
            //if (runState == RobotJobState.Assemble || runState == RobotJobState.All)
            //{
            //    State = RobotJobState.Assemble;
            //    AssembleParts();
            //}
            //if (runState == RobotJobState.ReleasePart || runState == RobotJobState.All)
            //{
            //    Debug.WriteLine($"ReleasePart J4: ");
            //    State = RobotJobState.ReleasePart;
            //    ReleasePartB();
            //}
            //if (runState == RobotJobState.ReturnHome || runState == RobotJobState.All)
            //{
            //    Debug.WriteLine($"ReturnHome  ");
            //    State = RobotJobState.ReturnHome;
            //    await _robot.MoveToPoseAsync(HomePose, TimeSpan.FromMilliseconds(1900));
            //}
            //State = RobotJobState.Complete;
        }
        private void StartCarrying()
        {
            if (_isCarryingPart)
                return;

            _isCarryingPart = true;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void StopCarrying()
        {
            if (!_isCarryingPart)
                return;

            _isCarryingPart = false;
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            if (!_isCarryingPart)
                return;

            UpdateCarriedPartPosition();
        }
        private void GrabPartB()
        {
            if (_partBVisual is Image sourceImage && _carriedPartBVisual is Image carriedImage)
            {
                carriedImage.Source = sourceImage.Source;
                carriedImage.Width = sourceImage.ActualWidth > 0 ? sourceImage.ActualWidth : sourceImage.Width;
                carriedImage.Height = sourceImage.ActualHeight > 0 ? sourceImage.ActualHeight : sourceImage.Height;
            }

            _partBVisual.Visibility = Visibility.Collapsed;
            _carriedPartBVisual.Visibility = Visibility.Visible;

            UpdateCarriedPartPosition();
            StartCarrying();
        }

        private void ReleasePartB()
        {
            StopCarrying();
            _carriedPartBVisual.Visibility = Visibility.Collapsed;
        }

        private void AssembleParts()
        {
            _partAVisual.Visibility = Visibility.Collapsed;
            _carriedPartBVisual.Visibility = Visibility.Collapsed;
            _assembledPartVisual.Visibility = Visibility.Visible;

            // Put assembled part where Part A was
            Canvas.SetLeft(_assembledPartVisual, Canvas.GetLeft(_partAVisual));
            Canvas.SetTop(_assembledPartVisual, Canvas.GetTop(_partAVisual));
        }
     
        private void UpdateCarriedPartPosition()
        {
            //Point handPoint = GetHandWorldPoint();

            double width = _carriedPartBVisual.ActualWidth > 0 ? _carriedPartBVisual.ActualWidth : _carriedPartBVisual.Width;
            double height = _carriedPartBVisual.ActualHeight > 0 ? _carriedPartBVisual.ActualHeight : _carriedPartBVisual.Height;

            double offsetX = width / 2.0;
            double offsetY = height / 2.0;

            //Canvas.SetLeft(_carriedPartBVisual, handPoint.X - offsetX);
            //Canvas.SetTop(_carriedPartBVisual, handPoint.Y - offsetY);
        }

        //private Point GetHandWorldPoint()
        //{
        //    // Replace "JointHandHost" or a child inside it with the exact element you want to track.
        //    // This uses the hand host’s origin as the carry point.
        //    GeneralTransform transform = _robot.HandHostElement.TransformToAncestor(_hostCanvas);
        //    return transform.Transform(new Point(0, _robot.HandHostElement.ActualHeight / 2));
        //}
    }
}
