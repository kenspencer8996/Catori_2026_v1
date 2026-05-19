namespace CatoriUCLibrary.Views.RobotArm
{
    public enum RobotJobState
    {
        Idle,
        WaitingForParts,
        MoveToPickupB,
        GrabPartB,
        LiftPartB,
        MoveToDropOnA,
        LowerPartB,
        Assemble,
        ReleasePart,
        ReturnHome,
        Complete,
        All
    }
}
