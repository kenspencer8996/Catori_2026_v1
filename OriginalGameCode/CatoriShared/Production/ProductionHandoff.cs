namespace CatoriApp.Core.Objects.Production;

public sealed record ProductionHandoff(string TargetName,ProductionAction NextAction);
