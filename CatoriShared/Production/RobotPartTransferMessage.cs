namespace CatoriApp.Core.Objects.Production;

public enum RobotPartTransferStage
{
    PickedUp,
    Dropped
}

public sealed record RobotPartTransferMessage(
    string RobotName,
    string PartName,
    RobotPartTransferStage Stage,
    FrameworkElement? Part);
