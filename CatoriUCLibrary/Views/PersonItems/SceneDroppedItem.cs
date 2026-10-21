using CatoriUCLibrary.Objects.PersonItems;

namespace CatoriUCLibrary.Views;

public sealed class SceneDroppedItem
{
    public PersonItem Item { get; init; } = null!;
    public double X { get; init; }
    public double Y { get; init; }
    public double Width { get; init; }
    public double Height { get; init; }
}
