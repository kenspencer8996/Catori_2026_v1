namespace CatoriApp.Core.Objects.Production;

public sealed record PathAnimationDefinition(
    string AnimationName,
    string PartName,
    string PathData,
    string TargetName,
    ProductionAction NextAction);
