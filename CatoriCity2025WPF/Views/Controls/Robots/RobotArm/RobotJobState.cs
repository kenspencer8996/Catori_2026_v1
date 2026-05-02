namespace CatoriCity2025WPF.Views.Controls.Robots.RobotArm.Robot
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
