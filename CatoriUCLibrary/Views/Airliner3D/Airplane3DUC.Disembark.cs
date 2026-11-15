namespace CatoriUCLibrary.Views.Airliner3D;

public partial class Airplane3DUC
{
    /// <summary>Ends this trip and releases all onboard occupants through the existing exit events.</summary>
    public void Disembark()
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
        SetTripState(AirplaneTripState.Disembarking);
        ReleaseOccupants();
        SetTripState(AirplaneTripState.Parked);
    }
}
