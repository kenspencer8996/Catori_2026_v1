using System.Windows;

namespace CatoriUCLibrary.Objects.PersonItems;

public static class PersonItemDrag
{
    public const string Format = "Catori.PersonItem";
    public static DataObject Create(PersonItem item)
    {
        return new DataObject(Format, item);
    }
    public static PersonItem? Read(IDataObject data)
    {
        return data.GetData(Format) as PersonItem;
    }
}
