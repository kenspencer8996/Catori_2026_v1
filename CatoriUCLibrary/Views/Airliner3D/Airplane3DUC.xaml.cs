using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace CatoriUCLibrary.Views.Airliner3D;

public partial class Airplane3DUC : UserControl
{
    private PathFlightPlayer? player;
    private double speed = 100;
    private double maximumBank = 30;
    public event Action<FlightPose>? PoseChanged;
    public FlightPose? CurrentPose { get; private set; }
    public bool LeftEngineRunning { get; private set; }
    public bool RightEngineRunning { get; private set; }
    public double LeftPropellerAngle => LeftPropellerRotation.Angle;
    public double RightPropellerAngle => RightPropellerRotation.Angle;
    public bool IsPathActive => player?.IsActive == true;
    public bool IsPathPaused => player?.IsPaused == true;
    public double Speed
    {
        get => speed;
        set
        {
            Dispatcher.VerifyAccess();
            if (!double.IsFinite(value) || value <= 0 || value > 2000)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            speed = value;
            if (player != null)
            {
                player.Simulation.Speed = value;
            }
        }
    }
    public double MaximumBankAngle
    {
        get => maximumBank;
        set
        {
            Dispatcher.VerifyAccess();
            if (!double.IsFinite(value) || value < 0 || value > 75)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            maximumBank = value;
            if (player != null)
            {
                player.Simulation.MaximumBankAngle = value;
            }
        }
    }

    public Airplane3DUC()
    {
        InitializeComponent();
        var meshes = new AirplaneMeshes();
        FuselageModel.Geometry = meshes.Fuselage;
        CockpitModel.Geometry = meshes.Cockpit;
        PassengerWindowsModel.Geometry = meshes.PassengerWindows;
        LeftBrandModel.Geometry = meshes.LeftBrand;
        RightBrandModel.Geometry = meshes.RightBrand;
        WingsModel.Geometry = meshes.Wings;
        TailModel.Geometry = meshes.Tail;
        FinModel.Geometry = meshes.Fin;
        EnginesModel.Geometry = meshes.Engines;
        LeftMainGearModel.Geometry = AirplaneMeshes.CreateGearStrut(-7, -18);
        LeftMainWheelModel.Geometry = AirplaneMeshes.CreateGearWheel(-7, -18);
        RightMainGearModel.Geometry = AirplaneMeshes.CreateGearStrut(-7, 18);
        RightMainWheelModel.Geometry = AirplaneMeshes.CreateGearWheel(-7, 18);
        NoseGearModel.Geometry = AirplaneMeshes.CreateGearStrut(45, 0);
        NoseWheelModel.Geometry = AirplaneMeshes.CreateGearWheel(45, 0);
        SpinnersModel.Geometry = meshes.Spinners;
        LeftPropellerModel.Geometry = meshes.Propeller;
        RightPropellerModel.Geometry = meshes.Propeller;
    }

    public void SetHeading(double degrees)
    {
        ValidateAngle(degrees);
        YawRotation.Angle = AngleMath.Unwrap(YawRotation.Angle, SceneProjection.ToYaw(degrees));
    }

    public void SetPitch(double degrees)
    {
        ValidateAngle(degrees);
        PitchRotation.Angle = -degrees;
    }

    public void SetRoll(double degrees)
    {
        ValidateAngle(degrees);
        RollRotation.Angle = degrees;
    }

    private void ValidateAngle(double value)
    {
        Dispatcher.VerifyAccess();
        if (!double.IsFinite(value))
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }
    }

    public void StartEngine(int engineNumber)
    {
        Dispatcher.VerifyAccess();
        var rotation = EngineRotation(engineNumber);
        double current = rotation.Angle;
        var animation = new DoubleAnimation(current, current + 360, TimeSpan.FromSeconds(engineNumber == 1 ? .43 : .57))
        {
            RepeatBehavior = RepeatBehavior.Forever
        };
        rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, animation, HandoffBehavior.SnapshotAndReplace);
        if (engineNumber == 1)
        {
            LeftEngineRunning = true;
        }
        else
        {
            RightEngineRunning = true;
        }
    }

    public void StopEngine(int engineNumber)
    {
        Dispatcher.VerifyAccess();
        var rotation = EngineRotation(engineNumber);
        double current = rotation.Angle;
        rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, null);
        rotation.Angle = current % 360;
        if (engineNumber == 1)
        {
            LeftEngineRunning = false;
        }
        else
        {
            RightEngineRunning = false;
        }
        if (TripState == AirplaneTripState.Taxiing && IsPathActive) PausePath();
    }

    private AxisAngleRotation3D EngineRotation(int engineNumber)
    {
        return engineNumber switch
        {
            1 => LeftPropellerRotation,
            2 => RightPropellerRotation,
            _ => throw new ArgumentOutOfRangeException(nameof(engineNumber), "Use 1 (left) or 2 (right).")
        };
    }

    public Task FollowPathAsync(PathGeometry path, CancellationToken cancellationToken = default)
    {
        Dispatcher.VerifyAccess();
        ResetPath(path);
        return player!.RunAsync(cancellationToken);
    }

    public Task FollowPathAsync(PathGeometry path, double durationSeconds, CancellationToken cancellationToken)
    {
        if (!double.IsFinite(durationSeconds) || durationSeconds <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(durationSeconds));
        }
        Speed = new PathFlightSampler(path).Length / durationSeconds;
        return FollowPathAsync(path, cancellationToken);
    }

    public void PausePath()
    {
        player?.Pause();
        if (TripState == AirplaneTripState.Taxiing) SetTripState(AirplaneTripState.Paused);
    }

    public void ResumePath()
    {
        if (tripCancellation != null && !AreEnginesRunning) return;
        player?.Resume();
        if (TripState == AirplaneTripState.Paused && IsPathActive) SetTripState(AirplaneTripState.Taxiing);
    }

    public void ResetPath(PathGeometry path)
    {
        Dispatcher.VerifyAccess();
        if (tripCancellation != null && !startingTaxi) CancelTrip();
        var simulation = new PathFlightSimulation(path) { Speed = Speed, MaximumBankAngle = MaximumBankAngle };
        player?.Dispose();
        player = new PathFlightPlayer(simulation, ApplyPose, Dispatcher);
        SetPitch(0);
        ApplyPose(simulation.Pose);
    }

    private void ApplyPose(FlightPose pose)
    {
        var position = SceneProjection.ToWorld(pose.Position);
        AirplanePosition.OffsetX = position.X;
        AirplanePosition.OffsetY = position.Y;
        AirplanePosition.OffsetZ = position.Z;
        SetHeading(pose.Heading);
        SetRoll(pose.Bank);
        CurrentPose = pose;
        UpdateCameraView();
        PoseChanged?.Invoke(pose);
    }

    private void Control_Unloaded(object sender, RoutedEventArgs e)
    {
        CancelTrip();
        CancelLandingGearMotion();
        ClearManualOrientation();
        CancelCameraTransition();
        player?.Dispose();
        player = null;
        StopEngine(1);
        StopEngine(2);
    }
}
