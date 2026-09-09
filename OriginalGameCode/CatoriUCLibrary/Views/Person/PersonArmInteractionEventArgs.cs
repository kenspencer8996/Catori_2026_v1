using System.Windows;

namespace CatoriUCLibrary.Views.Person;

public sealed class PersonArmInteractionEventArgs(Point handPoint) : EventArgs
{
    public Point HandPoint { get; } = handPoint;
}
