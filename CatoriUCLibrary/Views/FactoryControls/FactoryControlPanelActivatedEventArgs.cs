using CatoriUCLibrary.Views.Person;

namespace CatoriUCLibrary.Views.FactoryControls;

public sealed class FactoryControlPanelActivatedEventArgs(
    string animationTargetName,
    PersonUC? person) : EventArgs
{
    public string AnimationTargetName { get; } = animationTargetName;
    public PersonUC? Person { get; } = person;
}

public sealed record FactoryControlPanelActivatedMessage(
    string PanelName,
    string AnimationTargetName,
    PersonUC? Person);
