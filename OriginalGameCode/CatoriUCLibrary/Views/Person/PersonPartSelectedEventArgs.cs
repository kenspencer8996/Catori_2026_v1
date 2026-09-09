namespace CatoriUCLibrary.Views.Person;

public sealed class PersonPartSelectedEventArgs(PersonPartType part, double angle) : EventArgs
{
    public PersonPartType Part { get; } = part;
    public double Angle { get; } = angle;
}
