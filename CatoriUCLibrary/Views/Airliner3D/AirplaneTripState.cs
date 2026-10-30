namespace CatoriUCLibrary.Views.Airliner3D;

public enum AirplaneTripState
{
    Parked,
    BoardingPilot,
    WaitingForPassenger,
    BoardingPassenger,
    StartingLeftEngine,
    StartingRightEngine,
    ReadyToTaxi,
    Taxiing,
    Paused,
    Arrived,
    Disembarking,
    Cancelled,
    Faulted
}
