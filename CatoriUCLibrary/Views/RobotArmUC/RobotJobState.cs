namespace CatoriUCLibrary.Views.RobotArmUC
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
