namespace CatoriUCLibrary.Views.Person;

public sealed class PersonItemDroppedEventArgs(PersonDroppedItemType itemType, object item) : EventArgs
{
    public PersonDroppedItemType ItemType { get; } = itemType;
    public object Item { get; } = item;
}
