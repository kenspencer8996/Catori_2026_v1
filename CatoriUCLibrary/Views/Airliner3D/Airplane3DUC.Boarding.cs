using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace CatoriUCLibrary.Views.Airliner3D;

public partial class Airplane3DUC
{
    private CancellationTokenSource? tripCancellation;
    private PathGeometry? taxiPath;
    private int tripVersion;
    private bool startingTaxi;
    public AirplaneTripState TripState { get; private set; } = AirplaneTripState.Parked;
    public AirplaneOccupant? BoardedPilot { get; private set; }
    public AirplaneOccupant? BoardedPassenger { get; private set; }
    public bool AreEnginesRunning => LeftEngineRunning && RightEngineRunning;
    public Task DepartureTask { get; private set; } = Task.CompletedTask;
    public Exception? LastTripError { get; private set; }
    public event Action<AirplaneTripState>? TripStateChanged;
    public event Action<AirplaneOccupant>? OccupantBoarded;
    public event Action<AirplaneOccupant>? OccupantDisembarked;
    public event Action? EnginesReady;
    public event Action? TaxiStarted;
    public event Action? Arrived;
    public event Action<Exception>? TripFailed;

    public void ConfigureTaxiPath(PathGeometry path)
    {
        Dispatcher.VerifyAccess();
        if (tripCancellation != null || IsPathActive)
        {
            throw new InvalidOperationException("Reset the trip before assigning a different taxi path.");
        }
        _ = new PathFlightSampler(path);
        taxiPath = path.CloneCurrentValue();
        taxiPath.Freeze();
    }

    public bool CanBoard(AirplaneOccupant person)
    {
        Dispatcher.VerifyAccess();
        if (person == null || string.IsNullOrWhiteSpace(person.Id) || string.IsNullOrWhiteSpace(person.Name)
            || !IsLandingGearLowered || taxiPath == null || IsPathActive || LeftEngineRunning || RightEngineRunning)
        {
            return false;
        }
        taxiPath.GetPointAtFractionLength(0, out var start, out _);
        if (CurrentPose == null || (CurrentPose.Position - start).Length > 1) return false;
        return person.Role switch
        {
            AirplaneOccupantRole.Pilot => TripState == AirplaneTripState.Parked && BoardedPilot == null,
            AirplaneOccupantRole.Passenger => TripState == AirplaneTripState.WaitingForPassenger
                && BoardedPilot != null && BoardedPassenger == null && person.Id != BoardedPilot.Id,
            _ => false
        };
    }

    /// <summary>Accept pilot, then passenger. Departure continues through DepartureTask after boarding completes.</summary>
    public async Task<bool> BoardAsync(AirplaneOccupant person, CancellationToken cancellationToken = default)
    {
        Dispatcher.VerifyAccess();
        cancellationToken.ThrowIfCancellationRequested();
        if (!CanBoard(person)) return false;
        tripCancellation ??= new CancellationTokenSource();
        int version = tripVersion;
        using var boardingCancellation = CancellationTokenSource.CreateLinkedTokenSource(tripCancellation.Token, cancellationToken);
        try
        {
            SetTripState(person.Role == AirplaneOccupantRole.Pilot ? AirplaneTripState.BoardingPilot : AirplaneTripState.BoardingPassenger);
            await Task.Delay(300, boardingCancellation.Token);
            boardingCancellation.Token.ThrowIfCancellationRequested();
            if (version != tripVersion) return false;
            if (person.Role == AirplaneOccupantRole.Pilot)
            {
                BoardedPilot = person;
                OccupantBoarded?.Invoke(person);
                if (version != tripVersion) return false;
                SetTripState(AirplaneTripState.WaitingForPassenger);
            }
            else
            {
                BoardedPassenger = person;
                OccupantBoarded?.Invoke(person);
                if (version != tripVersion) return false;
                DepartureTask = DepartAsync(version, tripCancellation.Token);
            }
            return true;
        }
        catch (OperationCanceledException)
        {
            if (version == tripVersion) CancelTrip();
            return false;
        }
        catch (Exception exception)
        {
            if (version == tripVersion) FailTrip(exception);
            return false;
        }
    }

    private async Task DepartAsync(int version, CancellationToken token)
    {
        try
        {
            SetTripState(AirplaneTripState.StartingLeftEngine);
            await SpinUpEngineAsync(1, token);
            SetTripState(AirplaneTripState.StartingRightEngine);
            await SpinUpEngineAsync(2, token);
            token.ThrowIfCancellationRequested();
            if (version != tripVersion) return;
            if (BoardedPilot == null || BoardedPassenger == null || !AreEnginesRunning)
                throw new InvalidOperationException("Taxi requires both occupants and both engines ready.");
            SetTripState(AirplaneTripState.ReadyToTaxi);
            EnginesReady?.Invoke();
            token.ThrowIfCancellationRequested();
            if (!AreEnginesRunning) throw new InvalidOperationException("An engine stopped before taxi could begin.");
            Task movement;
            startingTaxi = true;
            try
            {
                movement = FollowPathAsync(taxiPath!, token);
            }
            finally
            {
                startingTaxi = false;
            }
            SetTripState(AirplaneTripState.Taxiing);
            TaxiStarted?.Invoke();
            await movement;
            token.ThrowIfCancellationRequested();
            if (version != tripVersion) return;
            StopEngine(1);
            StopEngine(2);
            SetTripState(AirplaneTripState.Arrived);
            Arrived?.Invoke();
            token.ThrowIfCancellationRequested();
            SetTripState(AirplaneTripState.Disembarking);
            await Task.Delay(400, token);
            if (version != tripVersion) return;
            ReleaseOccupants();
            tripCancellation?.Dispose();
            tripCancellation = null;
            SetTripState(AirplaneTripState.Parked);
        }
        catch (OperationCanceledException)
        {
            if (version == tripVersion) CancelTrip();
        }
        catch (Exception exception)
        {
            if (version == tripVersion) FailTrip(exception);
        }
    }

    private async Task SpinUpEngineAsync(int number, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        var rotation = EngineRotation(number);
        rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty,
            new DoubleAnimation(rotation.Angle, rotation.Angle + 720, TimeSpan.FromMilliseconds(800))
            {
                AccelerationRatio = 1,
                FillBehavior = FillBehavior.HoldEnd
            });
        await Task.Delay(800, token);
        token.ThrowIfCancellationRequested();
        StartEngine(number);
    }

    public void CancelTrip()
    {
        Dispatcher.VerifyAccess();
        tripVersion++;
        tripCancellation?.Cancel();
        tripCancellation?.Dispose();
        tripCancellation = null;
        player?.Dispose();
        player = null;
        StopEngine(1);
        StopEngine(2);
        ReleaseOccupants();
        SetTripState(AirplaneTripState.Cancelled);
    }

    public void ResetTrip()
    {
        CancelTrip();
        LastTripError = null;
        ResetLandingGear();
        if (taxiPath != null) ResetPath(taxiPath);
        ReturnToAutomaticOrientation();
        SetTripState(AirplaneTripState.Parked);
    }

    private void FailTrip(Exception exception)
    {
        CancelTrip();
        LastTripError = exception;
        SetTripState(AirplaneTripState.Faulted);
        TripFailed?.Invoke(exception);
    }

    private void ReleaseOccupants()
    {
        var passenger = BoardedPassenger;
        var pilot = BoardedPilot;
        BoardedPassenger = null;
        BoardedPilot = null;
        if (passenger != null) OccupantDisembarked?.Invoke(passenger);
        if (pilot != null) OccupantDisembarked?.Invoke(pilot);
    }

    private void SetTripState(AirplaneTripState state)
    {
        if (TripState == state) return;
        TripState = state;
        TripStateChanged?.Invoke(state);
    }
}
